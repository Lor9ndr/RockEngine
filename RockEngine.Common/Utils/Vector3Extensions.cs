using OpenTK.Mathematics;

namespace RockEngine.Common.Utils
{
    public static class Vector3Extensions
    {
        public static float ProjectOntoAxis(this Vector3 vector, Vector3 axis)
        {
            return Vector3.Dot(vector, Vector3.Normalize(axis));
        }
    }
}
