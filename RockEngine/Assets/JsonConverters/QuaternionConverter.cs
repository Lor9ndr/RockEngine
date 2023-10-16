using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OpenTK.Mathematics;

namespace RockEngine.Assets.JsonConverters
{
    public class QuaternionJsonConverter : JsonConverter<Quaternion>
    {
        public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
        {
            // Create a JObject to hold the serialized properties
            var jsonObject = new JObject
            {
                // Serialize the Quaternion properties
                { "X", value.X },
                { "Y", value.Y },
                { "Z", value.Z },
                { "W", value.W }
            };

            // Write the JObject to the writer
            jsonObject.WriteTo(writer);
        }

        public override Quaternion ReadJson(JsonReader reader, Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load the JObject from the reader
            var jsonObject = JObject.Load(reader);

            // Read the Quaternion properties from the JObject
            var x = jsonObject.GetValue("X").ToObject<float>();
            var y = jsonObject.GetValue("Y").ToObject<float>();
            var z = jsonObject.GetValue("Z").ToObject<float>();
            var w = jsonObject.GetValue("W").ToObject<float>();

            // Create a new Quaternion instance
            var quaternion = new Quaternion(x, y, z, w);

            return quaternion;
        }
    }
}
