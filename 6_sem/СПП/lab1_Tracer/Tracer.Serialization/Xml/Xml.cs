using System.Xml;
using System.Xml.Linq;
using Abstractions;
using Core;

namespace Xml;

public class Xml : ITraceResultSerializer
{
    public string Format => "xml";

    public void Serialize(TraceResult traceResult, Stream to)
    {
        var root = new XElement("Root",
            traceResult.Threads.Select(thread => 
                new XElement("Thread",
                    new XAttribute("Id", thread.Id),
                    new XAttribute("Time", thread.Time),
                    thread.Methods.Select(SerializeMethod)
                )
            )
        );

        var document = new XDocument(root);

        var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };

        using var writer = XmlWriter.Create(to, settings);
        document.WriteTo(writer);
    }

    private static XElement SerializeMethod(MethodTrace method)
    {
        var element = new XElement("Method",
            new XAttribute("Name", method.Name),
            new XAttribute("ClassName", method.ClassName),
            new XAttribute("Time", method.Time)
        );

        if (method.Methods != null)
        {
            element.Add(method.Methods.Select(SerializeMethod));
        }

        return element;
    }
}