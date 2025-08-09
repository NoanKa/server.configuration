using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Server.Model;

namespace Server.Configuration
{
    public class Communicator : ICommunicator
    {
        #region Properties
        protected HttpMethod HttpRequestType { get; set; } = HttpMethod.Options;
        protected string? Bearer { get; set; }
        protected string ClientName { get; set; } = "";
        protected string AdaptiveUri { get; set; } = "";
        protected string HttpContentType { get; set; } = "";
        protected Dictionary<string, string> CustomHeaders { get; set; } = new Dictionary<string, string>();
        protected Dictionary<string, string> CustomQueryParams { get; set; } = new Dictionary<string, string>();
        protected Dictionary<string, string> CustomPathParams { get; set; } = new Dictionary<string, string>();
        #endregion

        #region Dependency Injections
        protected readonly IHttpClientFactory httpClientFactory;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger logger;
        #endregion

        #region Builder Interfaces
        public interface IClient { IUri Client(string clientName); }
        public interface IUri { IMethod Uri(string adaptiveUri); }
        public interface IMethod { IContentType Method(HttpMethod httpRequestType); }
        public interface IContentType { IBuild ContentType(string engine); }
        public interface IBuild 
        {
            IBuild Auth(string bearer);
            IBuild Headers(Dictionary<string, string> customHeaders);
            IBuild Queries(Dictionary<string, string> customQueryParams);
            IBuild Paths(Dictionary<string, string> customPathParams);
            Communicator Build();
        }
        #endregion

        public Communicator(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        #region Call
        public IClient Builder()
        {
            return new CommunicatorBuilder(httpClientFactory, httpContextAccessor, logger);
        }

        public async Task<TResponse?> CallApi<TRequest, TResponse>(TRequest request) where TRequest : class
        {
            TResponse? response = default;

            var adaptiveUri = PrepareAdaptiveUri();
            var language = httpContextAccessor?.HttpContext?.Request.Headers.AcceptLanguage.ToString();
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true
            };

            try
            {
                using (var userHttpClient = httpClientFactory.CreateClient(ClientName))
                {
                    if (string.IsNullOrEmpty(Bearer))
                    {
                        string jwtToken = (httpContextAccessor?.HttpContext?.Request.Headers.Authorization.ToString() ?? "").Replace($"{"Bearer"}", "").Trim();
                        userHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                    }
                    else
                    {
                        userHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Bearer);
                    }

                    userHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpContentType ?? ContentType.ApplicationJson));

                    var headerValues = ParseAcceptLanguageHeader(language);

                    foreach (var value in headerValues)
                    {
                        userHttpClient.DefaultRequestHeaders.AcceptLanguage.Add(value);
                    }

                    if (CustomHeaders != null && CustomHeaders.Count > 0)
                    {
                        foreach (var header in CustomHeaders)
                        {
                            if (!string.IsNullOrEmpty(header.Key) && !string.IsNullOrEmpty(header.Value))
                                userHttpClient.DefaultRequestHeaders.Add(name: header.Key, value: header.Value);
                        }
                    }

                    var timer = new Stopwatch();
                    timer.Start();

                    HttpContent content;
                    if (request is MultipartFormDataContent multipartContent)
                    {
                        content = multipartContent;
                        HttpContentType = ContentType.MultipartFormdata;
                    }
                    else if (request is string && (HttpContentType ?? "").StartsWith("text/"))
                    {
                        content = new StringContent(request.ToString() ?? "", Encoding.UTF8, HttpContentType ?? "");
                        HttpContentType += "; charset=utf-8";
                    }
                    else if (request is string || request.GetType().IsPrimitive)
                    {
                        content = new StringContent(request.ToString() ?? "", Encoding.UTF8, HttpContentType ?? "");
                    }
                    else
                    {
                        var json = JsonSerializer.Serialize(request, options);
                        content = new StringContent(json, Encoding.UTF8, ContentType.ApplicationJson);
                        HttpContentType = ContentType.ApplicationJson;
                    }

