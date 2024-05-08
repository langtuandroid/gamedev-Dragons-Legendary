using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroAttackUI : MonoBehaviour
{
	[SerializeField]
	private List<Text> listTextAttack = new List<Text>();

	private bool _isAttackStart;

	private int _currentAttack;

	private Vector3 _scale = new Vector3(1.2f, 1.2f, 1.2f);

	private Transform _trUI;

	private Coroutine _coroutineCount;

	private Coroutine _coroutineRemove;

	public void ShowAttack(int attack, int color, Vector3 position)
	{
		if (!_isAttackStart)
		{
			_isAttackStart = true;
			_trUI.position = position;
			for (int i = 0; i < listTextAttack.Count; i++)
			{
				listTextAttack[i].gameObject.SetActive(i == color);
			}
			_coroutineCount = StartCoroutine(GameUtil.ProcessCountNumber(listTextAttack[color], 0f, attack, string.Empty));
			LeanTween.scale(listTextAttack[color].gameObject, _scale, 0.2f).setLoopPingPong(1).setEase(LeanTweenType.linear);
		}
		else
		{
			StopCountCoroutine();
			_coroutineCount = StartCoroutine(GameUtil.ProcessCountNumber(listTextAttack[color], _currentAttack, attack, string.Empty, 4f));
		}
		_currentAttack = attack;
	}

	public void Clear()
	{
		_isAttackStart = false;
		StopCountCoroutine();
		MWPoolManager.DeSpawn("Effect", _trUI);
	}

	private void StopCountCoroutine()
	{
		if (_coroutineCount != null)
		{
			StopCoroutine(_coroutineCount);
			_coroutineCount = null;
		}
		if (_coroutineRemove != null)
		{
			StopCoroutine(_coroutineRemove);
			_coroutineRemove = null;
		}
	}

	private void Awake()
	{
		_trUI = base.gameObject.transform;
	}

	private void OnDisable()
	{
		StopCountCoroutine();
		_isAttackStart = false;
	}
}
