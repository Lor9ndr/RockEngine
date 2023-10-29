using FontAwesome.Constants;

using ImGuiNET;

using RockEngine.Assets;
using RockEngine.Editor;
using RockEngine.Editor.ImguiEditor;
using RockEngine.Engine.ECS;
using RockEngine.Engine.ECS.GameObjects;
using RockEngine.Engine.EngineStates;

using System.Numerics;
using System.Reflection;
using System.Text;

using OpenMath = OpenTK.Mathematics;

namespace RockEngine.Rendering.Layers
{
    public partial class ImGuiRenderer
    {
        private string? _gameObjectName;

        private bool _componentChanged;

        private readonly Dictionary<Type, FieldInfo[]> cachedFields = new Dictionary<Type, FieldInfo[]>();
        private readonly Dictionary<Type, PropertyInfo[]> cachedProperties = new Dictionary<Type, PropertyInfo[]>();

        private readonly Dictionary<Type, string> typeIcons = new Dictionary<Type, string>()
        {
            { typeof(DirectLight), FA.LIGHTBULB_O },
            { typeof(MaterialComponent), FA.IMAGE },
            { typeof(Transform), FA.CUBE },
            { typeof(Camera), FA.CAMERA },
            { typeof(MeshComponent), FA.CUBES },
            { typeof(EngineRigidBody), FA.RSS_SQUARE }
        };

        private static readonly Type[] _componentTypes = new Type[]
        {
            typeof(Camera),
            typeof(DirectLight),
            typeof(MeshComponent),
            typeof(MaterialComponent),
            typeof(Transform)
         };

        partial void ProcessGameObjectComponents(GameObject gameObject)
        {
            var winSizeX = ImGui.GetWindowWidth();

            foreach(var component in gameObject.GetComponents()!)
            {
                //EngineStateManager.SaveState(gameObject, component);
                var type = component.GetType();

                ImguiHelper.FaIconText(typeIcons[type]);
                ImGui.SameLine();
                ImGui.Text(type.Name);
                ImGui.SameLine();
                var endPosX = winSizeX - 30;
                ImGui.SetCursorPosX(endPosX);

                if (ImguiHelper.FaIconButton($"{FA.BARS}##{type}"))
                {
                    ImGui.CloseCurrentPopup();
                    ImGui.OpenPopup($"ComponentContextMenu##{type}");

                }
                ContextMenuComponents(type, component);
                
                DrawFields(component, type);
                
                DrawProperties(component, type);

                if(_componentChanged)
                {
                    //EngineStateManager.Undo();
                }

                ImGui.Separator();
            }
        }

        private void DrawProperties(IComponent component, Type type)
        {
            // Check if the properties are already cached
            if (!cachedProperties.TryGetValue(type, out var properties))
            {
                // Cache the properties if not already cached
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                cachedProperties[type] = properties;
            }

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<UIAttribute>();
                if (attribute != null)
                {
                    ProcessGameObjectComponentProperties(component, property, attribute);
                }
            }
        }

