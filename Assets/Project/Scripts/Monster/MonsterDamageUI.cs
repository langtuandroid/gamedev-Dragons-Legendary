using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterDamageUI : MonoBehaviour
{
	public enum MonsterDamageType
	{
		Normal,
		Weak,
		Critical
	}

	[Serializable]
	public class MonsterDamageData
	{
		public MonsterDamageType Type;

		public Text TextDamage;
	}

	private const float DAMAGE_TWEEN_MOTION_Y = 1.8f;

	private const float DAMAGE_UI_TWEEN_DURATION = 0.6f;

	private const float DAMAGE_UI_SHOW_DURATION = 0.8f;

	[SerializeField]
	private List<MonsterDamageData> listDamageData = new List<MonsterDamageData>();

	private int currentDamage;

	private Text CurrentTextDamage;

	private Transform trDamageUI;

	public void ShowDamageUI(MonsterDamageType type, int addDamage, Vector3 position)
	{
		currentDamage += addDamage;
		position += new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), UnityEngine.Random.Range(-0.2f, 0.2f), 0f);
		trDamageUI.position = position;
		foreach (MonsterDamageData listDamageDatum in listDamageData)
		{
			if (listDamageDatum.Type == type)
			{
				listDamageDatum.TextDamage.gameObject.SetActive(value: true);
				listDamageDatum.TextDamage.text = $"{currentDamage:#,###}";
				Color color = listDamageDatum.TextDamage.color;
				color.a = 255f;
				listDamageDatum.TextDamage.color = color;
				CurrentTextDamage = listDamageDatum.TextDamage;
				StartCoroutine(ProcessTweenDamage(base.gameObject));
			}
			else
			{
				listDamageDatum.TextDamage.gameObject.SetActive(value: false);
			}
		}
	}

	private IEnumerator ProcessTweenDamage(GameObject goTarget)
	{
		Vector3 position = goTarget.transform.position;
		float posY = position.y;
		LeanTween.moveY(goTarget, posY + 1.8f, 0.6f).setEaseOutCubic();
		Color textColor = CurrentTextDamage.color;
		textColor.a = 0f;
		LeanTween.colorText(CurrentTextDamage.GetComponent<RectTransform>(), textColor, 0.6f).setEaseOutCubic();
		yield return new WaitForSeconds(0.6f);
		StopAllCoroutines();
		MWPoolManager.DeSpawn("Effect", base.transform);
	}

	private void Awake()
	{
		trDamageUI = base.gameObject.transform;
	}

	private void OnDisable()
	{
		currentDamage = 0;
	}
}
