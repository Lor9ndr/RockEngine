using OpenTK.Mathematics;

using System.Diagnostics;

namespace RockEngine.Physics
{
    public class PhysicsWorld
    {
        private const float PERCENT = 0.2f;
        private const float SLOP = 0.01f;
        public Stopwatch Watcher;

        public List<RigidBody> RigidBodies { get; set; }
        public Vector3 Gravity { get; set; } = new Vector3(0, -9.8f, 0); // Updated gravity value
        public DebugWorld DebugWorld;

        public PhysicsWorld(IWorldRenderer worldRenderer)
        {
            RigidBodies = new List<RigidBody>();
            Watcher = new Stopwatch();
            DebugWorld = new DebugWorld(worldRenderer);
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
                bodyA.Collider.WasCollided = false;
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
                        ResolveStaticVsMovable(movableBody, staticBody,deltaTime);
                    }
                    else
                    {
                        ResolveMovableVsMovable(bodyA, bodyB, deltaTime);
                    }
                }
            });
            Parallel.For(0, cnt, i =>
            {
                RigidBodies[i].Update(deltaTime, Gravity);
            });
            DebugWorld.Update();
            Watcher.Stop();
        }

        private void ResolveStaticVsMovable(RigidBody movableBody, RigidBody staticBody, float deltaTime)
        {
            var result = movableBody.Collider.CheckCollision(staticBody.Collider);
            if(!result.IsCollided)
            {
                return;
            }
            movableBody.Collider.WasCollided = true;
            staticBody.Collider.WasCollided = true;
            float invMassSum = movableBody.InverseMass;
            Vector3 correction = Math.Max(result.PenetrationDepth - SLOP, 0.0001f) / invMassSum * PERCENT * result.Normal;
            movableBody.Position += correction * movableBody.InverseMass / invMassSum;

            foreach(var contactPoint in result.ContactPoints)
            {
                Vector3 rA = contactPoint - movableBody.CenterOfMassGlobal;
                Vector3 relativeVelocity = movableBody.Velocity + Vector3.Cross(movableBody.AngularVelocity, rA);
                float velocityAlongNormal = Vector3.Dot(relativeVelocity, result.Normal);
                if(velocityAlongNormal > 0)
                {
                    continue;
                }

                float e = Math.Min(movableBody.Collider.Material.Restitution, staticBody.Collider.Material.Restitution);
                Vector3 RA_cross_N = Vector3.Cross(rA, result.Normal);
                float denominator = invMassSum + Vector3.Dot(result.Normal, Vector3.Cross(movableBody.InverseInertiaTensorWorld * RA_cross_N, rA));

                float j = -(1 + e) * velocityAlongNormal;
                j /= (denominator);
                Vector3 impulse = j * result.Normal;
                movableBody.ApplyImpulse(impulse, contactPoint);

                Vector3 t = relativeVelocity - (velocityAlongNormal * result.Normal);
                if(t.LengthSquared <= 0)
                {
                    continue;
                }
                t.Normalize();
                float jt = -Vector3.Dot(relativeVelocity, t);
                jt /= invMassSum;
                if(Math.Abs(jt) < 0.01f)
                {
                    return;
                }
                Vector3 tangentImpulse;
                if(Math.Abs(jt) < j * e)
                {
                    tangentImpulse = jt * t;
                }
                else
                {
                    tangentImpulse = -j * movableBody.CoefficientOfFrictionDynamic * t;
                }
                movableBody.ApplyImpulse(tangentImpulse, contactPoint);
            }
        }

        private void ResolveMovableVsMovable(RigidBody bodyA, RigidBody bodyB, float deltaTime)
        {
            var result = bodyA.Collider.CheckCollision(bodyB.Collider);
            if(!result.IsCollided)
            {
                return;
            }
            bodyA.Collider.WasCollided = true;
            bodyB.Collider.WasCollided = true;

            float invMassSum = bodyA.InverseMass + bodyB.InverseMass;
            Vector3 correction = Math.Max(result.PenetrationDepth - SLOP, 0.0001f) / invMassSum * PERCENT * result.Normal;
            bodyA.Position += correction * bodyA.InverseMass;
            bodyB.Position -= correction * bodyB.InverseMass;

            foreach(var contactPoint in result.ContactPoints)
            {
                Vector3 rA = contactPoint - bodyA.CenterOfMassGlobal;
                Vector3 rB = contactPoint - bodyB.CenterOfMassGlobal;
                Vector3 relativeVelocity = bodyA.Velocity + Vector3.Cross(bodyA.AngularVelocity, rA) - bodyB.Velocity - Vector3.Cross(bodyB.AngularVelocity, rB);
                float velocityAlongNormal = Vector3.Dot(relativeVelocity, result.Normal);
                if(velocityAlongNormal > 0)
                {
                    continue;
                }

                float e = Math.Min(bodyA.Collider.Material.Restitution, bodyB.Collider.Material.Restitution);
                Vector3 RA_cross_N = Vector3.Cross(rA, result.Normal);
                Vector3 RB_cross_N = Vector3.Cross(rB, result.Normal);

                float denominator = invMassSum +
                    Vector3.Dot(result.Normal, Vector3.Cross(bodyA.InverseInertiaTensorWorld * RA_cross_N, rA)) +
                    Vector3.Dot(result.Normal, Vector3.Cross(bodyB.InverseInertiaTensorWorld * RB_cross_N, rB));

                float j = -(1 + e) * velocityAlongNormal;
                j /= denominator;
                Vector3 impulse = j * result.Normal;
                bodyA.ApplyImpulse(impulse, contactPoint);
                bodyB.ApplyImpulse(-impulse, contactPoint);

                Vector3 t = relativeVelocity - (velocityAlongNormal * result.Normal);
                if(t.LengthSquared <= 0)
                {
                    continue;
                }
                t.Normalize();
                float jt = -Vector3.Dot(relativeVelocity, t);
                jt /= invMassSum;
                if(Math.Abs(jt) < 0.01f)
                {
                    return;
                }
                Vector3 tangentImpulse;
                if(Math.Abs(jt) < j * e)
                {
                    tangentImpulse = jt * t;
                }
                else
                {
                    tangentImpulse = -j * Math.Min(bodyA.CoefficientOfFrictionDynamic, bodyB.CoefficientOfFrictionDynamic) * t;
                }
                bodyA.ApplyImpulse(tangentImpulse, contactPoint);
                bodyB.ApplyImpulse(-tangentImpulse, contactPoint);
            }
        }
    }
}
