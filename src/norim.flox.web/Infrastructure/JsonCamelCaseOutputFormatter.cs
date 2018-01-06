using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using norim.flox.core;

namespace norim.flox.web.Infrastructure
{
    public class JsonCamelCaseOutputFormatter : OutputFormatter
    {
        public JsonCamelCaseOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));
        }

        protected override bool CanWriteType(System.Type type)
        {
            return true;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var response = context.HttpContext.Response;
            var json = JsonHelper.SerializeCamelCase(context.Object);

            response.ContentLength = Encoding.UTF8.GetBytes(json).LongLength;
            await response.WriteAsync(json);
        }
    }
}