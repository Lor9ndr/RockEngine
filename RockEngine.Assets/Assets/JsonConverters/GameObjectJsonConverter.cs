using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RockEngine.Engine.ECS;
using RockEngine.Engine.ECS.GameObjects;
using RockEngine.Utils;
using RockEngine.Assets.Assets;

namespace RockEngine.Assets.Assets.JsonConverters
{
    public class GameObjectJsonConverter : JsonConverter<GameObject>
    {
        public static Dictionary<int, object> _refHandlers = new Dictionary<int, object>();
        public override void WriteJson(JsonWriter writer, GameObject? value, JsonSerializer serializer)
        {
            Check.IsNull(value);
            // Create a JObject to hold the serialized properties
            var jsonObject = new JObject
            {
                // Serialize the properties of the GameObject
                { "GameObjectID", value.GameObjectID },
                { "Name", value.Name },
            };

            var componentsArray = new JArray();

            foreach(var component in value.GetComponents())
            {
                if(component is IAsset asset)
                {
                    // If the component is an IAsset, write its ID
                    var assetId = asset.ID;
                    componentsArray.Add(assetId);
                }
                else
                {
                    if(component is EngineRigidBody)
                    {
                        continue;
                    }
                    // If the component is not an IAsset, write the whole component object
                    var componentObject = JObject.FromObject(component, serializer);
                    componentsArray.Add(componentObject);
                }
            }
            jsonObject.Add("Components", componentsArray);

            // Write the JObject to the writer
            jsonObject.WriteTo(writer);
        }

        public override GameObject ReadJson(JsonReader reader, Type objectType, GameObject existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load the JObject from the reader
            var jsonObject = JObject.Load(reader);

            // Create a new GameObject instance
            var gameObject = new GameObject(jsonObject.GetValue("Name").Value<string>());

            // Deserialize the properties from the JObject
            gameObject.GameObjectID = jsonObject.GetValue("GameObjectID").Value<uint>();

            // Deserialize the parent GameObject if it exists
            if(jsonObject.TryGetValue("Parent", out var parentToken))
            {
                // TODO: Handle deserialization of the parent object
            }

            if(jsonObject.TryGetValue("Components", out var componentsToken))
            {

                var componentsArray = (JArray)componentsToken;
                if(componentsArray != null)
                {
                    foreach(var componentToken in componentsArray)
                    {
                        if(componentToken.Type == JTokenType.String)
                        {
                            // If the component is an asset ID, create a reference to the asset
                            var assetId = componentToken.ToObject<Guid>();
                            var assetReference = AssetManager.GetAssetByID(assetId);
                            if(assetReference is null)
                            {
                                Logger.AddLog($"Missing asset id:{assetId} of Gameobject: {gameObject.Name} with id {gameObject.GameObjectID}");
                            }
                            else
                            {
                                gameObject.AddComponent((IComponent)assetReference);
                            }
                        }
                        if(componentToken.Type == JTokenType.Object)
                        {
                            var type = componentToken["$type"].ToObject<Type>();
                            var component = (IComponent)serializer.Deserialize(componentToken.CreateReader(), type);
                            gameObject.AddComponent(component);
                        }
                    }
                }
            }
            return gameObject;
        }
    }
}
