using System;

namespace norim.flox.web.Models
{
    public class ErrorResponse : OkResponse
    {
        public ErrorResponse(string requestId, string msg)
            : base(requestId)
        {
            Message = msg;
        }

        public string Message { get; }
    }
}