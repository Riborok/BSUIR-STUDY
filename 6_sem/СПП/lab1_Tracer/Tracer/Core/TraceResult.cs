namespace Core;

public class TraceResult(IReadOnlyList<ThreadTrace> threads) 
{
    public IReadOnlyList<ThreadTrace> Threads => threads;
}

public class ThreadTrace(int id, IReadOnlyList<MethodTrace> methods)
{
    public int Id => id;
    public long Time => methods.Sum(t => t.Time);
    public IReadOnlyList<MethodTrace> Methods => methods;
}

public class MethodTrace(string name, string className, long time, IReadOnlyList<MethodTrace>? methods) 
{
    public string Name => name;
    public string ClassName => className;
    public long Time => time;
    public IReadOnlyList<MethodTrace>? Methods => methods;
}
