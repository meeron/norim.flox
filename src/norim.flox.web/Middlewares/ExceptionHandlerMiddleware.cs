using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace norim.flox.web.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;            
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (System.Exception ex)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    RequestId = context.TraceIdentifier,
                    ServerTimeUTC = DateTime.UtcNow.ToString("o"),
                    Message = ex.Message
                });

                var encoding = Encoding.UTF8;
                var data = encoding.GetBytes(json);

                context.Response.StatusCode = 500;
                context.Response.ContentLength = data.LongLength;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(json, encoding);
            }
        }
    }
}