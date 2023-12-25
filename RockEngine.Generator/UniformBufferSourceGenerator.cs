using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace RockEngine.Generator
{
    //[Generator]
    public class UniformBufferSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required
        }

        public void Execute(GeneratorExecutionContext context)
        {
            HashSet<string> addedFiles = new HashSet<string>();
            foreach(AdditionalText file in context.AdditionalFiles.Where(s => s.Path.EndsWith(".vert") || s.Path.EndsWith(".frag")))
            {
                SourceText? fileContent = file.GetText();
                if(fileContent is null)
                {
                    continue;
                }
                // Generate the C# code for the UniformBuffer struct using the parsed information
                List<UBO> ubos = GetUbos(fileContent.ToString());
                foreach(var ubo in ubos)
                {

                    // Create a unique name for the generated file
                    string fileName = $"{ubo.Name}_Generated.cs";

                    // Check if the file has already been added
                    if(addedFiles.Contains(fileName))
                    {
                        continue;
                    }

                    string generatedCode = GenerateUniformBufferCode(ubo);
                    // Add the generated file name to the addedFiles HashSet
                    addedFiles.Add(fileName);

                    // Add the generated C# code as a source file to the compilation
                    context.AddSource(fileName, SourceText.From(generatedCode, Encoding.UTF8));
                }
            }
            addedFiles.Clear();
        }

        private List<UBO> GetUbos(string file)
        {
            const string uboSplitter = "layout (std140,";
            const string bindingSplitter = "binding";

            // Use regex to split the string by the uboSplitter
            string[ ] ubosSplitted = Regex.Split(file, Regex.Escape(uboSplitter));

            // Initialize a list to store the UBOs
            List<UBO> ubos = new List<UBO>();

            // Loop through the splitted strings
            foreach(var uboString in ubosSplitted)
            {
                // Check if the string contains the binding splitter
                if(uboString.Contains(bindingSplitter))
                {
                    // Extract the binding value
                    string bindingPattern = @"binding\s*=\s*(\d+)";
                    Match bindingMatch = Regex.Match(uboString, bindingPattern);
                    string bindingValue = bindingMatch.Groups[1].Value;

                    // Extract the name of the UniformBuffer
                    string uniformBufferPattern = @"uniform\s+(\w+)";
                    Match uniformBufferMatch = Regex.Match(uboString, uniformBufferPattern);
                    string uniformBufferName = uniformBufferMatch.Groups[1].Value;

                    // Extract the fields
                    string fieldsPattern = @"\{([^}]*)\}";
                    Match fieldsMatch = Regex.Match(uboString, fieldsPattern);
                    string fields = fieldsMatch.Groups[1].Value.Trim();

                    // Extract the field types and names using regex
                    string fieldPattern = @"(\w+)\s+(\w+);";
                    MatchCollection fieldMatches = Regex.Matches(fields, fieldPattern);
                    List<(string Type, string FieldName)> fieldList = new List<(string Type, string FieldName)>();

                    foreach(Match fieldMatch in fieldMatches)
                    {
                        string fieldType = fieldMatch.Groups[1].Value;
                        string fieldName = fieldMatch.Groups[2].Value;
                        fieldList.Add((fieldType, fieldName));
                    }

                    // Create a new UBO object and add it to the list
                    UBO ubo = new UBO(int.Parse(bindingValue), uniformBufferName, fieldList);
                    ubos.Add(ubo);
                }
            }

            // Return the list of UBOs
            return ubos;
        }

        private string GenerateUniformBufferCode(UBO ubo)
        {
            StringBuilder sb = new StringBuilder();
            var uboValues = CalculateOffsetAndType(ubo.Fields);
            sb.AppendLine(@"using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using RockEngine.OpenGL.Settings;

namespace RockEngine.OpenGL.Buffers.UBOBuffers
{
    [StructLayout(LayoutKind.Explicit, Size = Size)]");
            sb.AppendLine($"    public struct {ubo.Name} : IUBOData<{ubo.Name}>");
            sb.AppendLine("    {");

            for(int i = 0; i < ubo.Fields.Count; i++)
            {
                (string Type, string FieldName) item = ubo.Fields[i];
                (int offset, string type) offsetType = uboValues[i];
                sb.AppendLine($"        [FieldOffset({offsetType.offset})]");
                sb.AppendLine($"        public {offsetType.type} {item.FieldName};");
                sb.AppendLine();
            }
            sb.AppendLine($@" 
        public const int Size = 0;
        private static UBO<{ubo.Name}> UBO => IUBOData<{ubo.Name}>.UBO;
        public readonly string Name => nameof({ubo.Name});

        /// <summary>
        /// Binding in the shader
        /// </summary>
        public readonly int BindingPoint => {ubo.BindingPoint};

        public {ubo.Name}()
        {{
            if (IUBOData<{ubo.Name}>.UBO is null)
            {{
                IUBOData<{ubo.Name}>.UBO = new UBO<{ubo.Name}>(new BufferSettings(Size, BufferUsageHint.StreamDraw, BindingPoint, Name)).Setup().SetLabel();
            }}
        }}

        public readonly void SendData()
        {{
            UBO.SendData(this);
        }}

        public readonly void SendData<TSub>([DisallowNull, NotNull] TSub data, nint offset, int size)
        {{
            UBO.SendData(data, offset, size);
        }}
    }}
}}");

            return sb.ToString();
        }

        private int CalculateSizeOfStruct(List<(int offset, string type)> fields)
        {
            int maxSize = 0;

            foreach(var (offset, fieldType) in fields)
            {
                int fieldSize = GetFieldSize(fieldType);
                int endOffset = offset + fieldSize;
                maxSize = Math.Max(maxSize, endOffset);
            }

            // Check if maxSize is divisible by 8 without a remainder
            if(maxSize % 8 == 0 && fields.Count != 1)
            {
                // Add 8 bytes to ensure rounding up to the nearest multiple of 8
                maxSize += 8;
            }

            // Round up the size to the nearest multiple of 8
            int roundedSize = (int)Math.Ceiling(maxSize / 8.0) * 8;

            return roundedSize;
        }

        private int GetFieldSize(string fieldType)
        {
            if(fieldType == "byte")
            {
                return 1;
            }
            else if(fieldType == "int")
            {
                return 4;
            }
            else if(fieldType == "uint")
            {
                return 4;
            }
            else if(fieldType == "float")
            {
                return 4;
            }
            else if(fieldType == "Vector3")
            {
                return 4*3;
            }

            else if (fieldType == "Matrix4")
            {
                return 64;
            }
            else 
            {
                throw new ArgumentException("Unknown fieldType");
            }
        }

        private List<(int offset, string type)> CalculateOffsetAndType(List<(string Type, string FieldName)> fields)
        {
            int offset = 0;
            List<(int offset, string type)> result = new List<(int offset, string type)>();
            foreach(var item in fields)
            {
                var currentOffset = offset;
                string type = string.Empty;
                if(item.Type == "vec3")
                {
                    offset += 16;
                    type = "Vector3";
                }
                else if(item.Type == "float")
                {
                    offset += 4;
                    type = item.Type;
                }
                else if(item.Type == "int" || item.Type == "uint")
                {
                    offset += 4;
                    type = item.Type;
                }
                else if(item.Type == "mat4")
                {
                    offset += 64;
                    type = "Matrix4";
                }

                result.Add((currentOffset, type));
            }
            return result;
        }

        internal class UBO
        {
            public int BindingPoint;
            public string Name;
            public List<(string Type, string FieldName)> Fields;
            public UBO(int bindingPoint, string name, List<(string Type, string FieldName)> fields)
            {
                BindingPoint = bindingPoint;
                Name = name;
                Fields = fields;
            }
        }
    }
}

