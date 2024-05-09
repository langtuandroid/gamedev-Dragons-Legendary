using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class InfoHand : MonoBehaviour
{
	[FormerlySerializedAs("goHandUp")] [SerializeField]
	private GameObject _goUp;

	[FormerlySerializedAs("goHandDown")] [SerializeField]
	private GameObject _downHand;

	private float _startPosY;

	private float _movePosX;

	private Transform _upHandTransform;

	private Vector3 _originalPos;

	private Vector3 _handUpPos;

	private Vector3 _targetPos;

	private Vector3 _defaultHand = new Vector3(0.55f, -0.55f, 1f);

	public void ShowUpHand(bool isDragAnim = false)
	{
		_goUp.SetActive(value: true);
		_downHand.SetActive(value: false);
	}

	public void DownHand()
	{
		_goUp.SetActive(value: false);
		_downHand.SetActive(value: true);
	}

	public void BottomAnimation()
	{
		_goUp.SetActive(value: true);
		_downHand.SetActive(value: false);
		_upHandTransform.localPosition = _defaultHand;
		Vector3 localPosition = _goUp.transform.localPosition;
		_startPosY = localPosition.y;
		Vector3 localPosition2 = _goUp.transform.localPosition;
		_movePosX = localPosition2.y - 1f;
		StartCoroutine(HandBottomAnimation());
	}

	public void HandLeftAnim()
	{
		_goUp.SetActive(value: true);
		_downHand.SetActive(value: false);
		_upHandTransform.localPosition = _defaultHand;
		Vector3 localPosition = _goUp.transform.localPosition;
		_startPosY = localPosition.x;
		Vector3 localPosition2 = _goUp.transform.localPosition;
		_movePosX = localPosition2.x - 1f;
		StartCoroutine(LeftHandAnim());
	}

	public void DiagonalHand()
	{
		_goUp.SetActive(value: true);
		_downHand.SetActive(value: false);
		_upHandTransform.localPosition = _defaultHand;
		_targetPos = new Vector3(_originalPos.x - 1f, _originalPos.y - 1f, 0f);
		StartCoroutine(DiagonalHandAnim());
	}

	public void DiagonalTopLeftHend()
	{
		_goUp.SetActive(value: true);
		_downHand.SetActive(value: false);
		_upHandTransform.localPosition = _defaultHand;
		_targetPos = new Vector3(_originalPos.x - 1f, _originalPos.y + 1f, 0f);
		StartCoroutine(DiagonalHandAnim());
	}

	private IEnumerator HandBottomAnimation()
	{
		_handUpPos = _upHandTransform.localPosition;
		_handUpPos.y = _startPosY;
		_upHandTransform.localPosition = _handUpPos;
		LeanTween.moveLocalY(_goUp, _movePosX, 0.8f);
		yield return new WaitForSeconds(1.3f);
		StartCoroutine(HandBottomAnimation());
	}

	private IEnumerator LeftHandAnim()
	{
		_handUpPos = _upHandTransform.localPosition;
		_handUpPos.x = _startPosY;
		_upHandTransform.localPosition = _handUpPos;
		LeanTween.moveLocalX(_goUp, _movePosX, 0.8f);
		yield return new WaitForSeconds(1.3f);
		StartCoroutine(LeftHandAnim());
	}

	private IEnumerator DiagonalHandAnim()
	{
		_upHandTransform.localPosition = _originalPos;
		LeanTween.moveLocal(_goUp, _targetPos, 0.8f);
		yield return new WaitForSeconds(1.3f);
		StartCoroutine(DiagonalHandAnim());
	}

	private void Awake()
	{
		_upHandTransform = _goUp.transform;
		Vector3 localPosition = _goUp.transform.localPosition;
		_startPosY = localPosition.y;
		_originalPos = _goUp.transform.localPosition;
		Vector3 localPosition2 = _goUp.transform.localPosition;
		_movePosX = localPosition2.x + 1f;
		_targetPos = default(Vector3);
	}

	private void OnDisable()
	{
		_upHandTransform.localPosition = _defaultHand;
		StopAllCoroutines();
	}
}
