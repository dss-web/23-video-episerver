23Video
===========

Controllers;
using System.Web.Mvc;
using EPiServer.Web;
using EPiCode.TwentyThreeVideo.Models;

namespace EPiCode.TwentyThreeVideo.Controllers
{
    public class TwentyThreeVideoController : Controller, IRenderTemplate<Video>
    {
        public ActionResult Index(Video currentContent)
        {
            return PartialView(currentContent);
        }
    }
}


using System.Web.Mvc;
using EPiCode.TwentyThreeVideo.Models;
using EPiServer.Web.Mvc;

namespace EPiCode.TwentyThreeVideo.Controllers
{
    public class TwentyThreeVideoPartialContentController : PartialContentController<Video>
    {
        public override ActionResult Index(Video currentContent)
        {
            return PartialView(currentContent);
        }
    }
}

Views
\Views\TwentyThreeVideo
\Views\TwentyThreeVideoPartialContent

@Model Models.Video
@Html.Raw(Model.VideoUrl)
