using UnityEngine;

public class CommonLoading : MonoBehaviour
{
	private const float LOADING_ROTAION_SPEED = -3f;

	[SerializeField]
	private Transform trLoading;

	private Vector3 loadingRotaion;

	private void Awake()
	{
	}

	private void Update()
	{
		if (trLoading != null)
		{
			loadingRotaion = trLoading.rotation.eulerAngles;
			loadingRotaion.z += -3f;
			trLoading.rotation = Quaternion.Euler(loadingRotaion);
		}
	}
}
