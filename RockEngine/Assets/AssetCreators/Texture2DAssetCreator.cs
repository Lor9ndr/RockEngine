using OpenTK.Mathematics;

using RockEngine.Assets;
using RockEngine.Assets.Converters;
using RockEngine.OpenGL.Settings;
using RockEngine.OpenGL.Textures;

using SkiaSharp;

namespace RockEngine.Assets.AssetCreators
{
    public class Texture2DAssetCreator : AAssetCreator<Texture2D>
    {
        private readonly IConverter<TextureSettings> _settingsConverter;

        internal Texture2DAssetCreator(IConverter<TextureSettings> settingsConverter)
        {
            _settingsConverter = settingsConverter;
        }
        public override Texture2D Load(string path)
        {
            Texture2D asset = new Texture2D();
            using var reader = new BinaryReader(File.Open(path, FileMode.OpenOrCreate));
            asset.Settings = _settingsConverter.Read(reader);
            asset.Path = reader.ReadString();
            asset.Name = reader.ReadString();
            asset.ID = new Guid(reader.ReadBytes(16));
            var type = (AssetType)reader.ReadInt32();
            asset.Size = new Vector2i(reader.ReadInt32(), reader.ReadInt32());
            asset.Bitmap = SKBitmap.Decode(reader.BaseStream);
            reader.Close();
            return asset;
        }

        public override void Save<TAsset>(TAsset asset)
        {
            var texture = asset as Texture2D;

            using var writer = new BinaryWriter(File.Open(GetFullPath(texture), FileMode.OpenOrCreate));
            _settingsConverter.Write(texture.Settings, writer);
            writer.Write(texture.Path);
            writer.Write(texture.Name);
            writer.Write(texture.ID.ToByteArray());
            writer.Write((int)texture.Type);
            writer.Write(texture.Size.X);
            writer.Write(texture.Size.Y);
            SKBitmap bitmap;

            // Если предварительно загрузили в ассет битмап и не освободили память
            if (texture.Bitmap != null)
            {
                bitmap = texture.Bitmap;
            }
            else
            {
                bitmap = GetBitmap(texture);
            }

            var data = bitmap.Encode(SKEncodedImageFormat.Png, 0);
            texture.BitmapSize = data.Size;
            writer.Write(texture.BitmapSize);
            data.SaveTo(writer.BaseStream);
            writer.Close();
            // Just be sure that loaded image is disposed and not required untill render
            texture.Bitmap?.Dispose();
            texture.Bitmap = null;
        }
        public SKBitmap GetBitmap(Texture2D asset)
        {
            using var reader = new BinaryReader(File.Open(GetFullPath(asset), FileMode.Open));
            // skip reading other fields by seeking to the position where Bitmap start
            reader.BaseStream.Seek(reader.BaseStream.Length - asset.BitmapSize, SeekOrigin.Begin);

            var bytes = reader.ReadBytes((int)asset.BitmapSize);
            return SKBitmap.Decode(bytes);
        }
    }
}
