using System.Collections.Generic;

namespace Visual
{
	/// <summary>
	/// Service providing access to the player interface
	/// </summary>
    public interface IPlayerService
    {
		/// <summary>
		/// Get a list of all available players on a site
		/// </summary>
		/// <returns>
		/// A list of players
		/// </returns>
        List<Domain.Player> GetList();
    }
}