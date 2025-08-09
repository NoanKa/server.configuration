namespace Server.Configuration
{
    public interface IAppMessageFactory
    {
        public string? Message(string code);
    }
}
