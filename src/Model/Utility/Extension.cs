using System.Net;

namespace Server.Model
{
    public static class Extension
    {
        public static int Status(this HttpStatusCode httpStatusCode)
        {
            return (int)httpStatusCode;
        }
    }
}
