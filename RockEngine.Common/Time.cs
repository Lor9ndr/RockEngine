using RockEngine.Common.Utils;

namespace RockEngine.Common
{
    public static class Time
    {
        public static float DeltaTime => WindowManager.GetMainWindow().DeltaTime;
        public static float RenderTime => (float)WindowManager.GetMainWindow().Time;
    }
}
