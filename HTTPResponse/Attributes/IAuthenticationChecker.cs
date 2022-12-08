using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTTPResponse.Attributes
{
    public interface IAuthenticationChecker
    {
        public bool AuthCheck { get; set; }
    }
}
