using ImGuiNET;

using RockEngine.Common.Editor;

using System.Collections;
using System.Reflection;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors.Processors
{
    internal sealed class DictionaryFieldProcessor : IUIFieldProcessor
    {
        public bool CanProcess(Type fieldType)
        {
            return fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        public void Process(ref object value, FieldInfo field, UIAttribute? attribute = null)
        {
            string alias = FieldProcessor.CreateAlias(value, field, attribute);
            ImGui.Text(alias);
            var dict = (IDictionary)value;
            var items = dict.Cast<object>().ToArray();

            dict.Clear();
            for(int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                
                FieldProcessor.ProcessField(ref item);
                items[i] = item; // Update the modified item in the array
                var kv = (KeyValuePair<object, object>)item;
                dict.Add(kv.Key, kv.Value);
            }
            
            value = dict;
        }
    }
}
