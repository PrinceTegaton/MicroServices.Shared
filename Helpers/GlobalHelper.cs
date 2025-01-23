using Newtonsoft.Json;
using System;

namespace MicroServices
{
    public static class GlobalHelper
    {
        public static string FormatDate(this DateTime date, string format = null)
        {
            return date.ToString(format ?? "dd-MMM-yyyy");
        }

        public static string FormatDate(this DateTime? date, string format = null)
        {
            return date.HasValue ? date.Value.ToString(format ?? "dd-MMM-yyyy") : null;
        }

        public static string ToJson(this object value, bool ignoreNull = false, JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (value == null)
            {
                return "{ }";
            }

            jsonSerializerSettings ??= new JsonSerializerSettings
            {
                NullValueHandling = ignoreNull ? NullValueHandling.Ignore : NullValueHandling.Include,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(value, jsonSerializerSettings);
        }

        public static T FromJson<T>(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}