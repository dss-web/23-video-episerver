namespace Visual
{
	/// <summary>
	/// Service providing access to the session interface
	/// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// Get a signed session containing a signed token and a URL for redeeming with a specific relative or absolute path
        /// </summary>
        /// <param name="returnUrl">Specific (http://example.23video.com/...) or relative path (/...) on site</param>
        /// <returns>A Session object</returns>
        Domain.Session GetToken(string returnUrl = "/", string email = null, string fullname = null);
    }
}