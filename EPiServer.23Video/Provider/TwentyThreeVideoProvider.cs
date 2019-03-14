/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */
using System;
using System.Collections.Generic;
using System.Linq;
using EPiCode.TwentyThreeVideo.Data;
using EPiCode.TwentyThreeVideo.Models;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
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
            get { return ContentProviderCapabilities.Create | ContentProviderCapabilities.Edit | ContentProviderCapabilities.Security; }
        }

        protected override IContent LoadContent(ContentReference contentLink, ILanguageSelector languageSelector)
        {
            var item = _items.FirstOrDefault(p => p.ContentLink.Equals(contentLink));
            if (item == null)
            {
                item = _items.FirstOrDefault(p => p.ContentLink.CompareToIgnoreWorkID(contentLink));
            }
            if (contentLink.WorkID > 0)
            {
                var video = item as Video;
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
                    try
                    {
                        int? videoId = TwentyThreeVideoRepository.UploadVideo(content.Name, blobData, content.ParentLink.ID);
                        if (videoId != null)
                        {

                            var video = GetDefaultContent(LoadContent(content.ParentLink, LanguageSelector.AutoDetect()),
                                        _contentTypeRepository.Load<Video>().ID, LanguageSelector.AutoDetect()) as Video;
                            var item = TwentyThreeVideoRepository.GetVideo((int)videoId);
                            helper.PopulateVideo(video, item);
                            _items.Add(video);
                            _intermediateVideoDataRepository.Update(video);
                            return video.ContentLink;
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error("An error occured while uploading and creating video object:" + ex);
                        throw;
                    }
                    finally
                    {
                        ServiceLocator.Current.GetInstance<IBlobFactory>().Delete((content as MediaData).BinaryData.ID);
                    }
                }
            }
            _items.Add(newVersion);
            return newVersion.ContentLink;
        }

        public void RefreshItems(List<BasicContent> items)
        {
            _items = items;
            ClearProviderPagesFromCache();
        }

        protected ContentResolveResult ResolveContent(BasicContent video)
        {
            var contentItem = new ContentCoreData()
            {
                ContentGuid = video.ContentGuid,
                ContentReference = video.ContentLink,
                ContentTypeID = _contentTypeRepository.Load(typeof(Video)).ID,
            };
            return base.CreateContentResolveResult(contentItem);
        }
    }
}