using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using RockEngine.OpenGL.Settings;

namespace RockEngine.OpenGL.Buffers.UBOBuffers
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = Size)]
    internal struct MaterialData : IUBOData<MaterialData>
    {
        [FieldOffset(0)]
        public Vector3 AlbedoColor;

        [FieldOffset(12)]
        public float Metallic;

        [FieldOffset(16)]
        public float Roughness;

        [FieldOffset(20)]
        public float Ao;

        public const int Size = 32;
        private static UBO<MaterialData> UBO => IUBOData<MaterialData>.UBO;
        public readonly string Name => nameof(MaterialData);

        /// <summary>
        /// Binding in the shader
        /// </summary>
        public readonly int BindingPoint => 3;

        public MaterialData()
        {
            if (IUBOData<MaterialData>.UBO is null)
            {
                IUBOData<MaterialData>.UBO = new UBO<MaterialData>(new BufferSettings(Size, BufferUsageHint.StreamDraw, BindingPoint, Name)).Setup().SetLabel();
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


/*
 i am creating a self made 3d app using opentk and C#
Please help me to generate C# code of ubo
here is my example in shader
layout (std140, binding = 3) uniform MaterialData
{
    vec3 albedo;
    float metallic;
    float roughness;
    float ao;
}materialData;
And here is the code which i need to be like that generated, it is very important to be unique location for each ubo and also fieldoffset is must be calculated somehow 
 
 
 
 
 
 
 */