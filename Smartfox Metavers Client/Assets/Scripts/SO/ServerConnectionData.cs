using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Requests.MMO;
using UnityEngine;

[CreateAssetMenu(menuName = "Metavers Template/Server Connection Data")]
public class ServerConnectionData : ScriptableObject
{
    [Tooltip("IP address or domain name of the SmartFoxServer instance")]
    [SerializeField] private string m_host = "127.0.0.1";

    [Tooltip("TCP listening port of the SmartFoxServer instance, used for TCP socket connection in all builds except WebGL")]
    [SerializeField] private int m_tcpPort = 9933;

    [Tooltip("HTTP listening port of the SmartFoxServer instance, used for WebSocket (WS) connections in WebGL build")]
    [SerializeField] private int m_httpPort = 8080;

    [Tooltip("Name of the SmartFoxServer Zone to join")]
    [SerializeField] private string m_zoneName = "BasicExamples";

    [Tooltip("Room group of the game")]
    [SerializeField] private string m_roomGroup = "cube_spawner";

    [Tooltip("Display SmartFoxServer client debug messages")]
    [SerializeField] private bool m_debug = false;

    [Header("Room Creation")]
    [SerializeField] private Vector3 m_defaultAOI = new Vector3(50f, 50f, 50f);
    [Tooltip("Folder where the extension jar is in the server")]
    [SerializeField] private string m_roomExtensionID = "CubeSpawnerRoomExtension";
    [Tooltip("Exact path to the extension class")]
    [SerializeField] private string m_roomExtensionClass = "CubeSpawnerExtensions.CubeSpawnerRoomExtension";
    [SerializeField] private Vector3 m_lowestMapLimits = new Vector3(-100f, -10f, -100f);
    [SerializeField] private Vector3 m_highestMapLimits = new Vector3(100f, 10f, 100f);

    public string Host
    {
        get => m_host;
        set => m_host = value;
    }

    public int TcpPort => m_tcpPort;
    public int HttpPort => m_httpPort;
    public string ZoneName => m_zoneName;
    public string RoomGroup => m_roomGroup;
    public bool Debug => m_debug;
    public Vec3D DefaultAOI => new Vec3D(m_defaultAOI.x, m_defaultAOI.y, m_defaultAOI.z);
    public RoomExtension RoomExtension => new RoomExtension(m_roomExtensionID, m_roomExtensionClass);
    public MapLimits DefaultMapLimits => new MapLimits(
        new Vec3D(m_lowestMapLimits.x, m_lowestMapLimits.y, m_lowestMapLimits.z),
        new Vec3D(m_highestMapLimits.x, m_highestMapLimits.y, m_highestMapLimits.z));
}