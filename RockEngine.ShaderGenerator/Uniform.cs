using System.Runtime.CompilerServices;

namespace RockEngine.ShaderGenerator
{
    public class Uniform<T> : Variable where T : struct
    {
        public override string GetString([CallerMemberName] string fieldName = "")
        {
            return $"uniform {this.ConvertToGLSLType(typeof(T))} {fieldName};";
        }
    }
}
