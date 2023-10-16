using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.OpenGL.Buffers;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using RockEngine.OpenGL.Settings;

namespace RockEngine.OpenGL.Buffers.UBOBuffers
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct PickingData : IUBOData<PickingData>
    {
        [FieldOffset(0)]
        public uint DrawIndex;
        [FieldOffset(4)]
        public uint ObjectIndex;
        /// <summary>
        /// 2 matrices of 4 floats
        /// </summary>
        public const int Size = 16;
        private static UBO<PickingData> UBO => IUBOData<PickingData>.UBO;
        public readonly string Name => nameof(PickingData);

        public readonly int BindingPoint => 5;

        public PickingData()
        {
            if (IUBOData<PickingData>.UBO is null)
            {
                IUBOData<PickingData>.UBO = new UBO<PickingData>(new BufferSettings(Size, BufferUsageHint.StreamDraw, BindingPoint, Name)).Setup().SetLabel();
            }
        }

        public readonly void SendData()
        {
            UBO.SendData(this);
        }

        public readonly void SendData<Tsub>([DisallowNull, NotNull] Tsub data, nint offset, int size)
        {
            UBO.SendData(data, offset, size);
        }
    }
}
