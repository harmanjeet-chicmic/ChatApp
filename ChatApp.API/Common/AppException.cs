using System;

namespace ChatApp.Application.Common
{
    /// <summary>
    /// Base exception type for all controlled business errors.
    /// These are safe to expose to the client.
    /// </summary>
    public class AppException : Exception
    {
        public string Code { get; }
        public int StatusCode { get; }

        public AppException(string code, string message, int statusCode = 400)
            : base(message)
        {
            Code = code;
            StatusCode = statusCode;
        }
    }
}
