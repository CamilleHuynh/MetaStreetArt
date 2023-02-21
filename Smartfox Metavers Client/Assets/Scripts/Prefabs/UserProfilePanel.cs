using UnityEngine;
using UnityEngine.UI;

/**
 * Script attached to User Profile Panel prefab.
 */
public class UserProfilePanel : BasePanel
{
	public Text usernameLabel;

	/**
	 * Show the generic user profile details.
	 */
	public void InitUserProfile(string username)
	{
		// Username
		usernameLabel.text = "<b>Username:</b> " + username;
	}
}
