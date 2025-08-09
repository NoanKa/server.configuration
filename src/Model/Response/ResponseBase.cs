namespace Server.Model
{
    public class ResponseBase<T>
    {
        public T? Data { get; set; } = default;
        public bool Success { get; set; }
        public string? Code { get; set; } = null;
        public string? Message { get; set; } = null;
    }
}
