using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace RockEngine.ShaderGenerator
{
    public static class ShaderGenerator
    {
        [RequiresUnreferencedCode("")]
        public static string GenerateShader<T>() where T : BaseVertexShader, new()
        {
            StringBuilder shaderBuilder = new StringBuilder();
            T shader = new T();
            var type = typeof(T);
            var properties = type.GetProperties();
            foreach(PropertyInfo property in properties)
            {
                if(property.PropertyType.IsSubclassOf(typeof(Variable)))
                {
                    Variable shaderVar = (Variable)property.GetValue(shader);
                    shaderBuilder.AppendLine(shaderVar.GetString(property.Name));
                }
            }
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            for(int i = 0; i < methods.Length; i++)
            {
                var method = methods[i];
                if(Attribute.IsDefined(method, typeof(MethodAttribute)))
                {
                    shaderBuilder.AppendLine(GetMethodBodyAsString(method));
                }
            }


            return shaderBuilder.ToString();
        }
        private static string GetMethodBodyAsString(MethodInfo method)
        {
            var methodBody = method.GetMethodBody();
            if(methodBody == null)
            {
                return string.Empty;
            }

            var ilBytes = methodBody.GetILAsByteArray();
            var ilString = BitConverter.ToString(ilBytes).Replace("-", string.Empty);
            return ilString;
        }
    }
}
