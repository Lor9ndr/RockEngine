
namespace RockEngine.ECS
{
    public interface IComponent
    {
        /// <summary>
        /// Ref to the component parent, so it is attached to that gameobject
        /// </summary>
        GameObject? Parent { get; set; }

        int Order { get; }

        void OnStart();

        void OnUpdate();

        void OnDestroy();

        dynamic GetState();

        void SetState(dynamic state);
    }
}
