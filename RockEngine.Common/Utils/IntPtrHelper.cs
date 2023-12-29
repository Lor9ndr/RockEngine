using System.Runtime.InteropServices;

namespace RockEngine.Common.Utils
{
    public static class IntPtrHelper
    {
        /// <summary>
        /// Converts structure to pointer using <see cref="Marshal.SizeOf{T}()"/>
        /// </summary>
        /// <typeparam name="T">Type of the structure</typeparam>
        /// <param name="structure">the structure itself</param>
        /// <returns>the pointer to the structure</returns>
        public static nint ToIntPtr<T>(this T structure) where T : struct
        {
            var bytenint = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
            Marshal.StructureToPtr(structure, bytenint, false);
            return bytenint;
        }
        public static nint ToIntPtr<T>(this T structure, int size) where T : struct
        {
            var bytenint = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structure, bytenint, false);
            return bytenint;
        }

        public static T? FromIntPtr<T>(this nint reference)
        {
            return Marshal.PtrToStructure<T>(reference);
        }
    }
}
