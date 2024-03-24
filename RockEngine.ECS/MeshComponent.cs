using RockEngine.Common;
using RockEngine.Rendering;

using System.Text.Json.Serialization;

namespace RockEngine.ECS
{
    public class MeshComponent : IComponent, IDisposable, IRenderable
    {
        public Mesh? Mesh;

        public GameObject? Parent { get; set; }

        public int Order => 999;

        [JsonConstructor]
        public MeshComponent(Mesh mesh)
        {
            Mesh = mesh;
        }

        public MeshComponent()
        {

        }

        public void OnStart()
        {
            IRenderingContext.Update(context =>
            {
                if(Mesh is not null && !Mesh.IsSetupped)
                {
                    if(Mesh.HasIndices)
                    {
                        Mesh.SetupMeshIndicesVertices(context,Mesh.Indices!, Mesh.Vertices!);
                    }
                    else
                    {
                        Mesh.SetupMeshVertices(context, Mesh.Vertices!);
                    }
                }
            });
            
        }

        public void Render(IRenderingContext context)
        {
            Mesh?.Render(context);
        }

        public void OnUpdate()
        {
        }

        public void OnDestroy()
        {
        }

        public void Dispose()
        {
            Mesh?.Dispose();
        }

        public dynamic GetState()
        {
            return new
            {
                Mesh = Mesh
            };
        }

        public void SetState(dynamic state)
        {
            Mesh = state.Mesh;
        }
    }
}
