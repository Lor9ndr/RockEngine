using System.Runtime.InteropServices;

namespace RockEngine.Assets
{
    public class BaseAsset : AAsset
    {
        public override AssetType Type { get; internal set; }
        public BaseAsset(string path, string name, Guid id, AssetType assetType)
           : base(path, name, id)
        {
            Type = assetType;
        }
        public BaseAsset()
            : base()
        {

        }
    }
}
