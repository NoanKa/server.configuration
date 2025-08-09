using System.Collections.Generic;
using System.Net;

namespace Server.Model
{
    public static class AppMessage
    {
        #region COMMON ERROR MESSAGES
        public static string UNHANDLED_ERROR = "1";
        public static string REQUIRED_PARAMETER = "2";
        public static string EXTERNAL_SERVICE_ERROR = "3";
        public static string AUTH_EMAIL_INVALID = "4";
        public static string PROJECT_ID_INVALID = "5";
        public static string INVALID_PARAMETER = "400";
        public static string UNAUTHORIZED = "401";
        public static string FORBIDDEN = "403";
        public static string CONTENT_TOO_LARGE = "413";
        public static string UNSUPPORTED_MEDIA_TYPE = "415";
        public static string SERVER_ERROR = "500";
        public static string SERVICE_UNAVAILABLE = "503";
        #endregion

        public static Dictionary<string, HttpStatusCode> HttpStatus =
            new Dictionary<string, HttpStatusCode>()
            {
                //The Http status codes for the server will go here
                {UNHANDLED_ERROR, HttpStatusCode.InternalServerError},
                {REQUIRED_PARAMETER, HttpStatusCode.BadRequest},
                {EXTERNAL_SERVICE_ERROR, HttpStatusCode.ServiceUnavailable},
                {AUTH_EMAIL_INVALID, HttpStatusCode.BadRequest},
                {PROJECT_ID_INVALID, HttpStatusCode.BadRequest},
                {INVALID_PARAMETER, HttpStatusCode.BadRequest},
                {UNAUTHORIZED, HttpStatusCode.Unauthorized},
                {FORBIDDEN, HttpStatusCode.Forbidden},
                {CONTENT_TOO_LARGE, HttpStatusCode.RequestEntityTooLarge},
                {UNSUPPORTED_MEDIA_TYPE, HttpStatusCode.UnsupportedMediaType},
                {SERVER_ERROR, HttpStatusCode.InternalServerError},
                {SERVICE_UNAVAILABLE, HttpStatusCode.ServiceUnavailable},
            };
    }
}
