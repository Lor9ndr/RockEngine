using OpenTK.Mathematics;

using RockEngine.Assets.Converters;
using RockEngine.OpenGL.Settings;
using RockEngine.OpenGL.Textures;

namespace RockEngine.Assets.AssetCreators
{
    public class TextureAssetCreator : AAssetCreator<Texture>
    {
        private readonly IConverter<TextureSettings> _settingsConverter;

        public TextureAssetCreator(IConverter<TextureSettings> settingsConverter)
        {
            _settingsConverter = settingsConverter;
        }
        public override Texture Load(string path)
        {
            Texture asset = new Texture();
            using var reader = new BinaryReader(File.Open(path, FileMode.OpenOrCreate));
            asset.Settings = _settingsConverter.Read(reader);
            asset.Path = reader.ReadString();
            asset.Name = reader.ReadString();
            asset.ID = new Guid(reader.ReadBytes(16));
            var type = (AssetType)reader.ReadInt32();
            asset.Size = new Vector2i(reader.ReadInt32(), reader.ReadInt32());
            return asset;
        }

        public override void Save<TAsset>(TAsset asset)
        {
            var texture = asset as Texture;
            using var writer = new BinaryWriter(File.Open(GetFullPath(texture), FileMode.OpenOrCreate));
            _settingsConverter.Write(texture.Settings, writer);
            writer.Write(texture.Path);
            writer.Write(texture.Name);
            writer.Write(texture.ID.ToByteArray());
            writer.Write((int)texture.Type);
            writer.Write(texture.Size.X);
            writer.Write(texture.Size.Y);
        }
    }
}
