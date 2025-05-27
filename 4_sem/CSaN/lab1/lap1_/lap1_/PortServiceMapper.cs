using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace lap1_
{
    public class PortServiceMapper
    {
        private const string IanaUrl = "https://www.iana.org/assignments/service-names-port-numbers/service-names-port-numbers.txt";

        public static async Task<Dictionary<int, string>> GetPortServiceDictionary()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(IanaUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch data from IANA.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return ParseServiceNames(content);
        }

        private static Dictionary<int, string> ParseServiceNames(string data)
        {
            var serviceDictionary = new Dictionary<int, string>();

            var lines = data.Split('\n');
            foreach (var line in lines)
            {
                if (line.StartsWith("     ") && !line.Contains("Unassigned"))
                {
                    var parts = line.Trim().Split('\t');
                    if (parts.Length >= 2 && int.TryParse(parts[0], out int port))
                    {
                        var serviceName = parts[1].Trim();
                        if (!serviceDictionary.ContainsKey(port))
                        {
                            serviceDictionary.Add(port, serviceName);
                        }
                    }
                }
            }

            return serviceDictionary;
        }
    }
}