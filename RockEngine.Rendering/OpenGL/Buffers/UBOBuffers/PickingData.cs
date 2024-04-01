using System.Runtime.InteropServices;

namespace RockEngine.Rendering.OpenGL.Buffers.UBOBuffers
{
    [StructLayout(LayoutKind.Explicit, Size = Size)]
    public struct PickingData 
    {
        [FieldOffset(0)]
        public uint gDrawIndex;

        [FieldOffset(4)]
        public uint gObjectIndex;

        public const int Size = 16;

        public PickingData()
        {
            
        }

        public PickingData(uint gDrawIndex, uint gObjectIndex)
        {
            this.gDrawIndex = gDrawIndex;
            this.gObjectIndex = gObjectIndex;
        }
    }
}
