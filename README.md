# 23 Video content provider for EPiServer

![23 Video - adding a video](screenshot001.png)

## About

http://world.episerver.com/Blogs/Per-Magne-Skuseth/

## Installation

* Prerequisite: EPiServer 7.5 or later
* Add a project reference to EPiCode.23Video from your EPiServer project.
You'll need to add a few manual steps:

1. Add the corresponding settings to web.config, [fetched from your 23 Video account](http://www.23video.com/api/oauth#setting-up-your-application):

```xml
    <add key="TwentyThreeVideoEnabled" value="true" /> 
    <add key="TwentyThreeVideoDomain" value="" /> 
    <add key="TwentyThreeVideoConsumerKey" value="" />
    <add key="TwentyThreeVideoConsumerSecret" value="" />
    <add key="TwentyThreeVideoAccessToken" value="" />
    <add key="TwentyThreeVideoAccessTokenSecret" value="" />
    <add key="TwentyThreeVideoEnableoEmbed" value="true" />
```
2. Create a folder beneath your "modules" folder in your EPiServer sites called 'twentythreevideo'. Copy the content from EPiCode.23Video\ClientResources into this folder.

3. Add controllers and views to display your videos! Examples are defined below:


###Controllers

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

###Views

#####Standard view:

```c#
@Model Models.Video
@Html.Raw(Model.VideoUrl)
or
@Html.Raw(Model.oEmbedHtml)
```

#####DisplayTemplate:

```c#
@if (Model != null && Model != ContentReference.EmptyReference)
{
    var oEmbedHtml = DataFactory.Instance.Get<Video>(@Model);

    if (oEmbedHtml != null)
    {
        @Html.Raw(oEmbedHtml.oEmbedHtml)
    }
}
```

## Copyright and License

The 23 Video content provider for EPiServer is copyright 2014 Norwegian Government Security and Service Organisation. 

23 Video content provider for EPiServer is free software: you can redistribute it and/or modify
it under the terms of the [GNU Lesser General Public License](lgpl-3.0.txt) as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

23 Video content provider for EPiServer is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with 23 Video content provider for EPiServer.  If not, see <http://www.gnu.org/licenses/>.

## DISCLAIMER

THIS SOFTWARE  IS PROVIDED "AS IS" AND ANY EXPRESSED OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE NORWEGIAN GOVERNMENT SECURITY AND SERVICE ORGANISATION , OR ANY OF THEIR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

Without limiting the foregoing, the Norwegian Government Security and Service Organisation make no warranty that:

* the software will meet your requirements.
* the software will be uninterrupted, timely, secure or error-free.
* the results that may be obtained from the use of the software will be effective, accurate or reliable.
* the quality of the software will meet your expectations.
* any errors in the software obtained from the OpenSHA.org web site will be corrected.

Software and its documentation made available:

* could include technical or other mistakes, inaccuracies or typographical errors. The Norwegian Government Security and Service Organisation and its contributors may make changes to the software or documentation made available on its web site.
* may be out of date and the Norwegian Government Security and Service Organisation  and its contributors make no commitment to update such materials.

The Norwegian Government Security and Service Organisation  and its contributors assume no responsibility for errors or ommissions in the software or documentation available.

In no event shall the Norwegian Government Security and Service Organisation  and it's contributors be liable to you or any third parties for any special, punitive, incidental, indirect or consequential damages of any kind, or any damages whatsoever, including, without limitation, those resulting from loss of use, data or profits, whether or not the Norwegian Government Security and Service Organisation  and its contributors has been advised of the possibility of such damages, and on any theory of liability, arising out of or in connection with the use of this software.

The use of the software is done at your own discretion and risk and with agreement that you will be solely responsible for any damage to your computer system or loss of data that results from such activities. No advice or information, whether oral or written, obtained by you from the Norwegian Government Security and Service Organisation , its its website or its contributors shall create any warranty for the software.