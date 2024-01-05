using OpenTK.Mathematics;

namespace RockEngine.Physics
{
    public class PhysicsWorld
    {
        private const float PERCENT = 0.2f;
        private const float SLOP = 0.01f;

        public List<RigidBody> RigidBodies { get; set; }
        public Vector3 Gravity { get; set; } = new Vector3(0, -9.8f, 0); // Updated gravity value

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
            int rigidBodyCount = RigidBodies.Count;
            // Check for collisions and resolve them
            for(int i = 0; i < rigidBodyCount; i++)
            {
                for(int j = i + 1; j < rigidBodyCount; j++)
                {
                    RigidBody bodyA = RigidBodies[i];
                    RigidBody bodyB = RigidBodies[j];
                    if(bodyA == bodyB || bodyA.Collider is null || bodyB.Collider is null)
                    {
                        continue;
                    }

                    if(bodyA.IsStatic && bodyB.IsStatic)
                    {
                        continue;
                    }

                    if(bodyA.IsStatic || bodyB.IsStatic)
                    {
                        RigidBody movableBody = bodyA.IsMovable ? bodyA : bodyB;
                        RigidBody staticBody = bodyA.IsStatic ? bodyA : bodyB;

                        ResolveStaticVsMovable(movableBody, staticBody);
                    }
                    else
                    {
                        ResolveMovableVsMovableObjects(bodyA, bodyB);
                    }
                }
                RigidBodies[i].Update(deltaTime, Gravity);
            }
        }

        private static void ResolveStaticVsMovable(RigidBody movableBody, RigidBody staticBody)
        {
            if(movableBody.Collider.CheckCollision(staticBody.Collider, out CollisionResult collisionResult))
            {
                Vector3 relativeVelocity = movableBody.Velocity;
                float velocityAlongNormal = Vector3.Dot(relativeVelocity, collisionResult.Normal);

                // Calculate impulse scalar
                float j = -(1 + staticBody.Collider.Restitution) * velocityAlongNormal / movableBody.InverseMass;
                Vector3 impulse = j * collisionResult.Normal;

                // Apply impulse
                movableBody.Velocity += impulse * movableBody.InverseMass;

                // Position correction
                float penetrationDepth = collisionResult.PenetrationDepth - SLOP;
                Vector3 correction = Math.Max(penetrationDepth, 0.0f) / (movableBody.InverseMass) * collisionResult.Normal;
                movableBody.Position += correction * movableBody.InverseMass;

                // If we have rotation
                //if(movableBody.HasRotation)
                //{
                    // Calculate rotational impulse using cross product
                    Vector3 r = collisionResult.ContactPoint - movableBody.CenterOfMass;
                    Vector3 impulsePerpendicular = Vector3.Cross(r, impulse);
                    Vector3 angularImpulse = impulsePerpendicular / movableBody.InverseInertiaTensor;

                    // Apply rotational impulse
                    movableBody.ApplyTorque(angularImpulse);
                //}
            }
        }

        private static void ResolveMovableVsMovableObjects(RigidBody bodyA, RigidBody bodyB)
        {
            if(bodyA.Collider.CheckCollision(bodyB.Collider, out CollisionResult collisionResult))
            {
                Vector3 relativeVelocity = bodyA.Velocity - bodyB.Velocity;
                float velocityAlongNormal = Vector3.Dot(relativeVelocity, collisionResult.Normal);

                // Calculate impulse scalar
                float j = -(1 + Math.Min(bodyA.Collider.Restitution, bodyB.Collider.Restitution)) * velocityAlongNormal;
                j /= (bodyA.InverseMass + bodyB.InverseMass);
                Vector3 impulse = j * collisionResult.Normal;

                // Apply impulse
                bodyA.Velocity += impulse * bodyA.InverseMass;
                bodyB.Velocity -= impulse * bodyA.InverseMass;

                // Position correction
                float penetrationDepth = collisionResult.PenetrationDepth - SLOP;
                Vector3 correction = Math.Max(penetrationDepth, 0.0f) / (bodyA.InverseMass + bodyB.InverseMass) * collisionResult.Normal;
                bodyA.Position += correction * bodyA.InverseMass;
                bodyB.Position -= correction * bodyB.InverseMass;

                // If we have rotation
                //if(bodyA.HasRotation || bodyB.HasRotation)
                //{
                    // Calculate rotational impulse using cross product
                    Vector3 rA = collisionResult.ContactPoint - bodyA.CenterOfMass;
                    Vector3 rB = collisionResult.ContactPoint - bodyB.CenterOfMass;
                    Vector3 impulsePerpendicularA = Vector3.Cross(rA, impulse);
                    Vector3 impulsePerpendicularB = Vector3.Cross(rB, -impulse);
                    Vector3 angularImpulseA = impulsePerpendicularA / bodyA.InverseInertiaTensor;
                    Vector3 angularImpulseB = impulsePerpendicularB / bodyB.InverseInertiaTensor;

                    // Apply rotational impulse
                    //bodyA.ApplyTorque(angularImpulseA);
                    //bodyB.ApplyTorque(angularImpulseB);
                //}
            }
        }
    }
}
