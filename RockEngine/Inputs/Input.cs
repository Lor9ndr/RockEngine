using OpenTK.Windowing.GraphicsLibraryFramework;

using RockEngine;

namespace RockEngine.Inputs
{
    internal static class Input
    {
        internal static KeyboardState KeyboardState { get => Game.MainWindow.KeyboardState; }
        internal static MouseState MouseState { get => Game.MainWindow.MouseState; }

        public static bool IsKeyDown(Keys key)
        {
            return KeyboardState.IsKeyDown(key);
        }

        public static bool IsButtonDown(MouseButton button)
        {
            return MouseState.IsButtonDown(button);
        }
        public static bool IsButtonPressed(MouseButton button)
        {
            return MouseState.IsButtonPressed(button);
        }
    }
}
