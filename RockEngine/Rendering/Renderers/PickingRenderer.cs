﻿using OpenTK.Mathematics;

using RockEngine.Assets;
using RockEngine.Engine.ECS;
using RockEngine.Engine.ECS.GameObjects;
using RockEngine.OpenGL.Shaders;
using RockEngine.OpenGL;
using RockEngine.OpenGL.Buffers.UBOBuffers;
using OpenTK.Graphics.OpenGL4;

namespace RockEngine.Rendering.Renderers
{
    public sealed class PickingRenderer : IRenderer, IDisposable
    {
        private readonly PickingTexture PickingTexture;
        private readonly AShaderProgram PickingShader;

        public PickingRenderer(Vector2i size)
        {
            var basePicking = Path.Combine(PathConstants.RESOURCES, PathConstants.SHADERS, PathConstants.DEBUG, PathConstants.PICKING);
            PickingTexture = new PickingTexture(size);
            PickingShader = ShaderProgram.GetOrCreate("PickingShader",
               new VertexShader(Path.Combine(basePicking, "Picking.vert")),
             new FragmentShader(Path.Combine(basePicking, "Picking.frag")));
        }

        public void Begin()
        {
            PickingShader.BindIfNotBinded();
            GL.Disable(EnableCap.Blend);
            PickingTexture.BeginWrite();
        }

        public void Render(GameObject go)
        {
            PickingData pd = new PickingData()
            {
                gObjectIndex = go.GameObjectID,
                gDrawIndex = 0,
            };
            pd.SendData();
            go.Render();
        }

        public void End()
        {
            PickingTexture.EndWrite();
            PickingShader.Unbind();
        }

        public void Render(IComponent component)
        {
            throw new NotImplementedException("Unable to pick a component");
        }

        public void Dispose()
        {
        }

        public void ResizeTexture(Vector2i size)
        {
            PickingTexture.CheckSize(size);
        }

        public PickingTexture.PixelInfo GetPixelData(int x, int y)
        {
            return PickingTexture.ReadPixel(x, y);
        }
    }
}