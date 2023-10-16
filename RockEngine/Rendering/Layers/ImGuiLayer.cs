
using OpenMath = OpenTK.Mathematics;
using System.Numerics;
using System.Reflection;
using RockEngine.Utils;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using FontAwesome.Constants;
//using ImGuizmoNET;
using NativeFileDialogSharp;
using RockEngine.OpenGL;
using RockEngine.Rendering.Layers;
using RockEngine.Engine.ECS.GameObjects;
using RockEngine;
using RockEngine.Engine;
using RockEngine.Rendering.Layers.ImguiEditor;
using RockEngine.Inputs;
using RockEngine.Utils;
using RockEngine.Engine.EngineStates;
using RockEngine.Rendering.imgui;
using RockEngine.DI;
using RockEngine.Assets;

namespace RockEngine.Rendering.Layers
{
    public partial class ImGuiLayer : ALayer, IDisposable
    {
        public static bool IsMouseOnEditorScreen { get; private set;}
        public static Vector2 EditorScreenMousePos { get; private set;}
        public override int Order => 999;

        private readonly ImGuiRenderer _controller;

        private const string LAYOUT_FILE = "Layout.ini";
        
        #region GUI Fields

        public GameObject? SelectedGameObject = null;
        private int _currentFontIndex;

        private readonly string[] _fontsPaths = new string[]
        {
            "Resources\\Fonts\\Roboto-Regular.ttf",
            "Resources\\Fonts\\InclusiveSans-Regular.ttf"
        };

        #endregion

        private readonly CameraTexture _editorScreen;
        private readonly CameraTexture _gameScreen;
        internal static ImFontPtr IconsFont;
        private ImFontPtr Mainfont;

        private bool _projectWindowIsOpened;

        public ImGuiLayer()
        {
            _controller = new ImGuiRenderer();
            _ = new ImGuiInput(Game.MainWindow);

            _editorScreen = Game.LayerStack.GetLayer<DefaultEditorLayer>()!.Screen;
            _gameScreen = Game.LayerStack.GetLayer<DefaultGameLayer>()!.Screen;

            Config();
            SetupImGuiStyle();

            var imguiContext = ImGui.GetCurrentContext();
            //ImGuizmo.SetImGuiContext(imguiContext);
            if (File.Exists(LAYOUT_FILE))
            {
                // Calling OnRender to initilize the gui and user no longer should press "Load layout" on first launch
                // i think it is should be reworked
                OnRender();
                ImGui.LoadIniSettingsFromDisk(LAYOUT_FILE);
            }
            _projectWindowIsOpened = Project.CurrentProject == null;
        }

        #region Partials
        partial void HandleGameObject(GameObject gameObject);

        partial void ProcessGameObjectComponents(GameObject gameObject);

        partial void AddComponentsWindow(GameObject gameObject);

        #endregion


        public override void OnRender()
        {
            IsMouseOnEditorScreen = false;
            ImGui.NewFrame();
            GL.Viewport(0,0, Game.MainWindow.Size.X, Game.MainWindow.Size.Y);
            var dockID = ImGui.DockSpaceOverViewport(ImGui.GetMainViewport(),  ImGuiDockNodeFlags.PassthruCentralNode | ImGuiDockNodeFlags.AutoHideTabBar);
            ImGui.PushFont(Mainfont);
            TopBar();
            
            ImGui.DockSpace(dockID);
            Logger.DrawDebugLogWindow();

            ImGui.DockSpace(dockID);
            DisplayScene();

            ImGui.DockSpace(dockID);
            DisplaySelectedGameObjectProperties();
           
            ImGui.DockSpace(dockID);
            EditorScreen();
           
            ImGui.DockSpace(dockID);
            GameScreen();

            SelectProjectWindow();

            ImGui.PopFont();

            if(Input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftControl) && Input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.S))
            {
                AssetManager.SaveAll();
            }

