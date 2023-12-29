
using OpenTK.Graphics.OpenGL4;

using System.Runtime.InteropServices;

namespace RockEngine.Rendering.OpenGL.Settings
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TextureSettings : ISettings
    {
        public Dictionary<TextureParameterName, int>? TextureParameters;
        public TextureTarget TextureTarget { get; set; }
        public SizedInternalFormat SizedInternalFormat { get; set; }
        public PixelFormat PixelFormat { get; set; }
        public PixelType PixelType { get; set; }
        public bool IsMultisampled { get; set; }
        public int SamplesCounter { get; set; }
        public FramebufferAttachment FramebufferAttachment { get; set; }
        public static Dictionary<TextureParameterName, int> DefaultParameters => new Dictionary<TextureParameterName, int>()
        {
            { TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat },
            { TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat },
            { TextureParameterName.TextureWrapR, (int)TextureWrapMode.Repeat },
            { TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear},
            { TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear},
            { TextureParameterName.TextureBaseLevel, 1 },
            { TextureParameterName.TextureMaxLevel, 5 }
        };
        public static TextureSettings DefaultSettings
            => new TextureSettings(DefaultParameters,
                                   TextureTarget.Texture2D,
                                   PixelType.UnsignedByte,
                                   SizedInternalFormat.Srgb8Alpha8);

        public TextureSettings(
                               Dictionary<TextureParameterName, int>? textureParameters,
                               TextureTarget target,
                               PixelType pixelType,
                               SizedInternalFormat sizedInternalFormat,
                               PixelFormat pixelFormat = PixelFormat.Rgba,
                               bool isMultisampled = false,
                               int samplesCounter = 0,
                               FramebufferAttachment framebufferAttachment = FramebufferAttachment.ColorAttachment0)
        {
            TextureParameters = textureParameters;
            TextureTarget = target;
            PixelType = pixelType;
            IsMultisampled = isMultisampled;
            SamplesCounter = samplesCounter;
            FramebufferAttachment = framebufferAttachment;
            SizedInternalFormat = sizedInternalFormat;
            PixelFormat = pixelFormat;
        }

        public TextureSettings()
        {
            TextureParameters = DefaultSettings.TextureParameters;
            TextureTarget = DefaultSettings.TextureTarget;
            PixelType = DefaultSettings.PixelType;
            IsMultisampled = DefaultSettings.IsMultisampled;
            SamplesCounter = DefaultSettings.SamplesCounter;
            FramebufferAttachment = FramebufferAttachment.ColorAttachment0;
            SizedInternalFormat = DefaultSettings.SizedInternalFormat;
            PixelFormat = PixelFormat.Rgba;
        }
    }
}