                    HttpResponseMessage httpResponseMessage;

                    httpResponseMessage = HttpRequestType.Method switch
                    {
                        "GET" => await userHttpClient.GetAsync(adaptiveUri),
                        "POST" => await userHttpClient.PostAsync(adaptiveUri, content),
                        "PUT" => await userHttpClient.PutAsync(adaptiveUri, content),
                        "PATCH" => await userHttpClient.PatchAsync(adaptiveUri, content),
                        "DELETE" => await userHttpClient.DeleteAsync(adaptiveUri),
                        _ => throw new BusinessException(AppMessage.INVALID_PARAMETER),
                    };
                    var readAsStringAsync = await httpResponseMessage.Content.ReadAsStringAsync();

                    timer.Stop();

                    logger.LogInformation(readAsStringAsync, MethodBase.GetCurrentMethod(), timer.ElapsedMilliseconds, httpResponseMessage.StatusCode.ToString());

                    if (httpResponseMessage.Content.Headers.ContentType?.MediaType == ContentType.ApplicationJson)
                    {
                        response = JsonSerializer.Deserialize<TResponse>(readAsStringAsync, options);
                    }
                    else
                    {
                        response = (TResponse?)(object)readAsStringAsync;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(BusinessException))
                {
                    throw (BusinessException)ex;
                }
                else
                {
                    throw new BusinessException(AppMessage.EXTERNAL_SERVICE_ERROR);
                }
            }

