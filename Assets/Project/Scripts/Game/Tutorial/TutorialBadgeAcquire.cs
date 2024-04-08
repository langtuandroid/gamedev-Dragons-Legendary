using System.Collections;
using UnityEngine;

public class TutorialBadgeAcquire : MonoBehaviour
{
	private int caslteId;

	private int floorId;

	private Vector3 originalSize;

	public void Show(int _seq)
	{
		switch (_seq)
		{
		case 1:
			StartCoroutine(ShowDelayTutorialView());
			break;
		case 2:
			originalSize = LobbyManager.GetFloorBadge(caslteId, floorId).localScale;
			TutorialManager.ShowHighLightUI(LobbyManager.GetFloorBadge(caslteId, floorId));
			LobbyManager.GetFloorBadge(caslteId, floorId).localScale = originalSize;
			break;
		case 5:
			TutorialManager.ReturnHighLightUI(originalSize);
			TutorialManager.ShowHighLightUI(LobbyManager.GetFloorTouchCollect(caslteId, floorId));
			break;
		}
	}

	public void SetCastleAndStoreId(int _castleId, int _floorId)
	{
		caslteId = _castleId;
		floorId = _floorId;
	}

	private IEnumerator ShowDelayTutorialView()
	{
		yield return null;
		LobbyManager.MoveStore(caslteId, floorId);
	}
}
