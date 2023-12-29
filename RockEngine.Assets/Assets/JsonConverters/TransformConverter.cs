using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using RockEngine.Engine.ECS;

namespace RockEngine.Assets.Assets.JsonConverters
{
    public class TransformConverter : JsonConverter<Transform>
    {
        public override void WriteJson(JsonWriter writer, Transform value, JsonSerializer serializer)
        {
            // Create a JObject to hold the serialized properties
            var jsonObject = new JObject
            {
                // Serialize the properties of the Transform
                { "Rotation", JToken.FromObject(value.Rotation, serializer) },
                { "Position", JToken.FromObject(value.Position, serializer) },
                { "Scale", JToken.FromObject(value.Scale, serializer) },
                { "$type", JToken.FromObject(typeof(Transform)) }
            };

            // Write the JObject to the writer
            jsonObject.WriteTo(writer);
        }

        public override Transform ReadJson(JsonReader reader, Type objectType, Transform existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load the JSON object from the reader
            JObject jsonObject = JObject.Load(reader);



            // Extract the properties from the JSON object
            JToken rotationToken = jsonObject["Rotation"];
            JToken positionToken = jsonObject["Position"];
            JToken scaleToken = jsonObject["Scale"];

            // Deserialize the properties using the provided serializer
            Vector3 rotation = rotationToken.ToObject<Vector3>(serializer);
            Vector3 position = positionToken.ToObject<Vector3>(serializer);
            Vector3 scale = scaleToken.ToObject<Vector3>(serializer);

            // Create a new Transform object with the deserialized properties
            Transform transform = new Transform(position, rotation, scale);

            return transform;
        }
    }
}
