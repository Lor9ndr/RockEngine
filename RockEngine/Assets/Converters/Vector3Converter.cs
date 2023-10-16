using OpenTK.Mathematics;

namespace RockEngine.Assets.Converters
{
    internal sealed class Vector3Converter : IConverter<Vector3>
    {
        public Vector3 Read(BinaryReader reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            return new Vector3(x, y, z);
        }

        public void Write(Vector3 data, BinaryWriter writer)
        {
            writer.Write(data.X);
            writer.Write(data.Y);
            writer.Write(data.Z);
        }
    }
}
