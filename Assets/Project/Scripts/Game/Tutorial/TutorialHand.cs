using System.Collections;
using UnityEngine;

public class TutorialHand : MonoBehaviour
{
	private const float HAND_UP_MOVE_POS_Y = 1f;

	private const float HAND_UP_MOVE_DURATION = 0.8f;

	private const float HAND_UP_MOVE_DELAY = 0.5f;

	[SerializeField]
	private GameObject goHandUp;

	[SerializeField]
	private GameObject goHandDown;

	private float originalPosY;

	private float movePosY;

	private Transform trHandUp;

	private Vector3 originalPos;

	private Vector3 handUpPosition;

	private Vector3 handDiagonalTargetPosition;

	private Vector3 defaultHandUpPosition = new Vector3(0.55f, -0.55f, 1f);

	public void ShowHandUp(bool isDragAnim = false)
	{
		goHandUp.SetActive(value: true);
		goHandDown.SetActive(value: false);
	}

	public void ShowHandDown()
	{
		goHandUp.SetActive(value: false);
		goHandDown.SetActive(value: true);
	}

	public void ShowHandBottomAnim()
	{
		goHandUp.SetActive(value: true);
		goHandDown.SetActive(value: false);
		trHandUp.localPosition = defaultHandUpPosition;
		Vector3 localPosition = goHandUp.transform.localPosition;
		originalPosY = localPosition.y;
		Vector3 localPosition2 = goHandUp.transform.localPosition;
		movePosY = localPosition2.y - 1f;
		StartCoroutine(ProcessHandBottomAnimation());
	}

	public void ShowHandLeftAnim()
	{
		goHandUp.SetActive(value: true);
		goHandDown.SetActive(value: false);
		trHandUp.localPosition = defaultHandUpPosition;
		Vector3 localPosition = goHandUp.transform.localPosition;
		originalPosY = localPosition.x;
		Vector3 localPosition2 = goHandUp.transform.localPosition;
		movePosY = localPosition2.x - 1f;
		StartCoroutine(ProcessHandLeftAnimation());
	}

	public void ShowHandDiagonalAnim()
	{
		goHandUp.SetActive(value: true);
		goHandDown.SetActive(value: false);
		trHandUp.localPosition = defaultHandUpPosition;
		handDiagonalTargetPosition = new Vector3(originalPos.x - 1f, originalPos.y - 1f, 0f);
		StartCoroutine(ProcessHandDiagonalAnimation());
	}

	public void ShowHandDiagonalAnimTopLeft()
	{
		goHandUp.SetActive(value: true);
		goHandDown.SetActive(value: false);
		trHandUp.localPosition = defaultHandUpPosition;
		handDiagonalTargetPosition = new Vector3(originalPos.x - 1f, originalPos.y + 1f, 0f);
		StartCoroutine(ProcessHandDiagonalAnimation());
	}

	private IEnumerator ProcessHandBottomAnimation()
	{
		handUpPosition = trHandUp.localPosition;
		handUpPosition.y = originalPosY;
		trHandUp.localPosition = handUpPosition;
		LeanTween.moveLocalY(goHandUp, movePosY, 0.8f);
		yield return new WaitForSeconds(1.3f);
		StartCoroutine(ProcessHandBottomAnimation());
	}

	private IEnumerator ProcessHandLeftAnimation()
	{
		handUpPosition = trHandUp.localPosition;
		handUpPosition.x = originalPosY;
		trHandUp.localPosition = handUpPosition;
		LeanTween.moveLocalX(goHandUp, movePosY, 0.8f);
		yield return new WaitForSeconds(1.3f);
		StartCoroutine(ProcessHandLeftAnimation());
	}

	private IEnumerator ProcessHandDiagonalAnimation()
	{
		trHandUp.localPosition = originalPos;
		LeanTween.moveLocal(goHandUp, handDiagonalTargetPosition, 0.8f);
		yield return new WaitForSeconds(1.3f);
		StartCoroutine(ProcessHandDiagonalAnimation());
	}

	private void Awake()
	{
		trHandUp = goHandUp.transform;
		Vector3 localPosition = goHandUp.transform.localPosition;
		originalPosY = localPosition.y;
		originalPos = goHandUp.transform.localPosition;
		Vector3 localPosition2 = goHandUp.transform.localPosition;
		movePosY = localPosition2.x + 1f;
		handDiagonalTargetPosition = default(Vector3);
	}

	private void OnDisable()
	{
		trHandUp.localPosition = defaultHandUpPosition;
		StopAllCoroutines();
	}
}
