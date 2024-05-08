using UnityEngine;
using UnityEngine.UI;

public class UpdateNotice : MonoBehaviour
{
	[SerializeField]
	private Text textUpdateTitle;

	[SerializeField]
	private Text textUpdtateMessage;

	private CHECK_VERSION_RESULT resultData;

	public void Show(CHECK_VERSION_RESULT _resultData)
	{
		base.gameObject.SetActive(value: true);
		resultData = _resultData;
		textUpdateTitle.text = MasterLocalize.GetData(_resultData.title);
		textUpdtateMessage.text = MasterLocalize.GetData(_resultData.message);
		SoundController.EffectSound_Play(EffectSoundType.OpenPopup);
	}

	public void OnClickGotoUpdate()
	{
		Application.OpenURL(resultData.link);
	}
}
