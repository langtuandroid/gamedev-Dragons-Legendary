using UnityEngine;

public class GameLoading : MonoBehaviour
{
	private const float LOADING_SPEED = 3f;

	private Vector3 rotationLoading;

	[SerializeField]
	private Transform trBigCircle;

	[SerializeField]
	private Transform trSmallCircle;

	private void Update()
	{
		rotationLoading = trBigCircle.localEulerAngles;
		rotationLoading.z += 3f;
		trBigCircle.localEulerAngles = rotationLoading;
		rotationLoading = trSmallCircle.localEulerAngles;
		rotationLoading.z -= 3f;
		trSmallCircle.localEulerAngles = rotationLoading;
	}
}
