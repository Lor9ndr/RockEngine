using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Common;
using RockEngine.Common.Editor;
using RockEngine.Rendering;
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
        }
        public override void Loaded()
        {
            IRenderingContext.Update(SetShaderUnfiromValues);
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

        private void SetShaderUnfiromValues(IRenderingContext context)
        {
            var uniforms = ShaderProgram.GetMaterialUniforms(context);
            foreach(var uniform in uniforms)
            {
                float[] value = new float[4 * 4]; // max is mat4x4
                context.GetUniform(ShaderProgram.Handle, uniform.Location, value);
                var obj = ConvertGettedUniformValue(uniform.Type, value);
                ShaderData.TryAdd(uniform.Name, obj);
            }
        }

        public void SendData(IRenderingContext context)
        {
            /*            MaterialData. = ShaderData.Values.ToArray();
                   MaterialData.SendData();*/
            ShaderProgram.Bind(context);
            foreach(var item in ShaderData)
            {
                ShaderProgram.SetShaderData(context,item.Key, item.Value);
            }
            ShaderProgram.Unbind(context);

        }
        public static object ConvertGettedUniformValue(ActiveUniformType t, float[] values)
        {
            return t switch
            {
                ActiveUniformType.Int => (int)values[0],
                ActiveUniformType.Float => values[0],// Default value for float
                ActiveUniformType.Bool => values[0] == 0 ? false : true,// Default value for bool
                ActiveUniformType.FloatVec3 => new Vector3(values[0], values[1], values[2]),
                ActiveUniformType.UnsignedInt => (uint)values[0],// Default value for unsigned int
                ActiveUniformType.Double => (double)values[0],// Default value for double
                ActiveUniformType.FloatVec2 => new Vector2(values[0], values[1]),
                ActiveUniformType.FloatVec4 => new Vector4(values[0], values[1], values[2], values[3]),
                ActiveUniformType.IntVec2 => new Vector2i((int)values[0], (int)values[1]),
                ActiveUniformType.IntVec3 => new Vector3i((int)values[0], (int)values[1], (int)values[2]),
                ActiveUniformType.IntVec4 => new Vector4i((int)values[0], (int)values[1], (int)values[2], (int)values[3]),
                ActiveUniformType.BoolVec2 => new Vector2i((int)values[0], (int)values[1]),
                ActiveUniformType.BoolVec3 => new Vector3i((int)values[0], (int)values[1], (int)values[2]),
                ActiveUniformType.BoolVec4 => new Vector4i((int)values[0], (int)values[1], (int)values[2], (int)values[3]),
                ActiveUniformType.FloatMat2 => new Matrix2(values[0], values[1], values[2], values[3]),
                ActiveUniformType.FloatMat3 => new Matrix3(values[0],
                                                       values[1],
                                                       values[2],
                                                       values[3],
                                                       values[4],
                                                       values[5],
                                                       values[6],
                                                       values[7],
                                                       values[8]),
                ActiveUniformType.FloatMat4 => new Matrix4(values[0],
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
                                                       values[16]),
                ActiveUniformType.Sampler2D => new Texture2D(),// Handle sampler2D case accordingly
                _ => throw new Exception("Unsupported material shader type"),// Return null for unsupported types or handle the case accordingly
            };
        }
    }
}
