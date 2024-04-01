using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Rendering.OpenGL.Settings;

using SkiaSharp;

namespace RockEngine.Rendering.OpenGL.Textures
{
    public class Texture2D : Texture
    {
        internal long BitmapSize { get; set; }
        internal SKBitmap? Bitmap { get; set; }

        public Texture2D(TextureSettings settings)
            : base(new Vector2i(512, 512), settings)
        {
        }
        public Texture2D()
            : base()
        {

        }

        /// <summary>
        /// Setup Texture2D,
        /// This is not full setup, after that you should setup a bitmap using <see cref="SetupBitmap(SKBitmap)"/>
        /// This method just initilize Opengl Texture
        /// </summary>
        /// <returns>Fluent</returns>
        public override Texture2D Setup(IRenderingContext context)
        {
            context.CreateTextures(Settings.TextureTarget, out int handle);
            Handle = handle;
            return this;
        }

        /// <summary>
        /// Setups an image bitamp to the GPU 
        /// </summary>
        /// <param name="bitmap">bitmap to upload to GPU</param>
        /// <returns>Fluent</returns>
        public Texture2D SetupBitmap(IRenderingContext contex, SKBitmap bitmap)
        {
            var bitHandle = bitmap.GetPixels();
            Size = new Vector2i(bitmap.Width, bitmap.Height);
            int maxDimension = Math.Max(bitmap.Width, bitmap.Height);
            int maxMipLevel = maxDimension > 0 ? 1 + (int)Math.Log2(maxDimension) : 1;

            contex.TextureStorage2D(Handle, maxMipLevel, SizedInternalFormat.Srgb8Alpha8, Size);

            int levelWidth = bitmap.Width;
            int levelHeight = bitmap.Height;

            for(int level = 0; level < maxMipLevel; level++)
            {
                contex.TextureSubImage2D(Handle, level, 0, 0, levelWidth, levelHeight, Settings.PixelFormat, Settings.PixelType, bitHandle);
                levelWidth = Math.Max(levelWidth >> 1, 1);
                levelHeight = Math.Max(levelHeight >> 1, 1);
            }

            contex.GenerateTextureMipmap(Handle);

            bitmap.Dispose();
            return this;
        }

        /// <summary>
        /// Decode a file to <see cref="SKBitmap"/> using filepath
        /// </summary>
        /// <param name="path">path to file</param>
        /// <returns>Bitmap which is going to be uploaded to the GPU</returns>
        public static SKBitmap GetBitmap(string path) => SKBitmap.Decode(path);

        /// <summary>
        /// Decode a file to <see cref="SKBitmap"/> using stream
        /// </summary>
        /// <param name="path">path to file</param>
        /// <returns>Bitmap which is going to be uploaded to the GPU</returns>
        public static SKBitmap GetBitmap(Stream stream) => SKBitmap.Decode(stream);
    }
}
