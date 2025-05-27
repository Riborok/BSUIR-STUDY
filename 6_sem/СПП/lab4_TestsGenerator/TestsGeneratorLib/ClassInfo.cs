namespace TestsGeneratorLib;

public class ClassInfo(string name, string fullName, string ns, List<ParameterInfo>? constructorParams, List<MethodInfo> methods) {
    public string Name { get; set; } = name;
    public string FullName { get; set; } = fullName;
    public string Namespace { get; set; } = ns;
    public List<ParameterInfo>? ConstructorParams { get; set; } = constructorParams;
    public List<MethodInfo> Methods { get; set; } = methods;
}

public class MethodInfo(string name, List<ParameterInfo> parameters, string returnType) {
    public string Name { get; set; } = name;
    public List<ParameterInfo> Parameters { get; set; } = parameters;
    public string ReturnType { get; set; } = returnType;
}

public class ParameterInfo(string name, string type) {
    public string Name { get; set; } = name;
    public string Type { get; set; } = type;
}