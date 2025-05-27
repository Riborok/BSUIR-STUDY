using System.Reflection;
using Abstractions;

namespace Serialization;

public static class SerializerPluginLoader
{
    public static IEnumerable<ITraceResultSerializer> LoadPlugins()
    {
        var plugins = new List<ITraceResultSerializer>();
        
        foreach (var dll in Directory.GetFiles("plugins", "*.dll"))
        {
            try
            {
                var assembly = Assembly.LoadFrom(dll);
                foreach (var type in assembly.GetTypes()) 
                {
                    try 
                    {
                        if (type.GetInterface(nameof(ITraceResultSerializer)) != null) 
                        {
                            plugins.Add((ITraceResultSerializer)Activator.CreateInstance(type)!);
                        }
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"Failed to load type {type.Name} - {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load dll {dll} - {ex.Message}");
            }
        }

        return plugins;
    }
}