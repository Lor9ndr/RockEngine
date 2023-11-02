using OpenTK.Mathematics;

using RockEngine.Editor;

namespace RockEngine.Assets
{
    public class Material : BaseAsset
    {
        [UI(isColor: true)] public Vector3 AlbedoColor;
        [UI] public float Metallic;
        [UI] public float Roughness;
        [UI] public float Ao;
        public Material(string path, string name, Guid id) 
            : base(path, name, id, AssetType.Material)
        {
            AlbedoColor = new Vector3(0.7f);
            Metallic = 1.0f;
            Roughness = 1.0f;
            Ao = 1.0f;
        }
    }
}
