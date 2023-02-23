using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class DecalController : MonoBehaviour
{
    [SerializeField] private float decalReach = 10f;
    [SerializeField] private float increaseSizeRate = 2f;
    [SerializeField] private Vector2 sizeClamp = new Vector2(0.1f, 7f);
    [SerializeField] private float rotateDegreeRate = 5f;
    private Camera mainCamera;
    private LayerMask playerLayer;

    private PlayerController playerController;
    private DecalApplicatorController decalApplicatorController;

    private int stickerID = 0;
    private float currentRotation;
    private float currentSize;

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

        currentRotation = 0f;
        currentSize = 2f;

        UpdatePreviewDecalSticker();
    }

    private void Update()
    {
        ManageDecalChange();
        ManageDecalPreview();

        // RaycastHit hit;

        // Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        // bool hasHit = Physics.Raycast(ray, out hit, decalReach, ~playerLayer);

        // EnablePreviewDecal(hasHit);

        // if (hasHit)
        // {
        //     // Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * hit.distance, Color.yellow, 1.0f);
        //     // Debug.Log("Did Hit");

        //     UpdatePreviewDecalTransform();

        //     if (Input.GetButtonDown("Fire1"))
        //     {
        //         Debug.Log("DecalController : Set decal, size: " + previewDecal.size);
        //         decalApplicatorController.SendStickerDecalRequest(previewDecal.transform.position, previewDecal.transform.rotation, previewDecal.size, stickerID);
        //     }
        // }
        // else
        // {
        //     // Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 1000, Color.white, 1.0f);
        //     // Debug.Log("Did not Hit");
        // }
    }

    public void SetStickerID(int i)
    {
        stickerID = i;

        UpdatePreviewDecalSticker();
    }

    private void ManageDecalChange()
    {
        float sizeDelta = Input.GetAxis("Mouse ScrollWheel");
        currentSize += increaseSizeRate * sizeDelta * Time.deltaTime;
        UpdatePreviewDecalSize();

        float rotationDelta = Input.GetAxis("Fire2");
        currentRotation += rotateDegreeRate * rotationDelta * Time.deltaTime;
    }

    private void ManageDecalPreview()
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
                Debug.Log("DecalController : Set decal, size: " + previewDecal.size);
                decalApplicatorController.SendStickerDecalRequest(previewDecal.transform.position, previewDecal.transform.rotation, previewDecal.size, stickerID);
            }
        }
        else
        {
            // Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 1000, Color.white, 1.0f);
            // Debug.Log("Did not Hit");
        }
    }

    private void UpdatePreviewDecalSticker()
    {
        previewDecal.material = stickersSO.stickerList[stickerID].mat;
    }

    private void UpdatePreviewDecalTransform()
    {
        previewDecal.transform.position = mainCamera.transform.position + (0.5f * decalReach + 0.1f) * mainCamera.transform.forward;
        previewDecal.transform.rotation = mainCamera.transform.rotation * Quaternion.Euler(0, 0, currentRotation);
    }

    private void UpdatePreviewDecalSize()
    {
        previewDecal.size = new Vector3(currentSize, currentSize, 10f);
    }

    private void EnablePreviewDecal(bool val)
    {
        previewDecal.enabled = val;
    }
}
