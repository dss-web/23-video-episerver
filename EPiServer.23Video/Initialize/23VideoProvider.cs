using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication.ExtendedProtection;
using System.Web.Helpers;
using System.Web.UI.WebControls;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Framework.Blobs;
using EPiServer._23Video.Models;
using Visual;
using Visual.Domain;

namespace EPiServer._23Video.Initialize
{
    public class _23VideoProvider : ContentProvider
    {
        private const string BaseApi = "https://www.googleapis.com/youtube/v3/";

        private readonly List<BasicContent> _items = new List<BasicContent>();
        private readonly _23VideoSettingsRepository _settingsRepository;

        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly _23VideoFolder _entryPoint;

        public _23VideoProvider(IContentTypeRepository contentTypeRepository,
            _23VideoFolder entryPoint,
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

            //var folder = GetDefaultContent(_entryPoint, _contentTypeRepository.Load<ContentFolder>().ID, LanguageSelector.AutoDetect()) as ContentFolder;
            //folder.ContentLink = new ContentReference(999, ProviderKey);
            //folder.Name = "Channel";
            //_items.Add(folder);

            //folder = GetDefaultContent(_entryPoint, _contentTypeRepository.Load<YouTubeFolder>().ID, LanguageSelector.AutoDetect()) as YouTubeFolder;
            //folder.ContentLink = new ContentReference(2, ProviderKey);
            //folder.Name = "Channels";
            //_items.Add(folder);

            List<Album> albums = GetAlbumList();

            foreach (var album in albums)
            {
                var folder = GetDefaultContent(_entryPoint, _contentTypeRepository.Load<_23VideoFolder>().ID, LanguageSelector.AutoDetect()) as _23VideoFolder;

                int id = (int)album.AlbumId;

                folder.ContentLink = new ContentReference(id*-1, ProviderKey);
                folder.Name = album.Title;
                _items.Add(folder);
            }
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

        private List<Photo> GetPhotoList()
        {
            // Check that we actually have everything configured
            IApiProvider apiProvider = _23Client.ApiProvider;

            // Get a list of videos to throw in there
            List<Photo> photos = GetPhotos(apiProvider);

            return photos;

        }

        private List<Photo> GetPhotos(IApiProvider apiProvider)
        {
            const string cacheKey = "PhotoService";
            //if (Caching.VideoProviderCache.Instance.InnerCache.ContainsKey(cacheKey))
            //{
            //    return Caching.VideoProviderCache.Instance.InnerCache.GetValue(cacheKey) as List<Photo>;
            //}

            List<Photo> result = new List<Photo>();
            IPhotoService photoService = new PhotoService(apiProvider);

            bool done = false;
            int page = 1;
            while (!done)
            {
                List<Photo> photos = photoService.GetList(new PhotoListParameters
                {
                    IncludeUnpublished = false,
                    Size = 100,
                    PageOffset = page++
                });

                if (photos.Count > 0)
                    result.AddRange(photos);

                if (photos.Count < 100)
                    done = true;
            }

            //Caching.VideoProviderCache.Instance.InnerCache.Add(cacheKey, result, result.Count, TimeSpan.FromMinutes(10));
            return result;
        }

        private List<Album> GetAlbumList()
        {
            // Check that we actually have everything configured
            IApiProvider apiProvider = _23Client.ApiProvider;

            // Get a list of videos to throw in there
            List<Album> albums = GetAlbums(apiProvider);

            return albums;

        }
        private List<Album> GetAlbums(IApiProvider apiProvider)
        {
            IAlbumService albumService = new AlbumService(apiProvider);

            return albumService.GetList();
        }



        protected override IList<GetChildrenReferenceResult> LoadChildrenReferencesAndTypes(ContentReference contentLink, string languageId, out bool languageSpecific)
        {
            languageSpecific = false;

            if (!_settingsRepository.ValidateAccessToken())
                return null;

            // Get default YouTube folders
            if (contentLink.CompareToIgnoreWorkID(EntryPoint))
                return _items
                    .Where(p => p is _23VideoFolder)
                    .Select(p => new GetChildrenReferenceResult() { ContentLink = p.ContentLink, ModelType = typeof(_23VideoFolder) }).ToList();



            //List<Photo> photos = GetPhotoList();



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