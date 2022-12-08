using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace HTTPResponse
{
    class HttpHandler
    {
        public static void HandleGET(HttpListenerContext context, out string[] strParams)
        {
            if (context.Request.Url.Segments.Length < 2)
            {
                strParams = null;
                return;
            }
            strParams = context.Request.Url
                                    .Segments
                                    .Skip(3)
                                    .Select(s => s.Replace("/", ""))
                                    .ToArray();
        }
        public static void HandlePOST(HttpListenerContext context, out Dictionary<string, string> strParams)
        {
            var body = GetPOSTBody(context.Request);

            if (body is null)
            {
                strParams = null;
                return;
            }

            strParams = new Dictionary<string, string>();
            // takes values from string
            var valuesOfPost = body.Split('&');
            for (int i = 0; i < valuesOfPost.Length; i++)
            {
                var param = valuesOfPost[i].Split("=");
                strParams.Add(param[0], param[1]);
            }
        }
        /// Takes body from POST request 
        /// Returns string
        private static string GetPOSTBody(HttpListenerRequest request)
        {
            if (!request.HasEntityBody || request.ContentType == null)
            {
                Console.WriteLine("No client data was sent with the request.");
                Console.WriteLine("Client data content type {0}", request.ContentType);
                return null;
            }
            Stream body = request.InputStream;
            Encoding encoding = request.ContentEncoding;
            StreamReader reader = new StreamReader(body, encoding);
            // Convert the data to a string and display it on the console.
            string s = reader.ReadToEnd();
            body.Close();
            reader.Close();
            return s;
        }
    }
}
