using RockEngine.ECS;

namespace RockEngine.Renderers
{
    internal class Batch
    {
        public MaterialComponent Material;
        public Mesh Mesh;

        public Batch(MaterialComponent material, Mesh mesh)
        {
            Material = material;
            Mesh = mesh;
        }

        public void Render()
        {
            Material.Render();
            Mesh.Render();
        }
    }
}
