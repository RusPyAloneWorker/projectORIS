using System;
using System.Threading;
using System.Net;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HTTPResponse.Contollers;

namespace HTTPResponse
{
    public class HttpServer : IDisposable
    {
        static HttpListener listener;
        static Stream output;
        public ServerStatus Status = ServerStatus.Stop;
        string _path;
        private readonly int _port;

        public HttpServer()
        {
            if (File.Exists(Path.GetFullPath("Config.json")))
            {
                string fileName = "Config.json";
                string jsonString = File.ReadAllText(fileName);
                ServerSettings setting = JsonSerializer.Deserialize<ServerSettings>(jsonString);
                _port = setting.Port;
                _path = setting.Path;
            }
            else
            {
                ServerSettings setting = new ServerSettings(); 
                _port = setting.Port;
                _path = setting.Path;
            }
            listener = new HttpListener();
            // установка адресов прослушки
            listener.Prefixes.Add($"http://localhost:{_port}/");
        }

        public void Begin()
        {
            if (Status == ServerStatus.Start)
            {
                Console.WriteLine("Уже запущен");
                return;
            }
            Console.WriteLine("Запуск сервера...");

            new HttpServer();

            listener = new HttpListener();


            listener.Start();
            
            Console.WriteLine("Ожидание подключений...");
            Status = ServerStatus.Start;
            Listen();
        }
        public async void Listen()
        {
            while (listener.IsListening)
            {
                    var context = await listener.GetContextAsync();
                    StaticFiles(context);
                
            }
        }
        private void StaticFiles(HttpListenerContext context)
        {
            var response = context.Response;

            var path = Directory.GetCurrentDirectory();
            byte[] buffer;

            if (Directory.Exists(path))

            {
                buffer = FileFinder.GetFile(context.Request.RawUrl.Replace("%20", " "), "index.html", _path);

                response.Headers.Set("Content-Type", "text/css");
                response.Headers.Add("Content-Type", "text/html");


                if (buffer == null)
                {
                    response.Headers.Set("Content-Type", "text/plain");
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    string err = "404 - Not Found";
                    buffer = Encoding.UTF8.GetBytes(err);
                }
            }
            else
            {

                string err = $"{path} is not found";
                buffer = Encoding.UTF8.GetBytes(err);

            }

            output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            context.Response.Close();
            
        }
        public void Stop()
        {
            if (Status == ServerStatus.Stop) return;
            // останавливаем прослушивание подключений
            listener.Stop();
            listener.Abort();
            listener.Close();
            
            listener = null;
            
            Status = ServerStatus.Stop;
            Console.WriteLine("Обработка подключений завершена");
        }
        public void Dispose()
        {
            Stop();
        }
        private bool MethodHandler(HttpListenerContext _httpContext)
        {
            // объект запроса
            HttpListenerRequest request = _httpContext.Request;

            // объект ответа
            HttpListenerResponse response = _httpContext.Response;

            if (_httpContext.Request.Url.Segments.Length < 2) return false;

            string controllerName = _httpContext.Request.Url.Segments[1].Replace("/", "");

            string[] strParams = _httpContext.Request.Url
                                    .Segments
                                    .Skip(2)
                                    .Select(s => s.Replace("/", ""))
                                    .ToArray();

            var assembly = Assembly.GetExecutingAssembly();

            var controller = assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(HttpController))).FirstOrDefault(c => c.Name.ToLower() == controllerName.ToLower());

            if (controller == null) return false;

            var test = typeof(HttpController).Name;
            var method = controller.GetMethods().Where(t => t.GetCustomAttributes(true)
                                                              .Any(attr => attr.GetType().Name == $"Http{_httpContext.Request.HttpMethod}"))
                                                 .FirstOrDefault();

            if (method == null) return false;

            object[] queryParams = method.GetParameters()
                                .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                                .ToArray();

            var ret = method.Invoke(Activator.CreateInstance(controller), queryParams);

            response.ContentType = "Application/json";

            byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            output.Close();
            _httpContext.Response.Close();
            return true;
        }
    }
}