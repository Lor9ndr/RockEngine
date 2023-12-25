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
        private const int LOG_ID = 0;
        private const int WARN_ID = 1;
        private const int ERROR_ID = 2;
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
            Vector4 color = Record.Log;
            int level = 0;
            switch(severity)
            {
                case DebugSeverity.DontCare:
                    break;
                case DebugSeverity.DebugSeverityNotification:
                    level = 1;
                    break;
                case DebugSeverity.DebugSeverityLow:
                    level = 2;
                    color = Record.Warn;
                    break;
                case DebugSeverity.DebugSeverityMedium:
                    level = 3;
                    color = Record.Warn;
                    break;
                case DebugSeverity.DebugSeverityHigh:
                    level = 4;
                    color = Record.Error;
                    break;
            }
            Debugger.Log(level, type.ToString(), log);
            _logs.Append(log, color);

            if (type == DebugType.DebugTypeError)
            {
                var exc = new Exception(messageString + paramString);
                throw exc;
            }
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
            GL.DebugMessageInsert(DebugSourceExternal.DebugSourceApplication, DebugType.DebugTypeMarker, LOG_ID, DebugSeverity.DebugSeverityNotification, -1, message);
        }

        public static void AddError(string message)
        {
            GL.DebugMessageInsert(DebugSourceExternal.DebugSourceApplication, DebugType.DebugTypeError, ERROR_ID, DebugSeverity.DebugSeverityHigh, -1, message);
        }

        public static void AddWarn(string message)
        {
            GL.DebugMessageInsert(DebugSourceExternal.DebugSourceApplication, DebugType.DebugTypeOther, WARN_ID, DebugSeverity.DebugSeverityMedium, -1, message);
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

                var logs = _logs.GetRecords();
                for(int i = 0; i < logs.Count; i++)
                {
                    var log = logs[i];
                    ImGui.TextColored(log.Color, log.Text);

                }

                ImGui.PopTextWrapPos();
                ImGui.EndMenuBar();
            }
        }
    }
}
