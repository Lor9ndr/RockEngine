using FontAwesome.Constants;

using ImGuiNET;

using RockEngine.Common.Utils;
using RockEngine.ECS;
using RockEngine.ECS.Assets;
using RockEngine.ECS.GameObjects;
using RockEngine.Editor.ImguiEditor;
using RockEngine.Editor.ImguiEditor.DragDrop;
using RockEngine.Editor.ImguiEditor.FieldProcessors;

namespace RockEngine.Rendering.Layers
{
    public partial class ImGuiRenderer
    {
        private string? _gameObjectName;

        private readonly Dictionary<Type, string> typeIcons = new Dictionary<Type, string>()
        {
            { typeof(DirectLight), FA.LIGHTBULB_O },
            { typeof(MaterialComponent), FA.IMAGE },
            { typeof(Transform), FA.CUBE },
            { typeof(Camera), FA.CAMERA },
            { typeof(MeshComponent), FA.CUBES },
            { typeof(EngineRigidBody), FA.RSS_SQUARE }
        };

        private static readonly Type[ ] _componentTypes = new Type[ ]
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
            var components = gameObject.GetComponents();
            for(int i = 0; i < components.Count; i++)
            {
                var component = components.ElementAt(i);

                var type = component.GetType();

                ImguiHelper.FaIconText(typeIcons[type]);
                ImGui.SameLine();
                ImGui.Text(type.Name);
                ImGui.SameLine();
                var endPosX = winSizeX - 30;
                ImGui.SetCursorPosX(endPosX);

                if(ImguiHelper.FaIconButton($"{FA.BARS}##{type}"))
                {
                    ImGui.CloseCurrentPopup();
                    ImGui.OpenPopup($"ComponentContextMenu##{type}");
                }

                ContextMenuComponents(type, component);

                FieldProcessor.ProcessComponent(ref component);

                //DrawProperties(component, type);

                ImGui.Separator();
            }
        }

        /*private unsafe void DrawProperties(object component, Type type)
        {
            // Check if the fields are already cached
            if(!_cachedProperties.TryGetValue(type, out var properties))
            {
                // Cache the properties if not already cached
                // Include BindingFlags.NonPublic to get private properties
                var nonSerializedTypeAttribute = typeof(NonSerializedAttribute);
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(s => Attribute.IsDefined(s, typeof(UIAttribute))).ToArray();

                _cachedProperties[type] = properties;
            }

            foreach(var property in properties)
            {
                var attribute = property.GetCustomAttribute<UIAttribute>()!; // we can be sure that attribute is not null because of the previous check and processing inside of if statement
                ProcessGameObjectComponentProperties(component, property, attribute);
            }
        }

        private unsafe void DrawFields(object component, Type type)
        {
            // Check if the fields are already cached
            if(!_cachedFields.TryGetValue(type, out var fields))
            {
                // Cache the fields if not already cached
                // Include BindingFlags.NonPublic to get private fields
                var nonSerializedTypeAttribute = typeof(NonSerializedAttribute);
                fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                // We don't need the fields that are defined by NonSerializedAttribute
                fields = fields.Except(fields.Where(s => s.IsPrivate  || Attribute.IsDefined(s, nonSerializedTypeAttribute))).ToArray();
                _cachedFields[type] = fields;
            }

            foreach(var field in fields)
            {
                var attribute = field.GetCustomAttribute<UIAttribute>();

                if(attribute != null)
                {
                    FieldProcessProcessGameObjectComponentFields(component, field, attribute);
                }
                // Check if the field is a class
                if(field.FieldType.IsClass && field.FieldType != typeof(string))
                {
                    var fieldValue = field.GetValue(component);
                    if(fieldValue != null)
                    {
                        var processor =  (IFieldProcessor<object>)ProcessorFactory.GetProcessor(field.FieldType);
                        processor.Process(field);
                       *//* if(field.FieldType.IsValueType && field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Tuple<,>))
                        {
                            // Process tuples
                            ProcessTupleField(fieldValue);
                        }
                        else if(field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                        {
                            // Process key-value pairs
                            ProcessKeyValuePairField(fieldValue);
                        }
                        else if(typeof(IEnumerable).IsAssignableFrom(field.FieldType))
                        {
                            // Process other IEnumerable items
                            ProcessEnumerableField(fieldValue);
                        }
                        else
                        {
                            // Process nested class objects
                            DrawFields(fieldValue, field.FieldType);
                            DrawProperties(fieldValue, field.FieldType);
                        }*//*
                    }
                }
                *//* else if(component is IAsset)
                 {
                     ProcessAssetField(component, field);
                 }*//*

            }
        }*/

        internal static unsafe bool TryGetAssetFromPayload(ImGuiPayloadPtr payload, out IAsset? asset, out DragDropAsset ddAsset)
        {
            asset = null;
            ddAsset = default;
            if(payload.NativePtr != null && payload.Data != IntPtr.Zero)
            {
                ddAsset = payload.Data.FromIntPtr<DragDropAsset>();
                asset = AssetManager.GetAssetByID(ddAsset.ID);
            }
            return asset != null;
        }

        partial void HandleGameObject(GameObject gameObject)
        {
            _gameObjectName = gameObject.Name;

            if(ImGui.InputText("GameObject name", ref _gameObjectName, 120, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.EnterReturnsTrue))
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
            if(ImGui.BeginPopupContextItem($"ComponentContextMenu##{type}"))
            {
                if(ImGui.MenuItem($"Remove Component##{type}", component is not Transform))
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
            if(ImGui.Button("Add component"))
            {
                ImGui.CloseCurrentPopup();
                ImGui.OpenPopup("Components");
            }
            if(ImGui.BeginPopup("Components"))
            {
                // Array of component type names
                string[ ] componentTypeNames = new string[_componentTypes.Length];
                for(int i = 0; i < _componentTypes.Length; i++)
                {
                    componentTypeNames[i] = _componentTypes[i].Name;
                }

                int selectedComponentIndex = -1;
                if(ImGui.ListBox(string.Empty, ref selectedComponentIndex, componentTypeNames, componentTypeNames.Length))
                {
                    Type selectedComponentType = _componentTypes[selectedComponentIndex];
                    IComponent selectedComponent = (IComponent)Activator.CreateInstance(selectedComponentType);
                    gameObject.AddComponent(selectedComponent);
                }
                ImGui.EndPopup();
            }
        }
    }
}
