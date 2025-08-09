using Server.Model;

namespace Server.Configuration
{
    public interface IResponseFactory
    {
        ResponseBase<object> Response();
        ResponseBase<T> Response<T>(T? data, string? code = null);
        ResponseListBase<T> ResponseList<T>(List<T> dataList, PagingRequest? pagingRequest = null, int? pageCount = null, string? code = null);
    }
}
