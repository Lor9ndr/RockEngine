using OpenTK.Graphics.OpenGL4;

using RockEngine.Rendering.OpenGL.Settings;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace RockEngine.Rendering.OpenGL.Buffers.UBOBuffers
{
    [StructLayout(LayoutKind.Explicit, Size = Size)]
    public struct PickingData : IUBOData<PickingData>
    {
        [FieldOffset(0)]
        public uint gDrawIndex;

        [FieldOffset(4)]
        public uint gObjectIndex;

        public const int Size = 16;
        private static UBO<PickingData> UBO {get=> IUBOData<PickingData>.UBO ; set => IUBOData<PickingData>.UBO = value;}
        public readonly string Name => nameof(PickingData);

        /// <summary>
        /// Binding in the shader
        /// </summary>
        public readonly int BindingPoint => 5;

        public PickingData()
        {
            if(IUBOData<PickingData>.UBO is null)
            {
                var name = Name;
                var bindingPoint = BindingPoint;
                IRenderingContext.Update(context =>
                {
                    UBO = new UBO<PickingData>(new BufferSettings(Size, BufferUsageHint.StreamDraw, bindingPoint, name))
                    .Setup(context)
                    .SetLabel(context);

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
