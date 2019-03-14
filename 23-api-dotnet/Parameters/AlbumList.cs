namespace Visual
{
	/// <summary>
	/// Sorting of the album list
	/// </summary>
    public enum AlbumListSort
    {
		/// <summary>
		/// Sort by the ordering of the list
		/// </summary>
        [RequestValue("sortkey")]
        SortKey,
		
		/// <summary>
		/// Sort by title
		/// </summary>
        [RequestValue("title")]
        Title,
		
		/// <summary>
		/// Sort by the time of the last edit
		/// </summary>
        [RequestValue("editing_date")]
        EditingDate,
		
		/// <summary>
		/// Sort by the time of creation
		/// </summary>
        [RequestValue("creation_date")]
        CreationDate
    }
	
	/// <summary>
	/// Parameters for getting a list of albums
	/// </summary>
    public class AlbumListParameters
    {
		/// <summary>
		/// Limit the list to a specific album ID
		/// </summary>
        public int? AlbumId = null;
		
		/// <summary>
		/// Limit the list to albums created by a specific user ID
		/// </summary>
        public int? UserId = null;
		
		/// <summary>
		/// Limit the list to channels containing a specific photo ID
		/// </summary>
        public int? PhotoId = null;
		
		/// <summary>
		/// Search string
		/// </summary>
        public string Search = null;
		
		/// <summary>
		/// Include hidden channels
		/// </summary>
        public bool IncludeHidden = false;
		
		/// <summary>
		/// Property to order the list of albums by
		/// </summary>
        public AlbumListSort OrderBy = AlbumListSort.CreationDate;
		
		/// <summary>
		/// Ordering of the albums
		/// </summary>
        public GenericSort Order = GenericSort.Descending;
		
		/// <summary>
		/// Page offset
		/// </summary>
        public int? PageOffset = null;
		
		/// <summary>
		/// Maximum number of albums to return
		/// </summary>
        public int? Size = null;
    }
}