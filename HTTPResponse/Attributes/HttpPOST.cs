using System;
using System.Collections.Generic;
using System.Text;

namespace HTTPResponse.Attributes
{
    internal class HttpPOST: Attribute, IHttpMethod, IAuthenticationChecker
    {
        public bool AuthCheck { get; set; }
        public string MethodURI { get; set; }
        public HttpPOST(string uri, bool authCheck = false)
        {
            this.MethodURI = uri;
            this.AuthCheck = authCheck;
        }
    }
}
