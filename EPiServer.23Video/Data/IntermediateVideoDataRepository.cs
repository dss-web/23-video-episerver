/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPiCode.TwentyThreeVideo.Models;
using EPiCode.TwentyThreeVideo.Provider;
using EPiServer;
using EPiServer.Construction;
using EPiServer.Core;
using EPiServer.Data.Dynamic;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using log4net;
using Newtonsoft.Json;
using Visual.Domain;

namespace EPiCode.TwentyThreeVideo.Data
{
    public class IntermediateVideoDataRepository
    {
        private static ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected Injected<IContentRepository> ContentRepositry { get; set; }
        protected Injected<ContentFactory> ContentFactory { get; set; }
        protected Injected<IContentTypeRepository> ContentTypeRepository { get; set; }

        public static DynamicDataStore VideoContentDataModelStore
        {
            get
            {
                return DynamicDataStoreFactory.Instance.CreateStore(typeof(VideoContentDataModel));
            }
        }

        public void Save(IEnumerable<BasicContent> contents)
        {
            var simpleContent = ConvertFromBasicContent(contents);
            string serializedContents = JsonConvert.SerializeObject(simpleContent);
            var store = VideoContentDataModelStore;
            store.Save(new VideoContentDataModel()
            {
                SerializedContent = serializedContents,
                LastUpdated = DateTime.Now
            });
        }

        public List<BasicContent> Load()
        {
            var store = VideoContentDataModelStore;
            var videocontentDatamodel = store.Load<VideoContentDataModel>(VideoContentDataModelId);

            if (videocontentDatamodel != null)
            {
                string serializedContents = videocontentDatamodel.SerializedContent;
                var contents = JsonConvert.DeserializeObject<List<IntermediateVideoDataModel>>(serializedContents);
                return ConvertToBasicContent(contents).ToList();
            }

            return null;
        }

        public Guid VideoContentDataModelId
        {
            get
            {
                return new Guid(Constants.VideoContentDataModel);
            }
        }

        public List<BasicContent> Update(BasicContent content)
        {
            var items = Load();
            var existingItem = items.FirstOrDefault(x => x.ContentLink.CompareToIgnoreWorkID(content.ContentLink));
            if (existingItem != null)
            {
                items.Remove(existingItem);
            }
            items.Add(content);
            Save(items);
            return items;
        }

        public List<BasicContent> Refresh()
        {
            var items = LoadFromService();
            Save(items);
            return items;
        }

        public List<BasicContent> LoadFromService()
        {

            var entryPoint = ContentRepositry.Service.GetChildren<VideoFolder>(ContentReference.RootPage).FirstOrDefault();
            var videoFolders = CreateFoldersFromChannels(entryPoint).ToList();
            var videoContentList = videoFolders.ToList();
            var videoHelper = new VideoHelper();
            
            Parallel.ForEach(videoFolders, folder =>
            {                            
                if (folder is VideoFolder)
                {
                    var videos = TwentyThreeVideoRepository.GetVideoList(folder.ContentLink.ID);
                    foreach (var videoData in videos)
                    {
                        var type = ContentTypeRepository.Service.Load<Video>();
                        var video =
                            ContentFactory.Service.CreateContent(type, new BuildingContext(type)
                            {
                                Parent = folder,
                                LanguageSelector = LanguageSelector.AutoDetect(),
                                SetPropertyValues = true
                            }) as Video;
                        if (video != null)
                        {
                            _log.DebugFormat("23Video: Added video with name {0}", video.Name);
                            videoHelper.PopulateVideo(video, videoData);

                            videoContentList.Add(video);
                        }
                        else
                        {
                            _log.InfoFormat(
                                "23Video: Video from 23Video can not be loaded in EPiServer as Video. Videoname from 23Video {0}",
                                videoData.One);
                        }
                    }
                }
           
            }
            );
            return videoContentList;
        }

