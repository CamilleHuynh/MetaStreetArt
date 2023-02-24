using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class DecalController : MonoBehaviour
{
    [SerializeField] private float decalReach = 10f;
    [SerializeField] private float increaseSizeRate = 2f;
    [SerializeField] private Vector2 sizeClamp = new(0.1f, 7f);
    [SerializeField] private float rotateDegreeRate = 5f;
    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private DecalProjector previewDecal;
    [SerializeField] private Stickers stickersSO;

    [SerializeField] private AudioClip tagSound;
    [SerializeField] private AudioSource oneshotSource;

    private float currentRotation;
    private float currentSize;
    private int flip;
    private DecalApplicatorController decalApplicatorController;
    private Camera mainCamera;

    private PlayerController playerController;

    private int stickerID;

    private Vector3 initialScale;


    // Start is called before the first frame update
    private void Start()
    {
        mainCamera = Camera.main;

        playerController = GetComponent<PlayerController>();

        decalApplicatorController = FindObjectOfType<DecalApplicatorController>();

        previewDecal.size = new Vector3(2, 2, decalReach);
        previewDecal.enabled = true;

        currentRotation = 0f;
        currentSize = 2f;
        flip = 1;

        initialScale = previewDecal.transform.localScale;

        UpdatePreviewDecalSticker();
    }

    private void Update()
    {
        ManageDecalChange();
        ManageDecalPreview();
    }

    public void SetStickerID(int i)
    {
        stickerID = i;

        UpdatePreviewDecalSticker();
    }

    private void ManageDecalChange()
    {
        var sizeDelta = Input.GetAxis("Mouse ScrollWheel");
        currentSize += increaseSizeRate * sizeDelta * Time.deltaTime;
        UpdatePreviewDecalSize();

        var rotationDelta = Input.GetAxis("Rotate");
        currentRotation += rotateDegreeRate * rotationDelta * Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.F))
        {
            flip = -flip;
        }
        UpdatePreviewDecalFlip();
    }

    private void ManageDecalPreview()
    {
        RaycastHit hit;

        var ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        var hasHit = Physics.Raycast(ray, out hit, decalReach, ~playerLayer);

        EnablePreviewDecal(hasHit);

        if (hasHit)
        {
            // Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * hit.distance, Color.yellow, 1.0f);
            // Debug.Log("Did Hit");

            UpdatePreviewDecalTransform();

            if (Input.GetButtonDown("Fire1") && !((GameSceneController)GameSceneController.instance).IsInMenu)
            {
                oneshotSource.PlayOneShot(tagSound);
                decalApplicatorController.SendStickerDecalRequest(previewDecal.transform.position,
                    previewDecal.transform.rotation, previewDecal.size, flip, stickerID);
            }
        }
        // Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 1000, Color.white, 1.0f);
        // Debug.Log("Did not Hit");
    }

    private void UpdatePreviewDecalSticker()
    {
        previewDecal.material = stickersSO.stickerList[stickerID].previewMat;
    }

    private void UpdatePreviewDecalTransform()
    {
        previewDecal.transform.position =
            mainCamera.transform.position + (0.5f * decalReach + 2f) * mainCamera.transform.forward;
        previewDecal.transform.rotation = mainCamera.transform.rotation * Quaternion.Euler(0, 0, currentRotation);
    }

    private void UpdatePreviewDecalSize()
    {
        previewDecal.size = new Vector3(currentSize, currentSize, 10f);
    }

    private void UpdatePreviewDecalFlip()
    {
        previewDecal.transform.localScale = new Vector3(flip * initialScale.x, initialScale.y, initialScale.z);
    }

    private void EnablePreviewDecal(bool val)
    {
        previewDecal.enabled = val;
    }
}