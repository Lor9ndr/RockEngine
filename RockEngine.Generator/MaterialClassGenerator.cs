/*using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RockEngine.Generator
{
    [Generator]
    public class MaterialClassGeneratorWithShader : ISourceGenerator
    {
        private static readonly Dictionary<string, string> TypeMappings = new Dictionary<string, string>
        {
            { "sampler2D", "Texture2D" },
            { "vec3", "Vector3" },
            { "vec2", "Vector2" },
            { "int", "int" },
            { "float", "float" }
        };

        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context)
        {
            Dictionary<string, string> shaderMaterialMap = new Dictionary<string, string>();

            foreach(var file in GL.AdditionalFiles)
            {
                if(file.Path.EndsWith(".frag", StringComparison.OrdinalIgnoreCase))
                {
                    string shaderName = Path.GetFileNameWithoutExtension(file.Path);
                    string className = GenerateMaterialClass(file, shaderName);
                    shaderMaterialMap.Add(shaderName, className);
                }
            }

            GenerateShaderMaterialMapClass(shaderMaterialMap);
        }

        private string GenerateMaterialClass(GeneratorExecutionContext context, AdditionalText file, string shaderName)
        {
            var sourceText = file.GetText(context.CancellationToken);
            var lines = sourceText.Lines;

            bool inMaterialStruct = false;
            string className = "Material_" + shaderName;
            bool anyFieldAdded = false;

            StringBuilder classSource = new StringBuilder();
            classSource.AppendLine("using RockEngine.Rendering.Materials;");
            classSource.AppendLine("using RockEngine.OpenGL.Textures;");
            classSource.AppendLine("using OpenTK.Mathematics;");
            classSource.AppendLine($"public class {className} : IMaterial");
            classSource.AppendLine("{");

            List<string> properties = new List<string>();

            foreach(var line in lines)
            {
                string trimmedLine = line.ToString().Trim();

                if(trimmedLine.StartsWith("struct Material"))
                {
                    inMaterialStruct = true;
                    continue;
                }

                if(inMaterialStruct)
                {
                    foreach(var mapping in TypeMappings)
                    {
                        if(trimmedLine.StartsWith(mapping.Key))
                        {
                            string propertyName = trimmedLine.Split(' ')[1].Trim(';');
                            classSource.AppendLine($"   public {mapping.Value} {propertyName} {{ get; set; }}");
                            properties.Add(propertyName);
                            break;
                        }
                    }

                    if(trimmedLine.StartsWith("}"))
                    {
                        inMaterialStruct = false;
                    }
                }
            }

            classSource.AppendLine("}");

            if(properties.Count > 0)
            {
                GL.AddSource($"{className}.g.cs", SourceText.From(classSource.ToString(), Encoding.UTF8));
            }

            return className;
        }

        private void GenerateShaderMaterialMapClass(GeneratorExecutionContext context, Dictionary<string, string> shaderMaterialMap)
        {
            StringBuilder classSource = new StringBuilder();
            classSource.AppendLine("using System;");
            classSource.AppendLine("using System.Collections.Generic;");
            classSource.AppendLine();
            classSource.AppendLine("namespace RockEngine.Generator");
            classSource.AppendLine("{");
            classSource.AppendLine("    public static class ShaderMaterialMap");
            classSource.AppendLine("    {");
            classSource.AppendLine("        public static Dictionary<string, Type> Map = new Dictionary<string, Type>");
            classSource.AppendLine("        {");

            foreach(var kvp in shaderMaterialMap)
            {
                classSource.AppendLine($"            {{ \"{kvp.Key}\", typeof({kvp.Value}) }},");
            }

            classSource.AppendLine("        };");
            classSource.AppendLine("    }");
            classSource.AppendLine("}");

            GL.AddSource("ShaderMaterialMap.g.cs", SourceText.From(classSource.ToString(), Encoding.UTF8));
        }
    }
}
*/