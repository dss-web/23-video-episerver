using System;
using System.Collections.Generic;
using System.Linq;
using EPiCode.TwentyThreeVideo.Data;
using EPiCode.TwentyThreeVideo.Models;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Framework.Blobs;
using EPiServer.Web;
using log4net;
using Visual.Domain;

namespace EPiCode.TwentyThreeVideo.Provider
{
    public class TwentyThreeVideoProvider : ContentProvider
    {
        private static ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<BasicContent> _items = new List<BasicContent>();
        private readonly IContentTypeRepository _contentTypeRepository;

        private readonly IntermediateVideoDataRepository _intermediateVideoDataRepository;

        public TwentyThreeVideoProvider(IContentTypeRepository contentTypeRepository,
            IntermediateVideoDataRepository intermediateVideoDataRepository)
        {
            _contentTypeRepository = contentTypeRepository;
            _intermediateVideoDataRepository = intermediateVideoDataRepository;
        }

        public override ContentProviderCapabilities ProviderCapabilities
        {
            get { return ContentProviderCapabilities.Create | ContentProviderCapabilities.Edit; }
        }

        protected override IContent LoadContent(ContentReference contentLink, ILanguageSelector languageSelector)
        {
            var item = _items.FirstOrDefault(p => p.ContentLink.Equals(contentLink));
            if (item == null)
            {
                item = _items.FirstOrDefault(p => p.ContentLink.CompareToIgnoreWorkID(contentLink));
            }

            

            // LoadContent will be called after a autosave or after publish on the item since EPiServer will try to update properties in this method;
            // EPiServer.Cms.Shell.UI.Rest.ContentChangeManager.UpdateContentProperties(ContentReference contentReference, IDictionary`2 properties, SaveAction saveAction). 
            // The UpdateContentProperties requires a writable icontent object. So in the Save method we set work id to 1 to tell EPiServer that we have a new version and that the publish-button is enabled.
            if (contentLink.WorkID > 0)
            {
                var video = item as Video;

                // Only for vidoes, not videofolder
                if (video != null)
                {
                    Video writeableVideo = video.CreateWritableClone() as Video;
                    ((IVersionable)writeableVideo).Status = VersionStatus.CheckedOut;
                    return writeableVideo;
                }
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

        protected override IList<GetChildrenReferenceResult> LoadChildrenReferencesAndTypes(ContentReference contentLink, string languageId, out bool languageSpecific)
        {
            var videoTypeId = _contentTypeRepository.Load(typeof(Video));
            var videoFolderTypeId = _contentTypeRepository.Load(typeof(VideoFolder));
            languageSpecific = false;
            if (contentLink.CompareToIgnoreWorkID(EntryPoint))
            {

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
                    newVersion.ContentLink = new ContentReference(int.Parse(newVersion.Id), 0, ProviderKey);
                    _intermediateVideoDataRepository.Update(newVersion);
                    VideoSynchronizationEventHandler.DataStoreUpdated();

                    // In LoadContent we check for the work id of IContent if changes has been done (work id > 0). If the id > 0
                    // then we set the status to CheckedOut which will trigger the publish-button to be active. When we do the publish, 
                    // we have to tell LoadContent that the work id == 0 so the publish-button will be inactive.
                    return newVersion.ContentLink;
                }
                
                //If we saved an item that already has a specific version, just return the current version...
                _items.RemoveAt(_items.FindIndex(x => x.ContentLink.Equals(newVersion.ContentLink))); 
                _items.Add(newVersion);
                return content.ContentLink;
            }

            // ...otherwise save the content (which has been made a copy of the UI) to the local repository.
            if (newVersion != null)
            {
                newVersion.Status = VersionStatus.CheckedOut;
                newVersion.ContentLink = new ContentReference(int.Parse(newVersion.Id), 1, ProviderKey);
            }
            // New content
            else
            {
                var mediaData = content as MediaData;
                if (mediaData != null && mediaData.BinaryData != null)
                {
                    Blob blobData = ((MediaData)content).BinaryData;
          
                        int? videoId = TwentyThreeVideoRepository.UploadVideo(content.Name, blobData, content.ParentLink.ID);
                        if (videoId != null)
                        {
                            BlobFactory.Instance.Delete((content as MediaData).BinaryData.ID);
                            var video = GetDefaultContent(LoadContent(content.ParentLink, LanguageSelector.AutoDetect()),
                                        _contentTypeRepository.Load<Video>().ID, LanguageSelector.AutoDetect()) as Video;
                            var item = TwentyThreeVideoRepository.GetVideo((int)videoId);
                            helper.PopulateVideo(video, item);
                            _items.Add(video);
                            _intermediateVideoDataRepository.Update(video);

                            return video.ContentLink;
                        }
                    
                }
            }
            _items.Add(newVersion);
            return newVersion.ContentLink;
        }


        public void RefreshItems(List<BasicContent> items)
        {
            _items = items;
            this.ClearProviderPagesFromCache();
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

    }
}