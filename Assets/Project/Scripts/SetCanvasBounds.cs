using UnityEngine;

public class SetCanvasBounds : MonoBehaviour
{
	public RectTransform canvas;

	public RectTransform panel;

	private Rect lastSafeArea = new Rect(0f, 0f, 0f, 0f);

	private Rect GetSafeArea()
	{
		float x = 0f;
		float y = 0f;
		float width = Screen.width;
		float height = Screen.height;
		return new Rect(x, y, width, height);
	}

	private void Start()
	{
		Rect safeArea = GetSafeArea();
		if (safeArea != lastSafeArea)
		{
			ApplySafeArea(safeArea);
		}
	}

	private void ApplySafeArea(Rect area)
	{
		Vector2 position = area.position;
		Vector2 anchorMax = area.position + area.size;
		position.x /= Screen.width;
		position.y /= Screen.height;
		anchorMax.x /= Screen.width;
		anchorMax.y /= Screen.height;
		panel.anchorMin = position;
		panel.anchorMax = anchorMax;
		lastSafeArea = area;
	}
}