            _controller.Render(Game.MainWindow.Size);
        }

        private void SelectProjectWindow()
        {
            if (_projectWindowIsOpened && ImGui.Begin("Select project", ref _projectWindowIsOpened))
            {
                if (ImguiHelper.FaIconButton(FA.FILE))
                {
                    var result = Dialog.FileOpen("asset");
                    if(!string.IsNullOrEmpty(result.Path))
                    {
                        AssetManager.LoadProject(result.Path);
                    }
                }
               
                ImGui.End();
            }
        }

        private void EditorScreen()
        {
            // TODO: FIX BUG WITH RESIZING THAT SCREEN
            if(ImGui.Begin("EDITOR SCREEN", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoTitleBar ))
            {
                CheckMouseIsOnEditorScreen();

                var size = ImGui.GetWindowContentRegionMax();
                _editorScreen.Resize(new OpenMath.Vector2i((int)size.X, (int)size.Y));
                ImGui.Image(_editorScreen.ScreenTexture.Handle, size, new Vector2(0, 1), new Vector2(1, 0));

                var mousePos = ImGui.GetMousePos();
                var padding = ImGui.GetStyle().WindowPadding;
                EditorScreenMousePos = mousePos - ImGui.GetWindowPos() - padding;
                ImGui.End();
            }
        }

        private void GameScreen()
        {
            if (ImGui.Begin("GAME SCREEN", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoScrollWithMouse))
            {
                var size = ImGui.GetWindowContentRegionMax();
                _gameScreen.Resize(new OpenMath.Vector2i((int)size.X, (int)size.Y));
                ImGui.Image(_gameScreen.ScreenTexture.Handle, size, new Vector2(0, 1), new Vector2(1, 0));
                ImGui.End();
            }
        }

        private void TopBar()
        {
            if (ImGui.BeginMainMenuBar())
            {
                ImGui.Columns(5, "TopBarColumns", true);
                ImGui.SetColumnWidth(0, 100);
                ImGui.SetColumnWidth(1, 100);
                ImGui.SetColumnWidth(2, ImGui.GetWindowSize().X / 2 - 100);
                ImGui.SetColumnWidth(3, 100);

                if (ImGui.BeginMenu("Project"))
                {
                    if (ImGui.Button("Open project"))
                    {
                        _projectWindowIsOpened = true;
                    }
                    ImGui.EndMenu();
                }
                ImGui.NextColumn();

                ImGui.Text($"FPS:{Game.MainWindow.FPS}");
                
                ImGui.NextColumn();
                if (ImGui.BeginMenu("Layouts"))
                {
                    if (ImGui.Button("Save layout"))
                    {
                        ImGui.SaveIniSettingsToDisk(LAYOUT_FILE);
                    }
                    if (ImGui.Button("Load layout"))
                    {
                        ImGui.LoadIniSettingsFromDisk(LAYOUT_FILE);
                    }
                    if(ImGui.Combo("Fonts", ref _currentFontIndex, _fontsPaths, _fontsPaths.Length))
                    {
                        SelectFont(_fontsPaths[_currentFontIndex]);
                    }
                    ImGui.EndMenu();
                }
                ImGui.PushFont(IconsFont);
                ImGui.NextColumn();


                if (EngineStateManager.GetCurrentStateKey() != "play")
                {
                    if (ImGui.SmallButton(FA.PLAY))
                    {
                        EngineStateManager.SetNextState("play");
                    }
                }
                else
                {
                    if (ImGui.SmallButton(FA.STOP))
                    {
                        EngineStateManager.SetNextState("dev");
                    }
                }
                

                ImGui.PopFont();

                ImGui.NextColumn();

                ImGui.EndMainMenuBar();
            }
        }

        private unsafe void SelectFont(string font,int size = 18)
        {
            var io = ImGui.GetIO();
            var cfg = new ImFontConfigPtr();
            Mainfont = io.Fonts.AddFontFromFileTTF(font, size, cfg, io.Fonts.GetGlyphRangesDefault());
          
            io.Fonts.Build();
            if (io.Fonts.IsBuilt())
            {
                Logger.AddLog("Builded fonts");
            }

            _controller.RecreateFontDeviceTexture();
        }
     

        private void DisplayScene()
        {
            var scene = Scene.CurrentScene;
            
            if (ImGui.Begin(scene.Name + "##SCENE_WINDOW"))
            {
                ImGui.Text("Current Scene: " + scene.Name);
                ImGui.Separator();
                foreach (var gameObject in scene.GetGameObjects())
                {
                    if (ImGui.Selectable(gameObject.Name, SelectedGameObject == gameObject))
                    {
                        SelectedGameObject = gameObject;
                    }
                }
                ImGui.End();
            }
        }

        private void DisplaySelectedGameObjectProperties()
        {
            if (ImGui.Begin("Field window"))
            {
                if (SelectedGameObject is null)
                {
                    return;
                }
                HandleGameObject(SelectedGameObject);
                ProcessGameObjectComponents(SelectedGameObject);
                AddComponentsWindow(SelectedGameObject);

                ImGui.End();
            }
        }
     

       

        private static void CheckMouseIsOnEditorScreen()
        {
            var rMin = ImGui.GetWindowPos();
            var rMax = rMin + ImGui.GetWindowContentRegionMax();

            // Check if the mouse is hovering over the EditorLayer window
            bool isHovering = ImGui.IsMouseHoveringRect(rMin, rMax);
            IsMouseOnEditorScreen = IsMouseOnEditorScreen ? true: isHovering && ImGui.IsWindowFocused();
        }


        private void DrawGuizmo()
        {
            /*if (SelectedGameObject is not null)
            {
                var camera = Game.LayerStack.GetLayer<DefaultEditorLayer>().DebugCamera.GetComponent<Camera>();
                var viewMat = camera.GetViewMatrix();
                var projectionMat = camera.GetProjectionMatrix();
                var goMatrix = SelectedGameObject.Transform.GetModelMatrix();
                // Begin ImGuizmo drawing
                ImGuizmo.SetOrthographic(false);
                ImGuizmo.SetDrawlist();
                ImGuizmo.SetRect(ImGui.GetWindowPos().X, ImGui.GetWindowPos().Y, _editorScreen.ScreenTexture.Size.X, _editorScreen.ScreenTexture.Size.Y);
                float[] viewArray = new float[16];
                float[] projectionArray = new float[16];
                float[] goArray = new float[16];
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        viewArray[i + j] = viewMat[i, j];
                        projectionArray[i + j] = projectionMat[i, j];
                        goArray[i + j] = goMatrix[i, j];
                    }
                }

               // ImGuizmo.DrawCubes(ref viewArray[0], ref projectionArray[0], ref goArray[0], 1);

            }*/
        }


        private static void SetupImGuiStyle()
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

        private unsafe void Config()
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

            SelectFont(path, 18);

            var iconsPath = "Resources\\Fonts\\forkawesome-webfont.ttf";

            unsafe
            {
                ImFontConfigPtr config = ImGuiNative.ImFontConfig_ImFontConfig();
                config.MergeMode = true; 
                config.PixelSnapH = true;
                GCHandle rangeHandle = GCHandle.Alloc(new ushort[] { 0xe000, 0xf8ff, 0 }, GCHandleType.Pinned);
                IconsFont = io.Fonts.AddFontFromFileTTF(iconsPath, 15, config, rangeHandle.AddrOfPinnedObject());
            }

            io.Fonts.Build();
            if (io.Fonts.IsBuilt())
            {
                Logger.AddLog("Builded fonts");
            }
            _controller.RecreateFontDeviceTexture();
        }

        public void Dispose()
        {
        }
    }
}
