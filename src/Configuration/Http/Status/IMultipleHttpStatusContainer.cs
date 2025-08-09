using System.Net;

namespace Server.Configuration
{
    public interface IMultipleHttpStatusContainer
    {
        void AddOrUpdateFromDictionary(Dictionary<string, HttpStatusCode> dictionary);
        int Status(string code);
    }
}
