using RockEngine.OpenGL;
using RockEngine.Utils;

namespace RockEngine.Engine.ECS.GameObjects
{
    public class GameObject : IDisposable, IRenderable
    {
        private static uint _objectID;

        private List<IComponent> _components;

        public Layer Layer = Layer.Default;

        public bool IsActive = true;

        /// <summary>
        /// ID of the currentObject 
        /// </summary>
        public uint GameObjectID;

        /// <summary>
        /// Name of the gameObject
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Transformation of the currentObject
        /// </summary>
        public Transform Transform;

        /// <summary>
        /// Parent gameobject
        /// </summary>
        public GameObject Parent { get; private set; }

        /// <summary>
        /// Children of the current gameobject
        /// </summary>
        public List<GameObject> Children { get; private set; }

        #region Ctor

        /// <summary>
        /// Initialize new gameobject without any components
        /// </summary>
        /// <param name="name">Name of the gameobject</param>
        public GameObject(string name) : this()
        {
            Name = name;

            Transform = AddComponent(new Transform());
            Logger.AddLog($"Created gameobject with name: {Name}");
        }

        /// <summary>
        /// Initialize new gameobject with name an components
        /// </summary>
        /// <param name="name">Name of the gameobject</param>
        /// <param name="components">array of components to attach to current gameobject</param>
        public GameObject(string name, params IComponent[] components)
            : this(name)
        {
            foreach (var item in components)
            {
                AddComponent(item);
            }

            Logger.AddLog($"Created gameobject with name: {Name}");
        }
        public GameObject()
        {
            Name = "Gameobject";
            GameObjectID = _objectID++;
            _components = new List<IComponent>();
            Children = new List<GameObject>();
            Transform = new Transform();
        }

        #endregion

        /// <summary>
        /// Render the components of the gameobject
        /// </summary>
        public virtual void Render()
        {
            if (!IsActive)
            {
                return;
            }
            for (int i = 0; i < _components.Count; i++)
            {
                IComponent? component = _components[i];
                if (component is IRenderable renderable)
                {
                    renderable.Render();
                }
            }
        }
       

        /// <summary>
        /// Update components of the gameobject
        /// </summary>
        public virtual void Update()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                IComponent? item = _components[i];
                item.OnUpdate();
            }
        }

        

        /// <summary>
        /// Dispose components of the gameobject
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                IComponent? item = _components[i];
                if (item is IDisposable disp)
                {
                    disp.Dispose();
                }
            }
            GC.SuppressFinalize(this);

        }

        #region ECS functions

        /// <summary>
        /// Add <paramref name="component"/> to the components container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public T AddComponent<T>(T component) where T : IComponent
        {
            Check.IsNull(component);
            if (component is Transform tr)
            {
                if (Transform is not null)
                {
                    RemoveComponent(Transform);
                }
                Transform = tr;
            }
            component.Parent = this;
            _components.Add(component);
            _components = _components.OrderBy(s => s.Order).ToList();

            return component;
        }

        /// <summary>
        /// Get the component of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The required type to get</typeparam>
        /// <returns><typeparamref name="T"/> value component </returns>
        public T? GetComponent<T>() where T : IComponent
        {
            return _components.OfType<T>().FirstOrDefault();
        }

        public IEnumerable<IComponent> GetComponents()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                IComponent? item = _components[i];
                yield return item;
            }
        }
        public void RemoveComponent(IComponent component)
        {
            if (component is Transform tr)
            {
                tr.ClearChildrenTransforms();
            }
            /*  if (component is MeshComponent mc)
              {
                  mc.InstanceCount--;
                  mc.Transforms.Remove(Transform);
              }*/
            _components.Remove(component);
        }

        public void AddChild(GameObject child)
        {
            Children.Add(child);
            Transform.AddChildTransform(child.Transform);
        }

        public void RemoveChild(GameObject child)
        {
            Children.Remove(child);
            Transform.RemoveChildTransform(child.Transform);
        }

        public void OnStart()
        {
            foreach (var item in _components)
            {
                item.OnStart();
            }
        }

        internal void OnExit()
        {
            foreach (var item in _components)
            {
                item.OnDestroy();
            }
        }

        public void RenderMeshWithoutMaterial()
        {
            if(IsActive)
            {
                Transform.Render();
                GetComponent<MeshComponent>()?.Render();
            }
        }

        #endregion
    }
}
