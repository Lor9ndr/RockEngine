using RockEngine.OpenGL.Buffers;
using RockEngine.OpenGL.Shaders;

namespace RockEngine.Rendering
{
    public interface IRenderingContext
    {
        public AShaderProgram CurrentShader { get; set; }
        public FBO CurrentFBO { get; set; }
    }
}
