using Newtonsoft.Json;

using RockEngine.Common;
using RockEngine.Common.Utils;
using RockEngine.Common.Vertices;
using RockEngine.ECS.Assets.JsonConverters;
using RockEngine.Rendering;
using RockEngine.Rendering.OpenGL.Shaders;
using RockEngine.Rendering.OpenGL.Textures;

using System.Collections.Concurrent;
using System.Threading;

namespace RockEngine.ECS.Assets
{
    public static class AssetManager
    {
        public const string EXTENSION_NAME = ".asset";

        public static List<IAsset> Assets = new List<IAsset>();
        private static bool _isSaving;
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
        };
        private static readonly ConcurrentQueue<IAsset> _assetsToLoadInOpenGL = new ConcurrentQueue<IAsset>();

        public static IAsset? GetAssetByID(Guid id)
        {
            return Assets.FirstOrDefault(s => s.ID == id);
        }

        public static async Task LoadProject(string path)
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
            var project = await LoadAssetFromFileAsync<Project>(path);
            string assetPath = PathsInfo.PROJECT_ASSETS_PATH;

            List<string> scenes = new List<string>();
            List<string> meshes = new List<string>();

            // Load textures
            foreach(var asset in Directory.EnumerateFiles(assetPath, "*.asset", SearchOption.AllDirectories))
            {
                var baseAsset = await LoadAssetFromFileAsync<BaseAsset>(asset);
                if(baseAsset.Type == AssetType.Texture)
                {
                    Assets.Add(await LoadAssetFromFileAsync<TextureAsset>(asset));
                }
                else if(baseAsset.Type == AssetType.Material)
                {
                    Assets.Add(await LoadAssetFromFileAsync<Material>(asset));
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
                Assets.Add(await LoadAssetFromFileAsync<Mesh>(meshPath));
            }

            // Load scenes
            foreach(var scenePath in scenes)
            {
                Assets.Add(await LoadAssetFromFileAsync<Scene>(scenePath));
            }
            //Scene.ChangeScene(project.FirstScene);
        }

        public static async Task SaveAll()
        {
            if(_isSaving)
            {
                return;
            }
            _isSaving = true;
            var tsks = new List<Task>(Assets.Count);
            foreach(var item in Assets)
            {
                tsks.Add(SaveAssetToFileAsync(item));
            }
            await Task.WhenAll(tsks);
            _isSaving = false;
        }

        public static async Task<Material> CreateMaterialAssetAsync(AShaderProgram shader, string path, string name = "Material", CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var asset = new Material(shader, path, name, Guid.NewGuid());
            Assets.Add(asset);
            await SaveAssetToFileAsync(asset, cancellationToken);
            _assetsToLoadInOpenGL.Enqueue(asset);
            return asset;
        }

        public static async Task<Mesh> CreateMesh(int[] indices, Vertex3D[ ] vertices, string name = "Mesh", string path = "", Guid id = default, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

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
            await SaveAssetToFileAsync(asset, cancellationToken);
            _assetsToLoadInOpenGL.Enqueue(asset);

            return asset;
        }

        public static async Task<Mesh> CreateMeshAsync(Vertex3D[ ] vertices, string name = "Mesh", string path = "", Guid id = default, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

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
            await SaveAssetToFileAsync(asset, cancellationToken);
            _assetsToLoadInOpenGL.Enqueue(asset);
            return asset;
        }

        public static async Task<TextureAsset> CreateTextureAsync(string name, string path, BaseTexture texture, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var asset = new TextureAsset(path, name, Guid.NewGuid(), AssetType.Texture, texture);
            Assets.Add(asset);
            await SaveAssetToFileAsync(asset, cancellationToken);
            _assetsToLoadInOpenGL.Enqueue(asset);
            return asset;
        }

        public static async Task<T> LoadAssetFromFileAsync<T>(string filePath, CancellationToken cancellationToken = default) where T : BaseAsset
        {
            cancellationToken.ThrowIfCancellationRequested();

            string json = await File.ReadAllTextAsync(filePath, cancellationToken);
            var obj = JsonConvert.DeserializeObject<T>(json, _jsonSettings);
            Check.IsNull(obj);
            _assetsToLoadInOpenGL.Enqueue(obj);
            return obj!;
        }

        public static async Task SaveAssetToFile<T>(string filePath, T objToSave, CancellationToken cancellationToken = default) where T:IAsset
        {
            cancellationToken.ThrowIfCancellationRequested();
            string jsonString = JsonConvert.SerializeObject(objToSave, _jsonSettings);

            // Write the JSON string to file asynchronously
            // Since Newtonsoft.Json does not provide an asynchronous method for writing to a file directly,
            // we use StreamWriter with an asynchronous method here.
            using var streamWriter = new StreamWriter(filePath);
            await streamWriter.WriteAsync(jsonString).ConfigureAwait(false);
        }

        public static Task SaveAssetToFileAsync(IAsset objToSave, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return SaveAssetToFile(GetFilePath(objToSave), objToSave, cancellationToken);
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

        public static async Task<Project> CreateProjectAsync(string name, string assetPath, Guid id, Scene firstScne, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Project p = new Project(name, assetPath, id, firstScne);
            await SaveAssetToFileAsync(p, cancellationToken);
            Assets.Add(p);
            return p;
        }

        public static IAsset? GetAssetByPath(string fullName)
            => Assets.FirstOrDefault(s => Path.GetFullPath(GetFilePath(s)) == Path.GetFullPath(fullName));

        public static async Task<Scene> CreateSceneAsync(string name, string assetPath, Guid id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Scene scene = new Scene(name, assetPath, id);
            await SaveAssetToFileAsync(scene, cancellationToken);
            Assets.Add(scene);
            return scene;
        }

        public static void LoadAssetsToOpenGL()
        {
            IRenderingContext.Update(context =>
            {
                while(!_assetsToLoadInOpenGL.IsEmpty)
                {
                    if(_assetsToLoadInOpenGL.TryDequeue(out var asset))
                    {
                        asset.Loaded();
                    }
                }
            });
           
        }
    }
}
