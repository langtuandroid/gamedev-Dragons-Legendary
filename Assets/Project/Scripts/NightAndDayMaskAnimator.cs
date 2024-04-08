using System;
using UnityEngine;

public class NightAndDayMaskAnimator : MonoBehaviour
{
	public float maxScale = 1f;

	public float speed;

	private void Start()
	{
		base.transform.localScale = new Vector3(0f, 0f, 0f);
	}

	private void Update()
	{
		float num = (Mathf.Sin(Time.time * speed - (float)Math.PI / 2f) + 1f) * 0.5f * maxScale;
		base.transform.localScale = new Vector3(num, num, num);
	}
}
