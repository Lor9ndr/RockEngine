using RockEngine.Engine.ECS;
using RockEngine.Engine.ECS.GameObjects;

namespace RockEngine.Engine.EngineStates
{
    internal sealed class Memento
    {
        private readonly Dictionary<GameObject, Dictionary<IComponent, object>> _objectStates;

        public Memento()
        {
            _objectStates = new Dictionary<GameObject, Dictionary<IComponent, object>>();
        }

        public void SaveState(Scene scene)
        {
            foreach(var item in scene.GetGameObjects())
            {
                SaveState(item);
            }
        }
        public void RestoreState(Scene scene)
        {
            foreach(var item in scene.GetGameObjects())
            {
                RestoreState(item);
            }
        }

        public void SaveState(GameObject gameObject)
        {
            var componentStates = new Dictionary<IComponent, object>();

            foreach(var component in gameObject.GetComponents())
            {
                var state = component.GetState();
                componentStates.Add(component, state);
            }

            _objectStates.Add(gameObject, componentStates);
        }

        public void RestoreState(GameObject gameObject)
        {
            if(_objectStates.TryGetValue(gameObject, out var componentStates))
            {
                foreach(var component in _objectStates.Values)
                {
                    foreach(var item in component)
                    {
                        item.Key.SetState(item.Value);
                    }
                }
            }
            _objectStates.Clear();
        }
    }
}