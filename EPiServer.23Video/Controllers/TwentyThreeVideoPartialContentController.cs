using System.Configuration;
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
            bool isoEmbedEnabled;
            if (bool.TryParse(ConfigurationManager.AppSettings.Get("TwentyThreeVideoEnableoEmbed"), out isoEmbedEnabled))
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