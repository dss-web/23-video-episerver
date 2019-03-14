using System.Collections.Generic;

namespace Visual
{
	/// <summary>
	/// Service providing access to the tag interface
	/// </summary>
    public interface ITagService
    {
		/// <summary>
		/// Get a list of tags
		/// </summary>
		/// <returns>
		/// List of tags
		/// </returns>
        List<Domain.Tag> GetList();
		
		/// <summary>
		/// Get a filtered list of tags
		/// </summary>
		/// <returns>
		/// List of tags
		/// </returns>
		/// <param name='requestParameters'>
		/// Filtering parameters
		/// </param>
        List<Domain.Tag> GetList(TagListParameters requestParameters);
		
		/// <summary>
		/// Get a list of tags related to a specific tag
		/// </summary>
		/// <returns>
		/// List of related tags
		/// </returns>
		/// <param name='name'>
		/// Name of the tag to which the results should relate
		/// </param>
        List<Domain.Tag> GetRelatedList(string name);
    }
}