using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Server {
    internal class HttpServer : IDisposable
    {
        private readonly HttpListener _listener;

        public HttpServer(string uri)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(uri);
        }

        public void Start()
        {
            _listener.Start();
            Console.WriteLine($"The server is running on {_listener.Prefixes.ToArray()[0]}");
            
            while (true)
            {
                HttpListenerContext context;
                try
                {
                    context = _listener.GetContext();
                }
                catch (HttpListenerException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }

                ProcessRequest(context);
            }
            Console.WriteLine($"The server is stopped on {_listener.Prefixes}");
        }

        private static void ProcessRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            try {
                switch (request.HttpMethod)
                {
                    case "GET":
                        HandleGetRequest(request, response);
                        break;
                    case "PUT":
                        HandlePutRequest(request, response);
                        break;
                    case "POST":
                        HandlePostRequest(request, response);
                        break;
                    case "DELETE":
                        HandleDeleteRequest(request, response);
                        break;
                    case "COPY":
                        HandleCopyRequest(request, response);
                        break;
                    case "MOVE":
                        HandleMoveRequest(request, response);
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                        break;
                }
            }
            catch (Exception e) {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Console.WriteLine(e.Message);
            }

            response.Close();
        }

        private static string GetFilePath(IEnumerable<string> segments) => string.Join("/", segments.Skip(1));
        
        private static void HandleGetRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            string filePath = GetFilePath(request.Url.Segments);
            if (!File.Exists(filePath))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            byte[] buffer = File.ReadAllBytes(filePath);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.StatusCode = (int)HttpStatusCode.OK;
        }

        private static void HandlePutRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            string filePath = GetFilePath(request.Url.Segments);
            CreatePathIfNotExists(filePath);

            using (Stream inputStream = request.InputStream)
            using (FileStream fileStream = File.Create(filePath))
                inputStream.CopyTo(fileStream);

            response.StatusCode = (int)HttpStatusCode.Created;
        }
        
        private static void CreatePathIfNotExists(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }
        
        private static void HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            string filePath = GetFilePath(request.Url.Segments);
            if (!File.Exists(filePath))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            using (Stream inputStream = request.InputStream)
            using (FileStream fileStream = File.Open(filePath, FileMode.Append))
                inputStream.CopyTo(fileStream);

            response.StatusCode = (int)HttpStatusCode.OK;
        }

        private static void HandleDeleteRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            string filePath = GetFilePath(request.Url.Segments);
            if (!File.Exists(filePath))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            File.Delete(filePath);
            response.StatusCode = (int)HttpStatusCode.OK;
        }

        private static void HandleCopyRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            string filePath = GetFilePath(request.Url.Segments);
            if (!File.Exists(filePath))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            string destinationPath = ReadPathInContent(request);
            CreatePathIfNotExists(destinationPath);
            
            if (string.IsNullOrEmpty(destinationPath))
            {
                response.StatusCode = (int)HttpStatusCode.NoContent;
                return;
            }
            
            File.Copy(filePath, destinationPath);
            response.StatusCode = (int)HttpStatusCode.OK;
        }

        private static string ReadPathInContent(HttpListenerRequest request)
        {
            string destinationPath;
            using (StreamReader reader = new StreamReader(request.InputStream))
                destinationPath = reader.ReadToEnd();
            return destinationPath;
        }

        private static void HandleMoveRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            string filePath = GetFilePath(request.Url.Segments);
            if (!File.Exists(filePath))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            string destinationPath = ReadPathInContent(request);
            CreatePathIfNotExists(destinationPath);
            
            if (string.IsNullOrEmpty(destinationPath))
            {
                response.StatusCode = (int)HttpStatusCode.NoContent;
                return;
            }
            
            File.Move(filePath, destinationPath);
            response.StatusCode = (int)HttpStatusCode.OK;
        }

        public void Stop() => _listener.Stop();
        
        public void Dispose() => _listener.Close();
    }
}
