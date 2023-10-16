using RockEngine.Assets.Converters;

namespace RockEngine.Assets.AssetCreators
{
    internal sealed class BaseAssetCreator : AAssetCreator<BaseAsset>
    {
        private readonly IConverter<BaseAsset> _baseConverter;

        public BaseAssetCreator(IConverter<BaseAsset> baseConverter)
        {
            _baseConverter = baseConverter;
        }
        public override BaseAsset Load(string path)
        {
            using var reader = new BinaryReader(File.Open(path, FileMode.OpenOrCreate));
            return _baseConverter.Read(reader);
        }

        public override void Save<TAsset>(TAsset asset)
        {
            var baseAsset = asset as BaseAsset;
            using var writer = new BinaryWriter(File.Open(GetFullPath(baseAsset), FileMode.OpenOrCreate));
            _baseConverter.Write(baseAsset, writer);
        }
    }
}
