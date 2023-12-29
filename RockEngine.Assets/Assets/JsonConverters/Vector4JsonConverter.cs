using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OpenTK.Mathematics;

namespace RockEngine.Assets.Assets.JsonConverters
{
    internal sealed class Vector4JsonConverter : JsonConverter<Vector4>
    {
        public override Vector4 ReadJson(JsonReader reader, Type objectType, Vector4 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load the JSON object
            JObject jsonObject = JObject.Load(reader);

            // Extract the X, Y, Z, and W values from the JSON object
            float x = (float)jsonObject["X"];
            float y = (float)jsonObject["Y"];
            float z = (float)jsonObject["Z"];
            float w = (float)jsonObject["W"];

            // Create and return a new Vector4 object
            return new Vector4(x, y, z, w);
        }

        public override void WriteJson(JsonWriter writer, Vector4 value, JsonSerializer serializer)
        {
            // Create a JSON object with the X, Y, Z, and W values
            JObject jsonObject = new JObject();
            jsonObject["X"] = value.X;
            jsonObject["Y"] = value.Y;
            jsonObject["Z"] = value.Z;
            jsonObject["W"] = value.W;

            // Write the JSON object to the writer
            jsonObject.WriteTo(writer);
        }
    }
}
