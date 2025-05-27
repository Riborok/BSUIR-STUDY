using System.Text.Json;
using System.Text.Json.Serialization;
using Abstractions;
using Core;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Json;

public class Json : ITraceResultSerializer
{
    public string Format => "json";

    private static readonly JsonSerializerOptions Options = new() {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    
    public void Serialize(TraceResult traceResult, Stream to)
    {
        JsonSerializer.Serialize(to, traceResult, typeof(TraceResult), Options);
    }
}