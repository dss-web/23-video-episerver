using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiCode.TwentyThreeVideo.Models;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace EPiCode.TwentyThreeVideo.Provider
{
    public static class Extensions
    {
        public static IEnumerable<Video> Videos(this IContent content)
        {
            var contentSoftLinkIndexer = ServiceLocator.Current.GetInstance<ContentSoftLinkIndexer>();
            var links = contentSoftLinkIndexer.GetLinks(content);
            var repository = ServiceLocator.Current.GetInstance<IContentRepository>();
            foreach (var softLink in links)
            {
                Video videoContent;
                if (
                    repository
                        .TryGet<Video>(softLink.ReferencedContentLink, out videoContent))
                {
                    yield return videoContent;
                }
            }
        }
    }
}
