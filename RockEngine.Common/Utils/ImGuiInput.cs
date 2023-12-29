using ImGuiNET;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RockEngine.Common.Utils
{
    internal sealed class ImGuiInput
    {
        private readonly List<char> _pressedChars = new List<char>();
        private readonly Array _keys;

        //
        // Сводка:
        //     Create a new instance
        //
        // Параметры:
        //   window:
        //     A OpenTK.Windowing.Desktop.GameWindow for connecting input and update event handler.
        public ImGuiInput(GameWindow window)
        {
            ImGuiIOPtr iO = ImGui.GetIO();

            var imguiKeyLookup = Enum.GetValues<ImGuiKey>()
                .ToLookup(name => name.ToString().ToLowerInvariant(), key => key);

            var keysMap = Enum.GetValues<Keys>()
                .Distinct()
                .ToDictionary(name => name.ToString().ToLowerInvariant(), key => key);

            RangeAccessor<int> keyMap = iO.KeyMap;

            foreach(var imgui in imguiKeyLookup)
            {
                if(keysMap.TryGetValue(imgui.Key, out var item2))
                {
                    foreach(var imguiValue in imgui)
                    {
                        keyMap[(int)imguiValue] = (int)item2;
                    }
                }
            }

            keyMap[513] = 263;
            keyMap[514] = 262;
            keyMap[515] = 265;
            keyMap[516] = 264;

            for(int i = 0; i < 10; i++)
            {
                keyMap[536 + i] = 48 + i;
            }

            iO.DeltaTime = Time.DeltaTime;
            window.TextInput += delegate (TextInputEventArgs args)
            {
                PressChar((char)args.Unicode);
            };
            window.UpdateFrame += delegate (FrameEventArgs args)
            {
                Update(window.MouseState, window.KeyboardState, Time.DeltaTime);
            };
            _keys = Enum.GetValues(typeof(Keys));
        }

        //
        // Сводка:
        //     Update the ImGui input state
        //
        // Параметры:
        //   mouseState:
        //     The OpenTK.Windowing.GraphicsLibraryFramework.MouseState.
        //
        //   keyboardState:
        //     The OpenTK.Windowing.GraphicsLibraryFramework.KeyboardState.
        //
        //   deltaTime:
        //     Delta time in seconds.
        private void Update(MouseState mouseState, KeyboardState keyboardState, float deltaTime)
        {
            ImGuiIOPtr iO = ImGui.GetIO();
            iO.DeltaTime = deltaTime;
            RangeAccessor<bool> rangeAccessor = iO.MouseDown;
            rangeAccessor[0] = mouseState[MouseButton.Button1];
            rangeAccessor = iO.MouseDown;
            rangeAccessor[1] = mouseState[MouseButton.Button2];
            rangeAccessor = iO.MouseDown;
            rangeAccessor[2] = mouseState[MouseButton.Button3];
            iO.MouseWheel = mouseState.ScrollDelta.Y;
            iO.MouseWheelH = mouseState.ScrollDelta.X;
            iO.MousePos = mouseState.Position;
            rangeAccessor = iO.KeysDown;
            foreach(Keys value in _keys)
            {
                if(value != Keys.Unknown)
                {
                    rangeAccessor[(int)value] = keyboardState.IsKeyDown(value);
                }
            }

            foreach(char pressedChar in _pressedChars)
            {
                iO.AddInputCharacter(pressedChar);
            }

            _pressedChars.Clear();
            iO.KeyCtrl = keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl);
            iO.KeyAlt = keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt);
            iO.KeyShift = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
            iO.KeySuper = keyboardState.IsKeyDown(Keys.LeftSuper) || keyboardState.IsKeyDown(Keys.RightSuper);
        }

        private void PressChar(char keyChar)
        {
            _pressedChars.Add(keyChar);
        }
    }
}
