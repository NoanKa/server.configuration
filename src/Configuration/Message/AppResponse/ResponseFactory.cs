using Server.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server.Configuration
{
    public class ResponseFactory : IResponseFactory
    {
        private readonly IAppMessageFactory appMessageFactory;

        public ResponseFactory(IAppMessageFactory appMessageFactory)
        {
            this.appMessageFactory = appMessageFactory;
        }

        #region RESPONSEBASE CONSTRUCTORS
        public ResponseBase<T> Response<T>(T? data, string? code = null)
        {
            var response = new ResponseBase<T>
            {
                Data = data,
                Code = code,
                Success = true,
                Message = code != null ? appMessageFactory.Message(code) : null
            };

            return response;
        }

        public ResponseBase<object> Response()
        {
            var response = new ResponseBase<object>
            {
                Success = true
            };

            return response;
        }
        #endregion

        #region RESPONSELISTBASE CONSTRUCTORS
        public ResponseListBase<T> ResponseList<T>(List<T> dataList, PagingRequest? pagingRequest = null, int? pageCount = null, string? code = null)
        {
            var response = new ResponseListBase<T>
            {
                Data = dataList,
                Code = code,
                Success = true,
                PageIndex = pagingRequest?.Index,
                PageSize = pagingRequest?.Size == 0 ? 50 : pagingRequest?.Size,
                PageCount = pageCount,
                Message = code != null ? appMessageFactory.Message(code) : null
            };
            return response;
        }
        #endregion
    }
}
