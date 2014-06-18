using System.Configuration;
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
            bool isoEmbedEnabled;
            if ( bool.TryParse( ConfigurationManager.AppSettings.Get("TwentyThreeVideoEnableoEmbed"), out isoEmbedEnabled))
            {
                if (isoEmbedEnabled)
                {
                    currentContent.VideoUrl =
                        TwentyThreeVideoRepository.GetoEmbedCodeForVideo(currentContent.oEmbedVideoName);
                }
            }
            
            return PartialView(currentContent);
        }
    }
}
