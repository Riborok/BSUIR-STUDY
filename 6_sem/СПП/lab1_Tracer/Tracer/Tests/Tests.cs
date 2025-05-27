using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests;

[TestClass]
public class Tests 
{
    [TestMethod]
    public void SingleMethodTrace()
    {
        var tracer = new Tracer();

        tracer.StartTrace();
        Thread.Sleep(100);
        tracer.StopTrace();

        var result = tracer.GetTraceResult();

        Assert.AreEqual(1, result.Threads.Count);
        var threadTrace = result.Threads[0];
        Assert.AreEqual(1, threadTrace.Methods.Count);
        var methodTrace = threadTrace.Methods[0];

        Assert.AreEqual("SingleMethodTrace", methodTrace.Name);
        Assert.IsTrue(methodTrace.Time >= 100);
    }

    [TestMethod]
    public void NestedMethodTrace()
    {
        var tracer = new Tracer();

        OuterMethod(tracer);

        var result = tracer.GetTraceResult();

        Assert.AreEqual(1, result.Threads.Count);
        var threadTrace = result.Threads[0];
        Assert.AreEqual(1, threadTrace.Methods.Count);

        var outerMethodTrace = threadTrace.Methods[0];
        Assert.AreEqual("OuterMethod", outerMethodTrace.Name);
        Assert.IsNotNull(outerMethodTrace.Methods);
        Assert.AreEqual(1, outerMethodTrace.Methods.Count);
        var innerMethodTrace = outerMethodTrace.Methods[0];
        Assert.AreEqual("InnerMethod", innerMethodTrace.Name);
    }
    
    void OuterMethod(Tracer tracer)
    {
        tracer.StartTrace();
        InnerMethod(tracer);
        tracer.StopTrace();
    }

    private void InnerMethod(Tracer tracer)
    {
        tracer.StartTrace();
        Thread.Sleep(50);
        tracer.StopTrace();
    }

    [TestMethod]
    public void MultiThreadTrace_ShouldRecordSeparateThreadTraces()
    {
        var tracer = new Tracer();
        var thread1 = new Thread(() => MethodForThread(tracer, 100));
        var thread2 = new Thread(() => MethodForThread(tracer, 150));

        thread1.Start();
        thread2.Start();
        thread1.Join();
        thread2.Join();

        var result = tracer.GetTraceResult();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Threads.Count);
        foreach (var threadTrace in result.Threads)
        {
            Assert.AreEqual(1, threadTrace.Methods.Count);
        }
    }

    private void MethodForThread(ITracer tracer, int sleepTime)
    {
        tracer.StartTrace();
        Thread.Sleep(sleepTime);
        tracer.StopTrace();
    }
}