using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using norim.flox.web.Models;

namespace norim.flox.web.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;            
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

                if (context.Response.StatusCode == 404)
                    await WriteResponse(context.Response,
                        new ErrorResponse(context.TraceIdentifier, $"Resource '{context.Request.Path}' not found."), 404);
            }
            catch (System.Exception ex)
            {
                await WriteResponse(context.Response,
                    new ErrorResponse(context.TraceIdentifier, ex.Message), 500);
            }
        }

        private static async Task WriteResponse(HttpResponse response, ErrorResponse data, int statusCode)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var encoding = Encoding.UTF8;
            var jsonData = encoding.GetBytes(json);

            response.StatusCode = statusCode;
            response.ContentLength = jsonData.LongLength;
            response.ContentType = "application/json";

            await response.WriteAsync(json, encoding);
        }
    }
}