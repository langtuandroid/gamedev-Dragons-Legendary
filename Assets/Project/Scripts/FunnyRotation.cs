using System.Collections.Generic;
using UnityEngine;

public class FunnyRotation : MonoBehaviour
{
	public List<Transform> targets;

	public float angularVelocity = 10f;

	public float radialVelocity = 10f;

	public float minRadialDistance = 1f;

	public float maxRadialDistance = 5f;

	private List<int> _curRadialDirection;

	private void Start()
	{
		if (targets == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < targets.Count; i++)
		{
			if (targets[i] != null)
			{
				num++;
			}
		}
		num = Mathf.Max(1, num);
		float num2 = 360 / num;
		float num3 = 0f;
		Vector2 v = new Vector2((minRadialDistance + maxRadialDistance) / 2f, 0f);
		for (int j = 0; j < targets.Count; j++)
		{
			Transform transform = targets[j];
			if (transform != null)
			{
				transform.localPosition = Quaternion.AngleAxis(num3, Vector3.forward) * v;
				num3 += num2;
			}
		}
		_curRadialDirection = new List<int>();
		for (int k = 0; k < targets.Count; k++)
		{
			_curRadialDirection.Add(1);
		}
	}

	private void FixedUpdate()
	{
		if (targets == null)
		{
			return;
		}
		for (int i = 0; i < targets.Count; i++)
		{
			Transform transform = targets[i];
			if (transform != null)
			{
				transform.RotateAround(base.transform.position, Vector3.forward, angularVelocity * Time.deltaTime);
				float magnitude = transform.localPosition.magnitude;
				if (Mathf.Abs(magnitude - minRadialDistance) < 0.001f)
				{
					_curRadialDirection[i] = 1;
				}
				else if (Mathf.Abs(maxRadialDistance - magnitude) < 0.001f)
				{
					_curRadialDirection[i] = -1;
				}
				float b = magnitude + radialVelocity * (float)_curRadialDirection[i] * Time.deltaTime;
				b = Mathf.Min(maxRadialDistance, Mathf.Max(minRadialDistance, b));
				transform.localPosition = transform.localPosition.normalized * b;
			}
		}
	}
}
