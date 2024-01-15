﻿using OpenTK.Graphics.OpenGL4;

using RockEngine.Common.Vertices;
using RockEngine.Rendering.OpenGL;
using RockEngine.Rendering.OpenGL.Buffers;
using RockEngine.Rendering.OpenGL.Settings;
using RockEngine.Rendering.OpenGL.Textures;

namespace RockEngine.ECS
{
    internal sealed class Sprite : ARenderable<Vertex2D>, IComponent
    {
        private VAO? _vao;
        private VBO? _vbo;

        public bool IsSetupped => _vao != null && _vao.Handle != IGLObject.EMPTY_HANDLE;

        public GameObject? Parent { get; set; }

        public int Order => 999;

        public Texture SpriteTexture;

        public Sprite(Texture spriteTexture, in Vertex2D[ ] vertices) : base(in vertices)
        {
            Vertices = vertices;
            SpriteTexture = spriteTexture;
            Setup();
        }

        private void Setup()
        {
            _vao = new VAO()
                .Setup()
                .Bind()
                .SetLabel();

            _vbo = new VBO(BufferSettings.DefaultVBOSettings with { BufferSize = Vertex2D.Size * Vertices.Length })
                .Setup()
                .Bind()
                .SendData(Vertices)
                .SetLabel();

            _vao.EnableVertexArrayAttrib(0)
                .VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Vertex2D.Size, Vertex2D.PositionOffset)
                .EnableVertexArrayAttrib(1)
                .VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vertex2D.Size, Vertex2D.TexCoordsOffset);

            _vbo.Unbind();
            _vao.Unbind();
        }

        public override void Render()
        {
            SpriteTexture.BindIfNotBinded();

            _vao?.BindIfNotBinded();
            GL.DrawArrays(PrimitiveType.Triangles, 0, Vertices.Length);
            SpriteTexture.Unbind();
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
                Setup();
            }
        }

        public void OnUpdate()
        {
        }

        public void OnDestroy()
        {
            Dispose();
        }

        public void OnUpdateDevelepmentState()
        {
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