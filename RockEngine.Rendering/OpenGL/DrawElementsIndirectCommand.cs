using System.Runtime.InteropServices;

namespace RockEngine.Rendering.OpenGL
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = sizeof(int) * 5)]
    internal struct DrawElementsIndirectCommand
    {
        public int Count;
        public int InstanceCount;
        public int FirstIndex;
        public int BaseVertex;
        public int baseInstance;
    }
}
