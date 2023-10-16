using RockEngine.Assets;

namespace RockEngine.Assets.AssetCreators
{
    public abstract class AAssetCreator<T> : IAssetCreator<T> where T : IAsset
    {
        private const string EXTENSION_NAME = ".asset";
        public abstract void Save<TAsset>(TAsset asset) where TAsset : IAsset;
        public abstract T Load(string path);

        public virtual string GetFullPath(T asset)
        {
            var result = asset.Path + "\\" + asset.Name + EXTENSION_NAME;
            if (!Directory.Exists(asset.Path))
            {
                Directory.CreateDirectory(asset.Path);
            }
            return result;
        }
    }
}
