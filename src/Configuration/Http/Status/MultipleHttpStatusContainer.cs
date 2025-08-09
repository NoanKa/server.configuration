using System.Net;

namespace Server.Configuration
{
    public class MultipleHttpStatusContainer : IMultipleHttpStatusContainer
    {
        private readonly Dictionary<string, HttpStatusCode> statusCodes;

        public MultipleHttpStatusContainer(Dictionary<string, HttpStatusCode> statusCodes)
        {
            this.statusCodes = statusCodes;
        }

        public void AddOrUpdateFromDictionary(Dictionary<string, HttpStatusCode> dictionary)
        {
            foreach (var status in dictionary)
            {
                if (statusCodes.ContainsKey(status.Key))
                {
                    statusCodes[status.Key] = status.Value;
                }
                else
                {
                    statusCodes.Add(status.Key, status.Value);
                }
            }
        }

        public int Status(string code)
        {
            if(statusCodes.TryGetValue(code, out var value))
            {
                return (int)value;
            }

            return (int)HttpStatusCode.BadRequest;
        }
    }
}
