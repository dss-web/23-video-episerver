using System.Web.Mvc;
using EPiCode.TwentyThreeVideo.Models;
using EPiCode.TwentyThreeVideo.Provider;
using EPiServer.Web.Mvc;

namespace EPiCode.TwentyThreeVideo.Controllers
{
    public class TwentyThreeVideoPartialContentController : PartialContentController<Video>
    {
        public override ActionResult Index(Video currentContent)
        {
            if (Client.Settings.OEmbedIsEnabled)
            {
                currentContent.VideoUrl = currentContent.oEmbedHtml;
            }

            return PartialView(currentContent);
        }
    }
}