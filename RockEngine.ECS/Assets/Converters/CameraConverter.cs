using OpenTK.Mathematics;

using RockEngine.ECS.GameObjects;

namespace RockEngine.ECS.Assets.Converters
{
    internal sealed class CameraConverter : IConverter<Camera>
    {
        private readonly IConverter<Vector3> _v3Converter;

        public CameraConverter(IConverter<Vector3> v3Converter)
        {
            _v3Converter = v3Converter;
        }

        public Camera Read(BinaryReader reader)
        {
            Camera cam = new Camera(1);
            cam.Front = _v3Converter.Read(reader);
            cam.Yaw = reader.ReadSingle();
            cam.Pitch = reader.ReadSingle();
            cam.AspectRatio = reader.ReadSingle();
            return cam;
        }

        public void Write(Camera data, BinaryWriter writer)
        {
            _v3Converter.Write(data.Front, writer);
            writer.Write(data.Yaw);
            writer.Write(data.Pitch);
            writer.Write(data.AspectRatio);

        }
    }
}
