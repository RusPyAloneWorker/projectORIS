using System;
using System.Collections.Generic;
using System.Text;

namespace HTTPResponse.Attributes
{
    public class HttpController : Attribute
    {
        public string ControllerName;
        public HttpController(string name)
        {
            ControllerName = name;
        }
    }
}
