using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AccountManager.Common
{
    public class PatchConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var jToken = JToken.Load(reader);
            var type = objectType.GetGenericArguments()[0];
            var value = jToken.ToObject(type);

            var patch = Activator.CreateInstance(objectType, value, true);
            return patch;
        }

        public override bool CanConvert(Type objectType)
        {
            var result = objectType.IsGenericType &&
                         objectType.GetGenericTypeDefinition() == typeof(Patch<>);

            return result;
        }
    }
}