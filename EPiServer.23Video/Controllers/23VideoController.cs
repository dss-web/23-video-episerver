using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using EPiServer.Web;
using EPiServer._23Video.Models;

namespace EPiServer._23Video.Controllers
{
    public class _23VideoController : Controller, IRenderTemplate<_23VideoVideo>
    {
        public ActionResult Index(_23VideoVideo currentContent)
        {
            return null;
        }
    }
}
