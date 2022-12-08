using System;
using System.Net;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Linq;
using System.Reflection;
using HTTPResponse.Attributes;
using System.Threading.Tasks;

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
            Console.WriteLine ("Запуск сервера...");

            new HttpServer();

            listener.Start();
            
            Console.WriteLine("Ожидание подключений...");
            Status = ServerStatus.Start;
            Listen();
        }
        public async void Listen()
        {
            while (listener.IsListening)
            {
                try
                {
                    var context = await listener.GetContextAsync();

                    if (MethodHandler(context)) continue;

                    StaticFiles(context);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    continue;
                }
            }
        }
        private void StaticFiles(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            var path = Directory.GetCurrentDirectory();
            byte[] buffer;

            if (Directory.Exists(path))
            {
                var url = context.Request.RawUrl;

                buffer = FileFinder.GetFileBytes(url.Replace("%20", " "), _path);
                //HTMLGeneratorClass.HTMLGenerator.GetHTML(strings);
                // strings бывает null
                //buffer = Encoding.UTF8.GetBytes(strings);
                

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

            Status = ServerStatus.Stop;
            Console.WriteLine("Обработка подключений завершена");
        }
        public void Dispose()
        {
            Stop();
        }
        private bool MethodHandler(HttpListenerContext context)
        {
            // объект запроса
            HttpListenerRequest request = context.Request;
            

            // объект ответа
            HttpListenerResponse response = context.Response;

            var segmentsTemp = context.Request.Url.Segments;


            /// При http://localhost:8080/ или http://localhost:8080/sign_up
            /// Меняю segmentsTemp так, чтобы вызывался HomePageController
            if (segmentsTemp.Length < 3)
            {
                if (segmentsTemp.Length == 1)
                {
                    string[] segmentsTemp1 = new string[] { "/", "HomePage/", "show_homepage" };
                    segmentsTemp = segmentsTemp1;
                }
                else if (segmentsTemp.Length == 2 && segmentsTemp[1].Replace("/", "") == "sign_up")
                {
                    string[] segmentsTemp1 = new string[] { "/", "HomePage/", "sign_up" };
                    segmentsTemp = segmentsTemp1;
                }
                else 
                    return false;
            }

            string controllerName = segmentsTemp[1].Replace("/", "");

            var assembly = Assembly.GetExecutingAssembly();

            var controller = assembly.GetTypes()
                .Where(
                    t => Attribute.IsDefined(t, typeof(HttpController)) 
                    && ((HttpController) Attribute.GetCustomAttribute(t,typeof(HttpController))).ControllerName == controllerName
                ).FirstOrDefault();

            if (controller == null) return false;

            var attribute = $"Http{context.Request.HttpMethod}"; 

            var method = controller.GetMethods()
                .Where(t => t.GetCustomAttributes(true)
                    .Any(attr => attr.GetType().Name == attribute && ((IHttpMethod)attr).MethodURI == segmentsTemp[2].Replace("/", "") ))
                .FirstOrDefault();

            if (method == null) return false;

            var tempAttr = method.GetCustomAttributes().Where(t=>t is IAuthenticationChecker).FirstOrDefault();

            if (tempAttr is not null && ((IAuthenticationChecker)tempAttr).AuthCheck == true)
            {
                if (!CookieManager.IsAuthorized(context))
                {
                    var headers = new WebHeaderCollection();
                    headers.Set("Content-Type", "text/css");
                    headers.Add("Content-Type", "text/html");
                    context.Response.Headers = headers;

                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Access denyed!!");
                    context.Response.ContentLength64 = buffer.Length;
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                }
            }

            var instance = Activator.CreateInstance(controller);
            dynamic ret =  method.Invoke(instance, new object[] { context });

            Stream output = context.Response.OutputStream;

            output.Close();
            return true;
        }
  
    }
}
