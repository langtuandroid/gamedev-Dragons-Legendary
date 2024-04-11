using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboMultiply : MonoBehaviour
{
	private const float COMBO_MULTIPLY_TWEEN_DURATION = 0.1f;

	private const float COMBO_MULTIPLY_SHOW_DURATION = 0.2f;

	[SerializeField]
	private List<Text> listTextMultiply = new List<Text>();

	private float comboDuration;

	private Transform trCombo;

	private Vector3 pingPongScale = new Vector3(1.2f, 1.2f, 1.2f);

	private Coroutine coroutineCombo;

	public void ShowCombo(int color, float combo, Vector3 position)
	{
		if (color < listTextMultiply.Count)
		{
			trCombo.position = position;
			for (int i = 0; i < listTextMultiply.Count; i++)
			{
				listTextMultiply[i].gameObject.SetActive(i == color);
				listTextMultiply[i].text = $"{1f + (combo - 1f) / 10f} x";
			}
			ShowComboUI(listTextMultiply[color].gameObject);
		}
	}

	private void ShowComboUI(GameObject goCombo)
	{
		if (coroutineCombo != null)
		{
			StopCoroutine(coroutineCombo);
			coroutineCombo = null;
		}
		trCombo.localScale = Vector3.one;
		coroutineCombo = StartCoroutine(ProcessShowComboUI(goCombo));
	}

	private IEnumerator ProcessShowComboUI(GameObject goCombo)
	{
		LeanTween.scale(goCombo, pingPongScale, comboDuration).setLoopPingPong(1).setEase(LeanTweenType.linear);
		if (comboDuration > 0.1f)
		{
			comboDuration -= 0.02f;
		}
		yield return new WaitForSeconds(comboDuration * 2f + 0.2f);
		MWPoolManager.DeSpawn("Effect", trCombo);
		comboDuration = 0.24f;
	}

	private void Awake()
	{
		trCombo = base.gameObject.transform;
		comboDuration = 0.24f;
	}

	private void OnEnable()
	{
		comboDuration = 0.24f;
	}
}
