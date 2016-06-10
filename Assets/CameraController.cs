using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public static Transform target;

	public float distanceFromTarget = 100.0f;

	Quaternion autoRotation;
	Quaternion mouseRotation;

	float mouseWheel;

	void Awake()
	{
		if (gameObject == Camera.main.gameObject)
		{
			var childCamera = GetComponentInChildren<Camera>(true);
			for (int i = 1; i < Display.displays.Length; i++)
			{
				Display.displays[i].Activate();

				var subCamera = Instantiate(childCamera);
				subCamera.gameObject.SetActive(true);
				subCamera.targetDisplay = i;
			}
		}
	}

	IEnumerator Start()
	{
		transform.rotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), 0.0f);
		var rotation = Quaternion.Euler(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0.0f);

		mouseRotation = Quaternion.identity;
		while (true)
		{
			float time = 0.0f;
			while (!Input.GetMouseButton(0))
			{
				autoRotation = Quaternion.Slerp(Quaternion.identity, rotation, Mathf.Clamp01(time));
				yield return null;
				time += Time.deltaTime * 0.1f;
			}

			autoRotation = Quaternion.identity;
			while (Input.GetMouseButton(0))
			{
				var mouseX = Input.GetAxis("Mouse X");
				var mouseY = Input.GetAxis("Mouse Y");
				mouseRotation = Quaternion.Euler(-mouseY, mouseX, 0.0f);
				if (mouseRotation != Quaternion.identity)
				{
					rotation = mouseRotation;
				}
				yield return null;
			}
		}
	}

	void Update()
	{
		mouseWheel = 0.9f * mouseWheel - 10.0f * Input.GetAxis("Mouse ScrollWheel");
		distanceFromTarget = Mathf.Clamp(distanceFromTarget + mouseWheel, 50.0f, 500f);

		transform.rotation *= autoRotation * mouseRotation;
		transform.position = target.position - (transform.forward * distanceFromTarget);
	}
}
