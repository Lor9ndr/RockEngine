﻿using OpenTK.Mathematics;

namespace RockEngine.Rendering.OpenGL
{
    public abstract class ARenderable<TVertex> : IRenderable where TVertex : struct
    {
        private int _instances;

        public RenderType RenderType { get; protected set; }

        public TVertex[ ] Vertices;

        public int[ ]? Indices;

        public bool HasIndices => Indices is not null && Indices.Length != 0;

        public Matrix4[ ]? Models;

        public int InstanceCount
        {
            get => RenderType.HasFlag(RenderType.Instanced) || RenderType.HasFlag(RenderType.Indirect) ? _instances : 1;
            set => _instances = value;
        }

        public ARenderable(in TVertex[ ] vertices, in int[ ] indices)
        {
            Indices = indices;
            Vertices = vertices;
            RenderType = RenderType.Indices;
        }

        public ARenderable(in TVertex[ ] vertices)
        {
            Vertices = vertices;
            RenderType = RenderType.Forward;
        }

        public abstract void Render(IRenderingContext context);

        /// <summary>
        /// Render without using indices (EBO)
        /// </summary>
        public void ForwardRender()
        {
            RenderType = RenderType.Forward;
            _instances = 1;
        }

        /// <summary>
        /// Render with using indices (EBO)
        /// </summary>
        public void IndicesRender()
        {
            RenderType = RenderType.Indices;
            _instances = 1;
        }

        public void IndirectRendering(int quantity)
        {
            RenderType = RenderType.Indirect;
            _instances = quantity;
        }

        /// <summary>
        /// Render at one time <paramref name="quantity"/> entities
        /// If we have indices, then we render with using them
        /// </summary>
        /// <param name="quantity">Number of instances to render</param>
        public void InstancedRender(int quantity)
        {
            InstanceCount = quantity;
            RenderType = HasIndices ? RenderType.IndicesAndInstanced : RenderType.Instanced;
        }
        public void SetInstanceMatrices(ref Matrix4[ ] matrices)
        {
            Models = matrices;
        }
        public void SetInstanceMatrices(Matrix4[ ] matrices)
        {
            Models = matrices;
        }
        public abstract void RenderOnEditorLayer();
    }
}
