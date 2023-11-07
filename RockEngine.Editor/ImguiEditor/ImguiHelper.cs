using ImGuiNET;

using RockEngine.Rendering.Layers;
using RockEngine.Utils;

using System.Numerics;
using System.Runtime.InteropServices;

namespace RockEngine.Editor.ImguiEditor
{
    internal static class ImguiHelper
    {
        public static ImFontPtr Mainfont;

        public static ImFontPtr IconsFont;

        public static void FaIconText(string icon)
        {
            ImGui.PushFont(IconsFont);
            ImGui.Text(icon);
            ImGui.PopFont();
        }
        public static bool FaIconButton(string icon)
        {
            ImGui.PushFont(IconsFont);
            bool result = ImGui.Button(icon);
            ImGui.PopFont();
            return result;
        }
        public static void Config(ImGuiOpenGL controller)
        {
            var io = ImGui.GetIO();
            var path = "Resources\\Fonts\\Roboto-Regular.ttf";

            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable |
                ImGuiConfigFlags.ViewportsEnable |
                ImGuiConfigFlags.IsSRGB |
                ImGuiConfigFlags.DpiEnableScaleFonts |
                ImGuiConfigFlags.DpiEnableScaleViewports;

            io.ConfigDockingTransparentPayload = false;
            io.FontGlobalScale = 1f;

            SelectFont(controller, path, 18);

            var iconsPath = "Resources\\Fonts\\forkawesome-webfont.ttf";

            unsafe
            {
                ImFontConfigPtr config = ImGuiNative.ImFontConfig_ImFontConfig();
                config.MergeMode = true;
                config.PixelSnapH = true;
                GCHandle rangeHandle = GCHandle.Alloc(new ushort[ ] { 0xe000, 0xf8ff, 0 }, GCHandleType.Pinned);
                IconsFont = io.Fonts.AddFontFromFileTTF(iconsPath, 15, config, rangeHandle.AddrOfPinnedObject());
            }

            io.Fonts.Build();
            if(io.Fonts.IsBuilt())
            {
                Logger.AddLog("Builded fonts");
            }
            controller.RecreateFontDeviceTexture();
        }

        public static void SelectFont(ImGuiOpenGL controller, string font, int size = 18)
        {
            var io = ImGui.GetIO();
            var cfg = new ImFontConfigPtr();
            Mainfont = io.Fonts.AddFontFromFileTTF(font, size, cfg, io.Fonts.GetGlyphRangesDefault());

            io.Fonts.Build();
            if(io.Fonts.IsBuilt())
            {
                Logger.AddLog("Builded fonts");
            }

            controller.RecreateFontDeviceTexture();
        }

        public static void SetupImGuiStyle()
        {
            // Discord (Dark) styleBttrDrgn from ImThemes
            var style = ImGui.GetStyle();

            style.WindowPadding = new Vector2(15, 15);
            style.WindowRounding = 5.0f;
            style.FramePadding = new Vector2(5, 5);
            style.FrameRounding = 4.0f;
            style.ItemSpacing = new Vector2(12, 8);
            style.ItemInnerSpacing = new Vector2(8, 6);
            style.IndentSpacing = 25.0f;
            style.ScrollbarSize = 15.0f;
            style.ScrollbarRounding = 9.0f;
            style.GrabMinSize = 5.0f;
            style.GrabRounding = 3.0f;
            style.Colors[(int)ImGuiCol.Text] = new Vector4(1f, 1f, 1f, 1.00f);
            style.Colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.24f, 0.23f, 0.29f, 1.00f);
            style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.06f, 0.05f, 0.07f, 1.00f);
            style.Colors[(int)ImGuiCol.ChildBg] = new Vector4(0.07f, 0.07f, 0.09f, 1.00f);
            style.Colors[(int)ImGuiCol.PopupBg] = new Vector4(0.07f, 0.07f, 0.09f, 1.00f);
            style.Colors[(int)ImGuiCol.Border] = new Vector4(0.80f, 0.80f, 0.83f, 0.88f);
            style.Colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.92f, 0.91f, 0.88f, 0.00f);
            style.Colors[(int)ImGuiCol.FrameBg] = new Vector4(0.10f, 0.09f, 0.12f, 1.00f);
            style.Colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.24f, 0.23f, 0.29f, 1.00f);
            style.Colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.56f, 0.56f, 0.58f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBg] = new Vector4(0.10f, 0.09f, 0.12f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(1.00f, 0.98f, 0.95f, 0.75f);
            style.Colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.07f, 0.07f, 0.09f, 1.00f);
            style.Colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.10f, 0.09f, 0.12f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.10f, 0.09f, 0.12f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.80f, 0.80f, 0.83f, 0.31f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.56f, 0.56f, 0.58f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.06f, 0.05f, 0.07f, 1.00f);
            style.Colors[(int)ImGuiCol.CheckMark] = new Vector4(0.80f, 0.80f, 0.83f, 0.31f);
            style.Colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.80f, 0.80f, 0.83f, 0.31f);
            style.Colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.06f, 0.05f, 0.07f, 1.00f);
            style.Colors[(int)ImGuiCol.Button] = new Vector4(0.10f, 0.09f, 0.12f, 1.00f);
            style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.24f, 0.23f, 0.29f, 1.00f);
            style.Colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.56f, 0.56f, 0.58f, 1.00f);
            style.Colors[(int)ImGuiCol.Header] = new Vector4(0.10f, 0.09f, 0.12f, 1.00f);
            style.Colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.56f, 0.56f, 0.58f, 1.00f);
            style.Colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.06f, 0.05f, 0.07f, 1.00f);
            style.Colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            style.Colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.56f, 0.56f, 0.58f, 1.00f);
            style.Colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.06f, 0.05f, 0.07f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotLines] = new Vector4(0.40f, 0.39f, 0.38f, 0.63f);
            style.Colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(0.25f, 1.00f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.40f, 0.39f, 0.38f, 0.63f);
            style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(0.25f, 1.00f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.25f, 1.00f, 0.00f, 0.43f);
        }
    }
}
