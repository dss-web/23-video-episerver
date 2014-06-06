using System;
using System.Collections.Generic;
using System.Globalization;
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
using EPiServer._23Video.Factory;
using EPiServer._23Video.Models;
using Visual;
using Visual.Domain;

namespace EPiServer._23Video.Initialize
{
    public class _23VideoProvider : ContentProvider
    {
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
            get { return ContentProviderCapabilities.Create; }
            //ContentProviderCapabilities.Edit | ContentProviderCapabilities.Create | ContentProviderCapabilities.Move | ContentProviderCapabilities.Security; }

        }

        #region ContentProvider

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            CreateFoldersFromChannels();
        }

        private void CreateFoldersFromChannels()
        {
            List<Album> albums = _23VideoFactory.GetAlbumList();

            foreach (var album in albums)
            {
                var folder = GetDefaultContent(_entryPoint, _contentTypeRepository.Load<_23VideoFolder>().ID,
                        LanguageSelector.AutoDetect()) as _23VideoFolder;

                if (folder == null) continue;

                if (album.AlbumId != null)
                {
                    var id = (int)album.AlbumId;
                    folder.ContentLink = new ContentReference(id, ProviderKey);
                    folder.Name = album.Title;
                    _items.Add(folder);
                }
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

        protected override IList<GetChildrenReferenceResult> LoadChildrenReferencesAndTypes(ContentReference contentLink, string languageId, out bool languageSpecific)
        {
            languageSpecific = false;

            if (contentLink.CompareToIgnoreWorkID(EntryPoint))
                return _items
                    .Where(p => p is _23VideoFolder)
                    .Select(p => new GetChildrenReferenceResult() { ContentLink = p.ContentLink, ModelType = typeof(_23VideoFolder) }).ToList();

            var content = LoadContent(contentLink, LanguageSelector.AutoDetect());

            if (content is _23VideoFolder)
            {


                var videos = _23VideoFactory.GetPhotoList(contentLink.ID);

                foreach (var item in videos)
                {
                    var video =
                        GetDefaultContent(LoadContent(contentLink, LanguageSelector.AutoDetect()),
                            _contentTypeRepository.Load<_23VideoVideo>().ID, LanguageSelector.AutoDetect()) as
                            _23VideoVideo;

                    if (item.PhotoId != null)
                    {
                        int id = (int)(item.PhotoId);

                        video.ContentLink = new ContentReference((id).GetHashCode(), ProviderKey);
                        video.Created = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));
                        video.Changed = video.Created;
                        video.IsPendingPublish = false;
                        video.StartPublish = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
                        video.Status = VersionStatus.Published;
                        video.Id = id.ToString();
                        video.ContentGuid = Guid.NewGuid();
                        video.VideoId = item.PhotoId.ToString();
                        video.Name = item.Title;
                        video.BinaryData = GetThumbnail(item);
                        _items.Add(video);
                    }
                }
            }
            return _items
               .Where(p => p is _23VideoVideo && p.ParentLink.ID.Equals(content.ContentLink.ID))
               .Select(p => new GetChildrenReferenceResult() { ContentLink = p.ContentLink, ModelType = typeof(_23VideoVideo) }).ToList();
        }



        public override ContentReference Save(IContent content, SaveAction action)
        {
            BlobFactory.Instance.Delete((content as MediaData).BinaryData.ID);
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

        private Blob GetThumbnail(Photo item)
        {
            var webClient = new WebClient();
            var url = "http://" + _23Client._23VideoSettings.Domain + item.Original.Download;
            var imageData = webClient.DownloadData(url);
            string container = item.PhotoId.ToString();
            var blob = BlobFactory.Instance.GetBlob(new Uri(string.Format("{0}://{1}/{2}/{3}", Blob.BlobUriScheme, Blob.DefaultProvider, container, "original.jpg")));
            using (var stream = new MemoryStream(imageData))
            {
                blob.Write(stream);
            }
            return blob;
        }

        #endregion
    }
}