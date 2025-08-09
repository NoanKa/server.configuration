using System;

namespace Server.Model
{
    public class BusinessException : Exception
    {
        public string Code { get; set; }

        public BusinessException(string code)
        {
            Code = code;
        }
    }
}
