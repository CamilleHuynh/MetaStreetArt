using UnityEngine;

public class PlayerController : MonoBehaviour
{
	const float FORWARD_SPEED = 10;
	const float ROTATION_SPEED = 100;

	private Vector3 lowerLimit;
	private Vector3 higherLimit;

	[SerializeField] private LayerMask playerLayer;
	[SerializeField] private float interactionDistance = 50.0f;

	// Dirty flag for checking if movement was made or not
	public bool MovementDirty {get; set;}

	void Start()
	{
		MovementDirty = false;
	}

	void Update ()
	{
		// Forward/backward makes player model move
		float translation = Input.GetAxis("Vertical");

		if (translation != 0)
		{
			// Translate object
			this.transform.Translate(0, 0, translation * Time.deltaTime * FORWARD_SPEED);

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

			MovementDirty = true;
		}

		// Left/right makes player model rotate around own axis
		float rotation = Input.GetAxis("Horizontal");

		if (rotation != 0)
		{
			this.transform.Rotate(Vector3.up, rotation * Time.deltaTime * ROTATION_SPEED);

			MovementDirty = true;
		}
	}

	void FixedUpdate ()
	{
		// Interaction
		
		RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, interactionDistance, playerLayer))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
	}

	public void SetLimits(float minX, float minZ, float maxX, float maxZ)
    {
		lowerLimit = new Vector3(minX, 0, minZ);
		higherLimit = new Vector3(maxX, 0, maxZ);
	}
}
