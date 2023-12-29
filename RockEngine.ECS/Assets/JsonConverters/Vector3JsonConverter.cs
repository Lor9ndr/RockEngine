using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OpenTK.Mathematics;

namespace RockEngine.ECS.Assets.JsonConverters
{
    internal sealed class Vector3JsonConverter : JsonConverter<Vector3>
    {
        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load the JSON object
            JObject jsonObject = JObject.Load(reader);

            // Extract the X, Y, and Z values from the JSON object
            float x = (float)jsonObject["X"];
            float y = (float)jsonObject["Y"];
            float z = (float)jsonObject["Z"];

            // Create and return a new Vector3 object
            return new Vector3(x, y, z);
        }

        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            // Create a JSON object with the X, Y, and Z values
            JObject jsonObject = new JObject();
            jsonObject["X"] = value.X;
            jsonObject["Y"] = value.Y;
            jsonObject["Z"] = value.Z;

            // Write the JSON object to the writer
            jsonObject.WriteTo(writer);
        }
    }
}
