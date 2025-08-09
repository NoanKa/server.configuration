using System.Collections.Generic;

namespace Server.Model
{
    public class ResponseListBase<T> : PagingResponse
    {
        public List<T>? Data { get; set; } = null;
        public bool Success { get; set; }
        public string? Code { get; set; } = null;
        public string? Message { get; set; } = null;
    }
}
