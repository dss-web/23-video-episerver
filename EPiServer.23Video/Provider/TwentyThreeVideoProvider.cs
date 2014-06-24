using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Helpers;
using System.Web.Script.Serialization;
using EPiCode.TwentyThreeVideo.Models;
using EPiCode.TwentyThreeVideo.oEmbed;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using log4net;
using Visual.Domain;

namespace EPiCode.TwentyThreeVideo.Provider
{
    public class TwentyThreeVideoProvider : ContentProvider
    {
        private static ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<BasicContent> _items = new List<BasicContent>();
        private readonly SettingsRepository _settingsRepository;
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly VideoFolder _entryPoint;
        private readonly ThumbnailManager _thumbnailManager;

        public TwentyThreeVideoProvider(IContentTypeRepository contentTypeRepository,
            ThumbnailManager thumbnailManager,
            VideoFolder entryPoint,
            SettingsRepository settingsRepository)
        {
            _contentTypeRepository = contentTypeRepository;
            _entryPoint = entryPoint;
            _settingsRepository = settingsRepository;
            _thumbnailManager = thumbnailManager;
            _settingsRepository = settingsRepository;

        }

        public override ContentProviderCapabilities ProviderCapabilities
        {
            get { return ContentProviderCapabilities.Create | ContentProviderCapabilities.Edit; }
        }

        #region ContentProvider

        public void RefreshItems(List<BasicContent> items)
        {
              _items = items;
            this.ClearProviderPagesFromCache();
        }

        //public List<BasicContent> LoadFromService()
        //{
        //    var tempItems = new List<BasicContent>();
        //    bool refresh = false;
        //    if (_items.Any())
        //    {
        //        refresh = true;
        //        _log.DebugFormat("Refreshing 23 video content");
        //    }

        //    var videoFolders = CreateFoldersFromChannels().ToList();
        //    foreach (var folder in videoFolders)
        //    {
        //        if (refresh)
        //            tempItems.Add(folder);
        //        else
        //            _items.Add(folder);

        //    }
        //    foreach (var folder in videoFolders)
        //    {
        //        if (folder is VideoFolder)
        //        {
        //            var videos = TwentyThreeVideoRepository.GetVideoList(folder.ContentLink.ID);
        //            foreach (var videoData in videos)
        //            {
        //                var video =
        //                    GetDefaultContent(LoadContent(folder.ContentLink, LanguageSelector.AutoDetect()),
        //                        _contentTypeRepository.Load<Video>().ID, LanguageSelector.AutoDetect()) as
        //                        Video;
        //                if (video != null)
        //                {
        //                    _log.DebugFormat("23Video: Added video with name {0}", video.Name);
        //                    new VideoHelper().PopulateVideo(video, videoData);
        //                    if (refresh)
        //                        tempItems.Add(video);
        //                    else
        //                        _items.Add(video);
        //                }
        //                else
        //                {
        //                    _log.InfoFormat("23Video: Video from 23Video can not be loaded in EPiServer as Video. Videoname from 23Video {0}", videoData.One);
        //                }

        //            }
        //        }
        //    }
        //    if (refresh)
        //    {
        //        _items.Clear();
        //        foreach (var basicContent in tempItems)
        //        {
        //            _items.Add(basicContent);
        //        }
        //    }
        //    return _items;

        //}

        private IEnumerable<BasicContent> CreateFoldersFromChannels()
        {
            List<Album> albums = TwentyThreeVideoRepository.GetChannelList();
            foreach (var album in albums)
            {
                var folder = GetDefaultContent(_entryPoint, _contentTypeRepository.Load<VideoFolder>().ID,
                        LanguageSelector.AutoDetect()) as VideoFolder;

                if (folder == null) continue;

                if (album.AlbumId != null)
                {
                    var id = (int)album.AlbumId;
                    folder.ContentLink = new ContentReference(id, ProviderKey);
                    folder.Name = album.Title;
                    _log.InfoFormat("23Video: Channel {0} created.", album.Title);
                    yield return folder;

                }
            }
        }

        protected override IContent LoadContent(ContentReference contentLink, ILanguageSelector languageSelector)
        {
            var item = _items.FirstOrDefault(p => p.ContentLink.Equals(contentLink));
            if (item == null)
            {
                item = _items.FirstOrDefault(p => p.ContentLink.CompareToIgnoreWorkID(contentLink));
            }
            return item;
        }

