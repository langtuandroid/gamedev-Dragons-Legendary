using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ComboPhrase : MonoBehaviour
{
	private enum ComboWordType
	{
		Lv1,
		Lv2,
		Lv3
	}

	private const float WORD_TWEEN_DURATION = 0.2f;

	private const float WORD_SHOW_COMBO_DURATION = 0.9f;

	private const float WORD_SHOW_PHRASE_DURATION = 0.5f;

	[SerializeField]
	private Text textCombo;

	[SerializeField]
	private Text textWordLevel1;

	[SerializeField]
	private Text textWordLevel2;

	[SerializeField]
	private Text textWordLevel3;

	[SerializeField]
	private string[] sArrWordLevel1 = new string[0];

	[SerializeField]
	private string[] sArrWordLevel2 = new string[0];

	[SerializeField]
	private string[] sArrWordLevel3 = new string[0];

	private int currentCombo;

	private Vector2 defaultPosition = new Vector2(0f, -365f);

	private Vector3 pingPongScale = new Vector3(1.1f, 1.1f, 1.1f);

	private RectTransform rtComboPhrase;

	public void Init()
	{
		currentCombo = 0;
		rtComboPhrase.anchoredPosition = defaultPosition;
		textCombo.gameObject.SetActive(value: false);
		textWordLevel1.gameObject.SetActive(value: false);
		textWordLevel2.gameObject.SetActive(value: false);
		textWordLevel3.gameObject.SetActive(value: false);
	}

	public void Show(int _combo)
	{
		if (_combo >= 1)
		{
			currentCombo = _combo;
			textCombo.gameObject.SetActive(value: true);
			textCombo.gameObject.transform.localScale = Vector3.one;
			textCombo.text = string.Format("<color=#FCF13E>{0}</color> {1}!", _combo, MasterLocalize.GetData("ingame_play_text_combo"));
			LeanTween.scale(textCombo.gameObject, pingPongScale, 0.2f).setLoopPingPong(1).setEase(LeanTweenType.linear);
			switch (currentCombo)
			{
			case 1:
				SoundController.EffectSound_Play(EffectSoundType.Combo1);
				break;
			case 2:
				SoundController.EffectSound_Play(EffectSoundType.Combo2);
				break;
			case 3:
				SoundController.EffectSound_Play(EffectSoundType.Combo3);
				break;
			case 4:
				SoundController.EffectSound_Play(EffectSoundType.Combo4);
				break;
			default:
				SoundController.EffectSound_Play(EffectSoundType.Combo5);
				break;
			}
		}
	}

	public void Complete()
	{
		if (currentCombo > 0)
		{
			StartCoroutine(ProcessComboComplete());
		}
	}

	private IEnumerator ProcessComboComplete()
	{
		textCombo.gameObject.SetActive(value: false);
		if (currentCombo > 1)
		{
			switch (GetComboType(currentCombo))
			{
			case ComboWordType.Lv1:
				textWordLevel1.gameObject.SetActive(value: true);
				textWordLevel1.text = MasterLocalize.GetData(sArrWordLevel1[Random.Range(0, sArrWordLevel1.Length)]);
				LeanTween.scale(textWordLevel1.gameObject, pingPongScale, 0.2f).setLoopPingPong(1).setEase(LeanTweenType.linear);
				break;
			case ComboWordType.Lv2:
				textWordLevel2.gameObject.SetActive(value: true);
				textWordLevel2.text = MasterLocalize.GetData(sArrWordLevel2[Random.Range(0, sArrWordLevel2.Length)]);
				LeanTween.scale(textWordLevel2.gameObject, pingPongScale, 0.2f).setLoopPingPong(1).setEase(LeanTweenType.linear);
				break;
			case ComboWordType.Lv3:
				textWordLevel3.gameObject.SetActive(value: true);
				textWordLevel3.text = MasterLocalize.GetData(sArrWordLevel3[Random.Range(0, sArrWordLevel3.Length)]);
				LeanTween.scale(textWordLevel3.gameObject, pingPongScale, 0.2f).setLoopPingPong(1).setEase(LeanTweenType.linear);
				break;
			}
			yield return new WaitForSeconds(0.9f);
		}
		LeanTween.moveY(base.gameObject.GetComponent<RectTransform>(), -774f, 0.4f).setEaseInBack();
		yield return new WaitForSeconds(0.4f);
		Init();
	}

	private ComboWordType GetComboType(int _combo)
	{
		if (_combo < 2)
		{
			return ComboWordType.Lv1;
		}
		if (_combo >= 2 && _combo <= 4)
		{
			return ComboWordType.Lv1;
		}
		if (_combo >= 5 && _combo <= 7)
		{
			return ComboWordType.Lv2;
		}
		return ComboWordType.Lv3;
	}

	private void Awake()
	{
		rtComboPhrase = base.gameObject.GetComponent<RectTransform>();
	}
}
