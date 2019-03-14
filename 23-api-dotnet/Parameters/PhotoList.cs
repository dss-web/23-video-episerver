using System.Collections.Generic;
namespace Visual
{
	/// <summary>
	/// Sorting for the photo list
	/// </summary>
    public enum PhotoListSort
    {
		/// <summary>
		/// Sort by the number of views
		/// </summary>
        [RequestValue("views")]
        Views,
		
		/// <summary>
		/// Sort by the number of comments
		/// </summary>
        [RequestValue("comments")]
        Comments,
		
		/// <summary>
		/// Sort by when the photo was taken
		/// </summary>
        [RequestValue("taken")]
        Taken,
		
		/// <summary>
		/// Sort by the title
		/// </summary>
        [RequestValue("title")]
        Title,
		
		/// <summary>
		/// Sort by the description
		/// </summary>
        [RequestValue("words")]
        Words,
		
		/// <summary>
		/// Sort by the rating
		/// </summary>
        [RequestValue("rating")]
        Rating,
		
		/// <summary>
		/// Sort by the time of creation
		/// </summary>
        [RequestValue("created")]
        Created,
		
		/// <summary>
		/// Sort by the time the photo was uploaded
		/// </summary>
        [RequestValue("uploaded")]
        Uploaded,
		
		/// <summary>
		/// Sort by the publishing time
		/// </summary>
        [RequestValue("published")]
        Published
    }
	
	/// <summary>
	/// Mode for handling photot filtering by tags
	/// </summary>
    public enum PhotoTagMode
    {
		/// <summary>
		/// Match photos that have any of the given tags
		/// </summary>
        [RequestValue("any")]
        Any,
		
		/// <summary>
		/// Match photos that have all of the given tags
		/// </summary>
        [RequestValue("and")]
        And
    }
	
	/// <summary>
	/// Parameters for listing photos
	/// </summary>
    public class PhotoListParameters
    {
		/// <summary>
		/// Limit to a specific album ID
		/// </summary>
        public int? AlbumId = null;
		
		/// <summary>
		/// Limit to a specific photo ID
		/// </summary>
        public int? PhotoId = null;
		
		/// <summary>
		/// Limit to a specific user ID
		/// </summary>
        public int? UserId = null;
		
		/// <summary>
		/// Limit to a specific player ID
		/// </summary>
        public int? PlayerId = null;
		
		/// <summary>
		/// Search for a specific token
		/// </summary>
        public string Token = null;
		
		/// <summary>
		/// Tags to limit the photo list by
		/// </summary>
        public List<string> Tags = new List<string>();
		
		/// <summary>
		/// Mode for interpreting multiple tags
		/// </summary>
        public PhotoTagMode TagMode = PhotoTagMode.And;

		/// <summary>
		/// Search string
		/// </summary>
        public string Search = null;
		
		/// <summary>
		/// Limit to a specific year
		/// </summary>
        public int? Year = null;
		
		/// <summary>
		/// Limit to a specific month
		/// </summary>
        public int? Month = null;
		
		/// <summary>
		/// Limit to a specific day
		/// </summary>
        public int? Day = null;
		
		/// <summary>
		/// Limit to objects with video
		/// </summary>
        public bool? Video = null;
		
		/// <summary>
		/// Limit to objects with audio
		/// </summary>
        public bool? Audio = null;
		
		/// <summary>
		/// Limit to fully transcoded objects
		/// </summary>
        public bool? VideoEncoded = null;
		
		/// <summary>
		/// Include unpublished photos
		/// </summary>
        public bool IncludeUnpublished = false;

        /// <summary>
        /// Prioritize promoted videos
        /// </summary>
		public bool PrioritizePromoted = true;

		/// <summary>
		/// Property to order the photo list by
		/// </summary>
        public PhotoListSort OrderBy = PhotoListSort.Published;
		
		/// <summary>
		/// Order of the photo list
		/// </summary>
        public GenericSort Order = GenericSort.Descending;
		
		/// <summary>
		/// Page offset
		/// </summary>
        public int? PageOffset = null;
		
		/// <summary>
		/// Maximum number of photos to return
		/// </summary>
        public int? Size = null;
    }
}