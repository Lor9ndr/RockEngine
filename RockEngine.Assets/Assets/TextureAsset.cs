using RockEngine.Rendering.OpenGL.Textures;

namespace RockEngine.Assets.Assets
{
    internal class TextureAsset : BaseAsset
    {
        public BaseTexture Texture;
        public TextureAsset(string path, string name, Guid id, AssetType assetType)
            : base(path, name, id, assetType)
        {
        }
    }
}
