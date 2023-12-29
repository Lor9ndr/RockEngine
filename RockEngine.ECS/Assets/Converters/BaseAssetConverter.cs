namespace RockEngine.ECS.Assets.Converters
{
    public sealed class BaseAssetConverter : IConverter<BaseAsset>
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

        public void Write(BaseAsset data, BinaryWriter writer)
        {
            writer.Write(data.Path);
            writer.Write(data.Name);
            writer.Write(data.ID.ToByteArray());
            writer.Write((int)data.Type);
        }
    }
}
