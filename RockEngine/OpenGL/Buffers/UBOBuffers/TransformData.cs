using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using RockEngine.OpenGL.Settings;

namespace RockEngine.OpenGL.Buffers.UBOBuffers
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
                IUBOData<TransformData>.UBO = new UBO<TransformData>(new BufferSettings(Size, BufferUsageHint.StreamDraw, BindingPoint, Name)).Setup().SetLabel();
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
