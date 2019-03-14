namespace Visual
{
	/// <summary>
	/// Parameters for getting a list of comments
	/// </summary>
    public class CommentListParameters
    {
		/// <summary>
		/// Limit to a specific object ID
		/// </summary>
        public int? ObjectId = null;
		
		/// <summary>
		/// Limit to a specific object type
		/// </summary>
        public CommentObjectType ObjectType = CommentObjectType.Empty;
		
		/// <summary>
		/// Limit to a specific comment ID
		/// </summary>
        public int? CommentId = null;
		
		/// <summary>
		/// Limit to comments from a specific user ID
		/// </summary>
        public int? CommentUserId = null;
		
		/// <summary>
		/// Search string
		/// </summary>
        public string Search = null;
		
		/// <summary>
		/// Ordering of the comment list
		/// </summary>
        public GenericSort Order = GenericSort.Descending;
		
		/// <summary>
		/// Page offset
		/// </summary>
        public int? PageOffset = null;
	
		/// <summary>
		/// Maximum number of comments to return
		/// </summary>
        public int? Size = null;
    }
}