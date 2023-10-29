using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace RockEngine.Generator
{
    [Generator]
    public class ComponentsStateGenerator : ISourceGenerator
    {
        private const string GeneratedClassNameSuffix = "State";

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this generator
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // Retrieve the compilation and syntax trees
            var compilation = context.Compilation;
            var syntaxTrees = compilation.SyntaxTrees;

            // Collect all classes that implement IComponent
            var componentClasses = new List<ClassDeclarationSyntax>();
            foreach(var syntaxTree in syntaxTrees)
            {
                var root = syntaxTree.GetRoot();
                var componentClassDeclarations = root.DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .Where(c => c.BaseList?.Types.Any(t => t.Type.ToString() == "IComponent") ?? false);

                componentClasses.AddRange(componentClassDeclarations);
            }
            // Generate code for each component class
            foreach(var componentClass in componentClasses)
            {
                var className = componentClass.Identifier.Text;

                // Retrieve the specific using directives from the component class
                var componentUsings = GetUsingDirectives(compilation, componentClass);
                var namespaceDeclaration = componentClass.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
                var namespaceName = string.Empty;
                if(namespaceDeclaration != null)
                {
                    namespaceName = namespaceDeclaration.Name.ToString();
                }

                // Generate public and private fields
                var fieldsBuilder = new StringBuilder();
                foreach(var fieldDeclaration in componentClass.Members.OfType<FieldDeclarationSyntax>())
                {
                    var fieldName = fieldDeclaration.Declaration.Variables.First().Identifier.Text;
                    var fieldType = fieldDeclaration.Declaration.Type.ToString();

                    fieldsBuilder.AppendLine($"internal {fieldType} {fieldName} {{get; set;}}");
                }

                // Generate the final code for the component class
                var classCode = @$"
{string.Join("\n", componentUsings)}

namespace {namespaceName}
{{
    internal class {className}{GeneratedClassNameSuffix}
    {{
        {fieldsBuilder}
        {GetInheritedFields(compilation, componentClass)}
    }}
}}
                ";

                // Create the source code syntax tree
                var sourceCode = classCode.Trim();
                var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

                // Determine the file name
                var fileName = $"{className}{GeneratedClassNameSuffix}.Generated.cs";

                // Add the generated syntax tree to the compilation
                context.AddSource(fileName, SourceText.From(sourceCode, Encoding.UTF8));
            }
        }

        private IEnumerable<string> GetUsingDirectives(Compilation compilation, ClassDeclarationSyntax classDeclaration)
        {
            var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            var usingDirectives = new List<string>();

            var usingDirectiveNodes = classDeclaration.SyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>();
            foreach(var usingDirectiveNode in usingDirectiveNodes)
            {
                var usingDirective = usingDirectiveNode.ToFullString().Trim();
                if(!string.IsNullOrEmpty(usingDirective))
                {
                    usingDirectives.Add(usingDirective);
                }
            }

            // Retrieve using directives from base classes
            var baseTypes = classDeclaration.BaseList?.Types;
            if(baseTypes != null)
            {
                foreach(var baseType in baseTypes)
                {
                    // Retrieve the base class symbol
                    var baseClassSymbol = semanticModel.GetSymbolInfo(baseType.Type).Symbol as INamedTypeSymbol;

                    // Recursively retrieve using directives from the base class
                    if(baseClassSymbol?.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is ClassDeclarationSyntax baseClassSyntax)
                    {
                        usingDirectives.AddRange(GetUsingDirectives(compilation, baseClassSyntax));
                    }
                }
            }

            return usingDirectives;
        }
        // Method to retrieve inherited fields
        private string GetInheritedFields(Compilation compilation, ClassDeclarationSyntax classDeclaration)
        {
            var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            var inheritedFieldsBuilder = new StringBuilder();

            var baseTypes = classDeclaration.BaseList?.Types;
            if(baseTypes != null)
            {
                foreach(var baseType in baseTypes)
                {
                    // Retrieve the base class symbol
                    var baseClassSymbol = semanticModel.GetSymbolInfo(baseType.Type).Symbol as INamedTypeSymbol;

                    // Recursively retrieve fields from the base class
                    if(baseClassSymbol?.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is ClassDeclarationSyntax baseClassSyntax)
                    {
                        inheritedFieldsBuilder.AppendLine(GetInheritedFields(compilation, baseClassSyntax));
                    }

                    // Retrieve fields from generic type arguments
                    if(baseType.Type is GenericNameSyntax genericNameSyntax)
                    {
                        foreach(var typeArgument in genericNameSyntax.TypeArgumentList.Arguments)
                        {
                            var typeArgumentSymbol = semanticModel.GetSymbolInfo(typeArgument).Symbol;
                            if(typeArgumentSymbol is INamedTypeSymbol namedTypeSymbol)
                            {
                                if(namedTypeSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is TypeDeclarationSyntax typeArgumentSyntax)
                                {
                                    inheritedFieldsBuilder.AppendLine(GetFieldsFromType(typeArgumentSyntax));
                                }
                            }
                        }
                    }
                }
            }

            return inheritedFieldsBuilder.ToString();
        }

        private string GetFieldsFromType(TypeDeclarationSyntax typeDeclaration)
        {
            var fieldsBuilder = new StringBuilder();

            foreach(var fieldDeclaration in typeDeclaration.Members.OfType<FieldDeclarationSyntax>())
            {
                var fieldName = fieldDeclaration.Declaration.Variables.First().Identifier.Text;
                var fieldType = fieldDeclaration.Declaration.Type.ToString();

                fieldsBuilder.AppendLine($"private {fieldType} _{fieldName};");
                fieldsBuilder.AppendLine($"internal {fieldType} {fieldName} {{ get => _{fieldName}; set => _{fieldName} = value; }}");
            }

            return fieldsBuilder.ToString();
        }
    }
}