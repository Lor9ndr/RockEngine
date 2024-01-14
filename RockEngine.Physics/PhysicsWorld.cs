using OpenTK.Mathematics;

using System.Diagnostics;

namespace RockEngine.Physics
{
    public class PhysicsWorld
    {
        private const float PERCENT = 0.7f;
        private const float SLOP = 0.01f;
        public Stopwatch Watcher;

        public List<RigidBody> RigidBodies { get; set; }
        public Vector3 Gravity { get; set; } = new Vector3(0, -9.8f, 0); // Updated gravity value

        public PhysicsWorld()
        {
            RigidBodies = new List<RigidBody>();
            Watcher = new Stopwatch();
        }

        public void AddRigidBody(RigidBody rigidBody)
        {
            RigidBodies.Add(rigidBody);
        }

        public void Simulate(float deltaTime = 0.0078036f)
        {
            Watcher.Restart();
            var cnt = RigidBodies.Count;
            Parallel.For(0, cnt, i =>
            {
                var bodyA = RigidBodies[i];
                for(int j = i + 1; j < cnt; j++)
                {
                    var bodyB = RigidBodies[j];
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
                        ResolveStaticVsMovable(movableBody, staticBody, deltaTime);
                    }
                    else
                    {
                        ResolveMovableVsMovableObjects(bodyA, bodyB);
                    }
                }
            });
            Parallel.For(0, cnt, i =>
            {
                RigidBodies[i].Update(deltaTime, Gravity);
            });

            Watcher.Stop();
        }

        private static void ResolveStaticVsMovable(RigidBody movableBody, RigidBody staticBody, float deltaTime)
        {
            var movableCollider = movableBody.Collider;
            var staticCollider = staticBody.Collider;

            // Check for collision between movable and static colliders
            var collisionResult = movableCollider.CheckCollision(staticCollider);

            if(collisionResult.IsCollided)
            {
                // Compute relative velocity
                Vector3 rA = collisionResult.ContactPoint - movableBody.CenterOfMassGlobal;
                Vector3 rB = collisionResult.ContactPoint - staticBody.CenterOfMassGlobal;

                Vector3 relativeVelocity = movableBody.Velocity + Vector3.Cross(movableBody.AngularVelocity, rA) -
                                           staticBody.Velocity - Vector3.Cross(staticBody.AngularVelocity, rB);

                float velocityAlongNormal = Vector3.Dot(relativeVelocity, collisionResult.Normal);

                // Objects are moving away from each other, so we exit
                if(velocityAlongNormal > 0)
                {
                    return;
                }

                // Compute an effective mass.
                float invMassSum = movableBody.InverseMass + staticBody.InverseMass;

                // Compute an impulse
                float e = Math.Min(movableBody.Collider.Restitution, staticBody.Collider.Restitution);

                // Compute impulse denominator
                Vector3 RA_cross_N = Vector3.Cross(rA, collisionResult.Normal);
                Vector3 RB_cross_N = Vector3.Cross(rB, collisionResult.Normal);
                float denominator = movableBody.InverseMass + staticBody.InverseMass + Vector3.Dot(collisionResult.Normal, Vector3.Cross(movableBody.InverseInertiaTensorWorld * RA_cross_N, rA) + Vector3.Cross(staticBody.InverseInertiaTensorWorld * RB_cross_N, rB));
                Vector3 correction = Math.Max(collisionResult.PenetrationDepth - SLOP, 0.0001f) / invMassSum * PERCENT * collisionResult.Normal;
                movableBody.Position += correction;
                // Compute impulse
                float j = -(1 + e) * velocityAlongNormal;
                j /= denominator;

                // Apply impulse
                Vector3 impulse = j * collisionResult.Normal;
                movableBody.ApplyImpulse(impulse, collisionResult.ContactPoint);
                movableBody.ApplyTorque(Vector3.Cross(rA, impulse));

                // Compute friction
                Vector3 t = relativeVelocity - (velocityAlongNormal * collisionResult.Normal);
                if(t.LengthSquared <= 0)
                {
                    return;
                }

                t.Normalize();

                // j tangent magnitude
                float jt = -Vector3.Dot(relativeVelocity, t);
                jt /= invMassSum;

                // Don't apply tiny friction impulses
                if(Math.Abs(jt) < 0.01f)
                {
                    return;
                }

                // Coulomb's law
                Vector3 tangentImpulse;
                if(Math.Abs(jt) < j * staticBody.CoefficientOfFrictionStatic)
                {
                    tangentImpulse = jt * t;
                }
                else
                {
                    tangentImpulse = -j * movableBody.CoefficientOfFrictionDynamic * t;
                }

                // Apply friction impulse
                movableBody.ApplyImpulse(tangentImpulse, collisionResult.ContactPoint);
            }
        }

        /// <summary>
        /// TODO: IMPLEMENT SAME Logic as in the ResolveStaticVsMovable but for two movable bodies
        /// </summary>
        /// <param name="bodyA"></param>
        /// <param name="bodyB"></param>

        private static void ResolveMovableVsMovableObjects(RigidBody bodyA, RigidBody bodyB)
        {
            var result = bodyA.Collider.CheckCollision(bodyB.Collider);

            if(result.IsCollided)
            {
                // Calculate the relative velocity
                Vector3 relativeVelocity = bodyB.Velocity - bodyA.Velocity;

                // Calculate the velocity along the normal
                float velAlongNormal = Vector3.Dot(relativeVelocity, result.Normal);

                // Do nothing if velocities are separating
                if(velAlongNormal > 0)
                {
                    return;
                }

                // Calculate the restitution
                float e = Math.Min(bodyA.Collider.Restitution, bodyB.Collider.Restitution);

                // Calculate the impulse scalar
                float j = -(1 + e) * velAlongNormal;
                j /= bodyA.InverseMass + bodyB.InverseMass;

                // Apply the impulse
                Vector3 impulse = j * result.Normal;
                bodyA.Velocity -= bodyA.InverseMass * impulse;
                bodyB.Velocity += bodyB.InverseMass * impulse;

                // Position correction
                Vector3 correction = Math.Max(result.PenetrationDepth - SLOP, 0.0f) / (bodyA.InverseMass + bodyB.InverseMass) * PERCENT * result.Normal;
                bodyA.Position -= bodyA.InverseMass * correction;
                bodyB.Position += bodyB.InverseMass * correction;
            }
        }
    }
}

/*private static void ResolveStaticVsMovable(RigidBody movableBody, RigidBody staticBody)
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

        // Apply angular impulse
        Vector3 ra = collisionResult.ContactPoint - movableBody.CenterOfMass;
        movableBody.ApplyImpulse(impulse, ra);
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

        // Apply angular impulse
        Vector3 ra = collisionResult.ContactPoint - bodyA.CenterOfMass;
        Vector3 rb = collisionResult.ContactPoint - bodyB.CenterOfMass;
        bodyA.ApplyImpulse(impulse, ra);
        bodyB.ApplyImpulse(-impulse, rb);
    }
}*/