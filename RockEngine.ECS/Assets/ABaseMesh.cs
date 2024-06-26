﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Common;
using RockEngine.Common.Vertices;
using RockEngine.Rendering;

namespace RockEngine.ECS.Assets
{
    public abstract class ABaseMesh : BaseAsset, IRenderable
    {

        public PrimitiveType PrimitiveType;
        public Vertex3D[]? Vertices;

        public int[]? Indices;

        public bool HasIndices => Indices is not null && Indices.Length != 0;

        public int InstanceCount { get; set; } = 1;

        public ABaseMesh(ref int[]? indices, ref Vertex3D[] vertices, string name, string path, Guid id)
            : base(path, name, id, AssetType.Mesh)
        {
            Indices = indices;
            Vertices = vertices;
            PrimitiveType = PrimitiveType.Triangles;

        }
        public ABaseMesh(ref Vertex3D[] vertices, string name, string path, Guid id)
            : base(path, name, id, AssetType.Mesh)
        {
            Vertices = vertices;
            PrimitiveType = PrimitiveType.Triangles;
        }

        protected ABaseMesh()
        {
            PrimitiveType = PrimitiveType.Triangles;
        }

        public abstract void Render(IRenderingContext context);
    }
}
