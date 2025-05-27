using System.Text;

namespace TestsGeneratorLib;

public static class TestCodeGenerator {
    public static string GenerateTestClassCode(ClassInfo classInfo)
    {
        var sb = new StringBuilder();
        var testClassName = GetTestClassName(classInfo);
        var sutFieldName = GetSutFieldName(classInfo.Name);

        GenerateUsings(sb, classInfo);
        sb.AppendLine();
        
        GenerateClassDeclaration(sb, testClassName);
        GenerateFields(sb, classInfo, sutFieldName);
        GenerateSetupMethod(sb, classInfo, sutFieldName);
        GenerateTestMethods(sb, classInfo.Methods, sutFieldName);
        
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static void GenerateUsings(StringBuilder sb, ClassInfo classInfo)
    {
        sb.AppendLine("using NUnit.Framework;");
        sb.AppendLine("using Moq;");
        if (classInfo.Namespace != CodeAnalyzer.GlobalNs)
        {
            sb.AppendLine($"using {classInfo.Namespace};");
        }
    }

    private static void GenerateClassDeclaration(StringBuilder sb, string testClassName)
    {
        sb.AppendLine($"public class {testClassName}");
        sb.AppendLine("{");
    }

    private static void GenerateFields(StringBuilder sb, ClassInfo classInfo, string sutFieldName)
    {
        sb.AppendLine($"\tprivate {classInfo.FullName} {sutFieldName};");

        if (classInfo.ConstructorParams != null)
        {
            foreach (var param in classInfo.ConstructorParams)
            {
                var mockFieldName = GetMockFieldName(param.Name);
                sb.AppendLine($"\tprivate Mock<{param.Type}> {mockFieldName};");
            }
        }
        sb.AppendLine();
    }

    private static void GenerateSetupMethod(StringBuilder sb, ClassInfo classInfo, string sutFieldName)
    {
        sb.AppendLine("\t[SetUp]");
        sb.AppendLine("\tpublic void Setup()");
        sb.AppendLine("\t{");

        if (classInfo.ConstructorParams != null)
        {
            GenerateConstructorWithParams(sb, classInfo, sutFieldName);
        }
        else
        {
            sb.AppendLine($"\t\t{sutFieldName} = new {classInfo.FullName}();");
        }

        sb.AppendLine("\t}");
        sb.AppendLine();
    }

    private static void GenerateConstructorWithParams(StringBuilder sb, ClassInfo classInfo, string sutFieldName)
    {
        foreach (var param in classInfo.ConstructorParams!)
        {
            var mockFieldName = GetMockFieldName(param.Name);
            sb.AppendLine($"\t\t{mockFieldName} = new Mock<{param.Type}>();");
        }

        var constructorParams = string.Join(", ", 
            classInfo.ConstructorParams.Select(p => $"{GetMockFieldName(p.Name)}.Object"));
        sb.AppendLine($"\t\t{sutFieldName} = new {classInfo.FullName}({constructorParams});");
    }

    private static void GenerateTestMethods(StringBuilder sb, List<MethodInfo> methods, string sutFieldName)
    {
        foreach (var method in methods)
        {
            GenerateTestMethod(sb, method, sutFieldName);
            sb.AppendLine();
        }
    }

    private static void GenerateTestMethod(StringBuilder sb, MethodInfo method, string sutFieldName)
    {
        var testName = GetTestMethodName(method);
        
        sb.AppendLine($"\t[Test]");
        sb.AppendLine($"\tpublic void {testName}()");
        sb.AppendLine("\t{");

        GenerateMethodParameters(sb, method.Parameters);
        GenerateMethodCall(sb, method, sutFieldName);
        GenerateAssertions(sb, method);

        sb.AppendLine("\t}");
    }

    private static void GenerateMethodParameters(StringBuilder sb, List<ParameterInfo> parameters)
    {
        foreach (var param in parameters)
        {
            var defaultValue = GetDefaultValue(param.Type);
            sb.AppendLine($"\t\t{param.Type} {param.Name} = {defaultValue};");
        }
    }

    private static void GenerateMethodCall(StringBuilder sb, MethodInfo method, string sutFieldName)
    {
        var parameters = string.Join(", ", method.Parameters.Select(p => p.Name));
        var methodCall = $"{sutFieldName}.{method.Name}({parameters})";

        if (method.ReturnType != "void")
        {
            sb.AppendLine($"\t\t{method.ReturnType} result = {methodCall};");
        }
        else
        {
            sb.AppendLine($"\t\t{methodCall};");
        }
    }

    private static void GenerateAssertions(StringBuilder sb, MethodInfo method)
    {
        if (method.ReturnType != "void")
        {
            var expectedDefault = GetDefaultValue(method.ReturnType);
            sb.AppendLine($"\t\t{method.ReturnType} expected = {expectedDefault};");
            sb.AppendLine("\t\tAssert.That(result, Is.EqualTo(expected));");
        }
        else
        {
            sb.AppendLine($"\t\t// Метод возвращает void");
        }
        //sb.AppendLine("\t\tAssert.Fail(\"Требуется реализация теста\");");
    }

    private static string GetTestClassName(ClassInfo classInfo)
    {
        var className = classInfo.Name;
        if (classInfo.Namespace == CodeAnalyzer.GlobalNs)
            return $"{className}Tests";
        
        var lastNamespacePart = classInfo.Namespace.Split('.').Last();
        return $"{lastNamespacePart}_{className}Tests";
    }

    private static string GetSutFieldName(string className)
    {
        return $"_{char.ToLowerInvariant(className[0])}{className[1..]}";
    }

    private static string GetTestMethodName(MethodInfo method)
    {
        if (!method.Parameters.Any())
            return $"{method.Name}_ShouldWork";

        var paramDescription = string.Join("_And_", 
            method.Parameters.Select(p => FormatParameterTypeForMethodName(p.Type)));
        
        return $"{method.Name}_With_{paramDescription}_ShouldWork";
    }

    private static string FormatParameterTypeForMethodName(string type)
    {
        return type
            .Replace("IEnumerable<", "")
            .Replace("List<", "")
            .Replace(">", "")
            .Replace("[]", "Array")
            .Replace("<", "Of")
            .Replace(">", "")
            .Replace(",", "And");
    }

    private static string GetMockFieldName(string paramName)
    {
        return $"_{char.ToLowerInvariant(paramName[0])}{paramName[1..]}Mock";
    }
    
    private static string GetDefaultValue(string type)
    {
        return type switch
        {
            "int" => "0",
            "long" => "0L",
            "short" => "0",
            "byte" => "0",
            "float" => "0f",
            "double" => "0.0",
            "decimal" => "0m",
            "bool" => "false",
            "char" => "'\\0'",
            "string" => "null",
            _ when type.StartsWith("List<") => $"new {type}()",
            _ when type.StartsWith("IEnumerable<") => 
                $"new List<{type["IEnumerable<".Length..^1]}>()",
            _ => "null"
        };
    }
}