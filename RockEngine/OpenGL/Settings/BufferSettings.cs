using OpenTK.Graphics.OpenGL4;

using RockEngine.OpenGL.Vertices;

namespace RockEngine.OpenGL.Settings
{
    internal sealed record BufferSettings : ISettings
    {
        public static BufferSettings DefaultVBOSettings => new BufferSettings(Vertex3D.Size, BufferUsageHint.StaticDraw);

        public int BufferSize { get; internal set; }
        public BufferUsageHint BufferUsageHint { get; internal set; }
        public int BindingPoint { get; internal set; }
        public string? BufferName { get; internal set; }
        public BufferSettings(int bufferSize, BufferUsageHint bufferUsageHint)
        {
            BufferSize = bufferSize;
            BufferUsageHint = bufferUsageHint;
        }

        public BufferSettings(int bufferSize, BufferUsageHint bufferUsageHint, int bindingPoint, string bufferName) : this(bufferSize, bufferUsageHint)
        {
            BindingPoint = bindingPoint;
            BufferName = bufferName;
        }
    }
}
