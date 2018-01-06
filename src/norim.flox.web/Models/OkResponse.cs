using System;

namespace norim.flox.web.Models
{
    public class OkResponse
    {
        public OkResponse(string requesteId)
        {
            RequestId = requesteId;
        }

        public string RequestId { get; }

        public string ServerTimeUtc => DateTime.UtcNow.ToString("o");
    }
}