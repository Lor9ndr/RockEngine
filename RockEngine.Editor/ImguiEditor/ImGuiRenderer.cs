
using OpenMath = OpenTK.Mathematics;
using System.Numerics;
using RockEngine.Utils;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using FontAwesome.Constants;
//using ImGuizmoNET;
using NativeFileDialogSharp;
using RockEngine.Engine.ECS.GameObjects;
using RockEngine.Engine;
using RockEngine.Inputs;
using RockEngine.Engine.EngineStates;
using RockEngine.Assets;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RockEngine.Editor.ImguiEditor;
using RockEngine.Editor.Layers;
using RockEngine.Physics;
using RockEngine.Editor;

namespace RockEngine.Rendering.Layers
{
    public partial class ImGuiRenderer : IDisposable
    {
        public static bool IsMouseOnEditorScreen { get; private set;}
        public static Vector2 EditorScreenMousePos { get; private set;}

        private readonly ImGuiOpenGL _controller;

        private const string LAYOUT_FILE = "Layout.ini";
        
        #region GUI Fields

        public GameObject? SelectedGameObject = null;
        private int _currentFontIndex;

        private readonly string[] _fontsPaths = new string[]
        {
            "Resources\\Fonts\\Roboto-Regular.ttf",
            "Resources\\Fonts\\InclusiveSans-Regular.ttf"
        };
        public bool DrawCollisions;

        #endregion

        private readonly CameraTexture _editorScreen;
        private readonly CameraTexture _gameScreen;

        private bool _projectWindowIsOpened;
        private readonly DefaultEditorLayer _editorLayer;
        private readonly DefaultGameLayer _gameLayer;
        private readonly PhysicsManager _physicsManager;
        private readonly EngineWindow _window;

        public ImGuiRenderer(EngineWindow window, DefaultEditorLayer editorLayer, PhysicsManager physics)
        {
            _window = window;
            _controller = new ImGuiOpenGL();
            _ = new ImGuiInput(window);
            var app = Application.GetCurrentApp()!;
            _editorLayer = editorLayer;
            _gameLayer = app.Layers.GetLayer<DefaultGameLayer>()!;
            _editorScreen = _editorLayer.Screen;
            _gameScreen = _gameLayer.Screen;

            ImguiHelper.Config(_controller);
            ImguiHelper.SetupImGuiStyle();

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

        partial void DisplayAssetFolders();

        partial void DisplayAssets();

        #endregion

        public void OnRender()
        {
            IsMouseOnEditorScreen = false;
            ImGui.NewFrame();
            GL.Viewport(0, 0, _window.Size.X, _window.Size.Y);
            var dockID = ImGui.DockSpaceOverViewport(ImGui.GetMainViewport(),  ImGuiDockNodeFlags.PassthruCentralNode | ImGuiDockNodeFlags.AutoHideTabBar);
            ImGui.PushFont(ImguiHelper.Mainfont);
            TopBar();
            
            ImGui.DockSpace(dockID, Vector2.Zero, ImGuiDockNodeFlags.PassthruCentralNode);
            Logger.DrawDebugLogWindow();

            ImGui.DockSpace(dockID, Vector2.Zero, ImGuiDockNodeFlags.PassthruCentralNode);
            DisplayScene();

            ImGui.DockSpace(dockID, Vector2.Zero, ImGuiDockNodeFlags.PassthruCentralNode);
            DisplaySelectedGameObjectProperties();

            ImGui.DockSpace(dockID, Vector2.Zero, ImGuiDockNodeFlags.PassthruCentralNode);
            EditorScreen();

            ImGui.DockSpace(dockID, Vector2.Zero, ImGuiDockNodeFlags.PassthruCentralNode);
            GameScreen();

            ImGui.DockSpace(dockID, Vector2.Zero, ImGuiDockNodeFlags.PassthruCentralNode);
            DisplayAssetFolders();

            SelectProjectWindow();

            ImGui.PopFont();

            if(Input.IsKeyDown(Keys.LeftControl) && Input.IsKeyDown(Keys.S))
            {
                AssetManager.SaveAll();
            }

            _controller.Render(_window.Size);
        }

        public void AddGameObjectContextWindow()
        {

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
                var mousePos = ImGui.GetMousePos();
                var padding = ImGui.GetStyle().WindowPadding;
                var winPos = ImGui.GetWindowPos();
                var winSize = ImGui.GetWindowSize();
                _editorScreen.Resize(new OpenMath.Vector2i((int)winSize.X, (int)winSize.Y));
                ImGui.Image(_editorScreen.ScreenTexture.Handle, winSize, new Vector2(0, 1), new Vector2(1, 0));

                var x = mousePos.X  - winPos.X - padding.X;
                x = (x / winSize.X) * winSize.X;

                var y = mousePos.Y - winPos.Y - padding.Y;
                y = winSize.Y - y / winSize.Y * winSize.Y;
                EditorScreenMousePos = new Vector2(x, y);
                ImGui.End();
            }
        }

        private void GameScreen()
        {
            if (ImGui.Begin("GAME SCREEN", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoTitleBar))
            {
                var size = ImGui.GetWindowContentRegionMax();
                //_gameScreen.Resize(new OpenMath.Vector2i((int)size.X, (int)size.Y));
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

                ImGui.Text($"FPS:{_window.FPS}");
                
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
                        ImguiHelper.SelectFont(_controller, _fontsPaths[_currentFontIndex]);
                    }
                    ImGui.EndMenu();
                }
                ImGui.PushFont(ImguiHelper.IconsFont);
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
                if(ImGui.RadioButton("Draw collisions", EditorSettings.DrawCollisions))
                {
                    EditorSettings.DrawCollisions = !EditorSettings.DrawCollisions;
                }
                ImGui.NextColumn();

                ImGui.EndMainMenuBar();
            }
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
                if(ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                {
                    if(SelectedGameObject is not null)
                    {
                        var camera = _editorLayer.DebugCamera.GetComponent<Camera>();
                        camera?.LookAt(SelectedGameObject!);
                        camera?.MoveToTarget(10f, SelectedGameObject!.Transform.Position);
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
            IsMouseOnEditorScreen = IsMouseOnEditorScreen || isHovering && ImGui.IsWindowFocused();
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

      

      

        public void Dispose()
        {
        }
    }
}
