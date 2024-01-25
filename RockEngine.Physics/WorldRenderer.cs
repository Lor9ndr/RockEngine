using OpenTK.Mathematics;

namespace RockEngine.Physics
{
    public interface IWorldRenderer
    {
        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        /// <param name="start">The start point of the line.</param>
        /// <param name="endPoint">The end point of the line.</param>
        void DrawLine(Vector3 start, Vector3 endPoint, Vector3 color );

        /// <summary>
        /// Draws a sphere at the specified position with the specified radius.
        /// </summary>
        /// <param name="center">The center position of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        void DrawSphere(Vector3 center, float radius, Vector3 color);

        /// <summary>
        /// Draws a cube at the specified position with the specified size.
        /// </summary>
        /// <param name="center">The center position of the cube.</param>
        /// <param name="size">The size of the cube.</param>
        void DrawCube(Vector3 center, Vector3 size, Vector3 color);

        /// <summary>
        /// Draws a convex shape defined by the specified vertices.
        /// </summary>
        /// <param name="vertices">The vertices of the convex shape.</param>
        void DrawConvex(Vector3[] vertices, Vector3 position, Vector3 color);
        void DrawConvex(Vector3[] vertices, Vector3 position, Vector3 scale, Quaternion rotation, Vector3 color);

        void DrawConvex(Vector3[] vertices, Vector3 color);

        /// <summary>
        /// Draws a sphere at the specified position with the specified radius.
        /// </summary>
        /// <param name="transform">the struct to store position, rotation,scale of the body</param>

        void DrawSphere(Vector3 position, Vector3 scale, Quaternion rotation, Vector3 color);

        /// <summary>
        /// Draws a cube at the specified position with the specified size.
        /// </summary>
        /// <param name="transform">the struct to store position, rotation,scale of the body</param>
        void DrawCube(Vector3 position, Vector3 scale, Quaternion rotation, Vector3 color);

        /// <summary>
        /// Draws a convex shape defined by the specified vertices.
        /// </summary>
        /// <param name="vertices">The vertices of the convex shape.</param>
        void DrawConvex(Vector3[] vertices, Matrix4 transform, Vector3 color);

        /// <summary>
        /// Render pass
        /// </summary>
        void Render();

        /// <summary>
        /// Update pass
        /// </summary>
        void Update();
    }
}
