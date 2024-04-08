using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
	private const float LOADING_MAX_GAUGE = 494f;

	[SerializeField]
	private Text textVersion;

	[SerializeField]
	private RectTransform rtLoading;
	private void Start()
	{
		StartGame();
	}
	private void StartGame()
	{
		textVersion.text = $"Ver. {Application.version}";
		StartCoroutine(LoadSceneProgress(SceneType.DontDestroy));
	}

	private IEnumerator LoadSceneProgress(SceneType type)
	{
		Vector2 sizeLoading = rtLoading.sizeDelta;
		sizeLoading.x = 0f;
		rtLoading.sizeDelta = sizeLoading;
		AsyncOperation async = SceneManager.LoadSceneAsync(type.ToString());
		while (!async.isDone)
		{
			async.allowSceneActivation = ((double)async.progress > 0.8);
			sizeLoading = rtLoading.sizeDelta;
			sizeLoading.x = async.progress / 1f * 494f;
			rtLoading.sizeDelta = sizeLoading;
			yield return null;
		}
	}

	
}
