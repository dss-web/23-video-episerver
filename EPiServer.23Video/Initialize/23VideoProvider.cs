using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Helpers;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Framework.Blobs;
using EPiServer._23Video.Models;

namespace EPiServer._23Video.Initialize
{
    public class _23VideoProvider : ContentProvider
    {
        private const string BaseApi = "https://www.googleapis.com/youtube/v3/";
        
        private readonly List<BasicContent> _items = new List<BasicContent>();
        private readonly _23VideoSettingsRepository _settingsRepository;

        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly ContentFolder _entryPoint;

        public _23VideoProvider(IContentTypeRepository contentTypeRepository,
            ContentFolder entryPoint,
            _23VideoSettingsRepository settingsRepository)
        {
            _contentTypeRepository = contentTypeRepository;
            _entryPoint = entryPoint;
            _settingsRepository = settingsRepository;
        }

        public override ContentProviderCapabilities ProviderCapabilities
        {
            get { return ContentProviderCapabilities.None; }
            //ContentProviderCapabilities.Edit | ContentProviderCapabilities.Create | ContentProviderCapabilities.Move | ContentProviderCapabilities.Security; }
        
        }
      
        #region ContentProvider

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            var folder = GetDefaultContent(_entryPoint, _contentTypeRepository.Load<ContentFolder>().ID, LanguageSelector.AutoDetect()) as ContentFolder;
            folder.ContentLink = new ContentReference(1, ProviderKey);
            folder.Name = "Channel";
            _items.Add(folder);

            //folder = GetDefaultContent(_entryPoint, _contentTypeRepository.Load<YouTubeFolder>().ID, LanguageSelector.AutoDetect()) as YouTubeFolder;
            //folder.ContentLink = new ContentReference(2, ProviderKey);
            //folder.Name = "Channels";
            //_items.Add(folder);
        }

        

        protected override void SetCacheSettings(ContentReference parentLink, string urlSegment, IEnumerable<MatchingSegmentResult> childrenMatches, CacheSettings cacheSettings)
        {
            //cacheSettings.CacheKeys.Add(EPiServer.DataFactoryCache.PageCommonCacheKey(new ContentReference(contentReference.ID)));
        }

        protected override IContent LoadContent(ContentReference contentLink, ILanguageSelector languageSelector)
        {
            var item = _items.FirstOrDefault(p => p.ContentLink.CompareToIgnoreWorkID(contentLink));

            return item;
        }
        
        protected override IList<GetChildrenReferenceResult> LoadChildrenReferencesAndTypes(ContentReference contentLink, string languageId, out bool languageSpecific)
        {
            languageSpecific = false;

            if (!_settingsRepository.ValidateAccessToken())
                return null;

            // Get default YouTube folders
            if (contentLink.CompareToIgnoreWorkID(EntryPoint))
                return _items
                    .Where(p => p is ContentFolder)
                    .Select(p => new GetChildrenReferenceResult() { ContentLink = p.ContentLink, ModelType = typeof(ContentFolder) }).ToList();


            //var currentSettings = _settingsRepository.LoadSettings();

            //// Get Channels
            //if (contentLink.ID == 1)
            //{
            //    // Remove Channel
            //    _items.RemoveAll(p => p is _23VideoChannel);

            //    // Request Channels
            //    var jsonResult = 
            //        GetJson(string.Format("{0}playlists?part=snippet&mine=true&access_token={1}",
            //        BaseApi, currentSettings.AccessToken));

            //    foreach (var item in jsonResult.items)
            //    {
            //        var playlist = GetDefaultContent(LoadContent(contentLink, LanguageSelector.AutoDetect()), _contentTypeRepository.Load<YouTubePlaylist>().ID, LanguageSelector.AutoDetect()) as YouTubePlaylist;
            //        //playlist.ContentLink = new ContentReference(_contentId++, 1, ProviderKey);
            //        playlist.ContentLink = new ContentReference(((string)item.id).GetHashCode(), ProviderKey);
            //        playlist.PlaylistId = item.id;
            //        playlist.Name = item.snippet.title;
            //        _items.Add(playlist);
            //    }

            //    return _items
            //        .Where(p => p is YouTubePlaylist)
            //        .Select(p => new GetChildrenReferenceResult() { ContentLink = p.ContentLink, ModelType = typeof(YouTubePlaylist) }).ToList();
            //}


            //var content = LoadContent(contentLink, LanguageSelector.AutoDetect());

            //// Get videos for a PlayList
            //if (content is YouTubePlaylist)
            //{
            //    // Request videos
            //    var jsonResult =
            //        GetJson(string.Format("{0}playlistItems?part=id%2Csnippet&playlistId={1}&access_token={2}",
            //        BaseApi,
            //        ((YouTubePlaylist)content).PlaylistId,
            //        currentSettings.AccessToken));

            //    foreach (var item in jsonResult.items)
            //    {
            //        var video = GetDefaultContent(LoadContent(contentLink, LanguageSelector.AutoDetect()), _contentTypeRepository.Load<YouTubeVideo>().ID, LanguageSelector.AutoDetect()) as YouTubeVideo;
            //        //video.ContentLink = new ContentReference(_contentId++, 1, ProviderKey);
            //        video.ContentLink = new ContentReference(((string)item.snippet.resourceId.videoId).GetHashCode(), ProviderKey);
            //        video.Created = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));
            //        video.Changed = video.Created;
            //        video.StartPublish = DateTime.Now.Subtract(new TimeSpan(1,0,0,0));
            //        video.Status = VersionStatus.Published;
            //        video.Id = item.id;
            //        video.ContentGuid = Guid.NewGuid();
            //        video.VideoId = item.snippet.resourceId.videoId;
            //        video.Name = item.snippet.title;
            //      //  video.BinaryData = GetThumbnail(item.snippet.thumbnails.medium.url, video.ContentGuid);
                    
              
            //        _items.Add(video);

                  
            //    }

            //    return _items
            //       .Where(p => p is YouTubeVideo)
            //       .Select(p => new GetChildrenReferenceResult() { ContentLink = p.ContentLink, ModelType = typeof(YouTubeVideo) }).ToList();
            //}

            return null;
        }
        

        public override ContentReference Save(IContent content, SaveAction action)
        {

            return content.ContentLink;

        }

   


        #endregion

        #region Helpers

        private dynamic GetJson(string url)
        {
            var webClient = new WebClient();
            var stringResult = webClient.DownloadString(url);
            return Json.Decode(stringResult);
        }

        private Blob GetThumbnail(string url, Guid guid)
        {
            var webClient = new WebClient();
            var imageData = webClient.DownloadData(url);

            //Define a container
            var container = Blob.GetContainerIdentifier(guid);

            //Uploading a file to a blob
            var thumbNailBlob = BlobFactory.Instance.CreateBlob(container, ".jpg");
            using (var stream = new MemoryStream(imageData))
            {
                thumbNailBlob.Write(stream);
            }
            return thumbNailBlob;
        }

        #endregion
    }
}