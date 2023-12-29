namespace RockEngine.ECS.Assets
{
    public interface IAssetCreator<out T> where T : IAsset
    {
        public void Save<TAsset>(TAsset asset) where TAsset : IAsset;
        public T Load(string path);
    }
}
