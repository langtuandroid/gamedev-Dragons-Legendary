using System.Collections;
using UnityEngine;

public class InfoBadgeAcquire : MonoBehaviour
{
	private int _castleId;

	private int _floorId;

	private Vector3 _thisSide;

	public void Open(int _seq)
	{
		switch (_seq)
		{
		case 1:
			StartCoroutine(DefaultTutorialView());
			break;
		case 2:
			_thisSide = LobbyManager.GetFloorBadge(_castleId, _floorId).localScale;
			InfoManager.HighLightUI(LobbyManager.GetFloorBadge(_castleId, _floorId));
			LobbyManager.GetFloorBadge(_castleId, _floorId).localScale = _thisSide;
			break;
		case 5:
			InfoManager.ReturnUILight(_thisSide);
			InfoManager.HighLightUI(LobbyManager.GetFloorTouchCollect(_castleId, _floorId));
			break;
		}
	}

	public void SetStoreID(int _castleId, int _floorId)
	{
		this._castleId = _castleId;
		this._floorId = _floorId;
	}

	private IEnumerator DefaultTutorialView()
	{
		yield return null;
		LobbyManager.MoveStore(_castleId, _floorId);
	}
}
