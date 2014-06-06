using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visual;
using Visual.Domain;

namespace EPiServer._23Video.Factory
{
    public static class _23VideoFactory
    {
        public static List<Photo> GetPhotoList()
        {
            // Check that we actually have everything configured
            IApiProvider apiProvider = _23Client.ApiProvider;

            // Get a list of videos to throw in there
            List<Photo> photos = GetPhotos(apiProvider, null);

            return photos;

        }

        public static List<Photo> GetPhotoList(int channelId)
        {
            // Check that we actually have everything configured
            IApiProvider apiProvider = _23Client.ApiProvider;

            // Get a list of videos to throw in there
            List<Photo> photos = GetPhotos(apiProvider, new PhotoListParameters(){AlbumId = channelId});

            return photos;

        }
        
        private static List<Photo> GetPhotos(IApiProvider apiProvider, PhotoListParameters photoListParameters)
        {
            const string cacheKey = "PhotoService";
            //if (Caching.VideoProviderCache.Instance.InnerCache.ContainsKey(cacheKey))
            //{
            //    return Caching.VideoProviderCache.Instance.InnerCache.GetValue(cacheKey) as List<Photo>;
            //}

            List<Photo> result = new List<Photo>();
            IPhotoService photoService = new PhotoService(apiProvider);

            PhotoListParameters p = photoListParameters;
            
            if (photoListParameters == null)
            {
                p = new PhotoListParameters();
            }
            
            p.IncludeUnpublished = false;
            p.Video = true;

            List<Photo> photos = photoService.GetList(p);

            
            //Caching.VideoProviderCache.Instance.InnerCache.Add(cacheKey, result, result.Count, TimeSpan.FromMinutes(10));
            return photos;
        }

        public static List<Album> GetAlbumList()
        {
            // Check that we actually have everything configured
            IApiProvider apiProvider = _23Client.ApiProvider;

            // Get a list of videos to throw in there
            List<Album> albums = GetAlbums(apiProvider);

            return albums;

        }

        private static List<Album> GetAlbums(IApiProvider apiProvider)
        {
            IAlbumService albumService = new AlbumService(apiProvider);

            return albumService.GetList();
        }

    }
}
