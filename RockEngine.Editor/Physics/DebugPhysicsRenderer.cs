using OpenTK.Mathematics;

using RockEngine.Common;
using RockEngine.Common.Vertices;
using RockEngine.ECS;
using RockEngine.Physics;
using RockEngine.Physics.Colliders;
using RockEngine.Physics.Drawing;
using RockEngine.Rendering;
using RockEngine.Rendering.OpenGL.Buffers.UBOBuffers;
using RockEngine.Rendering.OpenGL.Shaders;

namespace RockEngine.Editor.Physics
{
    public sealed class DebugPhysicsRenderer : ColliderRenderer, IDisposable
    {
        private readonly Mesh meshObject;
        private readonly List<Vertex3D> vertices;
        private readonly ShaderProgram _shader;

        public DebugPhysicsRenderer()
        {
            meshObject = new Mesh();
            meshObject.PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType.Lines;
            vertices = new List<Vertex3D>();
            _shader = ShaderProgram.GetOrCreate("DebugBox", 
                new VertexShader(PathConstants.RESOURCES + PathConstants.SHADERS+ "/BoxRenderShader/DebugBox.vert"),
                new FragmentShader(PathConstants.RESOURCES + PathConstants.SHADERS + "/BoxRenderShader/DebugBox.frag"));
            IRenderingContext.Update(context => _shader.Setup(context));
        }

        public override void DrawLine(Vector3 startVertex, Vector3 endVertex, Vector4 color)
        {
            vertices.Add(new Vertex3D(startVertex, color.Xyz));
            vertices.Add(new Vertex3D(endVertex, color.Xyz));
        }

        public override void DrawAABB(AABB aabb, Vector4 color)
        {
            var min = aabb.Center + aabb.Min;
            var max = aabb.Center + aabb.Max;

            // Bottom square
            DrawLine(new Vector3(min.X, min.Y, min.Z), new Vector3(max.X, min.Y, min.Z), color);
            DrawLine(new Vector3(max.X, min.Y, min.Z), new Vector3(max.X, min.Y, max.Z), color);
            DrawLine(new Vector3(max.X, min.Y, max.Z), new Vector3(min.X, min.Y, max.Z), color);
            DrawLine(new Vector3(min.X, min.Y, max.Z), new Vector3(min.X, min.Y, min.Z), color);

            // Top square
            DrawLine(new Vector3(min.X, max.Y, min.Z), new Vector3(max.X, max.Y, min.Z), color);
            DrawLine(new Vector3(max.X, max.Y, min.Z), new Vector3(max.X, max.Y, max.Z), color);
            DrawLine(new Vector3(max.X, max.Y, max.Z), new Vector3(min.X, max.Y, max.Z), color);
            DrawLine(new Vector3(min.X, max.Y, max.Z), new Vector3(min.X, max.Y, min.Z), color);

            // Connecting lines between top and bottom
            DrawLine(new Vector3(min.X, min.Y, min.Z), new Vector3(min.X, max.Y, min.Z), color);
            DrawLine(new Vector3(max.X, min.Y, min.Z), new Vector3(max.X, max.Y, min.Z), color);
            DrawLine(new Vector3(max.X, min.Y, max.Z), new Vector3(max.X, max.Y, max.Z), color);
            DrawLine(new Vector3(min.X, min.Y, max.Z), new Vector3(min.X, max.Y, max.Z), color);
        }

        public override void DrawOBB(OBB obb, Vector4 color)
        {
            // Define the local space corners of the OBB
            Vector3[] localCorners = new Vector3[8]
            {
                new Vector3(-obb.Extents.X, -obb.Extents.Y, -obb.Extents.Z),
                new Vector3(obb.Extents.X, -obb.Extents.Y, -obb.Extents.Z),
                new Vector3(obb.Extents.X, -obb.Extents.Y, obb.Extents.Z),
                new Vector3(-obb.Extents.X, -obb.Extents.Y, obb.Extents.Z),
                new Vector3(-obb.Extents.X, obb.Extents.Y, -obb.Extents.Z),
                new Vector3(obb.Extents.X, obb.Extents.Y, -obb.Extents.Z),
                new Vector3(obb.Extents.X, obb.Extents.Y, obb.Extents.Z),
                new Vector3(-obb.Extents.X, obb.Extents.Y, obb.Extents.Z)
            };

            // Transform the corners to world space
            Vector3[] worldCorners = new Vector3[8];
            for(int i = 0; i < localCorners.Length; i++)
            {
                worldCorners[i] = Vector3.Transform(localCorners[i], obb.Rotation) + obb.Center;
            }

            // Draw bottom square
            DrawLine(worldCorners[0], worldCorners[1], color);
            DrawLine(worldCorners[1], worldCorners[2], color);
            DrawLine(worldCorners[2], worldCorners[3], color);
            DrawLine(worldCorners[3], worldCorners[0], color);

            // Draw top square
            DrawLine(worldCorners[4], worldCorners[5], color);
            DrawLine(worldCorners[5], worldCorners[6], color);
            DrawLine(worldCorners[6], worldCorners[7], color);
            DrawLine(worldCorners[7], worldCorners[4], color);

            // Draw connecting lines between top and bottom
            DrawLine(worldCorners[0], worldCorners[4], color);
            DrawLine(worldCorners[1], worldCorners[5], color);
            DrawLine(worldCorners[2], worldCorners[6], color);
            DrawLine(worldCorners[3], worldCorners[7], color);
        }

      

        public override void Render()
        {
            if(meshObject.IsSetupped)
            {
                IRenderingContext.Render(context =>
                {
                    _shader.Bind(context);
                    TransformData td = new TransformData();
                    td.Model = Matrix4.Identity;
                    td.SendData(context);
                    meshObject.Render(context);
                    _shader.Unbind(context);
                });
            }
        }

        public override void Update()
        {
            if(vertices.Count == 0)
            {
                return;
            }
            meshObject.Dispose();
            var verticesArr = vertices.ToArray();
            IRenderingContext.Update(context =>
            {
                meshObject.SetupMeshVertices(context, verticesArr);

            });
            vertices.Clear();
        }

        public void Dispose()
        {
            meshObject.Dispose();
        }
    }
}
