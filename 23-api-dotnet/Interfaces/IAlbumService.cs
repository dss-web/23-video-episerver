using System.Collections.Generic;

namespace Visual
{
	/// <summary>
	/// Service providing access to the album interface
	/// </summary>
    public interface IAlbumService
    {
		/// <summary>
		/// Get a specific album by its ID
		/// </summary>
		/// <param name="albumId">
		/// Album ID
		/// </param>
        Domain.Album Get(int albumId);
		
		/// <summary>
		/// Get a list of albums
		/// </summary>
		/// <returns>
		/// A list of albums
		/// </returns>
        List<Domain.Album> GetList();
		
		/// <summary>
		/// Get a list of albums filtered by specific parameters
		/// </summary>
		/// <returns>
		/// A list of albums
		/// </returns>
		/// <param name="requestParameters">
		/// Request parameters to filter the list by
		/// </param>
        List<Domain.Album> GetList(AlbumListParameters requestParameters);
		
		/// <summary>
		/// Create an album
		/// </summary>
		/// <param name="title">
		/// Title of the album
		/// </param>
		/// <param name="description">
		/// Description of the album
		/// </param>
		/// <param name="hide">
		/// Visibility of the album
		/// </param>
		/// <param name="userId">
		/// User ID that created the album
		/// </param>
		int? Create(string title, string description = "", bool hide = false, int? userId = null);
		
		/// <summary>
		/// Update an album
		/// </summary>
		/// <param name="albumId">
		/// ID of the album to be updated
		/// </param>
		/// <param name="title">
		/// Title of the updated album
		/// </param>
		/// <param name="description">
		/// Description of the updated album
		/// </param>
		/// <param name="hide">
		/// Visibility of the updated album
		/// </param>
        bool Update(int albumId, string title, string description = "", bool hide = false);
		
		/// <summary>
		/// Delete an album
		/// </summary>
		/// <param name="albumId">
		/// ID of the album to be deleted
		/// </param>
        bool Delete(int albumId);
    }
}