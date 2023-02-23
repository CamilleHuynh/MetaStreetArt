using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Util;
using Sfs2X.Requests.MMO;

public class LoginSceneController : BaseSceneController
{
	//----------------------------------------------------------
	// Serialized Fields
	//----------------------------------------------------------

	[SerializeField] private ServerConnectionData m_serverConnectionData;
	
	[Header("UI Elements")]
	[SerializeField] private InputField ipInput;
	[SerializeField] private InputField nameInput;
	[SerializeField] private Button loginButton;
	[SerializeField] private Text errorText;

	//----------------------------------------------------------
	// Private properties
	//----------------------------------------------------------

	private SmartFox sfs;

	//----------------------------------------------------------
	// Unity callback methods
	//----------------------------------------------------------

	private void Start()
	{
		// Focus
		ipInput.Select();
		nameInput.ActivateInputField();
		ipInput.ActivateInputField();

		// Show connection lost message, in case the disconnection occurred in another scene
		string connLostMsg = gm.ConnectionLostMessage;
		if (connLostMsg != null)
			errorText.text = connLostMsg;
	}

	//----------------------------------------------------------
	// UI event listeners
	//----------------------------------------------------------
	#region UI Events

	public void OnIpInputEndEdit(String input)
	{
		if (input != "")
			m_serverConnectionData.Host = input;
	}


	/**
	 * On Login button click, connect to SmartFoxServer.
	 */
	public void OnLoginButtonClick()
	{
		Connect();
	}
	
	#endregion

	//----------------------------------------------------------
	// Helper methods
	//----------------------------------------------------------
	#region Helper methods
	
	/**
	 * Enable/disable username input interaction.
	 */
	private void EnableUI(bool enable)
	{
		ipInput.interactable = enable;
		nameInput.interactable = enable;
		loginButton.interactable = enable;
	}

	/**
	 * Connect to SmartFoxServer.
	 */
	private void Connect()
	{
		// Disable user interface
		EnableUI(false);

		// Clear any previour error message
		errorText.text = "";

		// Set connection parameters
		ConfigData cfg = new ConfigData();
		cfg.Host = m_serverConnectionData.Host;
		cfg.Port = m_serverConnectionData.TcpPort;
		cfg.Zone = m_serverConnectionData.ZoneName;
		cfg.Debug = m_serverConnectionData.Debug;

#if UNITY_WEBGL
		cfg.Port = httpPort;
#endif

		// Initialize SmartFox client
		// The singleton class GlobalManager holds a reference to the SmartFox class instance,
		// so that it can be shared among all the scenes
#if !UNITY_WEBGL
		sfs = gm.CreateSfsClient();
#else
		sfs = gm.CreateSfsClient(UseWebSocket.WS_BIN);
#endif

		// Configure SmartFox internal logger
		sfs.Logger.EnableConsoleTrace = m_serverConnectionData.Debug;

		// Add event listeners
		AddSmartFoxListeners();

		// Connect to SmartFoxServer
		sfs.Connect(cfg);
	}

	/**
	 * Add all SmartFoxServer-related event listeners required by the scene.
	 */
	private void AddSmartFoxListeners()
	{
		sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
		sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
		sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
	}

	/**
	 * Remove all SmartFoxServer-related event listeners added by the scene.
	 * This method is called by the parent BaseSceneController.OnDestroy method when the scene is destroyed.
	 */
	protected override void RemoveSmartFoxListeners()
	{
		// NOTE
		// If this scene is stopped before a connection is established, the SmartFox client instance
        // could still be null, causing an error when trying to remove its listeners

		if (sfs != null)
		{
			sfs.RemoveEventListener(SFSEvent.CONNECTION, OnConnection);
			sfs.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
			sfs.RemoveEventListener(SFSEvent.LOGIN, OnLogin);
			sfs.RemoveEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
		}
	}

	/**
	 * Hide all modal panels.
	 */
	protected override void HideModals()
	{
		// No modals used by this scene
	}
	
	#endregion

	//----------------------------------------------------------
	// SmartFoxServer event listeners
	//----------------------------------------------------------
	#region Event listeners
	
	private void OnConnection(BaseEvent evt)
	{
		// Check if the conenction was established or not
		if ((bool)evt.Params["success"])
		{
			Debug.Log("SFS2X API version: " + sfs.Version);
			Debug.Log("Connection mode is: " + sfs.ConnectionMode);

			// Login
			sfs.Send(new LoginRequest(nameInput.text));
		}
		else
		{
			// Show error message
			errorText.text = "Connection failed; is the server running at all?";

			// Enable user interface
			EnableUI(true);
		}
	}

	private void OnConnectionLost(BaseEvent evt)
	{
		// Remove SFS listeners
		RemoveSmartFoxListeners();

		// Show error message
		string reason = (string)evt.Params["reason"];
		
		if (reason != ClientDisconnectionReason.MANUAL)
			errorText.text = "Connection lost; reason is: " + reason;

		// Enable user interface
		EnableUI(true);
	}

	private void OnLogin(BaseEvent evt)
	{
		SceneManager.LoadScene("Lobby");
    }

	private void OnLoginError(BaseEvent evt)
	{
		// Disconnect
		// NOTE: this causes a CONNECTION_LOST event with reason "manual", which in turn removes all SFS listeners
		sfs.Disconnect();

		// Show error message
		errorText.text = "Login failed due to the following error:\n" + (string)evt.Params["errorMessage"];

		// Enable user interface
		EnableUI(true);
	}
	
	#endregion
}
