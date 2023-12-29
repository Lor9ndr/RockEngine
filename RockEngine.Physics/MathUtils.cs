using OpenTK.Mathematics;

namespace RockEngine.Physics
{
    internal static class MathUtils
    {
        public static float GetMagnitude(this Vector3 vector)
        {
            return MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }
        public static Vector3 Reflect(this Vector3 vector, Vector3 normal)
        {
            return vector - 2.0f * Vector3.Dot(vector, normal) * normal;
        }
    }
}
