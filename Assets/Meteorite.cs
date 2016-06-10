using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
public class Meteorite : MonoBehaviour
{
	Rigidbody _rigidbody;
	public new Rigidbody rigidbody
	{
		get
		{
			if (_rigidbody == null)
			{
				_rigidbody = GetComponent<Rigidbody>();
			}
			return _rigidbody;
		}
	}

	static Detonator detonator;

	void Awake()
	{
		if (Application.isPlaying && detonator == null)
		{
			detonator = FindObjectOfType<Detonator>();
			detonator.gameObject.SetActive(false);
		}
	}

	#if UNITY_EDITOR
	void Update()
	{
		if (!Application.isPlaying && transform.hasChanged)
		{
			transform.hasChanged = false;

			var size = GetComponent<Renderer>().bounds.size;
			GetComponent<Rigidbody>().mass = size.x * size.y * size.z;
		}
	}
	#endif

	readonly Dictionary<Meteorite, Detonator> explosions = new Dictionary<Meteorite, Detonator>();

	void OnCollisionEnter(Collision collision)
	{
		Meteorite opponent = collision.gameObject.GetComponent<Meteorite>();
		if (!explosions.ContainsKey(opponent))
		{
			var impact = Mathf.Sqrt(rigidbody.mass * opponent.rigidbody.mass * collision.impulse.magnitude * collision.relativeVelocity.magnitude);
			if (impact < 1000.0f)
			{
				return;
			}

			var center = Vector3.zero;
			foreach (var contact in collision.contacts)
			{
				center += contact.point;
			}
			center /= collision.contacts.Length;

			var instance = Instantiate(detonator);
			instance.transform.position = center;
			instance.size = Mathf.Clamp(impact * 0.001f, 1.0f, 10.0f);
			instance.gameObject.SetActive(true);

			AddExplosion(opponent, instance);
			opponent.AddExplosion(this, instance);
		}
	}

	void AddExplosion(Meteorite meteorite, Detonator detonator)
	{
		explosions.Add(meteorite, detonator);

		StartCoroutine(RemoveExplosion(meteorite, detonator));
	}

	IEnumerator RemoveExplosion(Meteorite meteorite, Detonator detonator)
	{
		yield return new WaitForSeconds(detonator.destroyTime);

		explosions.Remove(meteorite);
	}
}
