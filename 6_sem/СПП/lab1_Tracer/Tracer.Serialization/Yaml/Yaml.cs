using Abstractions;
using Core;
using YamlDotNet.Serialization;

namespace Yaml;

public class Yaml : ITraceResultSerializer
{
    public string Format => "yaml";

    private static readonly ISerializer Serializer = new SerializerBuilder()
        .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
        .Build();

    public void Serialize(TraceResult traceResult, Stream to)
    {
        using var writer = new StreamWriter(to);
        Serializer.Serialize(writer, traceResult);
    }
}