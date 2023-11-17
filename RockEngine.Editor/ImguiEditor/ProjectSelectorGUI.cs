using FontAwesome.Constants;
using ImGuiNET;
using NativeFileDialogSharp;
using RockEngine.Assets;

namespace RockEngine.Editor.ImguiEditor
{
    internal sealed class ProjectSelectorGUI : IDisposable
    {
        private readonly EngineWindow _window;
        private readonly ImGuiOpenGL _controller;

        public ProjectSelectorGUI(EngineWindow window)
        {
            _window = window;
            _controller = new ImGuiOpenGL();
            _ = new ImGuiInput(window);
            ImguiHelper.Config(_controller);
            ImguiHelper.SetupImGuiStyle();
        }

        public void OnRender()
        {
            ImGui.NewFrame();
            if(ImGui.Begin("Select project", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration))
            {
                ImGui.SetWindowPos(new System.Numerics.Vector2(0), ImGuiCond.Always);
                ImGui.SetWindowSize(_window.Size.ToVector2());
                if(ImguiHelper.FaIconButton(FA.FILE))
                {
                    var result = Dialog.FileOpen("asset");
                    if(!string.IsNullOrEmpty(result.Path))
                    {
                        _window.Close();
                        _window.Context.MakeNoneCurrent();
                        AssetManager.LoadProject(result.Path);
                        return;
                    }
                }

                ImGui.End();
            }
            _controller.Render(_window.Size);
        }

        public void Dispose()
        {
            _controller.Dispose();
        }
    }
}
