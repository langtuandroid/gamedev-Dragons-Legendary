using UnityEngine;

public class ShipMover : MonoBehaviour
{
	private Vector3 _primaryPosition = Vector3.zero;

	private void Start()
	{
		_primaryPosition = base.transform.position;
	}

	private void Update()
	{
		base.transform.position = _primaryPosition + new Vector3(0f, Mathf.Sin(Time.time * 2f) * 0.2f, 0f);
		base.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Cos((0f - Time.time) * 2f) * 5f);
	}
}
