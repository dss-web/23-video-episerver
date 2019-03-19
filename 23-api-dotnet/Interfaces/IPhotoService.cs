using System.Collections.Generic;
using System.IO;

namespace Visual
{
	/// <summary>
	/// Service providing access to the photo interface
	/// </summary>
    public interface IPhotoService
    {
		/// <summary>
		/// Get data for a specific photo
		/// </summary>
		/// <param name="photoId">
		/// Unique photo ID
		/// </param>
		/// <param name="includeUnpublished">
		/// Include or exclude published videos from the result
		/// </param>
        /// <param name="token">
        /// The token for the photo (in the case of hidden photos)
        /// </param>
        Domain.Photo Get(int photoId, bool includeUnpublished = false, string token = null);
		
		/// <summary>
		/// Gets a list of photos
		/// </summary>
		/// <returns>
		/// List of photos
		/// </returns>
        List<Domain.Photo> GetList();
		
		/// <summary>
		/// Gets a list of photos filtered by specific parameters
		/// </summary>
		/// <returns>
		/// List of photos
		/// </returns>
		/// <param name="requestParameters">
		/// Filtering parameters for the list
		/// </param>
        List<Domain.Photo> GetList(PhotoListParameters requestParameters);
		
		/// <summary>
		/// Upload the specified filename, fileContentType, filestream, userId, albumId, title, description, tags and publish.
		/// </summary>
		/// <param name="filename">
		/// Original file name of the file
		/// </param>
		/// <param name="fileContentType">
		/// Content type of the file
		/// </param>
		/// <param name="filestream">
		/// Stream to the contents of the file
		/// </param>
		/// <param name="userId">
		/// ID of the user uploading the video
		/// </param>
		/// <param name="albumId">
		/// Unique album ID
		/// </param>
		/// <param name="title">
		/// Title of the video
		/// </param>
		/// <param name="description">
		/// Description of the video
		/// </param>
		/// <param name="tags">
		/// Tags for the video
		/// </param>
		/// <param name="publish">
		/// Publish after uploading
		/// </param>
        int? Upload(string filename, string fileContentType, System.IO.Stream filestream, int? userId = null, int? albumId = null, string title = null, string description = null, string tags = null, bool? publish = null, Dictionary<string,string> variables = null);
		
		/// <summary>
		/// Delete the specified photo
		/// </summary>
		/// <param name="photoId">
		/// Unique photo ID
		/// </param>
        bool Delete(int photoId);
		
		/// <summary>
		/// Replace the specified photoId, filename, fileContentType and filestream.
		/// </summary>
		/// <param name="photoId">
		/// Unique photo ID to replace
		/// </param>
		/// <param name="filename">
		/// Original file name of the file
		/// </param>
		/// <param name="fileContentType">
		/// Content type of the file
		/// </param>
		/// <param name="filestream">
		/// Stream to the contents of the file
		/// </param>
        bool Replace(int photoId, string filename, string fileContentType, Stream filestream);
		
		/// <summary>
		/// Update the specific photo. Fields not to be updated should be null.
		/// </summary>
		/// <param name="photoId">
		/// Unique photo ID
		/// </param>
		/// <param name="albumId">
		/// Unique album ID
		/// </param>
		/// <param name="title">
		/// Title of the video
		/// </param>
		/// <param name="description">
		/// Description of the video
		/// </param>
		/// <param name="tags">
		/// Tags of the video
		/// </param>
		/// <param name="published">
		/// Published
		/// </param>
        bool Update(int photoId, int? albumId = null, string title = null, string description = null, string tags = null, bool? published = null, Dictionary<string,string> variables = null);
		
        Domain.PhotoUploadToken GetUploadToken(string returnUrl, bool? backgroundReturn, int? userId, int? albumId, string title, string description, string tags, bool? publish, int? validMinutes, int? maxUploads);
        bool RedeemUploadToken(string filename, string fileContentType, System.IO.Stream filestream, string uploadToken);
    }
}