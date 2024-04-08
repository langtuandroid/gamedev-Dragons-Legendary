using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyScene : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		Application.targetFrameRate = 60;
		QualitySettings.vSyncCount = 0;
	}

	private void Start()
	{
		Screen.sleepTimeout = -1;
		SceneManager.LoadScene(3);
	}
}
