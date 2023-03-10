using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using UnityEngine;

public class DecalApplicatorController : MonoBehaviour
{
    [SerializeField] private Stickers stickersSO;

    private GameSceneController gs;
    private PlayerController player;

    // Start is called before the first frame update
    private void Start()
    {
        gs = FindObjectOfType<GameSceneController>();

        if (!gs) Debug.LogWarning("No GameSceneController found");
    }

    // Decode parameters from IMMOItem and call spawning function
    public void SpawnStickerDecalFromServer(IMMOItem item)
    {
        // int id = item.GetVariable("stickerDecalID").GetIntValue();

        var pos = DecodePositionVector(item);

        var eulerAngles = DecodeRotationVector(item);
        var rot = Quaternion.Euler(eulerAngles);

        var size = DecodeSizeVector(item);
        // Vector3 size = new Vector3(5, 5, 5);

        var flip = item.GetVariable("flip").GetIntValue();
        // var flip = 1;

        var stickerID = item.GetVariable("stickerID").GetIntValue();

        SpawnStickerDecal(pos, rot, size, flip, stickerID);
    }

    public void SpawnStickerDecalFromEvent(SFSObject param)
    {
        // Debug.Log("SpawnStickerDecalFromEvent");

        // Vector3 pos = new Vector3(param.GetFloat("x"), param.GetFloat("y"), param.GetFloat("z"));

        // Vector3 eulerAngles = new Vector3(param.GetFloat("rotX"), param.GetFloat("rotY"), param.GetFloat("rotZ"));
        // Quaternion rot = Quaternion.Euler(eulerAngles);

        // Vector3 size = new Vector3(param.GetFloat("sizeX"), param.GetFloat("sizeY"), param.GetFloat("sizeZ"));

        // int stickerID = param.GetInt("stickerID");

        // SpawnStickerDecal(pos, rot, size, stickerID);
    }

    /**
     * Send spawn sticker decal request to server
     */
    public void SendStickerDecalRequest(Vector3 position, Quaternion rotation, Vector3 size, int flip, int stickerID)
    {
        ISFSObject param = new SFSObject();

        param.PutFloat("x", position.x);
        param.PutFloat("y", position.y);
        param.PutFloat("z", position.z);

        param.PutFloat("rotX", rotation.eulerAngles.x);
        param.PutFloat("rotY", rotation.eulerAngles.y);
        param.PutFloat("rotZ", rotation.eulerAngles.z);

        param.PutFloat("sizeX", size.x);
        param.PutFloat("sizeY", size.y);
        param.PutFloat("sizeZ", size.z);

        param.PutInt("flip", flip);

        // Add parameters for size when we know how to manage it

        // Add param.PutFloat for lifetime/date/something to make them disappear
        param.PutInt("stickerID", stickerID);

        gs.SpawnStickerDecalRequest(param);

        // Send request -> server will create MMOItem and add it to map
        // Then the cube will be detected by the proximity list to appear in the client side
        // sfs.Send(new ExtensionRequest("spawn_stickerDecal", param, sfs.LastJoinedRoom));
    }

    private void SpawnStickerDecal(Vector3 position, Quaternion rotation, Vector3 size, int flip, int stickerID)
    {
        var decal = Instantiate(stickersSO.decalProjectorPrefab, position, rotation);
        decal.size = size;
        // decal.size = new Vector3(5, 5, 5);
        decal.material = stickersSO.stickerList[stickerID].mat;

        decal.transform.localScale = new Vector3(flip * decal.transform.localScale.x, decal.transform.localScale.y, decal.transform.localScale.z);

        decal.enabled = true;
    }

    /* Utilities */

    private Vector3 DecodePositionVector(IMMOItem item)
    {
        // float x = (float)item.GetVariable("x").GetDoubleValue();
        // float y = (float)item.GetVariable("y").GetDoubleValue();
        // float z = (float)item.GetVariable("z").GetDoubleValue();

        var x = item.AOIEntryPoint.FloatX;
        var y = item.AOIEntryPoint.FloatY;
        var z = item.AOIEntryPoint.FloatZ;

        return new Vector3(x, y, z);
    }

    private Vector3 DecodeRotationVector(IMMOItem item)
    {
        var x = (float)item.GetVariable("rotX").GetDoubleValue();
        var y = (float)item.GetVariable("rotY").GetDoubleValue();
        var z = (float)item.GetVariable("rotZ").GetDoubleValue();

        return new Vector3(x, y, z);
    }

    private Vector3 DecodeSizeVector(IMMOItem item)
    {
        var x = (float)item.GetVariable("sizeX").GetDoubleValue();
        var y = (float)item.GetVariable("sizeY").GetDoubleValue();
        var z = (float)item.GetVariable("sizeZ").GetDoubleValue();

        return new Vector3(x, y, z);
    }
}