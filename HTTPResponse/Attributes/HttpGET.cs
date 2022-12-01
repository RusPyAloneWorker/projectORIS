using System;
using System.Collections.Generic;
using System.Text;

namespace HTTPResponse.Attributes
{
    public class HttpGET : Attribute, IHttpMethod
    {
        public string MethodURI { get; set; }
        public HttpGET(string uri)
        {
            MethodURI = uri;
        }
    }
}
