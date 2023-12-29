using OpenTK.Mathematics;
using RockEngine.Rendering;
using RockEngine.Common.Vertices;

namespace RockEngine.Assets.Assets
{
    public abstract class ABaseMesh : BaseAsset, IRenderable
    {

        public PrimitiveType PrimitiveType;
        public Vertex3D[ ]? Vertices;

        public int[ ]? Indices;

        public bool HasIndices => Indices is not null && Indices.Length != 0;

        public Matrix4[ ]? Models;

        public int InstanceCount { get; set; }

        public ABaseMesh(ref int[ ]? indices, ref Vertex3D[ ] vertices, string name, string path, Guid id)
            : base(path, name, id, AssetType.Mesh)
        {
            Indices = indices;
            Vertices = vertices;
            PrimitiveType = PrimitiveType.Triangles;

        }
        public ABaseMesh(ref Vertex3D[ ] vertices, string name, string path, Guid id)
            : base(path, name, id, AssetType.Mesh)
        {
            Vertices = vertices;
            PrimitiveType = PrimitiveType.Triangles;
        }

        protected ABaseMesh()
        {
            PrimitiveType = PrimitiveType.Triangles;
        }

        public abstract void Render();

        public void SetInstanceMatrices(in Matrix4[ ] matrices)
        {
            Models = matrices;
            InstanceCount = Models.Length;
        }
        public void SetInstanceMatrices(Matrix4[ ] matrices)
        {
            Models = matrices;
            InstanceCount = Models.Length;
        }
    }
}
