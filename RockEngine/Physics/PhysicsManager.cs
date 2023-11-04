using BulletSharp;

using OpenTK.Mathematics;

using RockEngine.BulletPhysics;
using RockEngine.Engine.ECS;
using RockEngine.Engine.ECS.GameObjects;

namespace RockEngine.Physics
{
    public sealed class PhysicsManager : IDisposable
    {
        public DiscreteDynamicsWorld World { get; set; }
        public DebugRenderer DebugRenderer;

        private readonly CollisionDispatcher dispatcher;
        private readonly DbvtBroadphase broadphase;
        private readonly List<CollisionShape> collisionShapes = new List<CollisionShape>();
        private readonly CollisionConfiguration collisionConf;

        public PhysicsManager()
        {
            // collision configuration contains default setup for memory, collision setup
            collisionConf = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConf);

            broadphase = new DbvtBroadphase();
            World = new DiscreteDynamicsWorld(dispatcher, broadphase, null, collisionConf);
            World.Gravity = new Vector3(0, -9.8f, 0);
        }

        public void Update(float elapsedTime)
        {
            DebugRenderer?.PrepareOS();
            World.StepSimulation(elapsedTime);
        }

        public void SetDebugRender(Camera debugCamera)
        {
            DebugRenderer = new DebugRenderer(debugCamera);
            World.DebugDrawer = new GLDebugDrawer(DebugRenderer);
        }

        private void ExitPhysics()
        {
            //remove/dispose constraints
            int i;
            for (i = World.NumConstraints - 1; i >= 0; i--)
            {
                TypedConstraint constraint = World.GetConstraint(i);
                World.RemoveConstraint(constraint);
                constraint.Dispose();
            }

            //remove the rigidbodies from the dynamics world and delete them
            for (i = World.NumCollisionObjects - 1; i >= 0; i--)
            {
                CollisionObject obj = World.CollisionObjectArray[i];
                RigidBody body = (RigidBody)obj;
                if (body != null && body.MotionState != null)
                {
                    body.MotionState.Dispose();
                }
                World.RemoveCollisionObject(obj);
                obj.Dispose();
            }

            //delete collision shapes
            foreach (CollisionShape shape in collisionShapes)
            {
                shape.Dispose();
            }

            collisionShapes.Clear();

            World.Dispose();
            broadphase.Dispose();
            dispatcher?.Dispose();
            collisionConf.Dispose();
        }

        public EngineRigidBody LocalCreateRigidBody(float mass, Matrix4 startTransform, CollisionShape shape)
        {
            DefaultMotionState myMotionState = new DefaultMotionState(startTransform);

            RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape);
            EngineRigidBody body = new EngineRigidBody(rbInfo);
            body.Mass = mass;
            World.AddRigidBody(body);

            return body;
        }

        public void Dispose()
        {
            ExitPhysics();
        }
    }
}
