using FontAwesome.Constants;

using ImGuiNET;

using RockEngine.Assets;
using RockEngine.Editor;
using RockEngine.Editor.ImguiEditor;
using RockEngine.Editor.ImguiEditor.DragDrop;
using RockEngine.Engine.ECS;
using RockEngine.Engine.ECS.GameObjects;
using RockEngine.Utils;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using OpenMath = OpenTK.Mathematics;

namespace RockEngine.Rendering.Layers
{
    public partial class ImGuiRenderer
    {
        private string? _gameObjectName;

        #region Reflection Cache 
        private static readonly Dictionary<Type, FieldInfo[]> _cachedFields = new Dictionary<Type, FieldInfo[]>();
        private static readonly Dictionary<Type, PropertyInfo[]> _cachedProperties = new Dictionary<Type, PropertyInfo[]>();
        private static readonly Dictionary<FieldInfo, Func<object, object>> _fieldGettersCache = new Dictionary<FieldInfo, Func<object, object>>();
        private static readonly Dictionary<PropertyInfo, Func<object, object>> _propertyGettersCache = new Dictionary<PropertyInfo, Func<object, object>>();
        private static readonly Dictionary<FieldInfo, Action<object, object>> _fieldSettersCache = new Dictionary<FieldInfo, Action<object, object>>();
        private static readonly Dictionary<PropertyInfo, Action<object, object>> _propertySettersCache = new Dictionary<PropertyInfo, Action<object, object>>();
        #endregion

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

