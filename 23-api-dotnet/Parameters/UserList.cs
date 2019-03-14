namespace Visual
{
	/// <summary>
	/// Sorting for the list of users
	/// </summary>
    public enum UserListSort
    {
		/// <summary>
		/// Sort by username
		/// </summary>
        [RequestValue("username")]
        Username,
		
		/// <summary>
		/// Sort by administrative privileges
		/// </summary>
        [RequestValue("site_admin_p")]
        SiteAdmin,
		
		/// <summary>
		/// Sort by e-mail address
		/// </summary>
        [RequestValue("email")]
        Email,
		
		/// <summary>
		/// Sort by creation date
		/// </summary>
        [RequestValue("creation_date")]
        CreationDate,
		
		/// <summary>
		/// Sort by time of last login
		/// </summary>
        [RequestValue("last_login")]
        LastLogin,
		
		/// <summary>
		/// Sort by display name
		/// </summary>
        [RequestValue("display_name")]
        DisplayName
    }
	
	/// <summary>
	/// Parameters for retrieving a list of users
	/// </summary>
    public class UserListParameters
    {
		/// <summary>
		/// User ID to limit the list to
		/// </summary>
        public int? UserId = null;
		
		/// <summary>
		/// Search string
		/// </summary>
        public string Search = null;
		
		/// <summary>
		/// Property to order the user list by
		/// </summary>
        public UserListSort OrderBy = UserListSort.DisplayName;
		
		/// <summary>
		/// Order of the user list
		/// </summary>
        public GenericSort Order = GenericSort.Descending;
		
		/// <summary>
		/// Page offset
		/// </summary>
        public int? PageOffset = null;
		
		/// <summary>
		/// Maximum number of users to return
		/// </summary>
        public int? Size = null;
    }
}