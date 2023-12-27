using ImGuiNET;

using RockEngine.Assets;
using RockEngine.Editor.ImguiEditor.FieldProcessors.Processors;
using RockEngine.Engine.ECS;
using RockEngine.Rendering.Layers;

using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using OpenMath = OpenTK.Mathematics;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors
{
    internal static class FieldProcessor
    {
        #region Reflection Cache 
        private static readonly Dictionary<Type, FieldInfo[ ]> _cachedFields = new Dictionary<Type, FieldInfo[ ]>();
        private static readonly Dictionary<Type, PropertyInfo[ ]> _cachedProperties = new Dictionary<Type, PropertyInfo[ ]>();
        private static readonly Dictionary<FieldInfo, Func<object, object>> _fieldGettersCache = new Dictionary<FieldInfo, Func<object, object>>();
        private static readonly Dictionary<PropertyInfo, Func<object, object>> _propertyGettersCache = new Dictionary<PropertyInfo, Func<object, object>>();
        private static readonly Dictionary<FieldInfo, Action<object, object>> _fieldSettersCache = new Dictionary<FieldInfo, Action<object, object>>();
        private static readonly Dictionary<PropertyInfo, Action<object, object>> _propertySettersCache = new Dictionary<PropertyInfo, Action<object, object>>();

        private static readonly Type _keyValuePairType  = typeof(KeyValuePair<,>);

        #endregion

        public static readonly Dictionary<Type, IUIFieldProcessor> FieldProcessors = new Dictionary<Type, IUIFieldProcessor>
        {
            { typeof(KeyValuePair<,>), new KeyValuePairFieldProcessor() },
            { typeof(OpenMath.Vector3), new Vector3FieldProcessor() },
            { typeof(OpenMath.Vector4), new Vector4FieldProcessor() },
            { typeof(OpenMath.Quaternion), new QuaternionFieldProcessor() },
            { typeof(Enum), new EnumFieldProcessor() },
            { typeof(Enumerable), new EnumerableFieldProcessor() },
            { typeof(IList), new EnumerableFieldProcessor() },
            { typeof(List<>), new EnumerableFieldProcessor() },
            { typeof(int), new IntFieldProcessor() },
            { typeof(float), new FloatFieldProcessor() },
            { typeof(IDictionary), new DictionaryFieldProcessor() },
            { typeof(DictionaryBase), new DictionaryFieldProcessor() },
            { typeof(Dictionary<,>), new DictionaryFieldProcessor() },
            { typeof(Dictionary<string,object>), new DictionaryFieldProcessor() },
            { typeof(Tuple), new TupleFieldProcessor() },
            { typeof(string), new StringFieldProcessor() }
        };

        public static void ProcessField(ref object obj, string alias = UIAttribute.UNKNOWN)
        {
            if(obj is null)
            {
                return;
            }

            var type = obj.GetType();
            // Check if the fields are already cached
            var fieldInfos = GetCachedFieldInfo(type);
            foreach(var processor in FieldProcessors)
            {
                if(processor.Value.CanProcess(type))
                {
                    processor.Value.Process(ref obj, null, new UIAttribute(alias));
                    return;
                }
            }

            ProcessFieldInfos(ref obj, alias, fieldInfos);
        }

        private static void ProcessFieldInfos(ref object obj, string alias, FieldInfo[]? fieldInfos)
        {
            foreach(var fieldInfo in fieldInfos)
            {
                var isProcessed = false;
                var fieldGetter = GetOrCreateFieldGetter(fieldInfo);
                var fieldSetter = GetOrCreateFieldSetter(fieldInfo);
                var value = fieldGetter(obj);
                foreach(var processor in FieldProcessors)
                {
                    if(processor.Value.CanProcess(fieldInfo.FieldType))
                    {
                        //var attribute = fieldInfo.FieldType.GetCustomAttribute<UIAttribute>();
                        var attribute = new UIAttribute(alias);
                        processor.Value.Process(ref value, fieldInfo, attribute);
                        fieldSetter(obj, value);
                        isProcessed = true;
                        break;
                    }
                }
                if(!isProcessed)
                {
                    ProcessField(ref value);
                    fieldSetter(obj, value);
                }
            }
        }

        internal static void ProcessComponent(ref IComponent component)
        {
            var value = (object)component;
            ProcessField(ref value);
            component = (IComponent)value;
        }

        public static FieldInfo[]? GetCachedFieldInfo(Type type)
        {
            if(!_cachedFields.TryGetValue(type, out var fields))
            {
                if(type.IsGenericType)
                {
                    var genericType = type.GetGenericTypeDefinition();
                    if(_keyValuePairType == genericType)
                    {
                        fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    }
                }
                else
                {
                    // Cache the fields if not already cached
                    fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                        .Where(field => !Attribute.IsDefined(field, typeof(DiableUIAttribute)))
                        .ToArray();
                }
                

                _cachedFields[type] = fields;
                return fields;
            }
            return fields;
        }

        public static string CreateAlias(object obj, FieldInfo field, UIAttribute attribute)
        {
            string alias;
            if(attribute is not null)
            {
                if(attribute.Alias == UIAttribute.UNKNOWN && field is not null)
                {
                    alias = ProcessAlias(field.Name);
                }
                else
                {
                    alias = attribute.Alias;
                }
            }
            else
            {
                alias = $"##{Guid.NewGuid()}";
            }
            
            if(obj is IAsset asset)
            {
                alias += $"##{asset.ID}";
            }

            return alias;
        }

        private static string ProcessAlias(string alias)
        {
            var aliasBuilder = new StringBuilder(alias);
            for(int i = 1; i < aliasBuilder.Length; i++)
            {
                if(char.IsUpper(aliasBuilder[i]))
                {
                    aliasBuilder.Insert(i, ' ');
                    i++;
                }
            }
            alias = aliasBuilder.ToString();
            return alias;
        }

        private static unsafe void ProcessAssetProperty(object component, PropertyInfo property)
        {
            if(typeof(IAsset).IsAssignableFrom(property.PropertyType))
            {
                // Retrieve the current value of the field
                var propertyGetter = CreatePropertyGetter(property);
                var fieldValue = (IAsset)propertyGetter(component);

                ImGui.BulletText(fieldValue != null ? fieldValue.Name : "No Asset");

                if(ImGui.BeginDragDropTarget())
                {
                    var payload = ImGui.AcceptDragDropPayload(ImGuiRenderer.SELECTED_ASSET_PAYLOAD);
                    if(ImGuiRenderer.TryGetAssetFromPayload(payload, out var asset, out var ddAsset))
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

        public static Func<object, object> CreatePropertyGetter(PropertyInfo property)
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

        public static Action<object, object> CreatePropertySetter(PropertyInfo property)
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
            
        public static Func<object, object> GetOrCreateFieldGetter(FieldInfo field)
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

        public static Action<object, object> GetOrCreateFieldSetter(FieldInfo field)
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
