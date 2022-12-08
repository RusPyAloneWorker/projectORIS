using System;
using System.Collections.Generic;
using System.Text;

namespace HTTPResponse.Attributes
{
    public class HttpGET : Attribute, IHttpMethod, IAuthenticationChecker
    {
        public bool AuthCheck {get;set;}
        public string MethodURI { get; set; }
        public HttpGET(string uri, bool authCheck = false)
        {
            MethodURI = uri;
            AuthCheck = authCheck;
        }
    }
}
