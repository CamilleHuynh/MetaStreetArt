using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class DecalController : MonoBehaviour
{
    [SerializeField] private float decalReach = 10f;
    private Camera mainCamera;
    private LayerMask playerLayer;

    private PlayerController playerController;
    private DecalApplicatorController decalApplicatorController;

    private int stickerID = 0;

    [SerializeField] private DecalProjector previewDecal;
    [SerializeField] private Stickers stickersSO;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        playerController = GetComponent<PlayerController>();
        playerLayer = playerController.playerLayer;

        decalApplicatorController = FindObjectOfType<DecalApplicatorController>();

        previewDecal.size = new Vector3(2, 2, decalReach);
        previewDecal.enabled = true;

        UpdatePreviewDecalSticker();
    }

    private void Update()
    {
        RaycastHit hit;

        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        bool hasHit = Physics.Raycast(ray, out hit, decalReach, ~playerLayer);

        EnablePreviewDecal(hasHit);

        if (hasHit)
        {
            // Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * hit.distance, Color.yellow, 1.0f);
            // Debug.Log("Did Hit");

            UpdatePreviewDecalTransform();

            if (Input.GetButtonDown("Fire1"))
            {
                decalApplicatorController.SendStickerDecalRequest(hit.point, mainCamera.transform.rotation, stickerID);
            }
        }
        else
        {
            // Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 1000, Color.white, 1.0f);
            // Debug.Log("Did not Hit");
        }
    }

    public void SetStickerID(int i)
    {
        stickerID = i;

        UpdatePreviewDecalSticker();
    }

    private void UpdatePreviewDecalSticker()
    {
        previewDecal.material = stickersSO.stickerList[stickerID].mat;
    }

    private void UpdatePreviewDecalTransform()
    {
        previewDecal.transform.position = mainCamera.transform.position + (0.5f * decalReach + 0.1f) * mainCamera.transform.forward;
        previewDecal.transform.forward = mainCamera.transform.forward;
    }

    private void EnablePreviewDecal(bool val)
    {
        previewDecal.enabled = val;
    }
}
