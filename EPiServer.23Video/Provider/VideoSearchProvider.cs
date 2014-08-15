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
