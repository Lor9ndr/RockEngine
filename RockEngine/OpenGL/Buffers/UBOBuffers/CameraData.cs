using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using RockEngine.OpenGL.Settings;

namespace RockEngine.OpenGL.Buffers.UBOBuffers
{
    [StructLayout(LayoutKind.Explicit, Size = Size)]
    public struct CameraData : IUBOData<CameraData>
    {
        [FieldOffset(0)]
        public Matrix4 View;

        [FieldOffset(64)]
        public Matrix4 Projection;

        [FieldOffset(128)]
        public Vector3 ViewPos;

        public const int Size = 144;
        private static UBO<CameraData> UBO => IUBOData<CameraData>.UBO;
        public readonly string Name => nameof(CameraData);

        /// <summary>
        /// Binding in the shader
        /// </summary>
        public readonly int BindingPoint => 2;

        public CameraData()
        {
            if(IUBOData<CameraData>.UBO is null)
            {
                IUBOData<CameraData>.UBO = new UBO<CameraData>(new BufferSettings(Size,
                                                                                  BufferUsageHint.StreamDraw,
                                                                                  BindingPoint,
                                                                                  Name)).Setup().SetLabel();
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
