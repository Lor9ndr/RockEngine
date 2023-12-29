using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using OpenTK.Mathematics;

namespace RockEngine.ECS.Assets.JsonConverters
{
    internal sealed class Vector2JsonConverter : JsonConverter<Vector2>
    {
        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load the JSON object
            JObject jsonObject = JObject.Load(reader);

            // Extract the X and Y values from the JSON object
            float x = (float)jsonObject["X"];
            float y = (float)jsonObject["Y"];

            // Create and return a new Vector2 object
            return new Vector2(x, y);
        }

        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            // Create a JSON object with the X and Y values
            JObject jsonObject = new JObject();
            jsonObject["X"] = value.X;
            jsonObject["Y"] = value.Y;

            // Write the JSON object to the writer
            jsonObject.WriteTo(writer);
        }
    }
}
