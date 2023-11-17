using ImGuiNET;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Diagnostics;
using System.Runtime.InteropServices;

using static RockEngine.Utils.ColoredLog;

namespace RockEngine.Utils
{
    public static class Logger
    {
        public static readonly DebugProc DebugProcCallback = DebugCallback;
        public static DebugProcKhr DebugMessageDelegate = DebugCallback;
        public static DebugProcArb DebugMessageDelegateARB = DebugCallback;
        private static readonly ColoredLog _logs = new ColoredLog();
        private static bool _scrollToButton;
        private static bool _autoScroll = true;

        private static void DebugCallback(DebugSource source, DebugType type, int id,
            DebugSeverity severity, int length, nint message, nint userParam)
        {

            string messageString = Marshal.PtrToStringAnsi(message, length);
            string paramString = string.Empty;
            if (userParam != IntPtr.Zero)
            {
                paramString = Marshal.PtrToStringAnsi(userParam, length);
            }
            var log = $"{severity} {type} | {messageString}";
            if (severity == DebugSeverity.DebugSeverityNotification)
            {
                //AddLog(log);
                Console.WriteLine(log);
                return;
            }
            if (severity == DebugSeverity.DebugSeverityMedium)
            {
                Debug.WriteLine(log, severity, type);
                AddWarn(log);
            }

            if (type == DebugType.DebugTypeError)
            {
                var exc = new Exception(messageString + paramString);
                Debug.WriteLine(log, severity, type);
                AddError(log);

                throw exc;
            }
            AddWarn(log);

        }

        public static void Clear()
        {
            _logs.Clear();
        }

        private static void AddLog(string message, Vector4 color)
        {
            _logs.Append(message, color);
            _scrollToButton = true;
        }

        public static void AddLog(string message)
        {
            AddLog(message, Record.Log);
        }

        public static void AddError(string message)
        {
            AddLog(message, Record.Error);
        }

        public static void AddWarn(string message)
        {
            AddLog(message, Record.Warn);
        }

        public static void DrawDebugLogWindow()
        {
            string name = "LogWindow";
            if (ImGui.Begin(name,
                ImGuiWindowFlags.MenuBar |
                ImGuiWindowFlags.AlwaysVerticalScrollbar |
                ImGuiWindowFlags.AlwaysUseWindowPadding |
                ImGuiWindowFlags.NoResize))
            {

                // Menu
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.MenuItem("Clear"))
                    {
                        Clear();
                    }
                    ImGui.Checkbox("Autoscroll", ref _autoScroll);
                    ImGui.EndMenuBar();
                }

                // Set auto scroll
                if(_autoScroll)
                {
                    ImGui.SetScrollHereY(-1.0f);
                }

                ImGui.PushTextWrapPos(1400);
                // Default display of logs
                foreach (var log in _logs)
                {
                    ImGui.TextColored(log.Color, log.Text);
                }

                ImGui.PopTextWrapPos();
                ImGui.EndMenuBar();
            }
        }
    }
}
