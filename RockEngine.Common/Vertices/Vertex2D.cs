using OpenTK.Mathematics;

using System.Runtime.InteropServices;

namespace RockEngine.Common.Vertices
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = Size)]
    public struct Vertex2D
    {
        public const int Size = sizeof(float) * (2 + 2);
        public static nint PositionOffset => Marshal.OffsetOf<Vertex2D>(nameof(Position));
        public static nint TexCoordsOffset => Marshal.OffsetOf<Vertex2D>(nameof(TexCoords));

        public Vector2 Position;
        public Vector2 TexCoords;
        public Vertex2D(Vector2 position, Vector2 texCoords)
        {
            Position = position;
            TexCoords = texCoords;
        }
    }
}
