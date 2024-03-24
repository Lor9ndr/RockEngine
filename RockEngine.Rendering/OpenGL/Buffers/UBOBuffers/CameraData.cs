using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Rendering.OpenGL.Settings;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace RockEngine.Rendering.OpenGL.Buffers.UBOBuffers
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
                var bindingPoint = BindingPoint;
                var name = Name;
                IRenderingContext.Update(context =>
                {
                    IUBOData<CameraData>.UBO = new UBO<CameraData>(new BufferSettings(Size,
                                                                  BufferUsageHint.StreamDraw,
                                                                  bindingPoint,
                                                                  name)).Setup(context).SetLabel(context);
                });

            }
        }

        public readonly void SendData(IRenderingContext context)
        {
            UBO.SendData(context, this);
        }

        public readonly void SendData<TSub>(IRenderingContext context,[DisallowNull, NotNull] TSub data, nint offset, int size)
        {
            UBO.SendData(context, data, offset, size);
        }
    }
}
