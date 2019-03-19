namespace Visual
{
	/// <summary>
	/// Sorting for the tag list
	/// </summary>
    public enum TagListSort
    {
		/// <summary>
		/// Sort by tag name
		/// </summary>
        [RequestValue("tag")]
        Tag,
		
		/// <summary>
		/// Sort by the number of times a tag is used
		/// </summary>
        [RequestValue("count")]
        Count
    }

	/// <summary>
	/// Parameters for listing tags
	/// </summary>
    public class TagListParameters
    {
		/// <summary>
		/// Search string to use. Set to null to exclude
		/// </summary>
        public string Search = null;
		
		/// <summary>
		/// Reformat tags to be humanly readable
		/// </summary>
        public bool ReformatTags = false;
		
		/// <summary>
		/// Exclude machine tags from the result
		/// </summary>
        public bool ExcludeMachineTags = true;
		
		/// <summary>
		/// Ordering property of the returned tag list
		/// </summary>
        public TagListSort OrderBy = TagListSort.Tag;
		
		/// <summary>
		/// Ordering of the returned tag list
		/// </summary>
        public GenericSort Order = GenericSort.Descending;
		
		/// <summary>
		/// Page offset
		/// </summary>
        public int? PageOffset = null;
		
		/// <summary>
		/// Maximum number of tags to return
		/// </summary>
        public int? Size = null;
    }
}