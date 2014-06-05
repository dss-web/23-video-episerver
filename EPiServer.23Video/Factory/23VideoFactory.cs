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
            List<Photo> photos = GetPhotos(apiProvider);

            return photos;

        }

        private static List<Photo> GetPhotos(IApiProvider apiProvider)
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
