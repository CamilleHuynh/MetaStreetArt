using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Requests.MMO;

public class LobbySceneController : BaseSceneController
{
	[SerializeField] private ServerConnectionData m_serverConnectionData;

	//----------------------------------------------------------
	// UI elements
	//----------------------------------------------------------

	[Header("UI Elements")]
	[SerializeField] private Text loggedInAsLabel;
	[SerializeField] private UserProfilePanel userProfilePanel;
	[SerializeField] private WarningPanel warningPanel;
	[SerializeField] private Transform gameListContent;
	[SerializeField] private GameListItem gameListItemPrefab;

	//----------------------------------------------------------
	// Private properties
	//----------------------------------------------------------

	private SmartFox sfs;
	private Dictionary<int, GameListItem> gameListItems;

	//----------------------------------------------------------
	// Unity callback methods
	//----------------------------------------------------------

	private void Start()
	{
		// Set a reference to the SmartFox client instance
		sfs = gm.GetSfsClient();

		// Hide modal panels
		HideModals();

		// Display username in footer and user profile panel
		loggedInAsLabel.text = "Logged in as <b>" + sfs.MySelf.Name + "</b>";
		userProfilePanel.InitUserProfile(sfs.MySelf.Name);

		AddSmartFoxListeners();

		// We'll populate the room list after the subscription is done so we can see the rooms in the group
		sfs.Send(new SubscribeRoomGroupRequest(m_serverConnectionData.RoomGroup));
	}

	//----------------------------------------------------------
	// UI event listeners
	//----------------------------------------------------------
	#region UI Events

	/**
	 * On Logout button click, disconnect from SmartFoxServer.
	 * This causes the SmartFox listeners added by this scene to be removed (see BaseSceneController.OnDestroy method)
	 * and the Login scene to be loaded (see GlobalManager.OnConnectionLost method).
	 */
	public void OnLogoutButtonClick()
	{
		// Disconnect from SmartFoxServer
		sfs.Disconnect();
	}

	/**
	 * On Start game button click, create a new game Room.
	 */
	public void OnStartGameButtonClick()
	{
		MMORoomSettings roomSettings = new MMORoomSettings(sfs.MySelf.Name + "'s room");
		roomSettings.GroupId = m_serverConnectionData.RoomGroup;
		roomSettings.DefaultAOI = m_serverConnectionData.DefaultAOI;
		roomSettings.MapLimits = m_serverConnectionData.DefaultMapLimits;
		roomSettings.Extension = m_serverConnectionData.RoomExtension;



		sfs.Send(new CreateRoomRequest(roomSettings));
	}

	/**
	 * On Play game button click in Game List Item prefab instance, join an existing game Room as a player.
	 */
	public void OnGameItemPlayClick(int roomId)
	{
		// Join game Room as player
		sfs.Send(new Sfs2X.Requests.JoinRoomRequest(roomId));
	}

	/**
	 * On Watch game button click in Game List Item prefab instance, join an existing game Room as a spectator.
	 */
	public void OnGameItemWatchClick(int roomId)
	{
		// Join game Room as spectator
		sfs.Send(new Sfs2X.Requests.JoinRoomRequest(roomId, null, null, true));
	}

	/**
	 * On User icon click, show User Profile Panel prefab instance.
	 */
	public void OnUserIconClick()
	{
		userProfilePanel.Show();
	}

	#endregion

	//----------------------------------------------------------
	// Helper methods
	//----------------------------------------------------------
	#region Helper methods

	/**
	 * Add all SmartFoxServer-related event listeners required by the scene.
	 */
	private void AddSmartFoxListeners()
	{
		sfs.AddEventListener(SFSEvent.ROOM_CREATION_ERROR, OnRoomCreationError);
		sfs.AddEventListener(SFSEvent.ROOM_ADD, OnRoomAdded);
		sfs.AddEventListener(SFSEvent.ROOM_REMOVE, OnRoomRemoved);
		sfs.AddEventListener(SFSEvent.USER_COUNT_CHANGE, OnUserCountChanged);
		sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
		sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
		sfs.AddEventListener(SFSEvent.ROOM_GROUP_SUBSCRIBE, OnRoomGroupSubscribe);
		sfs.AddEventListener(SFSEvent.ROOM_GROUP_SUBSCRIBE_ERROR, OnRoomGroupSubscriptionError);
	}

