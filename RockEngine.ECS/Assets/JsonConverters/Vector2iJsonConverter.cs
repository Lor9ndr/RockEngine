using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using OpenTK.Mathematics;

namespace RockEngine.ECS.Assets.JsonConverters
{
    internal sealed class Vector2iJsonConverter : JsonConverter<Vector2i>
    {
        public override Vector2i ReadJson(JsonReader reader, Type objectType, Vector2i existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load the JSON object
            JObject jsonObject = JObject.Load(reader);

            // Extract the X and Y values from the JSON object
            int x = (int)jsonObject["X"];
            int y = (int)jsonObject["Y"];

            // Create and return a new Vector2i object
            return new Vector2i(x, y);
        }

        public override void WriteJson(JsonWriter writer, Vector2i value, JsonSerializer serializer)
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
