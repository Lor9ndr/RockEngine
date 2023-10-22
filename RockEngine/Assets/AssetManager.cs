using Newtonsoft.Json;

using OpenTK.Mathematics;

using RockEngine.Engine;
using RockEngine.Engine.ECS;
using RockEngine.OpenGL.Settings;
using RockEngine.OpenGL.Shaders;
using RockEngine.OpenGL.Textures;
using RockEngine.OpenGL.Vertices;
using RockEngine.Utils;

using SkiaSharp;

using System.IO;

using RockEngine.Assets.JsonConverters;

namespace RockEngine.Assets
{
    public static class AssetManager
    {
        private const string EXTENSION_NAME = ".asset";

        public static List<IAsset> Assets = new List<IAsset>();
        private static readonly JsonConverter[ ] _converters = new JsonConverter[ ]
            {
                new Vector2JsonConverter(),
                new Vector2iJsonConverter(),
                new Vector3JsonConverter(),
                new Vector4JsonConverter(),
                new Matrix4JsonConverter(),
                new GameObjectJsonConverter(),
                new TransformConverter(),
                new QuaternionJsonConverter(),
                new SceneJsonConverter(),
                new MeshComponentJsonConverter()
            };
        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
        {
            Converters = _converters,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            Formatting = Formatting.Indented,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            TypeNameHandling = TypeNameHandling.Objects
        };


        public static IAsset? GetAssetByID(Guid id)
        {
            return Assets.FirstOrDefault(s => s.ID == id);
        }

        public static void LoadProject(string path)
        {
            Project.CurrentProject?.Dispose();
            foreach (var item in Assets)
            {
                if (item is IDisposable disp)
                {
                    disp.Dispose();
                }
            }
            Assets.Clear();
            LoadAssetFromFile<Project>(path);
            string assetPath = PathInfo.PROJECT_ASSETS_PATH;
            string[] assets = Directory.GetFiles(assetPath, "*.asset", SearchOption.AllDirectories);
            List<string> scenes = new List<string>();
            List<string> meshes = new List<string>();

            // Load textures
            foreach (var asset in assets)
            {
                var baseAsset = LoadAssetFromFile<BaseAsset>(asset);
                if (baseAsset.Type == AssetType.Texture)
                {
                    Assets.Add(LoadAssetFromFile<Texture>(asset));
                }
                else if (baseAsset.Type == AssetType.Material)
                {
                    Assets.Add(LoadAssetFromFile<MaterialComponent>(asset));
                }
                else if (baseAsset.Type == AssetType.Mesh)
                {
                    meshes.Add(asset);
                }
                else if (baseAsset.Type == AssetType.Texture2D)
                {
                    Assets.Add(LoadAssetFromFile<Texture2D>(asset));
                }
                else if (baseAsset.Type == AssetType.Scene)
                {
                    scenes.Add(asset);
                }
            }

            foreach (var meshPath in meshes)
            {
                Assets.Add(LoadAssetFromFile<MeshComponent>(meshPath));
            }

            // Load scenes
            foreach (var scenePath in scenes)
            {
                Assets.Add(LoadAssetFromFile<Scene>(scenePath));
            }
            Scene.ChangeScene(Assets.OfType<Scene>().First());
        }

        public static void SaveAll()
        {
            foreach (var item in Assets)
            {
                SaveAssetToFile(item);
            }
        }

        public static MaterialComponent CreateMaterialAsset(string path, string name = "Material")
        {
            var asset = new MaterialComponent(path, name);
            Assets.Add(asset);
            SaveAssetToFile(asset);
            return asset;
        }

        public static MeshComponent CreateMesh(ref int[] indices, ref Vertex3D[] vertices, string name = "Mesh", string path = "", Guid id = default)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = PathInfo.PROJECT_ASSETS_PATH;
            }
            if (id == default)
            {
                id = Guid.NewGuid();
            }
            var asset = new MeshComponent(ref vertices, ref indices, name, path, id);
            Assets.Add(asset);
            SaveAssetToFile(asset);
            return asset;
        }

        public static MeshComponent CreateMesh(ref Vertex3D[] vertices, string name = "Mesh", string path = "", Guid id = default)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = PathInfo.PROJECT_ASSETS_PATH;
            }
            if (id == default)
            {
                id = Guid.NewGuid();
            }
            var asset = new MeshComponent(ref vertices, name, path, id);
            Assets.Add(asset);
            SaveAssetToFile(asset);
            return asset;
        }

        public static Texture CreateTexture(Vector2i size, TextureSettings setting, string name, string path)
        {
            var asset = new Texture(size, setting, Guid.NewGuid(), path, name);
            Assets.Add(asset);
            SaveAssetToFile(asset);
            return asset;
        }

        public static T LoadAssetFromFile<T>(string filePath)
        {
            string json = File.ReadAllText(filePath);
            var obj = JsonConvert.DeserializeObject<T>(json, _jsonSettings);
            Validator.ThrowIfNull(obj);
            return obj!;
        }
        public static void SaveAssetToFile(string filePath, object objToSave)
        {
            string json = JsonConvert.SerializeObject(objToSave, _jsonSettings);
            File.WriteAllText(filePath, json);
        }
        public static void SaveAssetToFile(IAsset objToSave)
        {
            SaveAssetToFile(GetFilePath(objToSave), objToSave);
        }
        public static string GetFilePath(IAsset asset)
        {
            if (string.IsNullOrEmpty(asset.Path))
            {
                asset.Path = PathInfo.PROJECT_ASSETS_PATH;
            }
            var result = asset.Path + "\\" + asset.Name + EXTENSION_NAME;
            if (!Directory.Exists(asset.Path))
            {
                Directory.CreateDirectory(asset.Path);
            }
            return result;
        }

        public static Project CreateProject(string name, string assetPath, Guid id)
        {
            Project p = new Project(name, assetPath, id);
            return p;
        }
    }
}
