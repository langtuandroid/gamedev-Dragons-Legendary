using System.Collections;
using UnityEngine;

public class LobbyPopupBase : MonoBehaviour
{
	private const float LOBBY_POPUP_ANIM_DURATION = 0.1f;

	[SerializeField]
	private bool isAnim;

	[SerializeField]
	private GameObject goPopup;

	private Transform trLobbyPopup;

	private Vector3 startScaleValue = new Vector3(1.2f, 1.2f, 1.2f);

	private float startPosX = -820f;

	private float endPosX = -720f;

	private Vector3 popupPosition;

	public virtual void Show()
	{
		base.gameObject.SetActive(value: true);
		base.gameObject.transform.SetSiblingIndex(-1);
	}

	public virtual void Hide()
	{
		HideProcessComplete();
		base.gameObject.SetActive(value: false);
	}

	public virtual void HideProcessComplete()
	{
	}

	public void Pause()
	{
		base.gameObject.transform.localPosition = new Vector2(-720f, 0f);
	}

	public void Resume()
	{
		base.gameObject.transform.localPosition = Vector3.zero;
	}

	private IEnumerator StartProcessHide()
	{
		popupPosition = trLobbyPopup.localPosition;
		popupPosition.x = 0f;
		popupPosition.y = 0f;
		trLobbyPopup.localPosition = popupPosition;
		LeanTween.moveLocalX(base.gameObject, startPosX, 0.1f).setEaseOutCubic();
		yield return new WaitForSeconds(0.1f);
		HideProcessComplete();
		base.gameObject.SetActive(value: false);
	}

	private void Awake()
	{
		trLobbyPopup = base.gameObject.transform;
	}
}
