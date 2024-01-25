using OpenTK.Mathematics;

using RockEngine.Common;
using RockEngine.Common.Vertices;
using RockEngine.ECS;
using RockEngine.Physics;
using RockEngine.Rendering.OpenGL.Buffers.UBOBuffers;
using RockEngine.Rendering.OpenGL.Shaders;

using SkiaSharp;

namespace RockEngine.Editor.Physics
{
    internal sealed class DebugPhysicsRenderer : BaseWorldRenderer, IDisposable
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
        }

        public override void DrawLine(Vector3 start, Vector3 end, Vector3 color)
        {
            vertices.Add(new Vertex3D(start, color));
            vertices.Add(new Vertex3D(end, color));
        }

        public override void Render()
        {
            if(meshObject.IsSetupped)
            {
                _shader.Bind();
               TransformData td = new TransformData();
                td.Model = Matrix4.Identity;
                td.SendData();
                meshObject.Render();
                _shader.Unbind();
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
            meshObject.SetupMeshVertices(ref verticesArr);
            vertices.Clear();
        }

        public void Dispose()
        {
            meshObject.Dispose();
        }
    }
}
