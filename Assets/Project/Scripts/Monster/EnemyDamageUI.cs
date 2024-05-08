using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EnemyDamageUI : MonoBehaviour
{
	public enum EnemyDamageType
	{
		Normal,
		Weak,
		Critical
	}

	[Serializable]
	public class MonsterDamageData
	{
		public EnemyDamageType Type;

		public Text TextDamage;
	}
	

	[FormerlySerializedAs("listDamageData")] [SerializeField]
	private List<MonsterDamageData> _listDamageData = new List<MonsterDamageData>();

	private int _currentDamage;

	private Text _CurrentTextDamage;

	private Transform _trDamageUI;

	public void OpenDamageUI(EnemyDamageType type, int addDamage, Vector3 position)
	{
		_currentDamage += addDamage;
		position += new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), UnityEngine.Random.Range(-0.2f, 0.2f), 0f);
		_trDamageUI.position = position;
		foreach (MonsterDamageData listDamageDatum in _listDamageData)
		{
			if (listDamageDatum.Type == type)
			{
				listDamageDatum.TextDamage.gameObject.SetActive(value: true);
				listDamageDatum.TextDamage.text = $"{_currentDamage:#,###}";
				Color color = listDamageDatum.TextDamage.color;
				color.a = 255f;
				listDamageDatum.TextDamage.color = color;
				_CurrentTextDamage = listDamageDatum.TextDamage;
				StartCoroutine(TweenDamage(base.gameObject));
			}
			else
			{
				listDamageDatum.TextDamage.gameObject.SetActive(value: false);
			}
		}
	}

	private IEnumerator TweenDamage(GameObject goTarget)
	{
		Vector3 position = goTarget.transform.position;
		float posY = position.y;
		LeanTween.moveY(goTarget, posY + 1.8f, 0.6f).setEaseOutCubic();
		Color textColor = _CurrentTextDamage.color;
		textColor.a = 0f;
		LeanTween.colorText(_CurrentTextDamage.GetComponent<RectTransform>(), textColor, 0.6f).setEaseOutCubic();
		yield return new WaitForSeconds(0.6f);
		StopAllCoroutines();
		MasterPoolManager.ReturnToPool("Effect", base.transform);
	}

	private void Awake()
	{
		_trDamageUI = base.gameObject.transform;
	}

	private void OnDisable()
	{
		_currentDamage = 0;
	}
}
