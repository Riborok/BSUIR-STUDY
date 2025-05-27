using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestsGeneratorLib
{
    public static class CodeAnalyzer 
    {
        public const string GlobalNs = "";
        private const string Unknown = "unknown";

        public static async Task<List<ClassInfo>> GetClassesFromFileAsync(string filePath)
        {
            var root = await GetSyntaxRootAsync(filePath);
            return root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Select(ParseClassDeclaration)
                .ToList();
        }

        private static async Task<CompilationUnitSyntax> GetSyntaxRootAsync(string filePath)
        {
            var code = await ReadFileAsync(filePath);
            var syntaxTree = CSharpSyntaxTree.ParseText(code, encoding: Encoding.UTF8);
            
            if (await syntaxTree.GetRootAsync() is not CompilationUnitSyntax root)
            {
                throw new Exception("Failed to parse syntax tree root");
            }

            return root;
        }

        private static async Task<string> ReadFileAsync(string filePath)
        {
            using var reader = new StreamReader(filePath);
            return await reader.ReadToEndAsync();
        }

        private static ClassInfo ParseClassDeclaration(ClassDeclarationSyntax classNode)
        {
            var namespaceName = GetNamespace(classNode);
            var fullClassName = GetFullClassName(classNode);
            var constructorParams = GetConstructorParameters(classNode);
            var methods = GetPublicMethods(classNode);

            return new ClassInfo(
                classNode.Identifier.Text,
                fullClassName,
                namespaceName,
                constructorParams,
                methods
            );
        }

        private static string GetNamespace(ClassDeclarationSyntax classNode)
        {
            var namespaceNode = classNode.Ancestors()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault();
            return namespaceNode?.Name.ToString() ?? GlobalNs;
        }

        private static List<ParameterInfo>? GetConstructorParameters(ClassDeclarationSyntax classNode)
        {
            var constructorInfo = classNode.Members
                .OfType<ConstructorDeclarationSyntax>()
                .FirstOrDefault(IsPublic);

            return constructorInfo?.ParameterList.Parameters
                .Select(CreateParameterInfo)
                .ToList();
        }

        private static List<MethodInfo> GetPublicMethods(ClassDeclarationSyntax classNode)
        {
            return classNode.Members
                .OfType<MethodDeclarationSyntax>()
                .Where(IsPublic)
                .Select(CreateMethodInfo)
                .ToList();
        }

        private static string GetFullClassName(ClassDeclarationSyntax classNode)
        {
            var parts = new List<string>();
            var currentNode = classNode;

            while (currentNode != null)
            {
                parts.Add(currentNode.Identifier.Text);
                currentNode = currentNode.Parent as ClassDeclarationSyntax;
            }
            parts.Reverse();
        
            return string.Join(".", parts);
        }
        
        private static MethodInfo CreateMethodInfo(MethodDeclarationSyntax methodNode)
        {
            var parameters = methodNode.ParameterList.Parameters
                .Select(CreateParameterInfo)
                .ToList();

            return new MethodInfo(
                methodNode.Identifier.Text,
                parameters,
                methodNode.ReturnType.ToString()
            );
        }

        private static ParameterInfo CreateParameterInfo(ParameterSyntax parameter)
        {
            return new ParameterInfo(
                parameter.Identifier.Text,
                parameter.Type?.ToString() ?? Unknown
            );
        }

        private static bool IsPublic(MemberDeclarationSyntax member)
        {
            return member.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword));
        }
    }
}