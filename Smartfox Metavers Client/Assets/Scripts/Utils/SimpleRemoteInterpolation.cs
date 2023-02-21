using UnityEngine;
using System.Collections;

/**
 * Extremely simple interpolation script.
 * But it works for this example.
 */
public class SimpleRemoteInterpolation : MonoBehaviour
{
    const float DAMPING_FACTOR = 5f;

    private Vector3 desiredPos;
	private Quaternion desiredRot;
	
	void Start()
	{
		desiredPos = this.transform.position;
		desiredRot = this.transform.rotation;
	}
	
	public void SetTransform(Vector3 pos, Quaternion rot, bool interpolate)
	{
		// If interpolation is required, set the desired position + rotation
		if (interpolate)
		{
			desiredPos = pos;
			desiredRot = rot;
		}

        // Otherwise set position + rotation immediately (i.e. when models are spawned)
        else
        {
			this.transform.SetPositionAndRotation(pos, rot);
		}
	}
	
	void Update ()
	{
		this.transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * DAMPING_FACTOR);
		this.transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, Time.deltaTime * DAMPING_FACTOR);
	}
}
