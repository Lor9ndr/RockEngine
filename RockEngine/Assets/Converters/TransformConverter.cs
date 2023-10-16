using OpenTK.Mathematics;

using RockEngine.Engine.ECS;

namespace RockEngine.Assets.Converters
{
    internal sealed class TransformConverter : IConverter<Transform>
    {
        private readonly IConverter<Vector3> _v3Converter;

        public TransformConverter(IConverter<Vector3> v3converter)
        {
            _v3Converter = v3converter;
        }
        public Transform Read(BinaryReader reader)
        {
            Transform t = new Transform();
            t.Position = _v3Converter.Read(reader);
            t.Rotation = _v3Converter.Read(reader);
            t.Scale = _v3Converter.Read(reader);
            return t;
        }

        public void Write(Transform data, BinaryWriter writer)
        {
            _v3Converter.Write(data.Position, writer);
            _v3Converter.Write(data.Rotation, writer);
            _v3Converter.Write(data.Scale, writer);
        }
    }
}
