using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class UniversalGravitation : MonoBehaviour
{
	public float bangSize = 100.0f;
	public float bangPower = 1000.0f;

	Meteorite[] meteorites;

	static Vector3 RandomVector3(float min, float max)
	{
		Vector3 direction;
		direction.x = Random.Range(min, max);
		direction.y = Random.Range(min, max);
		direction.z = Random.Range(min, max);
		return direction.normalized;
	}

	void Awake()
	{
		int i = 0;
		var list = new List<Meteorite>();
		foreach (var prefab in GetComponentsInChildren<Meteorite>().OrderByDescending(x => x.rigidbody.mass))
		{
			int count = (int)Mathf.Pow(2, i++);
			for (int j = 0; j < count; j++)
			{
				var meteorite = Instantiate(prefab);
				meteorite.rigidbody.position = RandomVector3(0.0f, 1.0f) * Random.Range(1.0f, bangSize);
				meteorite.rigidbody.AddForce(RandomVector3(0.0f, 1.0f) * Random.Range(1.0f, bangPower), ForceMode.Acceleration);
				meteorite.rigidbody.AddTorque(RandomVector3(0.0f, 1.0f) * Random.Range(1.0f, bangPower), ForceMode.Acceleration);
				list.Add(meteorite);
			}
			prefab.gameObject.SetActive(false);
		}
		meteorites = list.ToArray();

		CameraController.target = meteorites.First().transform;
	}

	void FixedUpdate()
	{
		for (int i = 0; i < meteorites.Length; i++)
		{
			for (int j = i + 1; j < meteorites.Length; j++)
			{
				var meteorite0 = meteorites[i];
				var meteorite1 = meteorites[j];
				var position0 = meteorite0.transform.position;
				var position1 = meteorite1.transform.position;
				var difference = position1 - position0;
				var sqrMagnitude = difference.sqrMagnitude;
				var gravity = meteorite0.rigidbody.mass * meteorite1.rigidbody.mass / sqrMagnitude;
				var force = difference.normalized * gravity;
				meteorite0.rigidbody.AddForce(force);
				meteorite1.rigidbody.AddForce(-force);
			}
		}
	}
}
