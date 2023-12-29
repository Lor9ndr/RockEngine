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
            if(Mesh is not null && !Mesh.IsSetupped)
            {
                if(Mesh.HasIndices)
                {
                    Mesh.SetupMeshIndicesVertices(ref Mesh.Indices!, ref Mesh.Vertices!);
                }
                else
                {
                    Mesh.SetupMeshVertices(ref Mesh.Vertices!);
                }
            }
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

        public void Render()
        {
            Mesh?.Render();
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
