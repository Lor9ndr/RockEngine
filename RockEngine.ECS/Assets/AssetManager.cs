﻿using Newtonsoft.Json;

using RockEngine.Common.Utils;
using RockEngine.Common.Vertices;
using RockEngine.ECS.Assets.JsonConverters;
using RockEngine.Rendering.OpenGL.Shaders;
using RockEngine.Rendering.OpenGL.Textures;

namespace RockEngine.ECS.Assets
{
    public static class AssetManager
    {
        public const string EXTENSION_NAME = ".asset";

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
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
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
            foreach(var item in Assets)
            {
                if(item is IDisposable disp)
                {
                    disp.Dispose();
                }
            }
            Assets.Clear();
            var project = LoadAssetFromFile<Project>(path);
            string assetPath = PathsInfo.PROJECT_ASSETS_PATH;

            List<string> scenes = new List<string>();
            List<string> meshes = new List<string>();

            // Load textures
            foreach(var asset in Directory.EnumerateFiles(assetPath, "*.asset", SearchOption.AllDirectories))
            {
                var baseAsset = LoadAssetFromFile<BaseAsset>(asset);
                if(baseAsset.Type == AssetType.Texture)
                {
                    Assets.Add(LoadAssetFromFile<TextureAsset>(asset));
                }
                else if(baseAsset.Type == AssetType.Material)
                {
                    Assets.Add(LoadAssetFromFile<Material>(asset));
                }
                else if(baseAsset.Type == AssetType.Mesh)
                {
                    meshes.Add(asset);
                }
                else if(baseAsset.Type == AssetType.Scene)
                {
                    scenes.Add(asset);
                }
            }

            foreach(var meshPath in meshes)
            {
                Assets.Add(LoadAssetFromFile<Mesh>(meshPath));
            }

            // Load scenes
            foreach(var scenePath in scenes)
            {
                Assets.Add(LoadAssetFromFile<Scene>(scenePath));
            }
            //Scene.ChangeScene(project.FirstScene);
        }

        public static void SaveAll()
        {
            foreach(var item in Assets)
            {
                SaveAssetToFile(item);
            }
        }

        public static Material CreateMaterialAsset(AShaderProgram shader, string path, string name = "Material")
        {
            var asset = new Material(shader, path, name, Guid.NewGuid());
            Assets.Add(asset);
            SaveAssetToFile(asset);
            return asset;
        }

        public static Mesh CreateMesh(ref int[ ] indices, ref Vertex3D[ ] vertices, string name = "Mesh", string path = "", Guid id = default)
        {
            if(string.IsNullOrEmpty(path))
            {
                path = PathsInfo.PROJECT_ASSETS_PATH;
            }
            if(id == default)
            {
                id = Guid.NewGuid();
            }
            var asset = new Mesh(ref vertices, ref indices, name, path, id);
            Assets.Add(asset);
            SaveAssetToFile(asset);
            return asset;
        }

        public static Mesh CreateMesh(ref Vertex3D[ ] vertices, string name = "Mesh", string path = "", Guid id = default)
        {
            if(string.IsNullOrEmpty(path))
            {
                path = PathsInfo.PROJECT_ASSETS_PATH;
            }
            if(id == default)
            {
                id = Guid.NewGuid();
            }
            var asset = new Mesh(ref vertices, name, path, id);
            Assets.Add(asset);
            SaveAssetToFile(asset);
            return asset;
        }

        public static TextureAsset CreateTexture(string name, string path, BaseTexture texture)
        {
            var asset = new TextureAsset(path, name, Guid.NewGuid(), AssetType.Texture, texture);
            Assets.Add(asset);
            SaveAssetToFile(asset);
            return asset;
        }

        public static T LoadAssetFromFile<T>(string filePath) where T : BaseAsset
        {
            string json = File.ReadAllText(filePath);
            var obj = JsonConvert.DeserializeObject<T>(json, _jsonSettings);
            Check.IsNull(obj);
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
            if(asset is null)
            {
                return string.Empty;
            }
            if(string.IsNullOrEmpty(asset.Path))
            {
                asset.Path = PathsInfo.PROJECT_ASSETS_PATH;
            }
            var result = asset.Path + "\\" + asset.Name + EXTENSION_NAME;
            if(!Directory.Exists(asset.Path))
            {
                Directory.CreateDirectory(asset.Path);
            }
            return result;
        }

        public static Project CreateProject(string name, string assetPath, Guid id, Scene firstScne)
        {
            Project p = new Project(name, assetPath, id, firstScne);
            SaveAssetToFile(p);
            Assets.Add(p);
            return p;
        }

        public static IAsset? GetAssetByPath(string fullName)
            => Assets.FirstOrDefault(s => Path.GetFullPath(GetFilePath(s)) == Path.GetFullPath(fullName));

        public static Scene CreateScene(string name, string assetPath, Guid id)
        {
            Scene scene = new Scene(name, assetPath, id);
            SaveAssetToFile(scene);
            Assets.Add(scene);
            return scene;
        }
    }
}