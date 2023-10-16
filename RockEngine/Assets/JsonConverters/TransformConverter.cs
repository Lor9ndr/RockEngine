using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using RockEngine.Engine.ECS;

namespace RockEngine.Assets.JsonConverters
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
            // Load the JObject from the reader
            var jsonObject = JObject.Load(reader);

            // Create a new Transform instance
            var transform = new Transform();

            // Deserialize the properties from the JObject
            transform.Rotation = jsonObject.GetValue("Rotation").ToObject<Vector3>(serializer);
            transform.Position = jsonObject.GetValue("Position").ToObject<Vector3>(serializer);
            transform.Scale = jsonObject.GetValue("Scale").ToObject<Vector3>(serializer);

            return transform;
        }
    }
}
