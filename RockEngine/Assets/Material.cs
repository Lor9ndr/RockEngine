namespace RockEngine.Assets
{
    public class Material : BaseAsset
    {
        public Material(string path, string name, Guid id) 
            : base(path, name, id, AssetType.Material)
        {
        }
    }
}
