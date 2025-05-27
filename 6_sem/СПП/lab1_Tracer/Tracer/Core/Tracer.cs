using System.Collections.Concurrent;
using System.Diagnostics;

namespace Core;

public class Tracer : ITracer
{
    private class MethodTraceData
    {
        public required string Name { get; init; }
        public required string ClassName { get; init; }
        public required Stopwatch Stopwatch { get; init; }
        public List<MethodTraceData> ChildMethods { get; } = new();
    }

    private class ThreadTraceData
    {
        public int ThreadId { get; init; }
        public List<MethodTraceData> RootMethods { get; } = new();
        public Stack<MethodTraceData> CallStack { get; } = new();
    }

    private readonly ConcurrentDictionary<int, ThreadTraceData> _threads = new();

    public void StartTrace()
    {
        var threadId = Environment.CurrentManagedThreadId;
        var threadData = _threads.GetOrAdd(threadId, id => new ThreadTraceData { ThreadId = id });

        var stackTrace = new StackTrace();
        var frame = stackTrace.GetFrame(1);
        var method = frame?.GetMethod();
        var methodName = method?.Name ?? "Unknown";
        var className = method?.DeclaringType?.Name ?? "Unknown";

        var methodData = new MethodTraceData
        {
            Name = methodName,
            ClassName = className,
            Stopwatch = Stopwatch.StartNew()
        };

        if (threadData.CallStack.Count > 0)
        {
            threadData.CallStack.Peek().ChildMethods.Add(methodData);
        }
        else
        {
            threadData.RootMethods.Add(methodData);
        }

        threadData.CallStack.Push(methodData);
    }

    public void StopTrace()
    {
        var threadId = Environment.CurrentManagedThreadId;
        if (_threads.TryGetValue(threadId, out var threadData) && threadData.CallStack.Count > 0)
        {
            var methodData = threadData.CallStack.Pop();
            methodData.Stopwatch.Stop();
        }
    }

    public TraceResult GetTraceResult()
    {
        var threadTraces = new List<ThreadTrace>();

        foreach (var kvp in _threads)
        {
            var threadData = kvp.Value;
            var methods = threadData.RootMethods
                .Select(ConvertToMethodTrace)
                .ToList();

            threadTraces.Add(new ThreadTrace(threadData.ThreadId, methods));
        }

        return new TraceResult(threadTraces);
    }

    private static MethodTrace ConvertToMethodTrace(MethodTraceData data)
    {
        var time = data.Stopwatch.ElapsedMilliseconds;
        var childMethods = data.ChildMethods.Count != 0 
            ? data.ChildMethods.Select(ConvertToMethodTrace).ToList() 
            : null;

        return new MethodTrace(data.Name, data.ClassName, time, childMethods);
    }
}