using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using RockEngine.OpenGL.Settings;

namespace RockEngine.OpenGL.Buffers.UBOBuffers
{
    [StructLayout(LayoutKind.Explicit, Size = Size)]
    public struct MaterialData : IUBOData<MaterialData>
    {
        [FieldOffset(0)]
        public Vector3 albedo;

        [FieldOffset(12)]
        public float metallic;

        [FieldOffset(16)]
        public float roughness;

        [FieldOffset(20)]
        public float ao;

        public const int Size = 32;
        private static UBO<MaterialData> UBO => IUBOData<MaterialData>.UBO;
        public readonly string Name => nameof(MaterialData);

        /// <summary>
        /// Binding in the shader
        /// </summary>
        public readonly int BindingPoint => 3;

        public MaterialData()
        {
            if(IUBOData<MaterialData>.UBO is null)
            {
                IUBOData<MaterialData>.UBO = new UBO<MaterialData>(new BufferSettings(Size, BufferUsageHint.StreamDraw, BindingPoint, Name)).Setup().SetLabel();
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
