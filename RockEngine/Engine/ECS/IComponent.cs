using Newtonsoft.Json;

using RockEngine.Engine.ECS.GameObjects;

using System.Reflection;

namespace RockEngine.Engine.ECS
{
    public interface IComponent
    {
        /// <summary>
        /// Ref to the component parent, so it is attached to that gameobject
        /// </summary>
        [JsonIgnore]
        GameObject? Parent { get; set; }

        [JsonIgnore]
        int Order { get; }

        void OnStart();

        void OnUpdate();

        void OnUpdateDevelepmentState();

        void OnDestroy();

        object GetState();

        void SetState(object state);
    }
}
