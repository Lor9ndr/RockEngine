using ImGuiNET;

using RockEngine.Common.Editor;

using System.Collections;
using System.Reflection;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors.Processors
{
    internal sealed class EnumerableFieldProcessor : IUIFieldProcessor
    {
        public bool CanProcess(Type fieldType)
        {
            return fieldType.GetInterface(nameof(IEnumerable)) != null && fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() != typeof(Dictionary<,>);
        }

        public void Process(ref object value, FieldInfo field, UIAttribute attribute)
        {
            if(value is null)
            {
                return;
            }
            string alias = FieldProcessor.CreateAlias(value, field, attribute);
            var valueType = value.GetType();
            var fieldInfo = valueType.GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
            ImGui.Text(alias);
            // Convert the enumerable to an array to avoid modifying it while iterating
            object[ ] items = ((IEnumerable)value).Cast<object>().ToArray();

            for(int i = 0; i < items.Length; i++)
            {
                object item = items[i];
                FieldProcessor.ProcessField(ref item);
                items[i] = item; // Update the modified item in the array
            }
        }
    }
}
