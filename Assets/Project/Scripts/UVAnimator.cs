using UnityEngine;

public class UVAnimator : MonoBehaviour
{
	public float USpeed;

	public float VSpeed;

	private void Start()
	{
	}

	private void Update()
	{
		Vector2 mainTextureOffset = GetComponent<Renderer>().material.mainTextureOffset + new Vector2(USpeed * Time.deltaTime, VSpeed * Time.deltaTime);
		mainTextureOffset.x %= 1f;
		mainTextureOffset.y %= 1f;
		GetComponent<Renderer>().material.mainTextureOffset = mainTextureOffset;
	}
}
