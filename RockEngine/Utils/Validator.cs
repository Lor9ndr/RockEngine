using System.Runtime.CompilerServices;

namespace RockEngine.Utils
{
    public static class Validator
    {
        public static void ThrowIfNull<T>(T data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }
        }

        public static void ThrowIfEmpty(Array array, string message, [CallerMemberName()] string? name = "")
        {
            if (array.Length == 0)
            {
                throw new ArgumentException(message, name);
            }
        }
    }
}
