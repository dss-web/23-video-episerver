using System.Collections.Generic;

namespace Visual
{
	/// <summary>
	/// Service providing access to the user interface
	/// </summary>
    public interface IUserService
    {
		/// <summary>
		/// Get a user from its user ID
		/// </summary>
		/// <param name='userId'>
		/// Unique user ID
		/// </param>
        Domain.User Get(int userId);
		
		/// <summary>
		/// Get a list of users
		/// </summary>
		/// <returns>
		/// List of users
		/// </returns>
        List<Domain.User> GetList();
		
		/// <summary>
		/// Get a list of users filtered by specific parameters
		/// </summary>
		/// <returns>
		/// List of users
		/// </returns>
		/// <param name='requestParameters'>
		/// Filtering parameters
		/// </param>
        List<Domain.User> GetList(UserListParameters requestParameters);
		
		/// <summary>
		/// Create a user from the given information
		/// </summary>
		/// <param name='email'>
		/// E-mail address
		/// </param>
		/// <param name='username'>
		/// Username
		/// </param>
		/// <param name='password'>
		/// Password
		/// </param>
		/// <param name='fullName'>
		/// Full name
		/// </param>
		/// <param name='timezone'>
		/// Timezone of the user
		/// </param>
		/// <param name='siteAdmin'>
 		/// Is the user an administrator?
		/// </param>
        int? Create(string email, string username = null, string password = null, string fullName = null, Timezone timezone = Timezone.CET, bool siteAdmin = false);
		
		/// <summary>
		/// Get a login token for a specific user
		/// </summary>
		/// <returns>
		/// The login token
		/// </returns>
		/// <param name='userId'>
		/// User ID
		/// </param>
		/// <param name='returnUrl'>
		/// URL where the user should be redirected to after login has succeeded
		/// </param>
        Domain.Session GetLoginToken(int userId, string returnUrl = "/");

        /// <summary>
        /// Update a user with the given information
        /// </summary>
        /// <returns>
        /// True upon success, null on failure
        /// </returns>
        /// <param name='email'>
        /// E-mail address
        /// </param>
        /// <param name='username'>
        /// Username
        /// </param>
        /// <param name='password'>
        /// Password
        /// </param>
        /// <param name='fullName'>
        /// Full name
        /// </param>
        /// <param name='timezone'>
        /// Timezone of the user
        /// </param>
        /// <param name='siteAdmin'>
        /// Is the user an administrator?
        /// </param>
        /// <throws>
        /// PermissionDenied exception
        /// </throws>
        bool? Update(int userId, string email = null, string username = null, string password = null, string fullName = null, Timezone? timezone = null, bool siteAdmin = false);
    }
}