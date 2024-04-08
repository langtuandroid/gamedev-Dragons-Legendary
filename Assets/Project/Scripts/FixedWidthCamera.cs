using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FixedWidthCamera : MonoBehaviour
{
	public float screenWidth = 720f;

	private Camera camera;

	private float size;

	private float ratio;

	private float screenHeight;

	private float deviceWidth = 720f;

	private float deviceHeight = 1280f;

	private void Awake()
	{
		camera = GetComponent<Camera>();
		ratio = (float)Screen.height / (float)Screen.width;
		if (deviceHeight / deviceWidth < ratio)
		{
			camera.orthographicSize = ratio * 720f / 200f;
		}
	}
}
