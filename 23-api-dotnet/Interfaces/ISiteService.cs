namespace Visual
{
	/// <summary>
	/// Service providing access to the site interface
	/// </summary>
    public interface ISiteService
    {
		/// <summary>
		/// Get information for the specific site
		/// </summary>
        Domain.Site Get();
    }
}