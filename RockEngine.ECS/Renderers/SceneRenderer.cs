using OpenTK.Mathematics;

using RockEngine.ECS;
using RockEngine.ECS.Assets;
using RockEngine.Rendering;
using RockEngine.Rendering.Renderers;

namespace RockEngine.Renderers
{
    public sealed class SceneRenderer : IRenderer<Scene>
    {
        public void Render(IRenderingContext context, Scene item)
        {
            foreach(var meshEntry in item.GetGroupedGameObjects())
            {
                Mesh mesh = meshEntry.Key;
                var materialGroups = meshEntry.Value;
                if(!mesh.IsSetupped)
                {
                    continue;
                }

                int totalObjects = 0;
                foreach(var group in materialGroups.Values)
                {
                    totalObjects += group.Count;
                }
                Matrix4[] transformationMatrices = new Matrix4[totalObjects];
                Dictionary<Material, int> materialStartIndices = new Dictionary<Material, int>(materialGroups.Count);

                int currentIndex = 0;
                foreach(var materialGroup in materialGroups)
                {
                    materialStartIndices[materialGroup.Key] = currentIndex;
                    foreach(var gameObject in materialGroup.Value)
                    {
                        transformationMatrices[currentIndex++] = gameObject.Transform.GetModelMatrix();
                    }
                }
                mesh.PrepareSendingModel(context, transformationMatrices, 0, transformationMatrices.Length);

                foreach(var materialGroup in materialGroups)
                {
                    materialGroup.Key.SendData(context);
                    mesh.InstanceCount = materialGroup.Value.Count;

                    int startIndex = materialStartIndices[materialGroup.Key];
                    // Adjust instance attributes for the current material group
                    mesh.AdjustInstanceAttributesForGroup(context, startIndex);
                    mesh.Render(context);
                    foreach(var gameObject in materialGroup.Value)
                    {
                        gameObject.RenderWithoutMeshAndMaterial(context);
                    }
                }
            }
        }

        public void Update(Scene item)
        {
            foreach(var gameObject in item.GetGameObjects())
            {
                gameObject.Update();
            }
        }
    }
}
