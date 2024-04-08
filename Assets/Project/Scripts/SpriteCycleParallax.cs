using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteCycle))]
public class SpriteCycleParallax : MonoBehaviour
{
	public Transform target;

	public Vector2 factor;

	private SpriteCycle spriteCicle;

	private void Awake()
	{
		spriteCicle = GetComponent<SpriteCycle>();
	}

	private void Start()
	{
		if (!target && (bool)Camera.main)
		{
			target = Camera.main.transform;
		}
	}

	private void Update()
	{
		if ((bool)target && (bool)spriteCicle)
		{
			SpriteCycle spriteCycle = spriteCicle;
			Vector3 position = target.position;
			spriteCycle.position = position.x * factor.x;
			Vector3 localPosition = base.transform.localPosition;
			Vector3 position2 = target.position;
			localPosition.y = position2.y * factor.y;
			base.transform.localPosition = localPosition;
		}
	}
}
