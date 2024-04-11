using UnityEngine;
using UnityEngine.UI;

public class DebugFrameRate : MonoBehaviour
{
	private float deltaTime;

	private Text textFrameRate;

	private void Awake()
	{
		textFrameRate = base.gameObject.GetComponent<Text>();
	}

	private void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		float num = deltaTime * 1000f;
		float num2 = 1f / deltaTime;
		textFrameRate.text = $"({num2:0.} fps)";
	}
}