                ImGui.Separator();
            }
        }

        private unsafe void DrawProperties(IComponent component, Type type)
        {
            // Check if the properties are already cached
            if (!_cachedProperties.TryGetValue(type, out var properties))
            {
                // Cache the properties if not already cached
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                _cachedProperties[type] = properties;
            }

            foreach(var property in properties)
            {
                var attribute = property.GetCustomAttribute<UIAttribute>();
                if (attribute != null)
                {
                    ProcessGameObjectComponentProperties(component, property, attribute);
                }
                else
                {
                    if(typeof(IAsset).IsAssignableFrom(property.PropertyType))
                    {
                        // Retrieve the current value of the field
                        var fieldGetter = CreatePropertyGetter(property);
                        var fieldValue = (IAsset)fieldGetter(component);

                        ImGui.BulletText(fieldValue != null ? fieldValue.Name : "No Asset");

                        if(ImGui.BeginDragDropTarget())
                        {
                            var payload = ImGui.AcceptDragDropPayload(SELECTED_ASSET_PAYLOAD);
                            if(payload.NativePtr != null && payload.Data != IntPtr.Zero)
                            {
                                // Retrieve the dragged asset from the payload
                                var draggedAsset = payload.Data.FromIntPtr<DragDropAsset>();
                                var asset = AssetManager.GetAssetByID(draggedAsset.ID);

                                // Check if the AssetTypes match
                                if(fieldValue.Type == draggedAsset.AssetType)
                                {
                                    // The DragDropAsset.AssetType fits in the current field type
                                    property.SetValue(component, asset);
                                }
                            }

                            ImGui.EndDragDropTarget();
                        }
                    }
                }
            }
        }

        private unsafe void DrawFields(IComponent component, Type type)
        {
            // Check if the fields are already cached
            if(!_cachedFields.TryGetValue(type, out var fields))
            {
                // Cache the fields if not already cached
                fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                _cachedFields[type] = fields;
            }
            foreach(var field in fields)
            {
                var attribute = field.GetCustomAttribute<UIAttribute>();
                if(attribute != null)
                {
                    ProcessGameObjectComponentFields(component, field, attribute);
                }
                else
                {
                    ProcessAssetField(component, field);
                }
            }
        }

        private unsafe void ProcessAssetField(IComponent component, FieldInfo field)
        {
            if(typeof(IAsset).IsAssignableFrom(field.FieldType))
            {
                // Retrieve the current value of the field
                var fieldGetter = CreateFieldGetter(field);
                var fieldValue = (IAsset)fieldGetter(component);

                ImGui.BulletText(fieldValue != null ? fieldValue.Name : "No Asset");

                if(ImGui.BeginDragDropTarget())
                {
                    var payload = ImGui.AcceptDragDropPayload(SELECTED_ASSET_PAYLOAD);
                    if(TryGetAssetFromPayload(payload, out var asset, out var ddAsset))
                    {
                        // Check if the AssetTypes match
                        if(field.FieldType == asset.GetType())
                        {
                            // The DragDropAsset.AssetType fits in the current field type
                            field.SetValue(component, asset);
                        }
                    }
                    ImGui.EndDragDropTarget();
                }
            }
        }

        private unsafe void ProcessAssetProperty(IComponent component, PropertyInfo property)
        {
            if(typeof(IAsset).IsAssignableFrom(property.PropertyType))
            {
                // Retrieve the current value of the field
                var propertyGetter = CreatePropertyGetter(property);
                var fieldValue = (IAsset)propertyGetter(component);

                ImGui.BulletText(fieldValue != null ? fieldValue.Name : "No Asset");

                if(ImGui.BeginDragDropTarget())
                {
                    var payload = ImGui.AcceptDragDropPayload(SELECTED_ASSET_PAYLOAD);
                    if(TryGetAssetFromPayload(payload, out var asset, out var ddAsset))
                    {
                        // Check if the AssetTypes match
                        if(property.PropertyType == asset.GetType())
                        {
                            // The DragDropAsset.AssetType fits in the current field type
                            property.SetValue(component, asset);
                        }
                    }
                    ImGui.EndDragDropTarget();
                }
            }
        }

        private unsafe bool TryGetAssetFromPayload(ImGuiPayloadPtr payload, out IAsset? asset, out DragDropAsset ddAsset)
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
            var fieldGetter = CreateFieldGetter(field);
            var fieldSetter = CreateFieldSetter(field);
            var value = fieldGetter(component);

            var alias = attribute.Alias;
            if (alias == UIAttribute.UNKNOWN)
            {
                alias = ProcessAlias(field.Name);
            }
            if (component is IAsset asset)
            {
                alias += $"##{asset.ID}";
            }
            switch(value)
            {
                case OpenMath.Vector3 v3:
                    {
                        var realValue = new Vector3(v3.X, v3.Y, v3.Z);
                        if(attribute.IsColor)
                        {
                            if(ImGui.ColorEdit3(alias, ref realValue))
                            {
                                fieldSetter(component, new OpenMath.Vector3(realValue.X, realValue.Y, realValue.Z));
                            }
                        }
                        else
                        {
                            if(ImGui.DragFloat3(alias, ref realValue))
                            {
                                fieldSetter(component, new OpenMath.Vector3(realValue.X, realValue.Y, realValue.Z));
                            }
                        }

                        break;
                    }

                case OpenMath.Vector4 v4:
                    {
                        var realValue = new Vector4(v4.X, v4.Y, v4.Z, v4.W);
                        if(attribute.IsColor)
                        {
                            if(ImGui.ColorEdit4(alias, ref realValue))
                            {
                                fieldSetter(component, new OpenMath.Vector4(realValue.X, realValue.Y, realValue.Z, realValue.W));
                            }
                        }
                        else
                        {
                            if(ImGui.DragFloat4(alias, ref realValue))
                            {
                                fieldSetter(component, new OpenMath.Vector4(realValue.X, realValue.Y, realValue.Z, realValue.W));
                            }
                        }

                        break;
                    }

                case float valueF:
                    if(ImGui.DragFloat(alias, ref valueF, 0.1f))
                    {
                        fieldSetter(component, valueF);
                    }
                    break;
                case int number:
                    if(ImGui.DragInt(alias, ref number))
                    {
                        fieldSetter(component, number);
                    }
                    break;
                case Enum:
                    {
                        var type = value.GetType();
                        var names = Enum.GetNames(type);
                        var values = Enum.GetValues(type);
                        var selectedIndex = Array.IndexOf(values, value);

                        if(ImGui.Combo(alias, ref selectedIndex, names, names.Length))
                        {
                            fieldSetter(component, values.GetValue(selectedIndex));
                        }

                        break;
                    }
            }
        }

        private void ProcessGameObjectComponentProperties(IComponent component, PropertyInfo property, UIAttribute attribute)
        {
            var propertyGetter = CreatePropertyGetter(property);
            var propertySetter = CreatePropertySetter(property);
            var value = propertyGetter(component);

            var alias = attribute.Alias;
            if (alias == UIAttribute.UNKNOWN)
            {
                alias = ProcessAlias(property.Name);
            }
            switch(value)
            {
                case OpenMath.Vector3 v3:
                    {
                        var realValue = new Vector3(v3.X, v3.Y, v3.Z);
                        if(attribute.IsColor)
                        {
                            if(ImGui.ColorEdit3(alias, ref realValue))
                            {
                                propertySetter(component, new OpenMath.Vector3(realValue.X, realValue.Y, realValue.Z));
                            }
                        }
                        else
                        {
                            if(ImGui.DragFloat3(alias, ref realValue))
                            {
                                propertySetter(component, new OpenMath.Vector3(realValue.X, realValue.Y, realValue.Z));
                            }
                        }

                        break;
                    }

                case OpenMath.Vector4 v4:
                    {
                        var realValue = new Vector4(v4.X, v4.Y, v4.Z, v4.W);
                        if(attribute.IsColor)
                        {
                            if(ImGui.ColorEdit4(alias, ref realValue))
                            {
                                propertySetter(component, new OpenMath.Vector4(realValue.X, realValue.Y, realValue.Z, realValue.W));
                            }
                        }
                        else
                        {
                            if(ImGui.DragFloat4(alias, ref realValue))
                            {
                                propertySetter(component, new OpenMath.Vector4(realValue.X, realValue.Y, realValue.Z, realValue.W));
                            }
                        }

                        break;
                    }

                case float valueF:
                    if(ImGui.DragFloat(alias, ref valueF, 0.1f))
                    {
                        propertySetter(component, valueF);
                    }
                    break;
                case int number:
                    if(ImGui.DragInt(alias, ref number))
                    {
                        propertySetter(component, number);
                    }
                    break;
                case Enum:
                    {
                        var type = value.GetType();
                        var names = Enum.GetNames(type);
                        var values = Enum.GetValues(type);
                        var selectedIndex = Array.IndexOf(values, value);

                        if(ImGui.Combo(alias, ref selectedIndex, names, names.Length))
                        {
                            propertySetter(component, values.GetValue(selectedIndex));
                        }

                        break;
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
        
        private static Func<object, object> CreatePropertyGetter(PropertyInfo property)
        {
            if(_propertyGettersCache.TryGetValue(property, out var getter))
            {
                return getter;
            }

            var dynamicMethod = new DynamicMethod(
                "GetPropertyValue",
                typeof(object),
                new[ ] { typeof(object) },
                property.DeclaringType.Module,
                true
            );

            var il = dynamicMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0); // Load the target object
            il.Emit(OpCodes.Castclass, property.DeclaringType); // Cast the object to the declaring type
            il.Emit(OpCodes.Call, property.GetMethod); // Call the property getter
            if(property.PropertyType.IsValueType)
            {
                il.Emit(OpCodes.Box, property.PropertyType); // Box the value type
            }
            il.Emit(OpCodes.Ret); // Return

            var getterDelegate = (Func<object, object>)dynamicMethod.CreateDelegate(typeof(Func<object, object>));
            _propertyGettersCache[property] = getterDelegate;
            return getterDelegate;
        }

        private static Action<object, object> CreatePropertySetter(PropertyInfo property)
        {
            if(_propertySettersCache.TryGetValue(property, out var setter))
            {
                return setter;
            }

            var dynamicMethod = new DynamicMethod(
                "SetPropertyValue",
                typeof(void),
                new[ ] { typeof(object), typeof(object) },
                property.DeclaringType.Module,
                true
            );

            var il = dynamicMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0); // Load the target object
            il.Emit(OpCodes.Ldarg_1); // Load the value to set
            if(!property.PropertyType.IsClass)
            {
                il.Emit(OpCodes.Unbox_Any, property.PropertyType); // Unbox the value to the property type
            }
            il.Emit(OpCodes.Call, property.SetMethod); // Call the property setter
            il.Emit(OpCodes.Ret); // Return

            var setterDelegate = (Action<object, object>)dynamicMethod.CreateDelegate(typeof(Action<object, object>));
            _propertySettersCache[property] = setterDelegate;
            return setterDelegate;
        }

        private static Func<object, object> CreateFieldGetter(FieldInfo field)
        {
            if(_fieldGettersCache.TryGetValue(field, out var getter))
            {
                return getter;
            }

            var dynamicMethod = new DynamicMethod(
                "GetFieldValue",
                typeof(object),
                new[ ] { typeof(object) },
                field.DeclaringType.Module,
                true
            );

            var il = dynamicMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0); // Load the target object
            il.Emit(OpCodes.Castclass, field.DeclaringType); // Cast the object to the declaring type
            il.Emit(OpCodes.Ldfld, field); // Load the field value
            if(field.FieldType.IsValueType)
            {
                il.Emit(OpCodes.Box, field.FieldType); // Box the value type
            }
            il.Emit(OpCodes.Ret); // Return

            var getterDelegate = (Func<object, object>)dynamicMethod.CreateDelegate(typeof(Func<object, object>));
            _fieldGettersCache[field] = getterDelegate;
            return getterDelegate;
        }

        private static Action<object, object> CreateFieldSetter(FieldInfo field)
        {
            if(_fieldSettersCache.TryGetValue(field, out var setter))
            {
                return setter;
            }

            var dynamicMethod = new DynamicMethod(
                "SetFieldValue",
                typeof(void),
                new[ ] { typeof(object), typeof(object) },
                field.DeclaringType.Module,
                true
            );

            var il = dynamicMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0); // Load the target object
            il.Emit(OpCodes.Ldarg_1); // Load the value to set
            il.Emit(OpCodes.Unbox_Any, field.FieldType); // Unbox the value to the field type
            il.Emit(OpCodes.Stfld, field); // Set the field value
            il.Emit(OpCodes.Ret); // Return

            var setterDelegate = (Action<object, object>)dynamicMethod.CreateDelegate(typeof(Action<object, object>));
            _fieldSettersCache[field] = setterDelegate;
            return setterDelegate;
        }
    }
}
