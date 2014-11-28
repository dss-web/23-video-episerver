/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */
using System.Collections.Generic;
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

        public static IEnumerable<string> VideosIds(this IContent content)
        {
            var contentSoftLinkIndexer = ServiceLocator.Current.GetInstance<ContentSoftLinkIndexer>();
            var links = contentSoftLinkIndexer.GetLinks(content);
            var repository = ServiceLocator.Current.GetInstance<IContentRepository>();
            foreach (var softLink in links)
            {
                Video videoContent;
                if (repository.TryGet(softLink.ReferencedContentLink, out videoContent))
                {
                    yield return videoContent.Id;
                }
            }
        }
    }
}
