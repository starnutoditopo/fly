using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fly.Models.Nominatim
{
    internal class JsonSerializationHelper
    {
        public static JsonSerializerOptions JsonSerializerOptions { get; }

        static JsonSerializationHelper()
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
            var converter = new JsonStringEnumConverter();
            options.Converters.Add(converter);
            JsonSerializerOptions = options;
        }
    }
}
