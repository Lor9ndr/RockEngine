using RockEngine.Assets.Converters;

namespace RockEngine.Assets.AssetCreators
{
    internal sealed class ProjectAssetCreator : AAssetCreator<Project>
    {
        private readonly IConverter<BaseAsset> _baseConverter;

        public ProjectAssetCreator(IConverter<BaseAsset> baseConverter)
        {
            _baseConverter = baseConverter;
        }
        public override Project Load(string path)
        {
            using var reader = new BinaryReader(File.Open(path, FileMode.Open));
            // читаем id, name и path проекта
            var baseProject = _baseConverter.Read(reader);
            return new Project(baseProject);
        }

        public override void Save<TAsset>(TAsset asset)
        {
            var project = asset as Project;
            using var writer = new BinaryWriter(File.Open(GetFullPath(project), FileMode.OpenOrCreate));
            _baseConverter.Write(project, writer);
        }
    }
}
