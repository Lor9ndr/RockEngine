using RockEngine.Common.Utils;
using RockEngine.ECS.Assets;
using RockEngine.ECS.GameObjects;
using RockEngine.Renderers;
using RockEngine.Rendering;

using System.Collections;

namespace RockEngine.ECS
{
    public class Scene : BaseAsset, IDisposable, IEnumerable<GameObject>
    {
        private static readonly SceneRenderer _meshGroupRenderer = new SceneRenderer();
        /// <summary>
        /// Static property to track of the current scene
        /// </summary>
        public static Scene? CurrentScene { get; private set; }

        public static GameObject? MainCamera { get; private set; }

        /// <summary>
        /// List of game objects in the scene
        /// </summary>
        private readonly List<GameObject> _gameObjects;
        private readonly Dictionary<Mesh,Dictionary<Material, List<GameObject>>> _groupedGameObjects;
        private readonly List<GameObject> _unGroupedObjects = new List<GameObject>();
        #region Ctor
        public Scene(string name, string path, Guid id)
            : base(path, name, id, AssetType.Scene)
        {
            Name = name;
            _gameObjects = new List<GameObject>();
            _groupedGameObjects = new Dictionary<Mesh, Dictionary<Material, List<GameObject>>>();
        }
        public Scene(string path, string name, Guid id, AssetType type, List<GameObject> gameObjects)
            : base(path, name, id, type)
        {
            _gameObjects = new List<GameObject>();
            _groupedGameObjects = new Dictionary<Mesh, Dictionary<Material, List<GameObject>>>();
            foreach(var item in gameObjects)
            {
                Add(item);
            }
        }

        #endregion

        /// <summary>
        /// Method to change the current scene
        /// </summary>
        /// <param name="newScene">the new scene on which we are changing</param>
        public static void ChangeScene(Scene? newScene)
        {
            Check.IsNull(newScene);
            CurrentScene?.Stop();
            CurrentScene = newScene;
            CurrentScene!.Start();
        }

        private void Start()
        {
            foreach(var item in this)
            {
                item.OnStart();
            }
        }
        private void Stop()
        {
            for(int i = 0; i < _gameObjects.Count; i++)
            {
                var item = _gameObjects[i];
                item.Dispose();
            }
        }

        /// <summary>
        /// Method to add a game object to the scene
        /// </summary>
        /// <param name="gameObject">gameobject which to add to scene</param>
        public Scene Add(GameObject gameObject)
        {
            TakeNameForGameObject(gameObject);

            FillGameObjectGroups(gameObject);

            Logger.AddLog($"Created gameobject with name: {gameObject.Name}");

            return this;
        }

        private void TakeNameForGameObject(GameObject gameObject)
        {
            // Utilize a HashSet for quick lookups of existing names
            var existingNames = new HashSet<string>(_gameObjects.Select(go => go.Name));

            // Check if the initial name of the new game object is already in use
            if(existingNames.Contains(gameObject.Name))
            {
                int count = 1;
                string originalName = gameObject.Name;

                // Efficiently generate a unique name by appending a number
                // Avoid re-checking names that have already been generated
                string newName;
                do
                {
                    newName = $"{originalName} ({count++})";
                } while(existingNames.Contains(newName));
                gameObject.Name = newName;
            }
            // Add the gameObject to the main collection and the existingNames set
            _gameObjects.Add(gameObject);
            existingNames.Add(gameObject.Name); // This ensures the set is up-to-date without reselecting from _gameObjects
        }

        private void FillGameObjectGroups(GameObject gameObject)
        {
           
            // Assign MainCamera if it's null and the gameObject has a Camera component
            if(MainCamera is null && gameObject.GetComponent<Camera>() is not null)
            {
                MainCamera = gameObject;
            }
            // Get the Mesh and Material from the GameObject
            MeshComponent? mesh = gameObject.GetComponent<MeshComponent>();
            MaterialComponent? material = gameObject.GetComponent<MaterialComponent>();

            if(mesh is null || material is null)
            {
                _unGroupedObjects.Add(gameObject);
                Logger.AddLog($"Created gameobject with name: {gameObject.Name}");
                return;
            }
            // If the key doesn't exist in the dictionary, add it
            if(!_groupedGameObjects.TryGetValue(mesh!.Mesh!, out Dictionary<Material, List<GameObject>>? matGroup))
            {
                matGroup = new Dictionary<Material, List<GameObject>>();
                _groupedGameObjects.Add(mesh!.Mesh!, matGroup);
            }
            if(!matGroup.TryGetValue(material.Material, out List<GameObject>? value))
            {
                value = new List<GameObject>();
                matGroup[material.Material] = value;
            }
            matGroup[material.Material].Add(gameObject);
        }

        /// <summary>
        /// Method to add a child game object to a parent game object in the scene
        /// </summary>
        /// <param name="parent">Parent gameobject to which we are attaching new child</param>
        /// <param name="child">Child gameobject which we have to attach to <paramref name="parent"/></param>
        public void AddChild(GameObject parent, GameObject child)
        {
            parent.AddChild(child);
        }

        /// <summary>
        /// Removing child from parent
        /// </summary>
        /// <param name="parent">gameobject from we have to remove <paramref name="child"/></param>
        /// <param name="child">gameobject which we have to remove from <paramref name="parent"/></param>
        public void RemoveChild(GameObject parent, GameObject child)
        {
            parent.RemoveChild(child);
        }

        /// <summary>
        /// Method to get all game objects in the scene
        /// </summary>
        /// <returns>List of gameobjects</returns>
        public List<GameObject> GetGameObjects() => _gameObjects;

        public Dictionary<Mesh, Dictionary<Material, List<GameObject>>> GetGroupedGameObjects() => _groupedGameObjects;
        public List<GameObject> GetUngroupedGameObjects() => _unGroupedObjects;

        public void Render(IRenderingContext context)
        {
            RenderCamera(context);
            RenderObjectsOnly(context);
        }

        private void RenderObjectsOnly(IRenderingContext context)
        {
            _meshGroupRenderer.Render(context, this);
           
            foreach(var item in _unGroupedObjects)
            {
                if(item == MainCamera)
                {
                    continue;
                }
                item.Render(context);
            }
        }

        /// <summary>
        /// Default render without camera
        /// </summary>
        public void EditorLayerRender(IRenderingContext context)
        {
            RenderObjectsOnly(context);
        }

        public void RenderCamera(IRenderingContext context)
        {
            MainCamera?.Render(context);
        }

        public void Update()
        {
            _meshGroupRenderer.Update(this);
        }

        public void Dispose()
        {
            for(int i = 0; i < _gameObjects.Count; i++)
            {
                GameObject? item = _gameObjects[i];
                item.Dispose();
            }
            _gameObjects.Clear();

            GC.SuppressFinalize(this);
        }

        public IEnumerator<GameObject> GetEnumerator() => _gameObjects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
