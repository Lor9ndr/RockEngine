using System.Runtime.InteropServices;

namespace RockEngine.Utils
{
    public static class IntPtrHelper
    {
    
        public static IntPtr ToIntPtr<T>(this T structure) where T:struct
        {
            var bytenint = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
            Marshal.StructureToPtr(structure, bytenint, false);
            return bytenint;
        }
    }
}
