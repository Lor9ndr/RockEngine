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

            writer.Write(mcomp.Shininess);
            _v3Converter.Write(mcomp.DiffuseColor, writer);
            _v3Converter.Write(mcomp.AmbientColor, writer);
            _v3Converter.Write(mcomp.SpecularColor, writer);
            writer.Write(mcomp.Shader.ID.ToByteArray());
        }

        public override MaterialComponent Load(string path)
        {
            MaterialComponent m = new MaterialComponent();
            using var reader = new BinaryReader(File.Open(GetFullPath(m), FileMode.OpenOrCreate));

            m.Path = reader.ReadString();
            m.Name = reader.ReadString();
            m.ID = Guid.Parse(reader.ReadString());
            var x = (AssetType)reader.ReadInt32();
            m.Shininess = reader.ReadSingle();
            m.DiffuseColor = _v3Converter.Read(reader);
            m.AmbientColor = _v3Converter.Read(reader);
            m.SpecularColor = _v3Converter.Read(reader);
            m.Shader = (AShaderProgram)AssetManager.GetAssetByID(new Guid(reader.ReadBytes(16)));
            return m;
        }
    }
}
