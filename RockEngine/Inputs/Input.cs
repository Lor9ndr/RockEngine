using OpenTK.Windowing.GraphicsLibraryFramework;

using RockEngine;
using RockEngine.Utils;

namespace RockEngine.Inputs
{
    public static class Input
    {
        private static readonly EngineWindow _window = WindowManager.GetMainWindow();
        public static KeyboardState KeyboardState { get => _window.KeyboardState; }
        public static MouseState MouseState { get => _window.MouseState; }

        public static bool IsAnyButtoneDown => MouseState.IsAnyButtonDown;

        public static bool IsKeyDown(Keys key) => KeyboardState.IsKeyDown(key);

        public static bool IsButtonDown(MouseButton button) => MouseState.IsButtonDown(button);
        public static bool IsButtonPressed(MouseButton button) => MouseState.IsButtonPressed(button);
    }
}
