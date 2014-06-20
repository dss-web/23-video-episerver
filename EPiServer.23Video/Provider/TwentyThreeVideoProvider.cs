﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using EPiCode.TwentyThreeVideo.Models;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;
using EPiServer.Web;
using log4net;
using Visual.Domain;

namespace EPiCode.TwentyThreeVideo.Provider
{
    public class TwentyThreeVideoProvider : ContentProvider
    {
        private static ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<BasicContent> _items = new List<BasicContent>();
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

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            CreateFoldersFromChannels();
        }

        private void CreateFoldersFromChannels()
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
                    _items.Add(folder);
                    _log.InfoFormat("23Video: Channel {0} created.", album.Title);
                }
            }
        }


        protected override void SetCacheSettings(ContentReference parentLink, string urlSegment, IEnumerable<MatchingSegmentResult> childrenMatches, CacheSettings cacheSettings)
        {
            //cacheSettings.CacheKeys.Add(EPiServer.DataFactoryCache.PageCommonCacheKey(new ContentReference(contentReference.ID)));
        }

        protected override IContent LoadContent(ContentReference contentLink, ILanguageSelector languageSelector)
        {
            var item = _items.FirstOrDefault(p => p.ContentLink.CompareToIgnoreWorkID(contentLink)); //?? TwentyThreeVideoRepository.GetVideo(contentLink.ID) as ICo;

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
            languageSpecific = false;

            if (contentLink.CompareToIgnoreWorkID(EntryPoint))
                return _items
                    .Where(p => p is VideoFolder)
                    .Select(p => new GetChildrenReferenceResult() { ContentLink = p.ContentLink, ModelType = typeof(VideoFolder) }).ToList();

            var content = LoadContent(contentLink, LanguageSelector.AutoDetect());

            if (content is VideoFolder)
            {
                var videos = TwentyThreeVideoRepository.GetVideoList(contentLink.ID);

                foreach (var item in videos)
                {
                    var video = GetDefaultContent(LoadContent(contentLink, LanguageSelector.AutoDetect()),
                            _contentTypeRepository.Load<Video>().ID, LanguageSelector.AutoDetect()) as Video;

                    if (video != null)
                    {
                        PopulateVideo(video, item);
                        _items.Add(video);
                        _log.InfoFormat("23Video: Added video with name {0}", video.Name);
                    }
                    else
                    {
                        _log.InfoFormat("23Video: Video from 23Video can not be loaded in EPiServer as Video. Videoname from 23Video {0}", item.One);
                    }
                }
            }
            return _items
               .Where(p => p is Video && p.ParentLink.ID.Equals(content.ContentLink.ID))
               .Select(p => new GetChildrenReferenceResult() { ContentLink = p.ContentLink, ModelType = typeof(Video) }).ToList();
        }

        public Video PopulateVideo(Video video, Photo item)
        {
            if (item.PhotoId != null)
            {
                int id = (int)(item.PhotoId);
                video.VideoUrl = EmbedCode(item.PhotoId.ToString(), item.Token, item.VideoHD.Width ?? item.VideoMedium.Width ?? item.VideoSmall.Width, item.VideoHD.Height ?? item.VideoMedium.Height ?? item.VideoSmall.Height);
                video.ContentLink = new ContentReference((id).GetHashCode(), ProviderKey);
                video.Created = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));
                video.Changed = video.Created;
                video.IsPendingPublish = false;
                video.StartPublish = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
                video.Status = VersionStatus.Published;
                video.Id = id.ToString();
                video.ContentGuid = StringToGuid(id.ToString());
                video.VideoId = item.PhotoId.ToString();
                video.Name = item.Title;
                video.BinaryData = GetThumbnail(item);
                video.Thumbnail = _thumbnailManager.CreateImageBlob(video.BinaryData, "thumbnail", new ImageDescriptorAttribute(48, 48));
                video.oEmbedVideoName = item.One;

                if (_settingsRepository.oEmbedIsEnabled)
                {
                    video.oEmbedHtml = TwentyThreeVideoRepository.GetoEmbedCodeForVideo(item.One);
                }

                _log.InfoFormat("23Video: Created IContent for video with id {0} and name {1}", id, video.Name);

            }
            return video;
        }

        public static string EmbedCode(string photoId, string photoToken, int? width, int? height)
        {
            string domain = Client.Settings.Domain;

            string widthString = width.ToString();

            string heightString = (height == null ? Math.Round(width.Value / 16.0 * 9.0).ToString() : height.Value.ToString());

            return "<iframe src=\"http://" + domain + "/v.ihtml?token=" + photoToken + "&photo%5fid=" + photoId + "\" width=\"" + widthString + "\" height=\"" + heightString + "\" frameborder=\"0\" border=\"0\" scrolling=\"no\"></iframe>";

        }

        public override ContentReference Save(IContent content, SaveAction action)
        {
            //TwentyThreeVideoRepository.UpdateVideo(new Photo() { PhotoId = content.ContentLink.ID, Title = ((IContent)content).Name });
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
                        PopulateVideo(video, item);

                        _items.Add(video);
                        return video.ContentLink;

                    }
                }
            }
            return ContentReference.EmptyReference;
        }

        #endregion

        #region Helpers

        private static Guid StringToGuid(string value)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(value));
            return new Guid(data);
        }

        private Blob GetThumbnail(Photo item)
        {
            var webClient = new WebClient();
            var url = "http://" + Client.Settings.Domain + item.Original.Download;
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