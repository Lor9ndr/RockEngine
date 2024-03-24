
using OpenTK.Mathematics;

using RockEngine.ECS;
using RockEngine.ECS.GameObjects;
using RockEngine.Physics.Colliders;
using RockEngine.Physics.Drawing;

namespace RockEngine.Physics
{
    public sealed class PhysicsManager 
    {
        private Camera _camera;

        public PhysicsWorld World { get; set; }

        public PhysicsManager(ColliderRenderer renderer)
        {
            // IWorldRenderer worldRenderer
            World = new PhysicsWorld();
            World.ColliderRenderer = renderer;
        }

        public void Update(float elapsedTime)
        {
            World.Update(elapsedTime);
        }

        public void SetDebugRender(Camera debugCamera)
        {
            _camera = debugCamera;
        }

     
        public EngineRigidBody LocalCreateRigidBody(float mass, Vector3 startPos, ICollider collider)
        {
            var rb = new EngineRigidBody(startPos, mass, collider);
            World.AddRigidBody(rb);
            return rb;
        }
    }
}
