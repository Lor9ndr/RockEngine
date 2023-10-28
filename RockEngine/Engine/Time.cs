using RockEngine.Utils;

namespace RockEngine.Engine
{
    public static class Time
    {
        public static float DeltaTime => WindowManager.GetMainWindow().DeltaTime;
        public static float RenderTime => (float)WindowManager.GetMainWindow().Time;
    }
}
