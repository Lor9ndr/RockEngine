using OpenTK.Graphics.OpenGL4;

namespace RockEngine.OpenGL.Shaders
{
    public class UniformFieldInfo
    {
        public int Location { get; internal set; }
        public string Name { get; internal set; }
        public int Size { get; internal set; }
        public ActiveUniformType Type { get; internal set; }
    }
}