using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using norim.flox.web.Models;
using norim.flox.web.Extensions;
using norim.flox.core;

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
                        new ErrorResponse(context.GetRequestId(), $"Resource '{context.Request.Path}' not found."), 404);
            }
            catch (System.Exception ex)
            {
                await WriteResponse(context.Response,
                    new ErrorResponse(context.GetRequestId(), ex.Message), 500);
            }
        }

        private static async Task WriteResponse(HttpResponse response, ErrorResponse data, int statusCode)
        {
            var json = JsonHelper.SerializeCamelCase(data);
            response.StatusCode = statusCode;
            response.ContentType = "application/json";
            response.ContentLength = Encoding.UTF8.GetBytes(json).LongLength; 

            await response.WriteAsync(json);
        }
    }
}