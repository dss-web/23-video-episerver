23Video
======================


Installation:

Add a project reference to EPiCode.23Video from your EPiServer project.
You'll need to add a few manual steps:

1. Add the corresponding settings to web.config, fetched from your 23 Video account:

    <add key="TwentyThreeVideoEnabled" value="true" /> 
    <add key="TwentyThreeVideoDomain" value="" /> 
    <add key="TwentyThreeVideoConsumerKey" value="" />
    <add key="TwentyThreeVideoConsumerSecret" value="" />
    <add key="TwentyThreeVideoAccessToken" value="" />
    <add key="TwentyThreeVideoAccessTokenSecret" value="" />
    <add key="TwentyThreeVideoEnableoEmbed" value="true" />

2. Create a folder beneath your "modules" folder in your EPiServer sites called 'twentythreevideo'. Copy the content from EPiCode.23Video\ClientResources into this folder.

3. Add controllers and views to display your videos! Examples are defined below:


Controllers
======================

```c#
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
```

```c#
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
```

Views
======================

View

```
@Model Models.Video
@Html.Raw(Model.VideoUrl)
or
@Html.Raw(Model.oEmbedHtml)
```

DisplayTemplates

```
@if (Model != null && Model != ContentReference.EmptyReference)
{
    var oEmbedHtml = DataFactory.Instance.Get<Video>(@Model);

    if (oEmbedHtml != null)
    {
        @Html.Raw(oEmbedHtml.oEmbedHtml)
    }
}
```