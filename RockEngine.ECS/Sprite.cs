using OpenTK.Graphics.OpenGL4;

using RockEngine.Common;
using RockEngine.Common.Vertices;
using RockEngine.Rendering;
using RockEngine.Rendering.OpenGL;
using RockEngine.Rendering.OpenGL.Buffers;
using RockEngine.Rendering.OpenGL.Settings;
using RockEngine.Rendering.OpenGL.Textures;

namespace RockEngine.ECS
{
    internal sealed class Sprite : ARenderable<Vertex2D>, IComponent
    {
        private VAO _vao;
        private VBO _vbo;

        public bool IsSetupped => _vao != null && _vao.Handle != IGLObject.EMPTY_HANDLE;

        public GameObject? Parent { get; set; }

        public int Order => 999;

        public Texture SpriteTexture;

        public Sprite(Texture spriteTexture, in Vertex2D[ ] vertices) : base(in vertices)
        {
            Vertices = vertices;
            SpriteTexture = spriteTexture;
        }

        public void Setup(IRenderingContext context)
        {
            _vao = new VAO()
                .Setup(context)
                .Bind(context)
                .SetLabel(context);

            _vbo = new VBO(BufferSettings.DefaultVBOSettings with { BufferSize = Vertex2D.Size * Vertices.Length })
                .Setup(context)
                .Bind(context)
                .SendData(context, Vertices)
                .SetLabel(context);

            context.EnableVertexArrayAttrib(_vao.Handle, 0)
                .VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Vertex2D.Size, Vertex2D.PositionOffset)
                .EnableVertexArrayAttrib(_vao.Handle, 1)
                .VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vertex2D.Size, Vertex2D.TexCoordsOffset);

            _vbo.Unbind(context);
            _vao.Unbind(context);
        }

        public override void Render(IRenderingContext context)
        {
            SpriteTexture.BindIfNotBinded(context);

            _vao.BindIfNotBinded(context);
            context.DrawArrays(PrimitiveType.Triangles, 0, Vertices.Length);
            SpriteTexture.Unbind(context);
        }

        public void Dispose()
        {
            _vbo?.Dispose();
            _vao?.Dispose();
            SpriteTexture.Dispose();
            GC.SuppressFinalize(this);

        }

        public override void RenderOnEditorLayer()
        {
        }

        public void OnStart()
        {
            if(!IsSetupped)
            {
                IRenderingContext.Update(Setup);
            }
        }

        public void OnUpdate()
        {
        }

        public void OnDestroy()
        {
            Dispose();
        }

        public dynamic GetState()
        {
            return new
            {
                SpriteTexture = SpriteTexture
            };
        }

        public void SetState(dynamic state)
        {
            SpriteTexture = state.SpriteTexture;
        }
    }
}
