
using OpenTK.Mathematics;

using RockEngine.ECS;
using RockEngine.ECS.GameObjects;
using RockEngine.Physics.Colliders;

namespace RockEngine.Physics
{
    public sealed class PhysicsManager 
    {
        private Camera _camera;

        public PhysicsWorld World { get; set; }

        public PhysicsManager(IWorldRenderer worldRenderer)
        {
            World = new PhysicsWorld(worldRenderer);
            World.Gravity = new Vector3(0, -9.8f, 0);
        }

        public void Update(float elapsedTime)
        {
            World.Simulate();
        }

        public void SetDebugRender(Camera debugCamera)
        {
            _camera = debugCamera;
        }

     
        public EngineRigidBody LocalCreateRigidBody(float mass, Vector3 startPos, Collider collider)
        {
            var rb = new EngineRigidBody(startPos, mass);
            World.AddRigidBody(rb);
            rb.Collider = collider;
            collider.Body = rb;
            return rb;
        }
    }
}
