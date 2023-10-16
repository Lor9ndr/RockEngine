namespace RockEngine.Engine
{
    internal static class Time
    {
        public static float DeltaTime => Game.MainWindow.DeltaTime;
        public static float RenderTime => (float)Game.MainWindow.Time;
    }
}
