using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Rendering.OpenGL.Settings;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace RockEngine.Rendering.OpenGL.Buffers.UBOBuffers
{
    [StructLayout(LayoutKind.Explicit, Size = Size)]
    public struct TransformData : IUBOData<TransformData>
    {
        [FieldOffset(0)]
        public Matrix4 Model;

        public const int Size = 64;
        private static UBO<TransformData> UBO => IUBOData<TransformData>.UBO;
        public readonly string Name => nameof(TransformData);

        /// <summary>
        /// Binding in the shader
        /// </summary>
        public readonly int BindingPoint => 1;

        public TransformData()
        {
            if(IUBOData<TransformData>.UBO is null)
            {
                var bindingPoint = BindingPoint;
                var name = Name;
                IRenderingContext.Update(context =>
                {
                    if(IUBOData<TransformData>.UBO is null)
                    {
                        IUBOData<TransformData>.UBO = new UBO<TransformData>(new BufferSettings(Size, BufferUsageHint.DynamicDraw, bindingPoint, name))
                        .Setup(context)
                        .SetLabel(context);
                    }
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
