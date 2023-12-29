using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using OpenTK.Mathematics;
using RockEngine.Assets.Assets;
using RockEngine.Engine.ECS;
using RockEngine.OpenGL;
using RockEngine.OpenGL.Vertices;

namespace RockEngine.Assets.Assets.JsonConverters
{
    internal sealed class MeshComponentJsonConverter : JsonConverter<Mesh>
    {
        private const string POSITION_FLAG = "ps";
        private const string NORMAL_FLAG = "nml";
        private const string TEXCOORDS_FLAG = "tx";
        public override Mesh? ReadJson(JsonReader reader, Type objectType, Mesh? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load the JSON object
            JObject jsonObject = JObject.Load(reader);
            var name = jsonObject.Value<string>("Name");
            var id = jsonObject["ID"].ToObject<Guid>();
            var path = jsonObject.Value<string>("Path");
            var type = jsonObject["Type"].ToObject<AssetType>();

            JArray verticesArray = (JArray)jsonObject["Vertices"];

            Vertex3D[ ] vertices = verticesArray.Select(v => new Vertex3D(v[POSITION_FLAG].ToObject<Vector3>(), v[NORMAL_FLAG].ToObject<Vector3>(), v[TEXCOORDS_FLAG].ToObject<Vector2>())).ToArray();
            var indices = jsonObject.GetValue("Indices")?.ToObject<int[ ]>();
            bool hasIndices = indices is not null;
            if(hasIndices)
            {
                return new Mesh(ref vertices, ref indices, name, path, id);
            }
            else
            {
                return new Mesh(ref vertices, name, path, id);
            }
        }

        public override void WriteJson(JsonWriter writer, Mesh? value, JsonSerializer serializer)
        {
            if(value is null)
            {
                writer.WriteNull();
                return;
            }

            JObject jsonObject = new JObject();
            jsonObject["Name"] = value.Name;
            jsonObject["Path"] = value.Path;
            jsonObject["ID"] = value.ID;
            jsonObject["Type"] = (int)value.Type;

            // Add vertices
            JArray verticesArray = new JArray();
            foreach(var vertex in value.Vertices)
            {
                JObject vertexObject = new JObject();
                vertexObject[POSITION_FLAG] = JToken.FromObject(vertex.Position, serializer);
                vertexObject[NORMAL_FLAG] = JToken.FromObject(vertex.Normal, serializer);
                vertexObject[TEXCOORDS_FLAG] = JToken.FromObject(vertex.TexCoords, serializer);
                verticesArray.Add(vertexObject);
            }
            jsonObject["Vertices"] = verticesArray;

            // Add indices
            if(value.Indices is not null)
            {
                JArray indicesArray = new JArray(value.Indices);
                jsonObject["Indices"] = indicesArray;
            }

            jsonObject.WriteTo(writer);
        }
    }
}
