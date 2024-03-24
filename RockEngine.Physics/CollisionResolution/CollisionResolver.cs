using OpenTK.Mathematics;

using System;
using System.Collections.Generic;

namespace RockEngine.Physics.CollisionResolution
{
    public class CollisionResolver
    {
        private const float PenetrationAllowance = 0.001f;
        private const float PenetrationCorrection = 0.4f;

        public void ResolveCollisions(List<RigidBody> bodies)
        {
            foreach(var bodyA in bodies)
            {
                bodyA.Collider.WasCollided = false;

                foreach(var bodyB in bodies)
                {
                    if(bodyA == bodyB || (bodyA.IsStatic && bodyB.IsStatic))
                    {
                        continue;
                    }

                    if(bodyA.Collider.Accept(bodyB.Collider, out CollisionManifold manifold))
                    {
                        bodyA.Collider.WasCollided = true;
                        bodyB.Collider.WasCollided = true;

                        ResolveCollision(bodyA, bodyB, manifold);
                    }
                }
            }
        }

        private void ResolveCollision(RigidBody bodyA, RigidBody bodyB, CollisionManifold manifold)
        {
            bool isDynamicDynamic = !(bodyA.IsStatic || bodyB.IsStatic);
            float inverseMassSum = isDynamicDynamic ? bodyA.InverseMass + bodyB.InverseMass : 0;

            foreach(var contactPoint in manifold.ContactPoints)
            {
                Vector3 impulse = CalculateImpulse(bodyA, bodyB, manifold, contactPoint, isDynamicDynamic);
                if(isDynamicDynamic)
                {
                    bodyA.ApplyForce(impulse, contactPoint);
                    bodyB.ApplyForce(-impulse, contactPoint);
                }
                else
                {
                    RigidBody dynamicBody = bodyA.IsStatic ? bodyB : bodyA;
                    dynamicBody.ApplyForce(impulse, contactPoint);
                }
            }

            ApplyPositionalCorrection(bodyA, bodyB, manifold, inverseMassSum);
        }

        private Vector3 CalculateImpulse(RigidBody bodyA, RigidBody bodyB, CollisionManifold manifold, Vector3 contactPoint, bool isDynamicDynamic)
        {
            // This method now handles both static-dynamic and dynamic-dynamic scenarios.
            RigidBody dynamicBody = bodyA.IsStatic ? bodyB : bodyA;
            RigidBody staticBody = bodyA.IsStatic ? bodyA : bodyB;

            Matrix3 worldInverseInertiaTensorA = Matrix3.CreateFromQuaternion(dynamicBody.Rotation) * dynamicBody.InverseInertiaTensor * Matrix3.CreateFromQuaternion(Quaternion.Invert(dynamicBody.Rotation));
            Matrix3 worldInverseInertiaTensorB = isDynamicDynamic ? Matrix3.CreateFromQuaternion(staticBody.Rotation) * staticBody.InverseInertiaTensor * Matrix3.CreateFromQuaternion(Quaternion.Invert(staticBody.Rotation)) : Matrix3.Zero;

            Vector3 rA = contactPoint - dynamicBody.Position;
            Vector3 rB = isDynamicDynamic ? contactPoint - staticBody.Position : Vector3.Zero;
            Vector3 relativeVelocity = isDynamicDynamic ? staticBody.Velocity + Vector3.Cross(staticBody.AngularVelocity, rB) - dynamicBody.Velocity - Vector3.Cross(dynamicBody.AngularVelocity, rA) : dynamicBody.Velocity + Vector3.Cross(dynamicBody.AngularVelocity, rA);

            float velocityAlongNormal = Vector3.Dot(relativeVelocity, manifold.Normal);

            if(velocityAlongNormal > 0)
            {
                return Vector3.Zero;
            }

            float restitution = isDynamicDynamic ? Math.Min(dynamicBody.Collider.Restitution, staticBody.Collider.Restitution) : dynamicBody.Collider.Restitution;

            float raCrossN = Vector3.Dot(Vector3.Cross(rA, manifold.Normal), worldInverseInertiaTensorA * Vector3.Cross(rA, manifold.Normal));
            float rbCrossN = isDynamicDynamic ? Vector3.Dot(Vector3.Cross(rB, manifold.Normal), worldInverseInertiaTensorB * Vector3.Cross(rB, manifold.Normal)) : 0;

            float j = -(1 + restitution) * velocityAlongNormal;
            j /= dynamicBody.InverseMass + (isDynamicDynamic ? staticBody.InverseMass : 0) + raCrossN + rbCrossN;

            return j * manifold.Normal;
        }

        private void ApplyPositionalCorrection(RigidBody bodyA, RigidBody bodyB, CollisionManifold manifold, float inverseMassSum)
        {
            // This method now handles both static-dynamic and dynamic-dynamic scenarios.
            Vector3 correction = Math.Max(manifold.Depth - PenetrationAllowance, 0.0f) * PenetrationCorrection * manifold.Normal;
            if(bodyA.IsStatic || bodyB.IsStatic)
            {
                RigidBody dynamicBody = bodyA.IsStatic ? bodyB : bodyA;
                dynamicBody.Position += dynamicBody.InverseMass * correction;
            }
            else
            {
                bodyA.Position += bodyA.InverseMass / inverseMassSum * correction;
                bodyB.Position -= bodyB.InverseMass / inverseMassSum * correction;
            }
        }
    }
}