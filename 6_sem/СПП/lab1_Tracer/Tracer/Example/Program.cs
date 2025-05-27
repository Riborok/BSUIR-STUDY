using Core;
using Serialization;

namespace Example;

internal static class Program
{
    private static readonly Tracer Tracer = new Tracer();

    public static void Main()
    {
        var t1 = new Thread(() => {
            MethodA();
            MethodA();
        });
        var t2 = new Thread(MethodB);

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();

        var result = Tracer.GetTraceResult();

        var serializers = SerializerPluginLoader.LoadPlugins();

        foreach (var serializer in serializers)
        {
            var fileName = Path.Combine("result", $"result.{serializer.Format}");
            using (var fileStream = File.Create(fileName))
            {
                serializer.Serialize(result, fileStream);
            }
            Console.WriteLine(fileName);
        }
    }

    static void MethodA()
    {
        Tracer.StartTrace();
        Thread.Sleep(100);
        InnerMethodA();
        Tracer.StopTrace();
    }

    static void InnerMethodA()
    {
        Tracer.StartTrace();
        Thread.Sleep(50);
        Tracer.StopTrace();
    }

    static void MethodB()
    {
        Tracer.StartTrace();
        Thread.Sleep(150);
        Tracer.StopTrace();
    }
}