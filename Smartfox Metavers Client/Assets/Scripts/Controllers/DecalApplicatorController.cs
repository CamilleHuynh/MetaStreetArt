using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class DecalApplicatorController : MonoBehaviour
{
    [SerializeField] private Stickers stickersSO;

    // Start is called before the first frame update
    private void Start()
    {
        InitializeStickerDecalList();
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            SpawnStickerDecal(0, new Vector3(0, -0.5f, 0), Quaternion.Euler(90, 0, 0), new Vector3(1, 1, 1), 0);
        }

    }

    /**
     * Send spawn cube request to server
     */
    private void SpawnStickerDecalRequest(DecalProjector decal, Vector3 position, Quaternion rotation, int stickerID, int stickerDecalID)
    {
	    ISFSObject param = new SFSObject();
	    param.PutFloat("x", position.x);
	    param.PutFloat("y", position.y);
	    param.PutFloat("z", position.z);

        param.PutFloat("rotX", rotation.eulerAngles.x);
	    param.PutFloat("rotY", rotation.eulerAngles.y);
        param.PutFloat("rotZ", rotation.eulerAngles.z);

        // Add parameters for size when we know how to manage it

        // Add param.PutFloat for lifetime/date/something to make them disappear

        param.PutInt("id", stickerDecalID);

	    param.PutInt("stickerID", stickerID);

	    // Send request -> server will create MMOItem and add it to map
	    // Then the cube will be detected by the proximity list to appear in the client side
	    sfs.Send(new ExtensionRequest("spawn_stickerDecal", param, sfs.LastJoinedRoom));

		Debug.Log("Send spawn stickerDecal request");
    }

    private void SpawnStickerDecal(int id, Vector3 position, Quaternion rotation, Vector3 scale, int stickerID)
    {
        Debug.Log("Spawn sticker decal in world");

        DecalProjector decal = Instantiate(stickersSO.decalProjectorPrefab, position, rotation);
        decal.size = new Vector3(5, 5, 5);
        // decal.material = stickersSO.stickerList[stickerID].mat;
    }

    private void InitializeStickerDecalList()
    {
        int l = stickersSO.stickerList.Count;

        for(int i = 0; i < l; i++)
        {
            if(stickersSO.stickerList[i].Image != null)
            {
                stickersSO.stickerList[i].id = i;
                stickersSO.stickerList[i].mat = new Material(Shader.Find("HDRP/Decal"));
                stickersSO.stickerList[i].mat.SetTexture("_BaseColorMap", stickersSO.stickerList[i].Image);
            }
        }
    }
}
