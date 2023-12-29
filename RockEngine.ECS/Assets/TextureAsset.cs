using RockEngine.Rendering.OpenGL.Textures;

namespace RockEngine.ECS.Assets
{
    public class TextureAsset : BaseAsset
    {
        public BaseTexture Texture;

        public TextureAsset(string path, string name, Guid id, AssetType assetType, BaseTexture texture)
            : base(path, name, id, assetType)
        {
            Texture = texture;
        }
    }
}
