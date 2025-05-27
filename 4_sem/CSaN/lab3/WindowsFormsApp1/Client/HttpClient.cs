using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    public class HttpClient : IDisposable
    {
        private readonly System.Net.Http.HttpClient _client = new System.Net.Http.HttpClient();

        public HttpClient(string uri) => _client.BaseAddress = new Uri(uri);
        
        public async Task<HttpResponseMessage> GetAsync(string srcPath)
        {
            return await _client.GetAsync(srcPath);
        }

        public async Task<HttpResponseMessage> PutAsync(string srcPath, string text)
        {
            var content = new StringContent(text);
            return await _client.PutAsync(srcPath, content);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string srcPath)
        {
            return await _client.DeleteAsync(srcPath);
        }

        public async Task<HttpResponseMessage> PostAsync(string srcPath, string text)
        {
            var content = new StringContent(text);
            return await _client.PostAsync(srcPath, content);
        }

        public async Task<HttpResponseMessage> CopyAsync(string srcPath, string destPath)
        {
            var request = new HttpRequestMessage(new HttpMethod("COPY"), srcPath)
            {
                Content = new StringContent(destPath)
            };
            return await _client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> MoveAsync(string srcPath, string destPath)
        {
            var request = new HttpRequestMessage(new HttpMethod("MOVE"), srcPath)
            {
                Content = new StringContent(destPath)
            };
            return await _client.SendAsync(request);
        }

        public void Dispose() => _client.Dispose();
    }
}