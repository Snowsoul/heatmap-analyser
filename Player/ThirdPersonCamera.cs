using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {

	public bool lockCursor;
	public float mouseSense = 3;
	public Transform target;
	public float dstFromTarget = 2;
	public Vector2 pitchMinMax = new Vector2(-40, 85);

	public float rotationSmoothTime = .12f;

	public float cameraZoomStep = 0.5f;
	public float cameraZoomSmoothTime = .2f;
	public bool lockCamera = false;

	float cameraZoomVelocity;

	Vector3 rotationSmoothVelocity;
	Vector3 currentRotation;

	float yaw;
	float pitch;

	HeatmapTopDownCamera topDownManager;

	// Use this for initialization
	void Start () {

		topDownManager = GetComponent<HeatmapTopDownCamera>();

		if (lockCursor)
		{
			DisableCursor();
        }
	}

	public void DisableCursor()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void EnableCursor ()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (!topDownManager.topDown && !lockCamera)
		{
			yaw += Input.GetAxis("Mouse X") * mouseSense;
			pitch -= Input.GetAxis("Mouse Y") * mouseSense;
			pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

			currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
			transform.eulerAngles = currentRotation - new Vector3(0, 180, 0);
			transform.position = target.position - transform.forward * dstFromTarget;

			float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");

			if (scrollWheelInput > 0)
			{
				dstFromTarget = Mathf.SmoothDamp(dstFromTarget, dstFromTarget - cameraZoomStep, ref cameraZoomVelocity, cameraZoomSmoothTime);
			}
			else if (scrollWheelInput < 0)
			{
				dstFromTarget = Mathf.SmoothDamp(dstFromTarget, dstFromTarget + cameraZoomStep, ref cameraZoomVelocity, cameraZoomSmoothTime);
			}
		}
	}
}
