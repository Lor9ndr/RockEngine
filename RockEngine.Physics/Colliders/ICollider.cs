using OpenTK.Mathematics;

using RockEngine.Physics.CollisionResolution;

namespace RockEngine.Physics.Colliders
{
    public interface ICollider
    {
        bool WasCollided { get; set; }
        float Restitution { get; set; }

        /// <summary>
        /// Initiates collision detection and resolution with another collider.
        /// </summary>
        /// <param name="visitor">Another collider to check collision against.</param>
        /// <param name="manifold">The collision manifold that will be filled with collision details if a collision is detected.</param>
        /// <returns>Returns <see langword="true"/> if a collision was detected, <see langword="false"/> otherwise.</returns>
        bool Accept(ICollider visitor, out CollisionManifold manifold);

        /// <summary>
        /// Checks for collision with an <see cref="AABB"/> collider and calculates collision details.
        /// </summary>
        /// <param name="aabb">The AABB collider to check collision against.</param>
        /// <param name="manifold">The collision manifold that will be filled with collision details if a collision is detected.</param>
        /// <returns>Returns <see langword="true"/> if a collision was detected, <see langword="false"/> otherwise.</returns>
        bool Visit(AABB other, out CollisionManifold manifold);

        // <summary>
        /// Checks for collision with an <see cref="OBB"/> collider and calculates collision details.
        /// </summary>
        /// <param name="aabb">The AABB collider to check collision against.</param>
        /// <param name="manifold">The collision manifold that will be filled with collision details if a collision is detected.</param>
        /// <returns>Returns <see langword="true"/> if a collision was detected, <see langword="false"/> otherwise.</returns>
        bool Visit(OBB other, out CollisionManifold manifold);

        /// <summary>
        /// Converts the collider to an <see cref="AABB"/>.
        /// </summary>
        /// <returns>The converted <see cref="AABB"/>.</returns>
        AABB GetAABB();

        /// <summary>
        /// Updates the collider's data, such as position,rotation.
        /// </summary>
        /// <param name="body">the body from that we will take data</param>
        void GetUpdatesFromBody(RigidBody body);

        Matrix3 CalculateInertiaTensor(float mass);
    }
}