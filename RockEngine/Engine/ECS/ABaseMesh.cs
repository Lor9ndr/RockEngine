﻿using Newtonsoft.Json;

using OpenTK.Mathematics;

using RockEngine.Assets;
using RockEngine.OpenGL;
using RockEngine.OpenGL.Vertices;

namespace RockEngine.Engine.ECS
{
    public abstract class ABaseMesh : BaseAsset, IRenderable
    {

        public Vertex3D[]? Vertices;

        public int[]? Indices;

        [JsonIgnore]
        public bool HasIndices => Indices is not null && Indices.Length != 0;

        public Matrix4[]? Models;

        public List<Transform> Transforms;

        public int InstanceCount { get; set; }

        public ABaseMesh(ref int[] indices)
            : this()
            => Indices = indices;

        public ABaseMesh(ref Vertex3D[] vertices, ref int[] indices)
         : this()
        {
            Indices = indices;
            Vertices = vertices;
        }

        public ABaseMesh()
            : base(Project.CurrentProject!.Path, "Mesh", Guid.NewGuid(), AssetType.Mesh)
        {
            Transforms = new List<Transform>();
        }

        public ABaseMesh(ref int[]? indices, ref Vertex3D[] vertices, string name, string path, Guid id)
            : base(path, name, id, AssetType.Mesh)
        {
            Indices = indices;
            Vertices = vertices;
            Indices = indices;
            Transforms = new List<Transform>();

        }
        public ABaseMesh(ref Vertex3D[] vertices, string name, string path, Guid id)
            : base(path, name, id, AssetType.Mesh)
        {
            Vertices = vertices;
            Transforms = new List<Transform>();
        }

        public abstract void Render();

        public void SetInstanceMatrices(in Matrix4[] matrices)
        {
            Models = matrices;
            InstanceCount = Models.Length;
        }
        public void SetInstanceMatrices(List<Matrix4> matrices)
        {
            Models = matrices.ToArray();
            InstanceCount = Models.Length;
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
