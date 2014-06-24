using System.Web.Mvc;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web;
using EPiCode.TwentyThreeVideo.Models;

namespace EPiCode.TwentyThreeVideo.Controllers
{
    [TemplateDescriptor(TagString = EPiServer.Framework.Web.RenderingTags.Preview )]
    public class TwentyThreeVideoController : Controller, IRenderTemplate<Video>
    {
        public ActionResult Index(Video currentContent)
        {
            return PartialView(currentContent);
        }
    }
}