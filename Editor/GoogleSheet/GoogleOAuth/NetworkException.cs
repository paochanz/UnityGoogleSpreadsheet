using System;

namespace Editor.GoogleSheet.GoogleOAuth
{
    public class NetworkException : Exception
    {
        public long StatusCode;

        public NetworkException(string message, long statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
