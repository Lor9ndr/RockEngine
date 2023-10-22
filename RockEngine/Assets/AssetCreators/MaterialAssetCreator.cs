using OpenTK.Mathematics;

using RockEngine.Assets.Converters;
using RockEngine.Engine.ECS;
using RockEngine.OpenGL.Shaders;

namespace RockEngine.Assets.AssetCreators
{
    internal sealed class MaterialAssetCreator : AAssetCreator<MaterialComponent>
    {
        private readonly IConverter<Vector3> _v3Converter;

        public MaterialAssetCreator(IConverter<Vector3> v3Converter)
        {
            _v3Converter = v3Converter;
        }
        public override void Save<TAsset>(TAsset asset)
        {
            MaterialComponent mcomp = asset as MaterialComponent;
            using var writer = new BinaryWriter(File.Open(GetFullPath(mcomp), FileMode.OpenOrCreate));
            writer.Write(mcomp.Path);
            writer.Write(mcomp.Name);
            writer.Write(mcomp.ID.ToString());
            writer.Write((int)mcomp.Type);

            _v3Converter.Write(mcomp.AlbedoColor, writer);
            writer.Write(mcomp.Ao);
            writer.Write(mcomp.Metallic);
            writer.Write(mcomp.Roughness);
        }

        public override MaterialComponent Load(string path)
        {
            MaterialComponent m = new MaterialComponent();
            using var reader = new BinaryReader(File.Open(GetFullPath(m), FileMode.OpenOrCreate));

            m.Path = reader.ReadString();
            m.Name = reader.ReadString();
            m.ID = Guid.Parse(reader.ReadString());
            var x = (AssetType)reader.ReadInt32();
            m.AlbedoColor = _v3Converter.Read(reader);
            m.Ao = reader.ReadSingle();
            m.Metallic = reader.ReadSingle();
            m.Roughness = reader.ReadSingle();
            return m;
        }
    }
}
