using System.Runtime.InteropServices;

namespace RockEngine.Rendering.OpenGL.Buffers.UBOBuffers
{
    [StructLayout(LayoutKind.Explicit, Size = Size)]
    public struct PickingData 
    {

        [FieldOffset(0)]
        public uint gObjectIndex;

        public const int Size = 4;

        public PickingData()
        {
            
        }

        public PickingData(uint gObjectIndex)
        {
            this.gObjectIndex = gObjectIndex;
        }
    }
}
