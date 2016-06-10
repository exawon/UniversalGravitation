using UnityEngine;
using System.Collections;

/*

	Detonator - A parametric explosion system for Unity
	Created by Ben Throop in August 2009 for the Unity Summer of Code
	
	Simplest use case:
	
	1) Use a prefab
	
	OR
	
	1) Attach a Detonator to a GameObject, either through code or the Unity UI
	2) Either set the Detonator's ExplodeOnStart = true or call Explode() yourself when the time is right
	3) View explosion :)
	
	Medium Complexity Use Case:
	
	1) Attach a Detonator as above 
	2) Change parameters, add your own materials, etc
	3) Explode()
	4) View Explosion
	
	Super Fun Use Case:
	
	1) Attach a Detonator as above
	2) Drag one or more DetonatorComponents to that same GameObject
	3) Tweak those parameters
	4) Explode()
	5) View Explosion
	
	Better documentation is included as a PDF with this package, or is available online. Check the Unity site for a link
	or visit my site, listed below.
	
	Ben Throop
	@ben_throop
*/

[AddComponentMenu("Detonator/Detonator")]
public class Detonator : MonoBehaviour
{

	private static float _baseSize = 30f;
	private static Color _baseColor = new Color(1f, .423f, 0f, .5f);
	private static float _baseDuration = 3f;
	
	/*
		_baseSize reflects the size that DetonatorComponents at size 1 match. Yes, this is really big (30m)
		size below is the default Detonator size, which is more reasonable for typical useage. 
		It wasn't my intention for them to be different, and I may change that, but for now, that's how it is.
	*/
	public float size = 10f;
	public Color color = Detonator._baseColor;
	public bool explodeOnStart = true;
	public float duration = Detonator._baseDuration;
	public float detail = 1f;
	public float upwardsBias = 0f;
	
	public float destroyTime = 7f;
	//sorry this is not auto calculated... yet.
	public bool useWorldSpace = true;
	public Vector3 direction = Vector3.zero;
	
	public Material fireballAMaterial;
	public Material fireballBMaterial;
	public Material smokeAMaterial;
	public Material smokeBMaterial;
	public Material sparksMaterial;
	public Material glowMaterial;
		
	private DetonatorComponent[] components;

	void Awake()
	{
		components = GetComponents<DetonatorComponent>();
	}

	void Start()
	{
		if (explodeOnStart)
		{
			UpdateComponents();
			Explode();
		}
	}

	private float _lastExplosionTime = 1000f;

	void Update()
	{
		if (destroyTime > 0f)
		{
			if (_lastExplosionTime + destroyTime <= Time.time)
			{
				Destroy(gameObject);
			}
		}
	}

	private bool _firstComponentUpdate = true;

	void UpdateComponents()
	{
		if (_firstComponentUpdate)
		{
			foreach (DetonatorComponent component in components)
			{
				component.Init();
				component.SetStartValues();
			}
			_firstComponentUpdate = false;
		}
		
		if (!_firstComponentUpdate)
		{
			float s = size / _baseSize;
			
			Vector3 sdir = new Vector3(direction.x * s, direction.y * s, direction.z * s);
			
			float d = duration / _baseDuration;
			
			foreach (DetonatorComponent component in components)
			{
				if (component.detonatorControlled)
				{
					component.size = component.startSize * s;
					component.timeScale = d;
					component.detail = component.startDetail * detail;
					component.force = new Vector3(component.startForce.x * s + sdir.x, component.startForce.y * s + sdir.y, component.startForce.z * s + sdir.z);
					component.velocity = new Vector3(component.startVelocity.x * s + sdir.x, component.startVelocity.y * s + sdir.y, component.startVelocity.z * s + sdir.z);
					
					//take the alpha of detonator color and consider it a weight - 1=use all detonator, 0=use all components
					component.color = Color.Lerp(component.startColor, color, color.a);
				}
			}
		}
	}

	void Explode()
	{
		_lastExplosionTime = Time.time;
	
		foreach (DetonatorComponent component in components)
		{
			UpdateComponents();
			component.Explode();
		}
	}

	void Reset()
	{
		size = 10f; //this is hardcoded because _baseSize up top is not really the default as much as what we match to
		color = _baseColor;
		duration = _baseDuration;
	}
}
