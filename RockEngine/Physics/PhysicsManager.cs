using BulletSharp;

using OpenTK.Mathematics;

using RockEngine.Engine.ECS;

namespace RockEngine.Physics
{
    internal sealed class PhysicsManager : IDisposable
    {
        public DiscreteDynamicsWorld World { get; set; }

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
            World.StepSimulation(elapsedTime);
        }

        public void ExitPhysics()
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
            if (dispatcher != null)
            {
                dispatcher.Dispose();
            }
            collisionConf.Dispose();
        }

        public EngineRigidBody LocalCreateRigidBody(float mass, Matrix4 startTransform, CollisionShape shape)
        {
            bool isDynamic = mass != 0.0f;

            BulletSharp.Math.Vector3 localInertia = Vector3.Zero;
            if (isDynamic)
            {
                shape.CalculateLocalInertia(mass, out localInertia);
            }

            DefaultMotionState myMotionState = new DefaultMotionState(startTransform);

            RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia);
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
