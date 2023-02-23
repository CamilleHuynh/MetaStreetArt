using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameSceneController : BaseSceneController
{
    //----------------------------------------------------------
    // Serialized Fields
    //----------------------------------------------------------

    [Header("Linked Elements")] [SerializeField]
    private SettingsPanel settingsPanel;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject[] playerModels;
    [SerializeField] private Material[] playerMaterials;
    [SerializeField] private Collider terrainCollider;
    [SerializeField] private GameObject aoiPrefab;
    [SerializeField] private GameObject cubePrefab;

    [FormerlySerializedAs("decalController")] [HideInInspector]
    public DecalController localDecalController;

    private readonly Dictionary<int, GameObject> items = new();
    private readonly Dictionary<User, GameObject> remotePlayers = new();
    private GameObject aoi;
    private DecalApplicatorController decalApplicatorController;

    private GameObject localPlayer;
    private PlayerController localPlayerController;

    //----------------------------------------------------------
    // Private properties
    //----------------------------------------------------------

    private SmartFox sfs;

    public bool IsInMenu { get; private set; }

    //----------------------------------------------------------
    // Unity callback methods
    //----------------------------------------------------------

    private void Start()
    {
        decalApplicatorController = FindObjectOfType<DecalApplicatorController>();

        // Set a reference to the SmartFox client instance
        sfs = gm.GetSfsClient();

        // Hide modal panels
        HideModals();

        // Add event listeners
        AddSmartFoxListeners();

        // Set random model and material and spawn player model
        var numModel = Random.Range(0, playerModels.Length);
        var numMaterial = Random.Range(0, playerMaterials.Length);
        SpawnLocalPlayer(numModel, numMaterial);

        // Instantiate and set scale and position of game object representing the Area of Interest
        aoi = Instantiate(aoiPrefab);
        var aoiSize = ((MMORoom)sfs.LastJoinedRoom).DefaultAOI;
        aoi.transform.localScale = new Vector3(aoiSize.FloatX * 2, 10, aoiSize.FloatZ * 2);
        aoi.transform.position = new Vector3(localPlayer.transform.position.x, -3, localPlayer.transform.position.z);

        // Update settings panel with the selected model and material
        settingsPanel.SetModelSelection(numModel);
        settingsPanel.SetMaterialSelection(numMaterial);
    }

    protected override void Update()
    {
        base.Update();

        // If the player model was already spawned, set its position by means of User Variables (if movement is dirty only)
        if (localPlayer != null && localPlayerController != null && localPlayerController.MovementDirty)
        {
            var userVariables = new List<UserVariable>();
            userVariables.Add(new SFSUserVariable("x", (double)localPlayer.transform.position.x));
            userVariables.Add(new SFSUserVariable("y", (double)localPlayer.transform.position.y));
            userVariables.Add(new SFSUserVariable("z", (double)localPlayer.transform.position.z));
            userVariables.Add(new SFSUserVariable("rot", (double)localPlayer.transform.rotation.eulerAngles.y));

            sfs.Send(new SetUserVariablesRequest(userVariables));

            /*
			 * NOTE
			 * On the server side the User Variable Update event is captured and the coordinates are
			 * passed to the MMOApi.SetUserPosition() method to update the player position in the MMORoom.
			 * This in turn will keep this client in synch with all the other players within the current player's Area of Interest (AoI).
			 * Check the server-side Extension code.
			 */

            //localPlayerController.MovementDirty = false;
        }

        // Make AoI game object follow player
        if (localPlayer != null) aoi.transform.position = localPlayer.transform.position;

        // Cube creation
        // New input system should be integrated but this works for the basic cube creation example
        if (Input.GetButtonDown("Cube") && localPlayer != null)
            SpawnCubeRequest(localPlayer.transform.position, localPlayer.transform.rotation,
                sfs.MySelf.GetVariable("mat").GetIntValue());

        // UI Pause
        if (Input.GetButtonDown("Pause"))
        {
            Debug.Log("Pause");
            SetMenuActive(!IsInMenu);
        }
    }


    public void SetMenuActive(bool active)
    {
        IsInMenu = active;
        localPlayerController.canMove = !IsInMenu;
        Cursor.lockState = IsInMenu ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = IsInMenu;
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
	 * On Settings button click, show Settings Panel prefab instance.
	 */
    public void OnSettingsButtonClick()
    {
        settingsPanel.Show();
    }

    /**
	 * On AoI visibility changed in Settings panel, show/hide game object representing it.
	 */
    public void OnAoiVisibilityChange(bool showAoi)
    {
        aoi.SetActive(showAoi);
    }

    /**
	 * On model selected in Settings panel, spawn new player model.
	 */
    public void OnSelectedModelChange(int numModel)
    {
        SpawnLocalPlayer(numModel, sfs.MySelf.GetVariable("mat").GetIntValue());
    }

    /**
	 * On material selected in Settings panel, change player material.
	 */
    public void OnSelectedMaterialChange(int numMaterial)
    {
        localPlayer.GetComponentInChildren<Renderer>().material = playerMaterials[numMaterial];

        var userVariables = new List<UserVariable>();
        userVariables.Add(new SFSUserVariable("mat", numMaterial));

        sfs.Send(new SetUserVariablesRequest(userVariables));
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
        sfs.AddEventListener(SFSEvent.USER_VARIABLES_UPDATE, OnUserVariableUpdate);
        sfs.AddEventListener(SFSEvent.PROXIMITY_LIST_UPDATE, OnProximityListUpdate);
        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
    }

    /**
     * Remove all SmartFoxServer-related event listeners added by the scene.
     * This method is called by the parent BaseSceneController.OnDestroy method when the scene is destroyed.
     */
    protected override void RemoveSmartFoxListeners()
    {
        sfs.RemoveEventListener(SFSEvent.USER_VARIABLES_UPDATE, OnUserVariableUpdate);
        sfs.RemoveEventListener(SFSEvent.PROXIMITY_LIST_UPDATE, OnProximityListUpdate);
        sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
    }

    /**
	 * Hide all modal panels.
	 */
    protected override void HideModals()
    {
        settingsPanel.Hide();
    }

    /**
	 * Evaluate terrain height at given position.
	 */
    private float GetTerrainHeight(Vector3 pos)
    {
        var maxHeight = 10;
        var currPosY = pos.y;
        pos.y = maxHeight;

        var ray = new Ray(pos, Vector3.down);
        if (terrainCollider.Raycast(ray, out var hit, 2.0f * maxHeight))
            return hit.point.y;
        return currPosY;
    }

    #endregion


    //----------------------------------------------------------
    // Spawner methods
    //----------------------------------------------------------

    #region Spawner methods

    /**
	 * Add the game object representing the current player to the scene.
	 */
    private void SpawnLocalPlayer(int numModel, int numMaterial)
    {
        Vector3 pos;
        Quaternion rot;

        // In case a model already exists, get its position and rotation before spawning a new one
        // This occurs in case the current player selects a new model in the Settings panel
        if (localPlayer != null)
        {
            pos = localPlayer.transform.position;
            rot = localPlayer.transform.rotation;

            Destroy(localPlayer);
        }
        else
        {
            pos = new Vector3(0, 0, 0);
            rot = Quaternion.identity;

            pos.y = GetTerrainHeight(pos);
        }


        // Spawn local player model
        localPlayer = Instantiate(playerPrefab, pos, rot);

        // Assign starting material
        localPlayer.GetComponentInChildren<Renderer>().material = playerMaterials[numMaterial];

        // Since this is the local player, lets add a controller and set the camera
        localPlayerController = localPlayer.GetComponent<PlayerController>();
        localDecalController = localPlayer.GetComponent<DecalController>();
        Camera.main.transform.parent = localPlayer.transform;
        localPlayer.GetComponentInChildren<Text>().text = sfs.MySelf.Name;

        // Set movement limits based on map limits set for the MMORoom
        var lowerMapLimits = ((MMORoom)sfs.LastJoinedRoom).LowerMapLimit;
        var higherMapLimits = ((MMORoom)sfs.LastJoinedRoom).HigherMapLimit;
        if (lowerMapLimits != null && higherMapLimits != null)
            localPlayerController.SetLimits(lowerMapLimits.FloatX, lowerMapLimits.FloatZ, higherMapLimits.FloatX,
                higherMapLimits.FloatZ);

        // Save model, material and position in User Variables, causing other players
        // to be notified about the current player presence (see server-side Extension)
        var userVariables = new List<UserVariable>();

        userVariables.Add(new SFSUserVariable("x", (double)localPlayer.transform.position.x));
        userVariables.Add(new SFSUserVariable("y", (double)localPlayer.transform.position.y));
        userVariables.Add(new SFSUserVariable("z", (double)localPlayer.transform.position.z));
        userVariables.Add(new SFSUserVariable("rot", (double)localPlayer.transform.rotation.eulerAngles.y));
        userVariables.Add(new SFSUserVariable("model", numModel));
        userVariables.Add(new SFSUserVariable("mat", numMaterial));

        // Send request
        sfs.Send(new SetUserVariablesRequest(userVariables));
    }

    /**
	 * Add the game object representing another player to the scene.
	 */
    private void SpawnRemotePlayer(User user, Vector3 pos, Quaternion rot)
    {
        // Check if there already exists a model, and destroy it first
        // This occurs in case the player selects a new model in their Settings panel
        if (remotePlayers.ContainsKey(user) && remotePlayers[user] != null)
        {
            Destroy(remotePlayers[user]);
            remotePlayers.Remove(user);
        }

        // Get model and material from User Variables
        var numModel = user.GetVariable("model").GetIntValue();
        var numMaterial = user.GetVariable("mat").GetIntValue();

        // Spawn remote player model
        var remotePlayer = Instantiate(playerModels[numModel]);
        remotePlayer.AddComponent<SimpleRemoteInterpolation>();
        remotePlayer.GetComponent<SimpleRemoteInterpolation>().SetTransform(pos, rot, false);

        // Set material and name
        remotePlayer.GetComponentInChildren<Renderer>().material = playerMaterials[numMaterial];
        remotePlayer.GetComponentInChildren<Text>().text = user.Name;

        // Add the object to the list of remote players
        remotePlayers.Add(user, remotePlayer);
    }

    /**
     * Send spawn cube request to server
     */
    private void SpawnCubeRequest(Vector3 position, Quaternion rotation, int numMaterial)
    {
        ISFSObject param = new SFSObject();
        param.PutFloat("x", position.x);
        param.PutFloat("y", position.y);
        param.PutFloat("z", position.z);
        param.PutFloat("rot", rotation.eulerAngles.y);
        param.PutInt("mat", numMaterial);

        // Send request -> server will create MMOItem and add it to map
        // Then the cube will be detected by the proximity list to appear in the client side
        sfs.Send(new ExtensionRequest("spawn_cube", param, sfs.LastJoinedRoom));

        Debug.Log("Send spawn cube request");
    }

    /**
     * Spawn cube in the game and add it to item list
     */
    private void SpawnCube(int id, Vector3 position, Quaternion rotation, int numMaterial)
    {
        Debug.Log("Spawn cube in world");

        var cube = Instantiate(cubePrefab, position, rotation);
        cube.GetComponent<Renderer>().material = playerMaterials[numMaterial];

        // Replace cube if it already exists
        if (items.ContainsKey(id))
        {
            Destroy(items[id]);
            items.Remove(id);
        }

        items.Add(id, cube);
    }

    #endregion

    #region Decal Spawner methods

    public void SpawnStickerDecalRequest(ISFSObject param)
    {
        Debug.Log("Send sticker decal request");
        sfs.Send(new ExtensionRequest("spawn_stickerDecal", param, sfs.LastJoinedRoom));
    }

    private void SpawnStickerDecal(IMMOItem item)
    {
        decalApplicatorController.SpawnStickerDecalFromServer(item);
    }

    private void SpawnStickerDecalEvent(SFSObject param)
    {
        decalApplicatorController.SpawnStickerDecalFromEvent(param);
    }

    #endregion

    //----------------------------------------------------------
    // SmartFoxServer event listeners
    //----------------------------------------------------------

    #region Event listeners

    /**
     * This is where we receive events about users and items in proximity (inside the Area of Interest) of the current player.
     * We get four lists, one of new users that have entered the AoI, one with users that have left the proximity area,
     * and two for items that entered and left the proximity area respectively.
     */
    private void OnProximityListUpdate(BaseEvent evt)
    {
        Debug.Log("OnProximityListUpdate");

        var addedUsers = (List<User>)evt.Params["addedUsers"];
        var removedUsers = (List<User>)evt.Params["removedUsers"];
        var addedItems = (List<IMMOItem>)evt.Params["addedItems"];
        var removedItems = (List<IMMOItem>)evt.Params["removedItems"];

        // Handle new users
        foreach (var user in addedUsers)
            // Spawn model representing remote player
            SpawnRemotePlayer(user,
                new Vector3(user.AOIEntryPoint.FloatX, user.AOIEntryPoint.FloatY, user.AOIEntryPoint.FloatZ),
                Quaternion.Euler(0, (float)user.GetVariable("rot").GetDoubleValue(), 0)
            );

        // Handle removed users
        foreach (var user in removedUsers)
            if (remotePlayers.ContainsKey(user))
            {
                Destroy(remotePlayers[user]);
                remotePlayers.Remove(user);
            }

        foreach (var item in addedItems)
            switch (item.GetVariable("type").GetStringValue())
            {
                case "cube":
                    SpawnCube(item.Id,
                        new Vector3(item.AOIEntryPoint.FloatX, item.AOIEntryPoint.FloatY, item.AOIEntryPoint.FloatZ),
                        Quaternion.Euler(0, (float)item.GetVariable("rot").GetDoubleValue(), 0),
                        item.GetVariable("mat").GetIntValue());
                    break;

                case "sticker_decal":
                    SpawnStickerDecal(item);
                    break;
            }

        foreach (var item in removedItems)
            if (items.ContainsKey(item.Id))
            {
                Destroy(items[item.Id]);
                items.Remove(item.Id);
            }
    }

    /**
     * When a User Variable is updated on any client within the current player's AoI, this event is received.
     * This is where most of the game logic for this example is contained.
     */
    private void OnUserVariableUpdate(BaseEvent evt)
    {
        var changedVars = (List<string>)evt.Params["changedVars"];
        var user = (SFSUser)evt.Params["user"];

        // Ignore all updates for the current player
        if (user == sfs.MySelf)
            return;

        // Check if the remote user changed their position or rotation
        if (changedVars.Contains("x") || changedVars.Contains("y") || changedVars.Contains("z") ||
            changedVars.Contains("rot"))
            if (remotePlayers.ContainsKey(user))
                // Move the character to the new position using a simple interpolation
                remotePlayers[user].GetComponent<SimpleRemoteInterpolation>().SetTransform(
                    new Vector3((float)user.GetVariable("x").GetDoubleValue(),
                        (float)user.GetVariable("y").GetDoubleValue(), (float)user.GetVariable("z").GetDoubleValue()),
                    Quaternion.Euler(0, (float)user.GetVariable("rot").GetDoubleValue(), 0),
                    true
                );

        // Check if the remote player selected a new model
        if (changedVars.Contains("model"))
            // Spawn a new remote player model
            SpawnRemotePlayer(user, remotePlayers[user].transform.position, remotePlayers[user].transform.rotation);

        // Check if the remote player selected a new material
        if (changedVars.Contains("mat"))
            // Change material
            remotePlayers[user].GetComponentInChildren<Renderer>().material =
                playerMaterials[user.GetVariable("mat").GetIntValue()];
    }

    /**
	 * General messages from the server.
	 */
    private void OnExtensionResponse(BaseEvent evt)
    {
        var cmd = (string)evt.Params["cmd"];
        var args = (SFSObject)evt.Params["params"];

        switch (evt.Params["cmd"])
        {
            // This response is instant and is useful to have a fluid creation of cube
            // Proximity list will also create (and replace) the same cube but it is ruled by the
            // "Millis between proximity updates" constant and so is not time consistent.
            // If you don't understand you can test by commenting this case.
            case "spawn_cube_from_server":
                SpawnCube(args.GetInt("id"),
                    new Vector3(args.GetFloat("x"), args.GetFloat("y"), args.GetFloat("z")),
                    Quaternion.Euler(0, args.GetFloat("rot"), 0),
                    args.GetInt("mat"));
                break;

            case "spawn_stickerDecal_from_server":
                SpawnStickerDecalEvent(args);
                break;
        }
    }

    #endregion
}