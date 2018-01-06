using Microsoft.AspNetCore.Http;

namespace norim.flox.web.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetRequestId(this HttpContext context)
        {
            return context.Items["RequestId"].ToString();
        }
    }
}