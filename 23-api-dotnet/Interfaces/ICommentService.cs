using System.Collections.Generic;

namespace Visual
{
	/// <summary>
	/// Service providing access to the comment interface
	/// </summary>
    public interface ICommentService
    {
		/// <summary>
		/// Get a list of comments
		/// </summary>
		/// <returns>
		/// A list of comments
		/// </returns>
        List<Domain.Comment> GetList();
		
		/// <summary>
		/// Get a filtered list of comments
		/// </summary>
		/// <returns>
		/// A list of comments
		/// </returns>
		/// <param name="requestParameters">
		/// Parameters to filter the comments by
		/// </param>
        List<Domain.Comment> GetList(CommentListParameters requestParameters);
    }
}