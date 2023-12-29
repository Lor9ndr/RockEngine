using ImGuiNET;

using RockEngine.Common.Editor;

using System.Reflection;
using System.Runtime.CompilerServices;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors.Processors
{
    internal sealed class TupleFieldProcessor : IUIFieldProcessor
    {
        public bool CanProcess(Type fieldType)
        {
            return fieldType == typeof(ITuple);
        }

        public void Process(ref object value, FieldInfo field, UIAttribute attribute)
        {
            string alias = FieldProcessor.CreateAlias(value, field, attribute);
            ImGui.Text(alias);
            var tuple = (ITuple)value;
            for(int i = 0; i < tuple.Length; i++)
            {
                var item = tuple[i];
                // Process each item in the tuple
                FieldProcessor.ProcessField(ref item);
            }
        }
    }
}