        private void DrawFields(IComponent component, Type type)
        {
            // Check if the fields are already cached
            if (!cachedFields.TryGetValue(type, out var fields))
            {
                // Cache the fields if not already cached
                fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                cachedFields[type] = fields;
            }

            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<UIAttribute>();
                if (attribute != null)
                {
                    ProcessGameObjectComponentFields(component, field, attribute);
                }
                else
                {
                    // Check if the field is of type BaseAsset
                    if(field.FieldType == typeof(BaseAsset))
                    {
                        // Display a drag and drop area for the asset
                        if(ImGui.BeginDragDropTarget())
                        {
                            var payload = ImGui.AcceptDragDropPayload("ASSET_PAYLOAD");
                            if(payload.Data != IntPtr.Zero)
                            {
                                // Retrieve the dragged asset from the payload
                                var draggedAsset = (BaseAsset)payload.Data;

                                // Set the field value to the dragged asset
                                field.SetValue(component, draggedAsset);
                            }

                            ImGui.EndDragDropTarget();
                        }
                    }
                }
            }
        }

        partial void HandleGameObject(GameObject gameObject)
        {
            _gameObjectName = gameObject.Name;

            if (ImGui.InputText("GameObject name", ref _gameObjectName, 120, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.EnterReturnsTrue))
            {
                gameObject.Name = _gameObjectName;
            }

            ImGui.Separator();
            var text = "Components";
            var winSize = ImGui.GetWindowSize().X;
            var textWidth = ImGui.CalcTextSize(text).X;
            var centerPosX = (winSize - textWidth) / 2;

            ImGui.SetCursorPosX(centerPosX);
            ImGui.Text(text);

            ImGui.Separator();
        }

        private static void ContextMenuComponents(Type type, IComponent component)
        {
            if (ImGui.BeginPopupContextItem($"ComponentContextMenu##{type}"))
            {
                if (ImGui.MenuItem("Remove Component", component is not Transform))
                {
                    component.Parent.RemoveComponent(component);
                }

                ImGui.EndPopup();
            }
        }

        partial void AddComponentsWindow(GameObject gameObject)
        {
            var winSizeX = ImGui.GetWindowWidth();

            var text = "Add component";
            var centerPosX = (winSizeX - ImGui.CalcTextSize(text).X) / 2.0f;

            ImGui.SetCursorPosX(centerPosX);
            if (ImGui.Button("Add component"))
            {
                ImGui.CloseCurrentPopup();
                ImGui.OpenPopup("Components");
            }
            if (ImGui.BeginPopup("Components"))
            {
                // Array of component type names
                string[] componentTypeNames = new string[_componentTypes.Length];
                for (int i = 0; i < _componentTypes.Length; i++)
                {
                    componentTypeNames[i] = _componentTypes[i].Name;
                }

                int selectedComponentIndex = -1;
                if (ImGui.ListBox(string.Empty, ref selectedComponentIndex, componentTypeNames, componentTypeNames.Length))
                {
                    Type selectedComponentType = _componentTypes[selectedComponentIndex];
                    IComponent selectedComponent = (IComponent)Activator.CreateInstance(selectedComponentType);
                    gameObject.AddComponent(selectedComponent);
                }
                ImGui.EndPopup();
            }
        }
        private void ProcessGameObjectComponentFields(IComponent component, FieldInfo field, UIAttribute attribute)
        {
            var value = field.GetValue(component);

            var alias = attribute.Alias;
            if (alias == UIAttribute.UNKNOWN)
            {
                alias = ProcessAlias(field.Name);
            }
            if (component is IAsset asset)
            {
                alias += $"##{asset.ID}";
            }
            if (value is OpenMath.Vector3 v3)
            {
                var realValue = new Vector3(v3.X, v3.Y, v3.Z);
                if (attribute.IsColor)
                {
                    if (ImGui.ColorEdit3(alias, ref realValue))
                    {
                        field.SetValue(component, new OpenMath.Vector3(realValue.X, realValue.Y, realValue.Z));
                        _componentChanged = true;
                    }
                }
                else
                {
                    if (ImGui.DragFloat3(alias, ref realValue))
                    {
                        field.SetValue(component, new OpenMath.Vector3(realValue.X, realValue.Y, realValue.Z));
                        _componentChanged = true;
                    }
                }
            }
            else if (value is OpenMath.Vector4 v4)
            {
                var realValue = new Vector4(v4.X, v4.Y, v4.Z, v4.W);
                if (attribute.IsColor)
                {
                    if (ImGui.ColorEdit4(alias, ref realValue))
                    {
                        field.SetValue(component, new OpenMath.Vector4(realValue.X, realValue.Y, realValue.Z, realValue.W));
                        _componentChanged = true;
                    }
                }
                else
                {
                    if (ImGui.DragFloat4(alias, ref realValue))
                    {
                        field.SetValue(component, new OpenMath.Vector4(realValue.X, realValue.Y, realValue.Z, realValue.W));
                        _componentChanged = true;
                    }
                }
            }
            else if (value is float valueF)
            {
                if (ImGui.DragFloat(alias, ref valueF, 0.1f))
                {
                    field.SetValue(component, valueF);
                    _componentChanged = true;
                }
            }
            else if (value is int number)
            {
                if (ImGui.DragInt(alias, ref number))
                {
                    field.SetValue(component, number);
                    _componentChanged = true;
                }
            }
            else if (value is Enum)
            {
                var type = value.GetType();
                var names = Enum.GetNames(type);
                var values = Enum.GetValues(type);
                var selectedIndex = Array.IndexOf(values, value);

                if (ImGui.Combo(alias, ref selectedIndex, names, names.Length))
                {
                    field.SetValue(component, values.GetValue(selectedIndex));
                    _componentChanged = true;
                }
            }
        }

        private void ProcessGameObjectComponentProperties(IComponent component, PropertyInfo field, UIAttribute attribute)
        {
            var value = field.GetValue(component);

            var alias = attribute.Alias;
            if (alias == UIAttribute.UNKNOWN)
            {
                alias = ProcessAlias(field.Name);
            }
            if (value is OpenMath.Vector3 v3)
            {
                var realValue = new Vector3(v3.X, v3.Y, v3.Z);
                if (attribute.IsColor)
                {
                    if (ImGui.ColorEdit3(alias, ref realValue))
                    {
                        field.SetValue(component, new OpenMath.Vector3(realValue.X, realValue.Y, realValue.Z));
                        _componentChanged = true;
                    }
                }
                else
                {
                    if (ImGui.DragFloat3(alias, ref realValue))
                    {
                        field.SetValue(component, new OpenMath.Vector3(realValue.X, realValue.Y, realValue.Z));
                        _componentChanged = true;
                    }
                }
            }
            else if (value is OpenMath.Vector4 v4)
            {
                var realValue = new Vector4(v4.X, v4.Y, v4.Z, v4.W);
                if (attribute.IsColor)
                {
                    if (ImGui.ColorEdit4(alias, ref realValue))
                    {
                        field.SetValue(component, new OpenMath.Vector4(realValue.X, realValue.Y, realValue.Z, realValue.W));
                        _componentChanged = true;
                    }
                }
                else
                {
                    if (ImGui.DragFloat4(alias, ref realValue))
                    {
                        field.SetValue(component, new OpenMath.Vector4(realValue.X, realValue.Y, realValue.Z, realValue.W));
                        _componentChanged = true;
                    }
                }
            }
            else if (value is float valueF)
            {
                if (ImGui.DragFloat(alias, ref valueF, 0.1f))
                {
                    field.SetValue(component, valueF);
                     _componentChanged = true;
                }
            }
            else if (value is int number)
            {
                if (ImGui.DragInt(alias, ref number))
                {
                    field.SetValue(component, number);
                     _componentChanged = true;
                }
            }
            else if (value is Enum)
            {
                var type = value.GetType();
                var names = Enum.GetNames(type);
                var values = Enum.GetValues(type);
                var selectedIndex = Array.IndexOf(values, value);

                if (ImGui.Combo(alias, ref selectedIndex, names, names.Length))
                {
                    field.SetValue(component, values.GetValue(selectedIndex));
                     _componentChanged = true;
                }
            }
        }
        private static string ProcessAlias(string alias)
        {
            var aliasBuilder = new StringBuilder(alias);
            for (int i = 1; i < aliasBuilder.Length; i++)
            {
                if (char.IsUpper(aliasBuilder[i]))
                {
                    aliasBuilder.Insert(i, ' ');
                    i++;
                }
            }
            alias = aliasBuilder.ToString();
            return alias;
        }
    }
}
