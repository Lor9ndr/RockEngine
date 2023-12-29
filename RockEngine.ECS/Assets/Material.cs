using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Common.Editor;
using RockEngine.Rendering.OpenGL.Shaders;
using RockEngine.Rendering.OpenGL.Textures;

namespace RockEngine.ECS.Assets
{
    public class Material : BaseAsset
    {
        [UI]
        public Dictionary<string, object> ShaderData;
        public readonly AShaderProgram ShaderProgram;

        public Material(AShaderProgram shaderProgram, string path, string name, Guid id)
            : base(path, name, id, AssetType.Material)
        {
            ShaderData = new Dictionary<string, object>();
            ShaderProgram = shaderProgram;
            SetShaderUnfiromValues();
        }

        /// <summary>
        /// Use that constructor to set initial values of the material in shader 
        /// Do not setting default values from the shaderdata
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="shaderData"></param>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public Material(AShaderProgram shader, Dictionary<string, object> shaderData, string path, string name, Guid id)
            : base(path, name, id, AssetType.Material)
        {
            ShaderProgram = shader;
            ShaderData = shaderData;
        }

        private void SetShaderUnfiromValues()
        {
            var uniforms = ShaderProgram.GetMaterialUniforms();
            foreach(var uniform in uniforms)
            {
                float[ ] value = new float[4 * 4]; // max is mat4x4
                GL.GetUniform(ShaderProgram.Handle, uniform.Location, value);
                var obj = ConvertGettedUniformValue(uniform.Type, value);

                ShaderData.Add(uniform.Name,
                     obj);
            }
        }

        public static object ConvertGettedUniformValue(ActiveUniformType t, float[ ] values)
        {
            switch(t)
            {
                case ActiveUniformType.Int:
                    return (int)values[0];
                case ActiveUniformType.Float:
                    return values[0]; // Default value for float
                case ActiveUniformType.Bool:
                    return values[0] == 0 ? false : true; // Default value for bool
                case ActiveUniformType.FloatVec3:
                    return new Vector3(values[0], values[1], values[2]);
                case ActiveUniformType.UnsignedInt:
                    return (uint)values[0]; // Default value for unsigned int
                case ActiveUniformType.Double:
                    return (double)values[0]; // Default value for double
                case ActiveUniformType.FloatVec2:
                    return new Vector2(values[0], values[1]);
                case ActiveUniformType.FloatVec4:
                    return new Vector4(values[0], values[1], values[2], values[3]);
                case ActiveUniformType.IntVec2:
                    return new Vector2i((int)values[0], (int)values[1]);
                case ActiveUniformType.IntVec3:
                    return new Vector3i((int)values[0], (int)values[1], (int)values[2]);
                case ActiveUniformType.IntVec4:
                    return new Vector4i((int)values[0], (int)values[1], (int)values[2], (int)values[3]);
                case ActiveUniformType.BoolVec2:
                    return new Vector2i((int)values[0], (int)values[1]);
                case ActiveUniformType.BoolVec3:
                    return new Vector3i((int)values[0], (int)values[1], (int)values[2]);
                case ActiveUniformType.BoolVec4:
                    return new Vector4i((int)values[0], (int)values[1], (int)values[2], (int)values[3]);
                case ActiveUniformType.FloatMat2:
                    return new Matrix2(values[0], values[1], values[2], values[3]);
                case ActiveUniformType.FloatMat3:
                    return new Matrix3(values[0],
                                       values[1],
                                       values[2],
                                       values[3],
                                       values[4],
                                       values[5],
                                       values[6],
                                       values[7],
                                       values[8]);
                case ActiveUniformType.FloatMat4:
                    return new Matrix4(values[0],
                                       values[1],
                                       values[2],
                                       values[3],
                                       values[4],
                                       values[5],
                                       values[6],
                                       values[7],
                                       values[8],
                                       values[9],
                                       values[10],
                                       values[11],
                                       values[12],
                                       values[13],
                                       values[14],
                                       values[16]);
                case ActiveUniformType.Sampler2D:
                    return new Texture2D(); // Handle sampler2D case accordingly
                default:
                    throw new Exception("Unsupported material shader type"); // Return null for unsupported types or handle the case accordingly
            }
        }

        public void SendData()
        {
            /*            MaterialData. = ShaderData.Values.ToArray();
                        MaterialData.SendData();*/
            ShaderProgram.Bind();
            foreach(var item in ShaderData)
            {
                ShaderProgram.SetShaderData(item.Key, item.Value);
            }
            ShaderProgram.Unbind();
        }
    }
}
