using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HunterAttackUI : MonoBehaviour
{
	private const float ATTACK_UI_TWEEN_DURATION = 0.2f;

	private const float ATTACK_UI_SHOW_DURATION = 2f;

	[SerializeField]
	private List<Text> listTextAttack = new List<Text>();

	private bool isAttackStart;

	private int currentAttack;

	private Vector3 pingPongScale = new Vector3(1.2f, 1.2f, 1.2f);

	private Transform trUI;

	private Coroutine coroutineCount;

	private Coroutine coroutineRemove;

	public void ShowAttack(int attack, int color, Vector3 position)
	{
		if (!isAttackStart)
		{
			isAttackStart = true;
			trUI.position = position;
			for (int i = 0; i < listTextAttack.Count; i++)
			{
				listTextAttack[i].gameObject.SetActive(i == color);
			}
			coroutineCount = StartCoroutine(GameUtil.ProcessCountNumber(listTextAttack[color], 0f, attack, string.Empty));
			LeanTween.scale(listTextAttack[color].gameObject, pingPongScale, 0.2f).setLoopPingPong(1).setEase(LeanTweenType.linear);
		}
		else
		{
			StopCountCoroutine();
			coroutineCount = StartCoroutine(GameUtil.ProcessCountNumber(listTextAttack[color], currentAttack, attack, string.Empty, 4f));
		}
		currentAttack = attack;
	}

	public void Clear()
	{
		isAttackStart = false;
		StopCountCoroutine();
		MWPoolManager.DeSpawn("Effect", trUI);
	}

	private IEnumerator ProcessRemoveText()
	{
		UnityEngine.Debug.Log("ProcessRemoveText Start");
		yield return new WaitForSeconds(2f);
		UnityEngine.Debug.Log("ProcessRemoveText Remove");
		StopCountCoroutine();
	}

	private void StopCountCoroutine()
	{
		if (coroutineCount != null)
		{
			StopCoroutine(coroutineCount);
			coroutineCount = null;
		}
		if (coroutineRemove != null)
		{
			StopCoroutine(coroutineRemove);
			coroutineRemove = null;
		}
	}

	private void Awake()
	{
		trUI = base.gameObject.transform;
	}

	private void OnDisable()
	{
		StopCountCoroutine();
		isAttackStart = false;
	}
}
