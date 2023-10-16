using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.OpenGL.Buffers;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using RockEngine.OpenGL.Settings;

namespace RockEngine.OpenGL.Buffers.UBOBuffers
{
    [StructLayout(LayoutKind.Explicit, Size = Size)]
    public struct LightData : IUBOData<LightData>, IDisposable
    {
        [FieldOffset(0)]
        public Vector3 LightColor;

        [FieldOffset(16)]
        public Vector3 LightDirection;

        [FieldOffset(32)]
        public Vector3 LightPosition;

        [FieldOffset(44)]
        public float Intensity;
        public const int Size = 48;

        private static UBO<LightData> UBO => IUBOData<LightData>.UBO;
        public readonly string Name => nameof(LightData);

        public int BindingPoint => 4;

        public LightData()
        {
            if (IUBOData<LightData>.UBO is null)
            {
                IUBOData<LightData>.UBO = new UBO<LightData>(new BufferSettings(Size, BufferUsageHint.StreamDraw, BindingPoint, Name)).Setup().SetLabel();
            }
        }

        public void SendData()
        {
            UBO.SendData(this);
        }

        public void SendData<TSub>([DisallowNull, NotNull] TSub data, nint offset, int size)
        {
            UBO.SendData(data, offset, size);
        }

        public void Dispose()
        {
            UBO.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}