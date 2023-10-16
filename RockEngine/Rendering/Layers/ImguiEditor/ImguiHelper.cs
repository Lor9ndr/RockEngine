using ImGuiNET;

using RockEngine.Rendering.Layers;

namespace RockEngine.Rendering.Layers.ImguiEditor
{
    internal static class ImguiHelper
    {
        public static void FaIconText(string icon)
        {
            ImGui.PushFont(ImGuiLayer.IconsFont);
            ImGui.Text(icon);
            ImGui.PopFont();
        }
        public static bool FaIconButton(string icon)
        {
            ImGui.PushFont(ImGuiLayer.IconsFont);
            bool result = ImGui.Button(icon);
            ImGui.PopFont();
            return result;
        }
    }
}
