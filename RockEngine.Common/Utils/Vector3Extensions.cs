using OpenTK.Mathematics;

namespace RockEngine.Common.Utils
{
    public static class Vector3Extensions
    {
        public static float ProjectOntoAxis(this Vector3 vector, Vector3 axis)
        {
            return Vector3.Dot(vector, Vector3.Normalize(axis));
        }
        public static Vector3 Scale(this Vector3 vector, Vector3 scale)
        {
             return new Vector3(vector.X * scale.X, vector.Y * scale.Y, vector.Z * scale.Z);
        }
    }
}
