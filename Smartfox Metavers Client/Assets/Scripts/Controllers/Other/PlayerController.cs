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

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    private Vector3 lowerLimit;
    private Vector3 higherLimit;

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float interactionDistance = 50.0f;

    // Dirty flag for checking if movement was made or not
    public bool MovementDirty
    {
	    get => moveDirection != Vector3.zero;
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        
        // UI Pause
        if (Input.GetButtonDown("Pause"))
        {
            Debug.Log("Pause");
            canMove = !canMove;
            Cursor.lockState = canMove ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = canMove ? false : true;
        }
        
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        
        // Check movement limits
        Vector3 pos = this.transform.position;

        if (pos.x < lowerLimit.x)
	        this.transform.position = new Vector3(lowerLimit.x, pos.y, pos.z);
        if (pos.x > higherLimit.x)
	        this.transform.position = new Vector3(higherLimit.x, pos.y, pos.z);

        if (pos.z < lowerLimit.z)
	        this.transform.position = new Vector3(pos.x, pos.y, lowerLimit.z);
        if (pos.z > higherLimit.z)
	        this.transform.position = new Vector3(pos.x, pos.y, higherLimit.z);
    }

    void FixedUpdate()
    {
        // Interaction
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Input.GetButtonDown("Fire1"))
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit,
                    interactionDistance, ~playerLayer))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance,
                    Color.yellow);
                Debug.Log("Did Hit");
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                Debug.Log("Did not Hit");
            }
        }
    }

    public void SetLimits(float minX, float minZ, float maxX, float maxZ)
    {
		lowerLimit = new Vector3(minX, 0, minZ);
		higherLimit = new Vector3(maxX, 0, maxZ);
	}
}
