using RockEngine.Common.Editor;

using System.Reflection;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors.Processors
{
    internal sealed class KeyValuePairFieldProcessor : IUIFieldProcessor
    {
        public bool CanProcess(Type fieldType)
        {
            return fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
        }

        public void Process(ref object value, FieldInfo field, UIAttribute attribute)
        {
            var kvalue = (KeyValuePair<string, object>)value;
            var kvalueItem = kvalue.Value;
            FieldProcessor.ProcessField(ref kvalueItem, kvalue.Key);
            value = new KeyValuePair<object, object>(kvalue.Key, kvalueItem);
        }
    }
}
