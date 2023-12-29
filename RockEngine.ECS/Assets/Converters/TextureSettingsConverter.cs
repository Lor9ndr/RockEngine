using OpenTK.Graphics.OpenGL4;

using RockEngine.Rendering.OpenGL.Settings;

namespace RockEngine.ECS.Assets.Converters
{
    public sealed class TextureSettingsConverter : IConverter<TextureSettings>
    {
        public TextureSettings Read(BinaryReader reader)
        {
            TextureSettings settings = new TextureSettings();
            var textureParametersCount = reader.ReadInt32();
            if(textureParametersCount > 0)
            {
                settings.TextureParameters = new Dictionary<TextureParameterName, int>();
                for(int i = 0; i < textureParametersCount; i++)
                {
                    var parameterName = (TextureParameterName)reader.ReadInt32();
                    var parameterValue = reader.ReadInt32();
                    settings.TextureParameters.Add(parameterName, parameterValue);
                }
            }
            settings.TextureTarget = (TextureTarget)reader.ReadInt32();
            settings.SizedInternalFormat = (SizedInternalFormat)reader.ReadInt32();
            settings.PixelFormat = (PixelFormat)reader.ReadInt32();
            settings.PixelType = (PixelType)reader.ReadInt32();
            settings.IsMultisampled = reader.ReadBoolean();
            settings.SamplesCounter = reader.ReadInt32();
            settings.FramebufferAttachment = (FramebufferAttachment)reader.ReadInt32();
            return settings;
        }

        public void Write(TextureSettings data, BinaryWriter writer)
        {
            writer.Write(data.TextureParameters?.Count ?? 0);
            if(data.TextureParameters != null)
            {
                foreach(var parameter in data.TextureParameters)
                {
                    writer.Write((int)parameter.Key);
                    writer.Write(parameter.Value);
                }
            }
            writer.Write((int)data.TextureTarget);
            writer.Write((int)data.SizedInternalFormat);
            writer.Write((int)data.PixelFormat);
            writer.Write((int)data.PixelType);
            writer.Write(data.IsMultisampled);
            writer.Write(data.SamplesCounter);
            writer.Write((int)data.FramebufferAttachment);
        }
    }
}
