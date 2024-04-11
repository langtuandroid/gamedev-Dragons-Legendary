using UnityEngine;

public class FogOfWarAlphaPositioner : MonoBehaviour
{
	public Transform dungeonCharacter;

	private void Start()
	{
	}

	private void Update()
	{
		Transform transform = base.transform;
		Vector3 position = dungeonCharacter.position;
		float x = position.x;
		Vector3 position2 = dungeonCharacter.position;
		float y = position2.y;
		Vector3 position3 = base.transform.position;
		transform.position = new Vector3(x, y, position3.z);
	}
}
