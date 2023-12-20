using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Editor;
using RockEngine.OpenGL.Shaders;
using RockEngine.OpenGL.Textures;
using RockEngine.Rendering;

namespace RockEngine.Assets
{
    public class Material : BaseAsset
    {
        [UI]
        public Dictionary<string, object> ShaderData;

        public readonly AShaderProgram ShaderProgram;

        public Material(AShaderProgram shaderProgram,string path, string name, Guid id) 
            : base(path, name, id, AssetType.Material)
        {
            ShaderData = new Dictionary<string, object>();
            ShaderProgram = shaderProgram;
            SetShaderUnfiromValues();
        }

        public Material(AShaderProgram shader, Dictionary<string, object> shaderData, string path, string name, Guid id)
            :base(path,name, id, AssetType.Material)
        {
            ShaderProgram = shader;
            ShaderData = shaderData;
            SetShaderUnfiromValues();

        }

        private void SetShaderUnfiromValues()
        {
            var uniforms = ShaderProgram.GetMaterialUniforms();
            foreach(var uniform in uniforms)
            {
                var obj = ConvertActiveUniformType(uniform.Type, out Type t);
                ShaderData.Add(uniform.Name, 
                     obj);
            }
        }

        private static object ConvertActiveUniformType(ActiveUniformType uniformType, out Type t)
        {
            t = typeof(int);
            switch(uniformType)
            {
                case ActiveUniformType.Int:
                    return 0; // Default value for int
                case ActiveUniformType.Float:
                    t = typeof(float);
                    return 0.0f; // Default value for float
                case ActiveUniformType.Bool:
                    t = typeof(bool);
                    return false; // Default value for bool
                case ActiveUniformType.FloatVec3:
                    t = typeof(Vector3);
                    return new Vector3();
                case ActiveUniformType.UnsignedInt:
                    t = typeof(uint);
                    return 0u; // Default value for unsigned int
                case ActiveUniformType.Double:
                    t = typeof(double);
                    return 0.0; // Default value for double
                case ActiveUniformType.FloatVec2:
                    t = typeof(Vector2);
                    return new Vector2();
                case ActiveUniformType.FloatVec4:
                    t = typeof(Vector4);
                    return new Vector4();
                case ActiveUniformType.IntVec2:
                    t = typeof(Vector2i);
                    return new Vector2i();
                case ActiveUniformType.IntVec3:
                    t = typeof(Vector3i);
                    return new Vector3i();
                case ActiveUniformType.IntVec4:
                    t = typeof(Vector4i);
                    return new Vector4i();
                case ActiveUniformType.BoolVec2:
                    t = typeof(Vector2i);
                    return new Vector2i();
                case ActiveUniformType.BoolVec3:
                    t = typeof(Vector3i);
                    return new Vector3i();
                case ActiveUniformType.BoolVec4:
                    t = typeof(Vector4i);
                    return new Vector4i();
                case ActiveUniformType.FloatMat2:
                    t = typeof(Matrix2);
                    return Matrix2.Identity;
                case ActiveUniformType.FloatMat3:
                    t = typeof(Matrix3);
                    return Matrix3.Identity;
                case ActiveUniformType.FloatMat4:
                    t = typeof(Matrix4);
                    return Matrix4.Identity;
                case ActiveUniformType.Sampler2D:
                    t = typeof(Texture2D);
                    return new Texture2D(); // Handle sampler2D case accordingly
                default:
                    throw new Exception("Unsupported material shader type"); // Return null for unsupported types or handle the case accordingly
            }
        }

        }
    }
}
