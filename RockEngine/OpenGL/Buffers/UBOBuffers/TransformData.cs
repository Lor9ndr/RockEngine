using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.OpenGL.Buffers;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using RockEngine.OpenGL.Settings;

namespace RockEngine.OpenGL.Buffers.UBOBuffers
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct TransformData : IUBOData<TransformData>
    {
        [FieldOffset(0)]
        public Matrix4 Model;
        public readonly int Size => 64;
        private static UBO<TransformData> UBO => IUBOData<TransformData>.UBO;

        public readonly int BindingPoint => 1;

        public readonly string Name => nameof(TransformData);

        public TransformData(Matrix4 model) : this()
        {
            Model = model;
        }
        public TransformData()
        {
            if (IUBOData<TransformData>.UBO is null)
            {
                IUBOData<TransformData>.UBO = new UBO<TransformData>(new BufferSettings(Size, BufferUsageHint.StreamDraw, BindingPoint, Name))
                    .Setup()
                    .SetLabel();
            }
        }

        public readonly void SendData()
        {
            UBO.SendData(this);
        }

        public readonly void SendData<Tsub>([NotNull, DisallowNull] Tsub data, nint offset, int size)
        {
            UBO.SendData(data, offset, size);
        }
    }
}
