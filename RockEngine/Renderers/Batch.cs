using RockEngine.ECS;
using RockEngine.Rendering;

namespace RockEngine.Renderers
{
    internal sealed class Batch
    {
        public MaterialComponent Material;
        public Mesh Mesh;

        public Batch(MaterialComponent material, Mesh mesh)
        {
            Material = material;
            Mesh = mesh;
        }

        public void Render(IRenderingContext context)
        {
            Material.Render(context);
            Mesh.Render(context);
        }
    }
}
