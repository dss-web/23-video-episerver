﻿/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */
using System.Collections.Generic;
using System.Linq;
using EPiCode.TwentyThreeVideo.Data;
using EPiCode.TwentyThreeVideo.Models;
using EPiServer.Cms.Shell.Search;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Localization;
using EPiServer.Shell.Search;
using EPiServer.Web;

namespace EPiCode.TwentyThreeVideo.Provider
{
    [SearchProvider]
    public class VideoSearchProvider : ContentSearchProviderBase<BasicContent, ContentType>
    {
        public VideoSearchProvider(LocalizationService localizationService, SiteDefinitionResolver siteDefinitionResolver, IContentTypeRepository contentTypeRepository)
            : base(localizationService, siteDefinitionResolver, contentTypeRepository)
        {
        }

        public override IEnumerable<SearchResult> Search(Query query)
        {
            var videoRepo = new IntermediateVideoDataRepository();
            var videos = videoRepo.Load().Where(x => x is Video);
            foreach (var basicContent in videos.Where(x => x.Name.ToLower().Contains(query.SearchQuery)))
            {
                yield return this.CreateSearchResult(basicContent);
            }

        }
        public override string Area { get { return "ttv"; } }
        public override string Category { get { return "ttv"; } }
        protected override string IconCssClass{ get { return ""; }}
    }
}
