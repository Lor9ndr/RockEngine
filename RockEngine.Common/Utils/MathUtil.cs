using OpenTK.Mathematics;


namespace RockEngine.Common.Utils
{
    public static class MathUtil
    {
        public static Vector4 Transform(Vector4 vector, Matrix4 matrix)
        {
            Vector4 result;
            result.X = vector.X * matrix.M11 + vector.Y * matrix.M21 + vector.Z * matrix.M31 + vector.W * matrix.M41;
            result.Y = vector.X * matrix.M12 + vector.Y * matrix.M22 + vector.Z * matrix.M32 + vector.W * matrix.M42;
            result.Z = vector.X * matrix.M13 + vector.Y * matrix.M23 + vector.Z * matrix.M33 + vector.W * matrix.M43;
            result.W = vector.X * matrix.M14 + vector.Y * matrix.M24 + vector.Z * matrix.M34 + vector.W * matrix.M44;
            return result;
        }
    }
}
