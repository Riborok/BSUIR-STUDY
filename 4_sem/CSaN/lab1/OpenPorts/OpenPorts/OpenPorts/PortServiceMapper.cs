namespace OpenPorts;

public static class PortServiceMapper {
    private const string IanaUrl = "https://www.iana.org/assignments/service-names-port-numbers/service-names-port-numbers.txt";

    public static async Task<Dictionary<int, string>> GetPortServiceDictionary() {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Microsoft Edge");
        
        var response = await httpClient.GetAsync(IanaUrl);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Status code: " + response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        return ParseServiceNames(content);
    }

    private static Dictionary<int, string> ParseServiceNames(string data) {
        var serviceDictionary = new Dictionary<int, string>();

        var lines = SplitIntoLines(data);
        foreach (var line in lines)
            ProcessLine(line, serviceDictionary);

        return serviceDictionary;
    }
    
    private static string[] SplitIntoLines(string data) {
        return data.Split('\n');
    }
    
    private static void ProcessLine(string line, Dictionary<int, string> serviceDictionary) {
        var parts = SplitLine(line);
        if (IsValid(parts, out int port, out string serviceName, out string protocol))
            AddServiceName(port, serviceName, protocol, serviceDictionary);
    }
    
    private static string[] SplitLine(string line) {
        return line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
    }
    
    private static void AddServiceName(int port, string serviceName, string protocol, Dictionary<int, string> serviceDictionary) {
        var name = protocol + ' ' + serviceName;
        serviceDictionary[port] = serviceDictionary.TryGetValue(port, out var value) 
            ? value + ", " + name
            : name;
    }
    
    private static bool IsValid(string[] parts, out int port, out string serviceName, out string protocol) {
        port = 0;
        serviceName = string.Empty;
        protocol = string.Empty;

        if (!IsValidPartsLength(parts) || !TryParsePort(parts[1], out port))
            return false;

        serviceName = parts[0];
        protocol = parts[2];
        return IsTcpOrUdp(protocol);
    }
    
    private static bool IsValidPartsLength(string[] parts) {
        return parts.Length >= 3;
    }

    private static bool TryParsePort(string part, out int port) {
        return int.TryParse(part, out port);
    }
    
    private static bool IsTcpOrUdp(string protocol) {
        return protocol.Equals("tcp", StringComparison.OrdinalIgnoreCase) || 
               protocol.Equals("udp", StringComparison.OrdinalIgnoreCase);
    }
}