using ImGuiNET;

using RockEngine.Rendering.Layers;

namespace RockEngine.Editor.ImguiEditor
{
    internal static class ImguiHelper
    {
        public static void FaIconText(string icon)
        {
            ImGui.PushFont(ImGuiRenderer.IconsFont);
            ImGui.Text(icon);
            ImGui.PopFont();
        }
        public static bool FaIconButton(string icon)
        {
            ImGui.PushFont(ImGuiRenderer.IconsFont);
            bool result = ImGui.Button(icon);
            ImGui.PopFont();
            return result;
        }
    }
}
