using System;
using System.Collections.Generic;
using System.Text;

namespace HTTPResponse.Attributes
{
    internal class HttpPOST: Attribute, IHttpMethod
    {
        public string MethodURI { get; set; }
        public HttpPOST(string uri)
        {
            MethodURI = uri;
        }
    }
}
