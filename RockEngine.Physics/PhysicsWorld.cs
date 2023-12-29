using OpenTK.Mathematics;

using System;

namespace RockEngine.Physics
{
    public class PhysicsWorld
    {
        public List<RigidBody> RigidBodies { get; set; }
        public Vector3 Gravity { get; set; } = new Vector3(0, 0f, 0);

        public PhysicsWorld()
        {
            RigidBodies = new List<RigidBody>();
        }

        public void AddRigidBody(RigidBody rigidBody)
        {
            RigidBodies.Add(rigidBody);
        }

        public void Simulate(float deltaTime)
        {
            const float penetrationSlop = 0.01f;
            // Check for collisions and resolve them
/*            for(int i = 0; i < RigidBodies.Count; i++)
            {

            }*/
            Parallel.For(0, RigidBodies.Count, i =>
            {
                for(int j = i + 1; j < RigidBodies.Count; j++)
                {
                    RigidBody bodyA = RigidBodies[i];
                    RigidBody bodyB = RigidBodies[j];
                    if(bodyA == bodyB || bodyA.Collider is null || bodyB.Collider is null)
                    {
                        continue;
                    }
                    if(bodyA.IsStatic && bodyB.IsStatic)
                    {
                        // Both bodies are static, skip collision resolution
                        continue;
                    }
                    if(bodyA.IsStatic && bodyB.IsStatic)
                    {
                        // Both bodies are static, skip collision resolution
                        continue;
                    }

                    if(bodyA.IsStatic || bodyB.IsStatic)
                    {
                        // One of the bodies is static, resolve collision with static body
                        RigidBody movableBody = bodyA.IsMovable ? bodyA : bodyB;
                        RigidBody staticBody = bodyA.IsStatic ? bodyA : bodyB;

                        ResolveStaticVsMovable(penetrationSlop, movableBody, staticBody);
                        continue;
                    }

                    ResolveMovableVsMovableObjects(penetrationSlop, bodyA, bodyB);
                }
            });
            /*            for(int i = 0; i < RigidBodies.Count; i++)
                        {

                        }*/

            // Update the positions of the rigid bodies
            Parallel.For(0, RigidBodies.Count, i =>
            {
                RigidBodies[i].Update(deltaTime, Gravity);
            });
        }

        private static void ResolveStaticVsMovable(float penetrationSlop, RigidBody movableBody, RigidBody staticBody)
        {
            if(movableBody.Collider.CheckCollision(staticBody.Collider, out Vector3 collisionPoint, out Vector3 normal))
            {
                Vector3 separationVector = normal * penetrationSlop;
                movableBody.Position += separationVector;
                movableBody.Velocity = movableBody.Velocity.Reflect(normal);
                float velocityAlongNormal = Vector3.Dot(movableBody.Velocity, normal);
                float impulseMagnitude = velocityAlongNormal * movableBody.Mass;
                Vector3 impulse = normal * impulseMagnitude;
                movableBody.ApplyForce(impulse);
            }
        }

        private static void ResolveMovableVsMovableObjects(float penetrationSlop, RigidBody bodyA, RigidBody bodyB)
        {
            if(bodyA.Collider.CheckCollision(bodyB.Collider, out Vector3 collisionPoint, out Vector3 normal))
            {
                // Calculate separation vector
                Vector3 separationVector = normal * penetrationSlop;

                // Move bodies away from each other
                bodyA.Position += separationVector / 2;
                bodyB.Position -= separationVector / 2;

                // Reflect velocity vectors
                bodyA.Velocity = bodyA.Velocity.Reflect(normal);
                bodyB.Velocity = bodyB.Velocity.Reflect(normal);

                // Calculate impulses
                float velocityAlongNormalA = Vector3.Dot(bodyA.Velocity, normal);
                float impulseMagnitudeA = velocityAlongNormalA * bodyA.Mass;
                Vector3 impulseA = normal * impulseMagnitudeA;
                bodyA.ApplyForce(impulseA);

                float velocityAlongNormalB = Vector3.Dot(bodyB.Velocity, normal);
                float impulseMagnitudeB = velocityAlongNormalB * bodyB.Mass;
                Vector3 impulseB = normal * impulseMagnitudeB;
                bodyB.ApplyForce(impulseB);
            }
        }
    }
}
