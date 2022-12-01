using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTTPResponse.Attributes;

namespace HTTPResponse.Controllers
{
    [HttpController("HomePage")]
    class HomePageController
    {
        [HttpGET("get_homepage")]
        public void HomePage()
        {

        }
    }
}
