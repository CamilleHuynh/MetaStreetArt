using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    [HideInInspector] public bool canMove = true;

    [SerializeField] private AudioSource walkSource;


    private CharacterController characterController;
    private Vector3 higherLimit;

    private Vector3 lowerLimit;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX;


    // Dirty flag for checking if movement was made or not
    public bool MovementDirty => moveDirection != Vector3.zero;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Update()
    {
        // We are grounded, so recalculate move direction based on axes
        var forward = transform.TransformDirection(Vector3.forward);
        var right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        var isRunning = Input.GetKey(KeyCode.LeftShift);
        var curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        var curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        var movementDirectionY = moveDirection.y;
        moveDirection = forward * curSpeedX + right * curSpeedY;

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            moveDirection.y = jumpSpeed;
        else
            moveDirection.y = movementDirectionY;

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded) moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Sound walk
        if (characterController.isGrounded && moveDirection.x != 0 && moveDirection.z != 0)
        {
            if (!walkSource.isPlaying)
                walkSource.Play();
        }
        else
        {
            walkSource.Stop();
        }

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        // Check movement limits
        var pos = transform.position;

        if (pos.x < lowerLimit.x)
            transform.position = new Vector3(lowerLimit.x, pos.y, pos.z);
        if (pos.x > higherLimit.x)
            transform.position = new Vector3(higherLimit.x, pos.y, pos.z);

        if (pos.z < lowerLimit.z)
            transform.position = new Vector3(pos.x, pos.y, lowerLimit.z);
        if (pos.z > higherLimit.z)
            transform.position = new Vector3(pos.x, pos.y, higherLimit.z);
    }

    public void SetLimits(float minX, float minZ, float maxX, float maxZ)
    {
        lowerLimit = new Vector3(minX, 0, minZ);
        higherLimit = new Vector3(maxX, 0, maxZ);
    }
}