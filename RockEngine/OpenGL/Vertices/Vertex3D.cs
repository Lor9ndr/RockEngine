using OpenTK.Mathematics;

using System.Runtime.InteropServices;

namespace RockEngine.OpenGL.Vertices
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = Size)]
    public struct Vertex3D
    {
        public const int Size = sizeof(float) * (3 + 3 + 2);
        public static nint PositionOffset => Marshal.OffsetOf<Vertex3D>(nameof(Position));
        public static nint NormalOffset => Marshal.OffsetOf<Vertex3D>(nameof(Normal));
        public static nint TexCoordsOffset => Marshal.OffsetOf<Vertex3D>(nameof(TexCoords));

        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoords;

        public Vertex3D(Vector3 position, Vector3 normal, Vector2 texCoords)
        {
            Position = position;
            Normal = normal;
            TexCoords = texCoords;
        }
        public Vertex3D(float posx, float posy, float posz, float normalx, float normaly, float normalz, float texCoordx, float texCoordy)
        {
            Position = new Vector3(posx, posy, posz);
            Normal = new Vector3(normalx, normaly, normalz);
            TexCoords = new Vector2(texCoordx, texCoordy);
        }

        public Vertex3D(Vector3 position)
        {
            Position = position;
            Normal = Vector3.Zero;
            TexCoords = Vector2.Zero;
        }
        public Vertex3D(Vector3 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
            TexCoords = Vector2.Zero;
        }

        public Vertex3D()
        {
        }

        public static Vertex3D[] CubeVertices = new Vertex3D[]
            {
               new Vertex3D(-1.0f, -1.0f, -1.0f,  0.0f,  0.0f, -1.0f, 0.0f, 0.0f), // bottom-left
               new Vertex3D( 1.0f,  1.0f, -1.0f,  0.0f,  0.0f, -1.0f, 1.0f, 1.0f), // top-right
               new Vertex3D( 1.0f, -1.0f, -1.0f,  0.0f,  0.0f, -1.0f, 1.0f, 0.0f), // bottom-right         
               new Vertex3D( 1.0f,  1.0f, -1.0f,  0.0f,  0.0f, -1.0f, 1.0f, 1.0f), // top-right
               new Vertex3D(-1.0f, -1.0f, -1.0f,  0.0f,  0.0f, -1.0f, 0.0f, 0.0f), // bottom-left
               new Vertex3D(-1.0f,  1.0f, -1.0f,  0.0f,  0.0f, -1.0f, 0.0f, 1.0f), // top-left
               new Vertex3D(-1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f, 0.0f), // bottom-left
               new Vertex3D( 1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f, 0.0f), // bottom-right
               new Vertex3D( 1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f, 1.0f), // top-right
               new Vertex3D( 1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f, 1.0f), // top-right
               new Vertex3D(-1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f, 1.0f), // top-left
               new Vertex3D(-1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f, 0.0f), // bottom-left
               new Vertex3D(-1.0f,  1.0f,  1.0f, -1.0f,  0.0f,  0.0f, 1.0f, 0.0f), // top-right
               new Vertex3D(-1.0f,  1.0f, -1.0f, -1.0f,  0.0f,  0.0f, 1.0f, 1.0f), // top-left
               new Vertex3D(-1.0f, -1.0f, -1.0f, -1.0f,  0.0f,  0.0f, 0.0f, 1.0f), // bottom-left
               new Vertex3D(-1.0f, -1.0f, -1.0f, -1.0f,  0.0f,  0.0f, 0.0f, 1.0f), // bottom-left
               new Vertex3D(-1.0f, -1.0f,  1.0f, -1.0f,  0.0f,  0.0f, 0.0f, 0.0f), // bottom-right
               new Vertex3D(-1.0f,  1.0f,  1.0f, -1.0f,  0.0f,  0.0f, 1.0f, 0.0f), // top-right
               new Vertex3D( 1.0f,  1.0f,  1.0f,  1.0f,  0.0f,  0.0f, 1.0f, 0.0f), // top-left
               new Vertex3D( 1.0f, -1.0f, -1.0f,  1.0f,  0.0f,  0.0f, 0.0f, 1.0f), // bottom-right
               new Vertex3D( 1.0f,  1.0f, -1.0f,  1.0f,  0.0f,  0.0f, 1.0f, 1.0f), // top-right         
               new Vertex3D( 1.0f, -1.0f, -1.0f,  1.0f,  0.0f,  0.0f, 0.0f, 1.0f), // bottom-right
               new Vertex3D( 1.0f,  1.0f,  1.0f,  1.0f,  0.0f,  0.0f, 1.0f, 0.0f), // top-left
               new Vertex3D( 1.0f, -1.0f,  1.0f,  1.0f,  0.0f,  0.0f, 0.0f, 0.0f), // bottom-left     
               new Vertex3D(-1.0f, -1.0f, -1.0f,  0.0f, -1.0f,  0.0f, 0.0f, 1.0f), // top-right
               new Vertex3D( 1.0f, -1.0f, -1.0f,  0.0f, -1.0f,  0.0f, 1.0f, 1.0f), // top-left
               new Vertex3D( 1.0f, -1.0f,  1.0f,  0.0f, -1.0f,  0.0f, 1.0f, 0.0f), // bottom-left
               new Vertex3D( 1.0f, -1.0f,  1.0f,  0.0f, -1.0f,  0.0f, 1.0f, 0.0f), // bottom-left
               new Vertex3D(-1.0f, -1.0f,  1.0f,  0.0f, -1.0f,  0.0f, 0.0f, 0.0f), // bottom-right
               new Vertex3D(-1.0f, -1.0f, -1.0f,  0.0f, -1.0f,  0.0f, 0.0f, 1.0f), // top-right
               new Vertex3D(-1.0f,  1.0f, -1.0f,  0.0f,  1.0f,  0.0f, 0.0f, 1.0f), // top-left
               new Vertex3D( 1.0f,  1.0f , 1.0f,  0.0f,  1.0f,  0.0f, 1.0f, 0.0f), // bottom-right
               new Vertex3D( 1.0f,  1.0f, -1.0f,  0.0f,  1.0f,  0.0f, 1.0f, 1.0f), // top-right     
               new Vertex3D( 1.0f,  1.0f,  1.0f,  0.0f,  1.0f,  0.0f, 1.0f, 0.0f), // bottom-right
               new Vertex3D(-1.0f,  1.0f, -1.0f,  0.0f,  1.0f,  0.0f, 0.0f, 1.0f), // top-left
               new Vertex3D(-1.0f,  1.0f,  1.0f,  0.0f,  1.0f,  0.0f, 0.0f, 0.0f) // bottom-left        
            };
        public static int[] CreateCubeIndices()
        {
            // Define the indices for each face of the cube
            int[] indices = new int[]
            {
                // front
		        0, 1, 2,
                2, 3, 0,
		        // right
		        1, 5, 6,
                6, 2, 1,
		        // back
		        7, 6, 5,
                5, 4, 7,
		        // left
		        4, 0, 3,
                3, 7, 4,
		        // bottom
		        4, 5, 1,
                1, 0, 4,
		        // top
		        3, 2, 6,
                6, 7, 3
            };

            return indices;
        }
    }
}
