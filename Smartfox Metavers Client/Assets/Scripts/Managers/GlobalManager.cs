using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Util;

#if UNITY_EDITOR
[InitializeOnLoad]
public class UnityEditorStartSceneSetter
{
    static UnityEditorStartSceneSetter()
    {
        // In Unity Editor, set Play Mode scene to LOGIN scene
        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Login.unity");
    }
}
#endif

///=================================================================================================================///

/**
 * Singleton class holding a reference to the SmartFox class instance, to share the client-server connection among the project's multiple scenes.
 * This class also takes care of loading the required scene based on the connection state.
 */
public class GlobalManager : MonoBehaviour
{
    private static GlobalManager _instance;

    public static GlobalManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new GameObject("GlobalManager").AddComponent<GlobalManager>();
            
            return _instance;
        }
    }
    
    //----------------------------------------------------------
    // Private properties
    //----------------------------------------------------------

    private SmartFox sfs;
    private string connLostMsg;

    //----------------------------------------------------------
    // Unity callback methods
    //----------------------------------------------------------
    #region
    private void Awake()
    {
        // Do not destroy this object on scene change
        DontDestroyOnLoad(this);

        // Make sure the application runs in background
        Application.runInBackground = true;

        Debug.Log("Global Manager ready");
    }

    private void Update()
    {
        // Process the SmartFox events queue
        sfs?.ProcessEvents();
    }

    private void OnDestroy()
    {
        Debug.Log("Global Manager destroyed");
    }

    private void OnApplicationQuit()
    {
        // Disconnect from SmartFoxServer if a connection is active
        // This is required because an active socket connection during the application quit process can cause a crash on some platforms
        if (sfs != null && sfs.IsConnected)
            sfs.Disconnect();
    }
    #endregion

    //----------------------------------------------------------
    // Public methods
    //----------------------------------------------------------
    #region
    /**
     * Return and delete the last connection lost message.
     */
    public string ConnectionLostMessage
    {
        get
        {
            string m = connLostMsg;
            connLostMsg = null;
            return m;
        }
    }

    /**
	 * Create and return a SmartFox class instance for TCP socket connection.
	 * The CONNECTION_LOST listener is also added.
	 */
    public SmartFox CreateSfsClient()
    {
        sfs = new SmartFox();
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        return sfs;
    }

    /**
	 * Create and return a SmartFox class instance for WebSocket connection.
	 * The CONNECTION_LOST listener is also added.
	 */
    public SmartFox CreateSfsClient(UseWebSocket useWebSocket)
    {
        sfs = new SmartFox(useWebSocket);
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        return sfs;
    }

    /**
	 * Return the existing SmartFox class instance.
	 */
    public SmartFox GetSfsClient()
    {
        return sfs;
    }
    #endregion

    //----------------------------------------------------------
    // SmartFoxServer event listeners
    //----------------------------------------------------------
    
    private void OnConnectionLost(BaseEvent evt)
    {
        // Remove CONNECTION_LOST listener
        sfs.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs = null;

        // Get disconnection reason
        string connLostReason = (string)evt.Params["reason"];

        Debug.Log("Connection to SmartFoxServer lost; reason is: " + connLostReason);

        if (SceneManager.GetActiveScene().name != "Login")
        {
            if (connLostReason != ClientDisconnectionReason.MANUAL)
            {
                // Save disconnection message, which can be retrieved by the LOGIN scene to display an error message
                connLostMsg = "An unexpected disconnection occurred.\n";

                if (connLostReason == ClientDisconnectionReason.IDLE)
                    connLostMsg += "It looks like you have been idle for too much time.";
                else if (connLostReason == ClientDisconnectionReason.KICK)
                    connLostMsg += "You have been kicked by an administrator or moderator.";
                else if (connLostReason == ClientDisconnectionReason.BAN)
                    connLostMsg += "You have been banned by an administrator or moderator.";
                else
                    connLostMsg += "The reason of the disconnection is unknown.";
            }

            // Switch to the LOGIN scene
            SceneManager.LoadScene("Login");
        }
    }
}
