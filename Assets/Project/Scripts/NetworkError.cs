using UnityEngine;

public class NetworkError : MonoBehaviour
{
	public void Show()
	{
		UnityEngine.Debug.Log("NetworkError!! ");
		base.gameObject.SetActive(value: true);
		GameDataManager.HideNetworkLoading();
		GameDataManager.HideSceneLoading();
		SoundController.EffectSound_Play(EffectSoundType.OpenPopup);
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	public void OnClickNetworkRetry()
	{
		Hide();
		Protocol_Set.Send_Remain_Protocol();
	}
}