        public IEnumerable<IntermediateVideoDataModel> ConvertFromBasicContent(IEnumerable<BasicContent> contents)
        {
            foreach (var basicContent in contents)
            {
                var model = new IntermediateVideoDataModel()
                {
                    ContentLink = basicContent.ContentLink,
                    Guid = basicContent.ContentGuid,
                    ParentLink = basicContent.ParentLink,
                    Name = basicContent.Name
                };
                if (basicContent is Video)
                {
                    var video = basicContent as Video;
                    model.Thumbnail = video.Thumbnail;
                    model.Binarydata = video.BinaryData ?? video.Thumbnail;
                    model.oEmbedHtml = video.oEmbedHtml;
                    model.oEmbedVideoName = video.oEmbedVideoName;
                    model.VideoUrl = video.VideoUrl;
                    model.Id = video.Id;
                    model.VideoContentType = VideoContentType.Video;
                    yield return model;
                }
                else if(basicContent is VideoFolder)
                {
                    var videoFolder = basicContent as VideoFolder;
                    model.EditorGroup = videoFolder.EditorGroup;
                    model.VideoContentType = VideoContentType.VideoFolder;
                    yield return model;
                }
            }
        }

        private IEnumerable<BasicContent> ConvertToBasicContent(IEnumerable<IntermediateVideoDataModel> dataModels)
        {
            var helper = new VideoHelper();
            var entryPoint = ContentRepositry.Service.GetChildren<VideoFolder>(ContentReference.RootPage).FirstOrDefault();
            var videoType = ContentTypeRepository.Service.Load<Video>();
            var videoFolderType = ContentTypeRepository.Service.Load<VideoFolder>();
            var folders = new List<VideoFolder>();
            var rawcontents = dataModels as IList<IntermediateVideoDataModel> ?? dataModels.ToList();
            foreach (IntermediateVideoDataModel dataModel in rawcontents)
            {
                if (dataModel.VideoContentType == VideoContentType.VideoFolder)
                {
                    var folder = ContentFactory.Service.CreateContent(videoFolderType, new BuildingContext(videoFolderType)
                              {
                                  Parent = entryPoint,
                                  LanguageSelector = LanguageSelector.AutoDetect(),
                                  SetPropertyValues = true
                              }) as VideoFolder;
                    folder.Name = dataModel.Name;
                    folder.EditorGroup = dataModel.EditorGroup;
                    folder.ContentLink = dataModel.ContentLink;
                    folder.ContentGuid = dataModel.Guid;
                    folders.Add(folder);
                    yield return folder;
                }
                else
                {
                    var video = ContentFactory.Service.CreateContent(videoType, new BuildingContext(videoType)
                              {
                                  Parent = folders.FirstOrDefault(x => x.ContentLink == dataModel.ParentLink),
                                  LanguageSelector = LanguageSelector.AutoDetect(),
                                  SetPropertyValues = true
                              }) as Video;
                    video.Id = dataModel.Id;
                    video.ContentLink = dataModel.ContentLink;
                    video.Name = dataModel.Name;
                    video.oEmbedHtml = dataModel.oEmbedHtml;
                    video.oEmbedVideoName = dataModel.oEmbedVideoName;
                    video.VideoUrl = dataModel.VideoUrl;
                    video.Thumbnail = dataModel.Thumbnail;
                    video.BinaryData = dataModel.Binarydata;
                    video.ContentGuid = dataModel.Guid;
                    helper.PopulateStandardVideoProperties(video);
                    yield return video;
                }
            }
        }

        private IEnumerable<BasicContent> CreateFoldersFromChannels(VideoFolder entryPoint)
        {
            var type = ContentTypeRepository.Service.Load<VideoFolder>();
            List<Album> albums = TwentyThreeVideoRepository.GetChannelList();
            foreach (var album in albums)
            {
                var folder = ContentFactory.Service.CreateContent(type, new BuildingContext(type)
                {
                    Parent = entryPoint,
                    LanguageSelector = LanguageSelector.AutoDetect(),
                    SetPropertyValues = true
                }) as VideoFolder;
                if (folder == null) continue;                
                if (album.AlbumId != null)
                {
                    var id = (int)album.AlbumId;
                    folder.ContentLink = new ContentReference(id, Constants.ProviderKey);
                    folder.Name = album.Title;

                    var editorGroup = new EditorGroupChannelMappingRepository().GetEditorGroupForChannel(folder.Name);
                    if(!string.IsNullOrWhiteSpace(editorGroup))
                        folder.EditorGroup = editorGroup;
                    
                    _log.InfoFormat("23Video: Channel {0} created.", album.Title);
                    yield return folder;
                }
            }
        }
    }
}
