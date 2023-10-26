using RockEngine.Engine.ECS.GameObjects;
using RockEngine.Assets;
using RockEngine.OpenGL;

namespace RockEngine.Engine.ECS
{
    public class MeshComponent :  IComponent, IDisposable, IRenderable
    {
        public Mesh Mesh;

        public GameObject? Parent { get;set; }

        public int Order => 999;

        public MeshComponent(Mesh mesh)
        {
            Mesh = mesh;
        }

        public void OnStart()
        {
            if(!Mesh.IsSetupped)
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

        public void OnUpdateDevelepmentState()
        {
        }
        public void OnDestroy()
        {
        }

        public void Dispose()
        {
            Mesh.Dispose();
        }

        public void Render()
        {
            Mesh.Render();
        }

        public void RenderOnEditorLayer()
        {
            Mesh.RenderOnEditorLayer();
        }
    }
}
