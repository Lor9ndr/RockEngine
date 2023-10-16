namespace RockEngine.Assets.Converters
{
    internal sealed class BaseAssetConverter : IConverter<BaseAsset>
    {
        public BaseAsset Read(BinaryReader reader)
        {
            // читаем id, name и path проекта
            var path = reader.ReadString();
            var name = reader.ReadString();
            var id = new Guid(reader.ReadBytes(16));
            AssetType type = (AssetType)reader.ReadInt32();
            return new BaseAsset(path, name, id, type);
        }

        public void Write(BaseAsset asset, BinaryWriter writer)
        {
            writer.Write(asset.Path);
            writer.Write(asset.Name);
            writer.Write(asset.ID.ToByteArray());
            writer.Write((int)asset.Type);
        }
    }
}