        protected override ContentResolveResult ResolveContent(ContentReference contentLink)
        {
            if (_items == null)
            {
                _log.InfoFormat("23Video: _items is null when resolving content with it {0}. Return null", contentLink.ID);
                return null;
            }

            BasicContent video = _items.FirstOrDefault(p => p.ContentLink.Equals(contentLink));

            if (video == null)
            {
                return base.ResolveContent(contentLink);
            }
            return ResolveContent(video);
        }

        protected override ContentResolveResult ResolveContent(Guid contentGuid)
        {
            var video = _items.FirstOrDefault(p => p.ContentGuid.Equals(contentGuid));

            if (video == null)
            {
                return base.ResolveContent(contentGuid);
            }

            return ResolveContent(video);
        }

        protected ContentResolveResult ResolveContent(BasicContent video)
        {
            var contentItem = new ContentCoreData()
            {
                ContentGuid = video.ContentGuid,
                ContentReference = video.ContentLink,
                ContentTypeID = ContentTypeRepository.Load(typeof(Video)).ID,
            };
            return base.CreateContentResolveResult(contentItem);
        }

        protected override IList<GetChildrenReferenceResult> LoadChildrenReferencesAndTypes(ContentReference contentLink, string languageId, out bool languageSpecific)
        {
            var videoTypeId = _contentTypeRepository.Load(typeof (Video));
            var videoFolderTypeId = _contentTypeRepository.Load(typeof (VideoFolder));
            languageSpecific = false;
            if (contentLink.CompareToIgnoreWorkID(EntryPoint)){
                
                var test =
                    _items.Where(x => x.ContentTypeID.Equals(videoFolderTypeId.ID))
                    .Select(p => new GetChildrenReferenceResult() { ContentLink = p.ContentLink, ModelType = typeof(VideoFolder) }).ToList();
                return test;

            }
            var content = LoadContent(contentLink, LanguageSelector.AutoDetect());

            return _items
               .Where(p => p.ContentTypeID.Equals(videoTypeId.ID) && p.ParentLink.ID.Equals(content.ContentLink.ID))
               .Select(p => new GetChildrenReferenceResult() { ContentLink = p.ContentLink, ModelType = typeof(Video) }).ToList();
        }

        public override ContentReference Save(IContent content, SaveAction action)
        {
            var helper = new VideoHelper();
            var newVersion = content as Video;
            if (content.ContentLink.WorkID != 0)
            {
                if (action == SaveAction.Publish && newVersion != null)
                {
                    TwentyThreeVideoRepository.UpdateVideo(new Photo()
                    {
                        PhotoId = Convert.ToInt32(newVersion.Id),
                        Title = newVersion.Name
                    });
                }
                //If we saved an item that already has a specific version, just return the current version...
                return content.ContentLink;
            }

            //...otherwise save the content (which has been made a copy of the UI) to the local repository.
            if (newVersion != null)
            {
                newVersion.Status = VersionStatus.CheckedOut;
                newVersion.ContentLink = new ContentReference((newVersion.Id).GetHashCode(), 1, ProviderKey);
            }
            // New content
            else
            {
                var mediaData = content as MediaData;

                if (mediaData != null && mediaData.BinaryData != null)
                {
                    Blob blobData = ((MediaData)content).BinaryData;

                    using (var stream = blobData.OpenRead())
                    {
                        int? videoId = TwentyThreeVideoRepository.UploadVideo(content.Name, stream, content.ParentLink.ID);
                        if (videoId != null)
                        {
                            BlobFactory.Instance.Delete((content as MediaData).BinaryData.ID);
                            var video = GetDefaultContent(LoadContent(content.ParentLink, LanguageSelector.AutoDetect()),
                                        _contentTypeRepository.Load<Video>().ID, LanguageSelector.AutoDetect()) as Video;

                            var item = TwentyThreeVideoRepository.GetVideo((int)videoId);
                            helper.PopulateVideo(video, item);

                            _items.Add(video);
                            return video.ContentLink;
                        }
                    }
                }

            }
            _items.Add(newVersion);
            return newVersion.ContentLink; //;.EmptyReference;
        }

        #endregion

        #region Helpers

  

        #endregion
    }
}