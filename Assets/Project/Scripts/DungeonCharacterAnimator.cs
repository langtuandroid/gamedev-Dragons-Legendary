using System;
using UnityEngine;

public class DungeonCharacterAnimator : MonoBehaviour
{
	private float _initialRandomTime;

	private float _randomSpeed;

	private void Start()
	{
		_initialRandomTime = UnityEngine.Random.Range(0f, 3f);
		_randomSpeed = UnityEngine.Random.Range(1f, 1.7f);
	}

	private void Update()
	{
		float num = Time.time * _randomSpeed + _initialRandomTime;
		Transform transform = base.transform;
		float x = Mathf.Cos(num) * 3f;
		Vector3 position = base.transform.position;
		float y = position.y;
		Vector3 position2 = base.transform.position;
		transform.position = new Vector3(x, y, position2.z);
		if (num % ((float)Math.PI * 2f) < (float)Math.PI)
		{
			base.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		}
		else
		{
			base.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
		}
	}
}
