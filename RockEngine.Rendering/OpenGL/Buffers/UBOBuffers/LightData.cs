using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Rendering.OpenGL.Settings;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace RockEngine.Rendering.OpenGL.Buffers.UBOBuffers
{
    [StructLayout(LayoutKind.Explicit, Size = Size)]
    public struct LightData : IUBOData<LightData>
    {
        [FieldOffset(0)]
        public Vector3 lightColor;

        [FieldOffset(16)]
        public Vector3 lightDirection;

        [FieldOffset(32)]
        public Vector3 lightPosition;

        [FieldOffset(44)]
        public float intensity;

        public const int Size = 48;
        private static UBO<LightData> UBO => IUBOData<LightData>.UBO;
        public readonly string Name => nameof(LightData);

        /// <summary>
        /// Binding in the shader
        /// </summary>
        public readonly int BindingPoint => 4;

        public LightData()
        {
            if(IUBOData<LightData>.UBO is null)
            {
                var name = Name;
                var bindingPoint = BindingPoint;
                IRenderingContext.Update(context =>
                {
                    IUBOData<LightData>.UBO = new UBO<LightData>(new BufferSettings(Size, BufferUsageHint.StreamDraw, bindingPoint, name))
                    .Setup(context)
                    .SetLabel(context);

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
