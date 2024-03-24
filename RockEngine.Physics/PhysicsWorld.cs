using OpenTK.Mathematics;
using RockEngine.Physics.CollisionResolution;
using RockEngine.Physics.Constraints;
using RockEngine.Physics.Drawing;

using System.Diagnostics;

namespace RockEngine.Physics
{
    public class PhysicsWorld
    {
        private readonly List<RigidBody> _rigidBodies = new List<RigidBody>();
        private readonly CollisionResolver _collisionResolver = new CollisionResolver();
        private readonly ConstraintSolver _constraintSolver = new ConstraintSolver();
        public Stopwatch Watcher = new Stopwatch();
        public ColliderRenderer ColliderRenderer { get; set; }

        /// <summary>
        /// Gravity vector3
        /// </summary>
        public Vector3 Gravity { get; set; } = new Vector3(0, -9.8f, 0) ;

        public PhysicsWorld()
        {
        }

        public void AddRigidBody(RigidBody body)
        {
            _rigidBodies.Add(body);
        }

        public void Update(float deltaTime = 0.0013674f)
        {
            Watcher.Restart();
            
            // Update physics bodies
            foreach(var body in _rigidBodies)
            {
                body.Simulate(deltaTime, Gravity);
            }

            // Detect and resolve collisions
            _collisionResolver.ResolveCollisions(_rigidBodies);

            // Solve constraints
            _constraintSolver.SolveConstraints(deltaTime);

            Watcher.Stop();
        }
    }
}