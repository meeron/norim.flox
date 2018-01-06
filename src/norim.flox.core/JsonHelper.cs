using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace norim.flox.core
{
    public static class JsonHelper
    {
        public static string SerializeCamelCase(object value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings 
            { 
                ContractResolver = new CamelCasePropertyNamesContractResolver() 
            });
        }
    }
}