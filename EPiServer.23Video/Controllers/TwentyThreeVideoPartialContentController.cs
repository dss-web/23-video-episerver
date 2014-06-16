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