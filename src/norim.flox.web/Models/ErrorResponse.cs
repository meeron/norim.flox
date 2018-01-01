using System;

namespace norim.flox.web.Models
{
    public class ErrorResponse
    {
        public ErrorResponse(string requestId, string msg)
        {
            RequestId = requestId;
            Message = msg;
        }

        public string RequestId { get; }

        public string ServerTimeUTC => DateTime.UtcNow.ToString("o");

        public string Message { get; }
    }
}