using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RockEngine.Assets.Assets;
using RockEngine.Engine;
using RockEngine.Engine.ECS.GameObjects;

namespace RockEngine.Assets.Assets.JsonConverters
{
    internal sealed class SceneJsonConverter : JsonConverter<Scene>
    {
        public override Scene ReadJson(JsonReader reader, Type objectType, Scene existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            // Extract the properties from the JSON object
            string name = jsonObject["Name"]?.Value<string>() ?? string.Empty;
            string path = jsonObject["Path"]?.Value<string>() ?? string.Empty;
            Guid id = jsonObject["ID"]?.ToObject<Guid>() ?? Guid.Empty;
            AssetType type = AssetType.Scene;
            List<GameObject> gameObjects = jsonObject["GameObjects"]?.ToObject<List<GameObject>>(serializer) ?? new List<GameObject>();

            // Create a new Scene object with the extracted properties
            Scene scene = new Scene(path, name, id, type, gameObjects);

            return scene;
        }

        public override void WriteJson(JsonWriter writer, Scene value, JsonSerializer serializer)
        {
            // Create a JSON object and populate it with the properties from the Scene object
            JObject jsonObject = new JObject();
            jsonObject["Name"] = value.Name;
            jsonObject["Path"] = value.Path;
            jsonObject["ID"] = value.ID;
            jsonObject["Type"] = (int)value.Type;
            var gameobjects = value.GetGameObjects();
            if(gameobjects is null)
            {
                gameobjects = new List<GameObject>();
            }
            jsonObject["GameObjects"] = JToken.FromObject(gameobjects, serializer);

            // Write the JSON object to the writer
            jsonObject.WriteTo(writer);
        }
    }
}
