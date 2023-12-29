using System.Runtime.CompilerServices;

namespace RockEngine.Common.Utils
{
    public static class Check
    {
        public static void IsNull<T>(T data)
        {
            if(data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }
        }

        public static void IsEmpty(Array array, string message, [CallerMemberName()] string? name = "")
        {
            if(array.Length == 0)
            {
                throw new ArgumentException(message, name);
            }
        }
    }
}
