using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Assets;
using RockEngine.Engine.ECS.GameObjects;
using RockEngine.OpenGL.Buffers.UBOBuffers;
using RockEngine.OpenGL.Shaders;
using RockEngine.OpenGL.Vertices;

namespace RockEngine.BulletPhysics
{
    public sealed class DebugRenderer : IDisposable
    {
        public List<Vertex3D> DebugRenderVertices;
        public AShaderProgram ObjShader;
        public Mesh? DebugMesh;
        private readonly Camera _camera;

        public DebugRenderer(Camera camera)
        {
            DebugRenderVertices = new List<Vertex3D>();
            var vertices = DebugRenderVertices.ToArray();
            ObjShader = new VFShaderProgram("DebugRenderer",
                new VertexShader("Resources/Shaders/BoxRenderShader/DebugBox.vert"),
                new FragmentShader("Resources/Shaders/BoxRenderShader/DebugBox.frag"));
            _camera = camera;
            DebugMesh = new Mesh(ref vertices, "Collider mesh", string.Empty, Guid.Empty);
        }
      
            
        public void DebugRender()
        {
            if(DebugMesh is null || !DebugMesh.IsSetupped)
            {
                return;
            }
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            var td = new TransformData() { Model = Matrix4.Identity };
            ObjShader.Bind();
            td.SendData();
            _camera.Render();
            DebugMesh.Render();
            ObjShader.Unbind();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

        }
        public void PrepareOS()
        {
            if(DebugRenderVertices.Count == 0)
            {
                return;
            }
            DebugMesh?.Dispose();
            var vertices = DebugRenderVertices.ToArray();
            DebugMesh.SetupMeshVertices(ref vertices);
            DebugRenderVertices.Clear();

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
