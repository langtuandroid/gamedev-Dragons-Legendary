using System.Collections;
using UnityEngine;

public class LogoSpecularityAnimator : MonoBehaviour
{
	public float startX;

	public float endX;

	public float speed;

	public float delay;

	private void Start()
	{
		StartCoroutine(AnimateSpecularity());
	}

	private void Update()
	{
	}

	private IEnumerator AnimateSpecularity()
	{
		while (true)
		{
			Transform transform = base.transform;
			float x = startX;
			Vector3 position = base.transform.position;
			float y = position.y;
			Vector3 position2 = base.transform.position;
			transform.position = new Vector3(x, y, position2.z);
			while (true)
			{
				Vector3 position3 = base.transform.position;
				if (!(position3.x < endX))
				{
					break;
				}
				base.transform.position += new Vector3(Time.deltaTime * speed, 0f, 0f);
				yield return null;
			}
			yield return new WaitForSeconds(delay);
		}
	}
}
