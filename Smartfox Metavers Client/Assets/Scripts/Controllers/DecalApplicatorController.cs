using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;


public class DecalApplicatorController : MonoBehaviour
{
    [SerializeField] private Stickers stickersSO;

    private GameSceneController gs;
    private PlayerController player;

    // Start is called before the first frame update
    private void Start()
    {
        gs = FindObjectOfType<GameSceneController>();

        if(!gs)
        {
            Debug.LogWarning("No GameSceneController found");
        }

        InitializeStickerDecalList();
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("///// REMOVE DEBUG.LOG IN SCRIPT /////");

            if(player == null)
            {
                player = FindObjectOfType<PlayerController>();
            }

            SendStickerDecalRequest(player.transform.position, Quaternion.Euler(90, 0, 0), 0);
        }

    }

    // Decode parameters from IMMOItem and call spawning function
    public void SpawnStickerDecalFromServer(IMMOItem item)
    {
        // int id = item.GetVariable("stickerDecalID").GetIntValue();

        Vector3 pos = DecodePositionVector(item);

        Vector3 eulerAngles = DecodeRotationVector(item);
        Quaternion rot = Quaternion.Euler(eulerAngles);

        // Vector3 size = DecodeSizeVector(item);
        Vector3 size = new Vector3(5, 5, 5);

        int stickerID = item.GetVariable("stickerID").GetIntValue();

        SpawnStickerDecal(pos, rot, size, stickerID);
    }

    public void SpawnStickerDecalFromEvent(SFSObject param)
    {
        Vector3 pos = new Vector3(param.GetFloat("x"), param.GetFloat("y"), param.GetFloat("z"));

        Vector3 eulerAngles = new Vector3(param.GetFloat("rotZ"), param.GetFloat("rotY"), param.GetFloat("rotZ"));
        Quaternion rot = Quaternion.Euler(eulerAngles);

        Vector3 size = new Vector3(5, 5, 5);

        int stickerID = param.GetInt("stickerID");

        SpawnStickerDecal(pos, rot, size, stickerID);
    }

    /**
     * Send spawn sticker decal request to server
     */
    public void SendStickerDecalRequest(Vector3 position, Quaternion rotation, int stickerID)
    {
        Debug.Log("Set params for sticker decal spawn request");

	    ISFSObject param = new SFSObject();

	    param.PutFloat("x", position.x);
	    param.PutFloat("y", position.y);
	    param.PutFloat("z", position.z);

        param.PutFloat("rotX", rotation.eulerAngles.x);
	    param.PutFloat("rotY", rotation.eulerAngles.y);
        param.PutFloat("rotZ", rotation.eulerAngles.z);

        // param.PutFloat("sizeX", decal.size.x);
        // param.PutFloat("sizeY", decal.size.y);
        // param.PutFloat("sizeZ", decal.size.z);

        // Add parameters for size when we know how to manage it

        // Add param.PutFloat for lifetime/date/something to make them disappear
	    param.PutInt("stickerID", stickerID);

        gs.SpawnStickerDecalRequest(param);

	    // Send request -> server will create MMOItem and add it to map
	    // Then the cube will be detected by the proximity list to appear in the client side
	    // sfs.Send(new ExtensionRequest("spawn_stickerDecal", param, sfs.LastJoinedRoom));
    }

    private void SpawnStickerDecal(Vector3 position, Quaternion rotation, Vector3 size, int stickerID)
    {
        Debug.Log("Spawn sticker decal in world");

        DecalProjector decal = Instantiate(stickersSO.decalProjectorPrefab, position, rotation);
        // decal.size = size;
        decal.size = new Vector3(5, 5, 5);
        decal.material = stickersSO.stickerList[stickerID].mat;

        decal.enabled = true;
    }

    private void InitializeStickerDecalList()
    {
        int l = stickersSO.stickerList.Count;

        for(int i = 0; i < l; i++)
        {
            if(stickersSO.stickerList[i].Image != null)
            {
                stickersSO.stickerList[i].id = i;
                // stickersSO.stickerList[i].mat = new Material(Shader.Find("HDRP/Decal"));
                stickersSO.stickerList[i].mat.SetTexture("_BaseColorMap", stickersSO.stickerList[i].Image);
            }
        }
    }

    /* Utilities */

    private Vector3 DecodePositionVector(IMMOItem item)
    {
        // float x = (float)item.GetVariable("x").GetDoubleValue();
        // float y = (float)item.GetVariable("y").GetDoubleValue();
        // float z = (float)item.GetVariable("z").GetDoubleValue();

        float x = (float)item.AOIEntryPoint.FloatX;
        float y = (float)item.AOIEntryPoint.FloatY;
        float z = (float) item.AOIEntryPoint.FloatZ;

        return new Vector3(x, y, z);
    }

    private Vector3 DecodeRotationVector(IMMOItem item)
    {
        float x = (float)item.GetVariable("rotX").GetDoubleValue();
        float y = (float)item.GetVariable("rotY").GetDoubleValue();
        float z = (float)item.GetVariable("rotZ").GetDoubleValue();

        return new Vector3(x, y, z);
    }

    private Vector3 DecodeSizeVector(IMMOItem item)
    {
        float x = (float)item.GetVariable("sizeX").GetDoubleValue();
        float y = (float)item.GetVariable("sizeY").GetDoubleValue();
        float z = (float)item.GetVariable("sizeZ").GetDoubleValue();

        return new Vector3(x, y, z);
    }
}
