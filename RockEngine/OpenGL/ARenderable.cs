using OpenTK.Mathematics;

using RockEngine.Engine.ECS;

using RockEngine.Editor;

namespace RockEngine.OpenGL
{
    internal abstract class ARenderable<TVertex> : IRenderable where TVertex : struct
    {
        private int _instances;
        private int[]? _indices;

        [UI]
        public RenderType RenderType { get; protected set; }

        public TVertex[] Vertices;

        public int[]? Indices { get => _indices; protected set => _indices = value; }

        public bool HasIndices => _indices is not null && _indices.Length != 0;

        public Matrix4[]? Models;

        [UI]
        public int InstanceCount
        {
            get => RenderType.HasFlag(RenderType.Instanced) || RenderType.HasFlag(RenderType.Indirect) ? _instances : 1;
            set => _instances = value;
        }

        public ARenderable(in TVertex[] vertices, in int[] indices)
        {
            Indices = indices;
            Vertices = vertices;
            RenderType = RenderType.Indices;
        }

        public ARenderable(in TVertex[] vertices)
        {
            Vertices = vertices;
            RenderType = RenderType.Forward;
        }

        public abstract void Render();

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
        public void SetInstanceMatrices(ref Matrix4[] matrices)
        {
            Models = matrices;
        }
        public void SetInstanceMatrices(List<Matrix4> matrices)
        {
            Models = matrices.ToArray();
        }

        public void SetInstanceMatrices(Transform[] transforms)
        {
            Models = new Matrix4[transforms.Length];
            for (int i = 0; i < transforms.Length; i++)
            {
                Models[i] = transforms[i].GetModelMatrix();
            }
        }

        public abstract void RenderOnEditorLayer();
    }
}
