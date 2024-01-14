using System.Numerics;
using System.Runtime.CompilerServices;

namespace RockEngine.ShaderGenerator
{
    public class Variable
    {
        public dynamic Value { get; set; }
        public virtual string GetString([CallerMemberName] string fieldName = "")
        {
            return Value.ToString();
        }
        public string ConvertToGLSLType(Type t)
        {
            if(t == typeof(Vector3))
            {
                return "vec3";
            }
            else if(t == typeof(Vector4))
            {
                return "vec4";
            }
            else if(t == typeof(Matrix4x4))
            {
                return "mat4";
            }
            throw new NotSupportedException($"Not supported type {t}");
        }
    }
}