	/**
	 * Remove all SmartFoxServer-related event listeners added by the scene.
	 * This method is called by the parent BaseSceneController.OnDestroy method when the scene is destroyed.
	 */
	protected override void RemoveSmartFoxListeners()
	{
		sfs.RemoveEventListener(SFSEvent.ROOM_CREATION_ERROR, OnRoomCreationError);
		sfs.RemoveEventListener(SFSEvent.ROOM_ADD, OnRoomAdded);
		sfs.RemoveEventListener(SFSEvent.ROOM_REMOVE, OnRoomRemoved);
		sfs.RemoveEventListener(SFSEvent.USER_COUNT_CHANGE, OnUserCountChanged);
		sfs.RemoveEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
		sfs.RemoveEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
		sfs.RemoveEventListener(SFSEvent.ROOM_GROUP_SUBSCRIBE, OnRoomGroupSubscribe);
		sfs.RemoveEventListener(SFSEvent.ROOM_GROUP_SUBSCRIBE_ERROR, OnRoomGroupSubscriptionError);
	}

	/**
	 * Hide all modal panels.
	 */
	protected override void HideModals()
	{
		userProfilePanel.Hide();
		warningPanel.Hide();
	}

	/**
	 * Display list of existing games.
	 */
	private void PopulateGamesList()
	{
		// Initialize list
		if (gameListItems == null)
			gameListItems = new Dictionary<int, GameListItem>();

		// For the game list we use a scrollable area containing a separate prefab for each Game Room
		// The prefab contains clickable buttons to join the game
		List<Room> rooms = sfs.RoomManager.GetRoomListFromGroup(m_serverConnectionData.RoomGroup);

		// Display game list items
		foreach (Room room in rooms)
		{
			AddGameListItem(room);
		}
	}

	/**
	 * Create Game List Item prefab instance and add to games list.
	 */
	private void AddGameListItem(Room room)
	{
		// Show only game rooms
		// Also password protected Rooms are skipped, to make this example simpler
		// (protection would require an interface element to input the password)
		if (room.IsHidden || room.IsPasswordProtected)
			return;

		// Create game list item
		GameListItem gameListItem = Instantiate(gameListItemPrefab);
		gameListItems.Add(room.Id, gameListItem);

		// Init game list item
		gameListItem.Init(room);

		// Add listener to play and watch buttons
		gameListItem.playButton.onClick.AddListener(() => OnGameItemPlayClick(room.Id));
		gameListItem.watchButton.onClick.AddListener(() => OnGameItemWatchClick(room.Id));

		// Add game list item to container
		gameListItem.gameObject.transform.SetParent(gameListContent, false);
	}

	#endregion

	//----------------------------------------------------------
	// SmartFoxServer event listeners
	//----------------------------------------------------------
	#region Listeners

	private void OnRoomCreationError(BaseEvent evt)
	{
		// Show Warning Panel prefab instance
		warningPanel.Show("Room creation failed: " + (string)evt.Params["errorMessage"]);
	}

	private void OnRoomAdded(BaseEvent evt)
	{
		Room room = (Room)evt.Params["room"];

		// Display game list item
		AddGameListItem(room);
	}

	public void OnRoomRemoved(BaseEvent evt)
	{
		Room room = (Room)evt.Params["room"];

		// Get reference to game list item corresponding to Room
		gameListItems.TryGetValue(room.Id, out GameListItem gameListItem);

		// Remove game list item
		if (gameListItem != null)
		{
			// Remove listeners
			gameListItem.playButton.onClick.RemoveAllListeners();

			// Remove game list item from dictionary
			gameListItems.Remove(room.Id);

			// Destroy game object
			Destroy(gameListItem.gameObject);
		}
	}

	public void OnUserCountChanged(BaseEvent evt)
	{
		Room room = (Room)evt.Params["room"];

		// Get reference to game list item corresponding to Room
		gameListItems.TryGetValue(room.Id, out GameListItem gameListItem);

		// Update game list item
		if (gameListItem != null)
			gameListItem.SetState(room);
	}

	private void OnRoomJoin(BaseEvent evt)
	{
		// Load game scene
		SceneManager.LoadScene("Game");
	}

	private void OnRoomJoinError(BaseEvent evt)
	{
		// Show Warning Panel prefab instance
		warningPanel.Show("Room join failed: " + (string)evt.Params["errorMessage"]);
	}

	private void OnRoomGroupSubscribe(BaseEvent evt)
	{
		Debug.Log("Room group subscribed");

		// Populate list of available games
		PopulateGamesList();
	}

	private void OnRoomGroupSubscriptionError(BaseEvent evt)
	{
		Debug.Log("Room group subscription failed : " + (string)evt.Params["errorMessage"]);
	}

	#endregion
}
