using Newtonsoft.Json;

using RockEngine.Assets;
using RockEngine.Engine.ECS.GameObjects;
using RockEngine.Utils;

using System.Collections;

namespace RockEngine.Engine
{
    public class Scene : BaseAsset, IDisposable, IEnumerable<GameObject>
    {
        /// <summary>
        /// Static property to track of the current scene
        /// </summary>
        public static Scene? CurrentScene { get; private set; }

        public static GameObject? MainCamera { get; private set; }

        /// <summary>
        /// List of game objects in the scene
        /// </summary>
        [JsonRequired]
        private readonly List<GameObject> _gameObjects;

        #region Ctor
        public Scene(string name, string path, Guid id)
            : base(path, name, id, AssetType.Scene)
        {
            Name = name;
            _gameObjects = new List<GameObject>();
        }
        public Scene(string path, string name, Guid id, AssetType type, List<GameObject> gameObjects)
            : base(path, name, id, type)
        {
            _gameObjects = new List<GameObject>();
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
            CurrentScene.Start();
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
            foreach(var item in this)
            {
                item.Dispose();
            }
        }

        /// <summary>
        /// Method to add a game object to the scene
        /// </summary>
        /// <param name="gameObject">gameobject which to add to scene</param>
        public Scene Add(GameObject gameObject)
        {// Create a HashSet to store the names of existing game objects
            var existingNames = new HashSet<string>(_gameObjects.Select(go => go.Name));

            // Check if the initial name of the new game object is already in use
            if (existingNames.Contains(gameObject.Name))
            {
                int count = 1;
                string originalName = gameObject.Name;

                // Generate a unique name by appending a number
                while (existingNames.Contains(gameObject.Name))
                {
                    gameObject.Name = $"{originalName} ({count})";
                    count++;
                }
            }
            _gameObjects.Add(gameObject);
            if (MainCamera is null && gameObject.GetComponent<Camera>() is not null)
            {
                MainCamera = gameObject;
            }
            return this;
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
        public IEnumerable<GameObject> GetGameObjects()
        {
            foreach(var item in _gameObjects)
            {
                yield return item;
            }
        }

        public void Render()
        {
            MainCamera?.Render();
            foreach (var item in _gameObjects)
            {
                if (item == MainCamera)
                {
                    continue;
                }
                item.Render();
            }
        }

        /// <summary>
        /// Default render without camera
        /// </summary>
        public void EditorLayerRender()
        {
            foreach (var item in _gameObjects)
            {
                if (item == MainCamera)
                {
                    continue;
                }
                item.Render();
            }
        }

        public void Dispose()
        {
            foreach (var item in _gameObjects)
            {
                item.Dispose();
            }
            _gameObjects.Clear();

            GC.SuppressFinalize(this);
        }

        public IEnumerator<GameObject> GetEnumerator() => _gameObjects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