            return response;
        }

        public async Task<TResponse?> CallApi<TResponse>()
        {
            TResponse? response = default;

            var adaptiveUri = PrepareAdaptiveUri();
            var language = httpContextAccessor?.HttpContext?.Request.Headers.AcceptLanguage.ToString();
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true
            };

            try
            {
                using (var userHttpClient = httpClientFactory.CreateClient(ClientName))
                {
                    if (string.IsNullOrEmpty(Bearer))
                    {
                        string jwtToken = (httpContextAccessor?.HttpContext?.Request.Headers.Authorization.ToString() ?? "").Replace($"{"Bearer"}", "").Trim();
                        userHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                    }
                    else
                    {
                        userHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Bearer);
                    }

                    userHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpContentType ?? ContentType.ApplicationJson));

                    var headerValues = ParseAcceptLanguageHeader(language);

                    foreach (var value in headerValues)
                    {
                        userHttpClient.DefaultRequestHeaders.AcceptLanguage.Add(value);
                    }

                    if (CustomHeaders != null && CustomHeaders.Count > 0)
                    {
                        foreach (var header in CustomHeaders)
                        {
                            if (!string.IsNullOrEmpty(header.Key) && !string.IsNullOrEmpty(header.Value))
                                userHttpClient.DefaultRequestHeaders.Add(name: header.Key, value: header.Value);
                        }
                    }

                    var timer = new Stopwatch();
                    timer.Start();

                    var httpResponseMessage = new HttpResponseMessage();

                    httpResponseMessage = HttpRequestType.Method switch
                    {
                        "GET" => await userHttpClient.GetAsync(adaptiveUri),
                        "POST" => await userHttpClient.PostAsync(adaptiveUri, null),
                        "PUT" => await userHttpClient.PutAsync(adaptiveUri, null),
                        "PATCH" => await userHttpClient.PatchAsync(adaptiveUri, null),
                        "DELETE" => await userHttpClient.DeleteAsync(adaptiveUri),
                        _ => throw new BusinessException(AppMessage.INVALID_PARAMETER),
                    };
                    var readAsStringAsync = await httpResponseMessage.Content.ReadAsStringAsync();

                    timer.Stop();

                    logger.LogInformation(readAsStringAsync, MethodBase.GetCurrentMethod(), timer.ElapsedMilliseconds, httpResponseMessage.StatusCode.ToString());

                    if (httpResponseMessage.Content.Headers.ContentType?.MediaType == ContentType.ApplicationJson)
                    {
                        response = JsonSerializer.Deserialize<TResponse>(readAsStringAsync, options);
                    }
                    else
                    {
                        response = (TResponse?)(object)readAsStringAsync;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(BusinessException))
                {
                    throw (BusinessException)ex;
                }
                else
                {
                    throw;
                }
            }

            return response;
        }

        private string PrepareAdaptiveUri()
        {
            var adaptiveUri = AdaptiveUri;

            if (CustomPathParams.Count > 0)
            {
                foreach (var pathParam in CustomPathParams)
                {
                    var tempStr = "{" + pathParam.Key + "}";
                    adaptiveUri = adaptiveUri.Replace(tempStr, pathParam.Value);
                }
            }

            if (CustomQueryParams.Count > 0)
            {
                adaptiveUri = string.Concat(adaptiveUri, "?");
                var queryParams = new List<string>();
                foreach (var queryParam in CustomQueryParams)
                {
                    var strBuilder = new StringBuilder();
                    strBuilder.Append(queryParam.Key);
                    strBuilder.Append("=");
                    strBuilder.Append(queryParam.Value);
                    queryParams.Add(strBuilder.ToString());
                }
                adaptiveUri = string.Concat(adaptiveUri, string.Join("&", queryParams));
            }
            return adaptiveUri;
        }

        private List<StringWithQualityHeaderValue> ParseAcceptLanguageHeader(string acceptLanguageHeader)
        {
            return acceptLanguageHeader
                .Split(',')
                .Select(entry =>
                {
                    var parts = entry.Split(';');
                    var language = parts[0].Trim();

                    double quality = 1.0;
                    if (parts.Length > 1 && parts[1].Trim().StartsWith("q="))
                    {
                        var qPart = parts[1].Trim().Substring(2);
                        if (double.TryParse(qPart, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var qValue))
                        {
                            quality = qValue;
                        }
                    }

                    return new StringWithQualityHeaderValue(language, quality);
                })
                .Where(h => h != null)
                .ToList();
        }
        #endregion

        #region Builder Class
        private class CommunicatorBuilder : IClient, IUri, IMethod, IContentType, IBuild
        {
            private readonly Communicator communicatorSettings;


            public CommunicatorBuilder(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger logger)
            {
                communicatorSettings = new Communicator(httpClientFactory, httpContextAccessor, logger);
            }

            public IUri Client(string clientName)
            {
                communicatorSettings.ClientName = clientName;
                return this;
            }

            public IMethod Uri(string adaptiveUri)
            {
                communicatorSettings.AdaptiveUri = adaptiveUri;
                return this;
            }

            public IContentType Method(HttpMethod httpRequestType)
            {
                communicatorSettings.HttpRequestType = httpRequestType;
                return this;
            }

            public IBuild ContentType(string contentType)
            {
                communicatorSettings.HttpContentType = contentType;
                return this;
            }

            public IBuild Auth(string bearer)
            {
                communicatorSettings.Bearer = bearer;
                return this;
            }

            public IBuild Headers(Dictionary<string, string> headers)
            {
                communicatorSettings.CustomHeaders = headers;
                return this;
            }

            public IBuild Queries(Dictionary<string, string> queries)
            {
                communicatorSettings.CustomQueryParams = queries;
                return this;
            }

            public IBuild Paths(Dictionary<string, string> paths)
            {
                communicatorSettings.CustomPathParams = paths;
                return this;
            }

            public Communicator Build()
            {
                return communicatorSettings;
            }
        }
        #endregion
    }

    public static class ContentType
    {
        public const string ApplicationJson = "application/json";
        public const string MultipartFormdata = "multipart/formdata";
        public const string TextPlain = "text/plain";
        public const string TextHtml = "text/html";
    }
}
