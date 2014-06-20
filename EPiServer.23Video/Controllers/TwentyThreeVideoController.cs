using System.Web.Mvc;
using EPiCode.TwentyThreeVideo.Provider;
using EPiServer.Web;
using EPiCode.TwentyThreeVideo.Models;

namespace EPiCode.TwentyThreeVideo.Controllers
{
    public class TwentyThreeVideoController : Controller, IRenderTemplate<Video>
    {
        public ActionResult Index(Video currentContent)
        {
            if (Client.Settings.oEmbedIsEnabled)
            {
                currentContent.VideoUrl = currentContent.oEmbedHtml;
            }

            return PartialView(currentContent);
        }
    }
}
