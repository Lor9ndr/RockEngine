using OpenTK.Graphics.OpenGL4;

using RockEngine.Rendering.OpenGL.Settings;

using System.Diagnostics.CodeAnalysis;

namespace RockEngine.Rendering.OpenGL.Buffers.UBOBuffers
{
    public struct MaterialData : IUBOData<MaterialData>
    {
        public object[ ] Data;

        private static UBO<MaterialData> UBO => IUBOData<MaterialData>.UBO;
        public readonly string Name => nameof(MaterialData);

        /// <summary>
        /// Binding in the shader
        /// </summary>
        public readonly int BindingPoint => 3;

        public MaterialData()
        {
            if(IUBOData<MaterialData>.UBO is null)
            {
                IUBOData<MaterialData>.UBO = new UBO<MaterialData>(new BufferSettings(0, BufferUsageHint.DynamicDraw, BindingPoint, Name)).Setup().SetLabel();
            }
        }

        public readonly void SendData()
        {
            UBO.SendData(this);
        }

        public readonly void SendData<TSub>([DisallowNull, NotNull] TSub data, nint offset, int size)
        {
            UBO.SendData(data, offset, size);
        }
    }
}
