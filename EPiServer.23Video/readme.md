23Video
======================

Controllers
======================

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
======================

\View
@Model Models.Video
@Html.Raw(Model.VideoUrl) eller @Html.Raw(Model.oEmbedHtml)

\DisplayTemplates
@if (Model != null && Model != ContentReference.EmptyReference)
{
    var oEmbedHtml = DataFactory.Instance.Get<Video>(@Model);

    if (oEmbedHtml != null)
    {
        @Html.Raw(oEmbedHtml.oEmbedHtml)
    }
}