using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.OpenGL.Buffers;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using RockEngine.OpenGL.Settings;

namespace RockEngine.OpenGL.Buffers.UBOBuffers
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = Size)]
    internal struct MaterialData : IUBOData<MaterialData>
    {

        [FieldOffset(0)]
        public Vector3 AmbientColor;

        [FieldOffset(16)]
        public Vector3 DiffuseColor;

        [FieldOffset(32)]
        public Vector3 SpecularColor;

        [FieldOffset(44)]
        public float Shininess;


        public const int Size = 48;
        private static UBO<MaterialData> UBO => IUBOData<MaterialData>.UBO;
        public readonly string Name => nameof(MaterialData);

        /// <summary>
        /// Binding in the shader
        /// </summary>
        public int BindingPoint => 3;

        public MaterialData()
        {
            if (IUBOData<MaterialData>.UBO is null)
            {
                IUBOData<MaterialData>.UBO = new UBO<MaterialData>(new BufferSettings(Size, BufferUsageHint.StreamDraw, BindingPoint, Name)).Setup().SetLabel();
            }
        }

        public void SendData()
        {
            UBO.SendData(this);
        }

        public void SendData<Tsub>([DisallowNull, NotNull] Tsub data, nint offset, int size)
        {
            UBO.SendData(data, offset, size);
        }
    }
}
