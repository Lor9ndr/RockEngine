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
                var name = Name;
                var bindingPoint = BindingPoint;
                IRenderingContext.Update(context =>
                {
                    IUBOData<MaterialData>.UBO = new UBO<MaterialData>(new BufferSettings(0, BufferUsageHint.DynamicDraw, bindingPoint, name))
                    .Setup(context).SetLabel(context);
                });
            }
        }

        public readonly void SendData(IRenderingContext context)
        {
            UBO.SendData(context, this);
        }

        public readonly void SendData<TSub>(IRenderingContext context, [DisallowNull, NotNull] TSub data, nint offset, int size)
        {
            UBO.SendData(context, data, offset, size);
        }
    }
}
