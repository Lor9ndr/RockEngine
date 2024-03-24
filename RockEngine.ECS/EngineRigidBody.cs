
using OpenTK.Mathematics;

using RockEngine.Physics;
using RockEngine.Physics.Colliders;

namespace RockEngine.ECS
{
    public sealed class EngineRigidBody : RigidBody, IComponent
    {
        public EngineRigidBody(Vector3 position, float mass, ICollider collider)
            : base(position, mass, collider)
        {
        }

        public GameObject? Parent { get; set; }

        public int Order => 0;

        public void OnDestroy()
        {

        }

        public void OnStart()
        {
        }

        public void OnUpdate()
        {
        }

        public void OnUpdateDevelepmentState()
        {
        }

        public dynamic GetState()
        {
            return new { Mass = Mass };
        }

        public void SetState(dynamic state)
        {
            Mass = state.Mass;
        }
    }
}
