namespace Server.Configuration
{
    public class AppMessageFactory : IAppMessageFactory
    {
        private readonly IMultipleStringLocalizer localizer;

        public AppMessageFactory(IMultipleStringLocalizer localizer)
        {
            this.localizer = localizer;
        }

        public string? Message(string code)
        {
            return localizer[code].ResourceNotFound ? null : localizer?[code].Value;
        }
    }

}
