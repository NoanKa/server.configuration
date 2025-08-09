using Microsoft.AspNetCore.Mvc;

namespace Server.Model
{
    public class PagingRequest
    {
        [FromQuery(Name = "index")]
        public int Index { get; set; }
        [FromQuery(Name = "size")]
        public int Size { get; set; }
    }
}
