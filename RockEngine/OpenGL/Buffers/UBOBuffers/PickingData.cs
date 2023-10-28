using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using RockEngine.OpenGL.Settings;

namespace RockEngine.OpenGL.Buffers.UBOBuffers
{
    [StructLayout(LayoutKind.Explicit, Size = Size)]
    public struct PickingData : IUBOData<PickingData>
    {
        [FieldOffset(0)]
        public uint gDrawIndex;

        [FieldOffset(4)]
        public uint gObjectIndex;

        public const int Size = 16;
        private static UBO<PickingData> UBO => IUBOData<PickingData>.UBO;
        public readonly string Name => nameof(PickingData);

        /// <summary>
        /// Binding in the shader
        /// </summary>
        public readonly int BindingPoint => 5;

        public PickingData()
        {
            if(IUBOData<PickingData>.UBO is null)
            {
                IUBOData<PickingData>.UBO = new UBO<PickingData>(new BufferSettings(Size, BufferUsageHint.StreamDraw, BindingPoint, Name)).Setup().SetLabel();
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
