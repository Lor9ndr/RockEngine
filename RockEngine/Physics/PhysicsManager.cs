
using OpenTK.Mathematics;

using RockEngine.ECS;
using RockEngine.ECS.GameObjects;
using RockEngine.Physics.Colliders;

namespace RockEngine.Physics
{
    public sealed class PhysicsManager : IDisposable
    {
        public PhysicsWorld World { get; set; }

        public PhysicsManager()
        {

            World = new PhysicsWorld();
            World.Gravity = new Vector3(0, -9.8f, 0);
        }

        public void Update(float elapsedTime)
        {
            World.Simulate();
        }

        public void SetDebugRender(Camera debugCamera)
        {
        }

        private void ExitPhysics()
        {
        }

        public EngineRigidBody LocalCreateRigidBody(float mass, Vector3 startPos, Collider collider)
        {
            var rb = new EngineRigidBody(startPos, mass);
            World.AddRigidBody(rb);
            rb.Collider = collider;
            collider.Body = rb;
            return rb;
        }

        public void Dispose()
        {
            ExitPhysics();
        }
    }
}
