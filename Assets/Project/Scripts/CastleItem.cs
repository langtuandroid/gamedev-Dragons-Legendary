using UnityEngine;

public class CastleItem : MonoBehaviour
{
	[SerializeField]
	private GameObject goActive;

	[SerializeField]
	private GameObject goDeActive;

	[SerializeField]
	private GameObject goClose;

	[SerializeField]
	private GameObject goLock;

	[SerializeField]
	private Transform trNoticeAnchor;

	private int castleIdx;

	private LobbyCastleType castleType = LobbyCastleType.Lock;

	private Transform trInGameNotice;

	public LobbyCastleType CastleType => castleType;

	public void SetIndex(int _index)
	{
		castleIdx = _index;
	}

	public void SetType(LobbyCastleType _type)
	{
		castleType = _type;
		RefreshState();
	}

	public void ShowNotice()
	{
		ClearNotice();
		trInGameNotice = MasterPoolManager.SpawnObject("Lobby", "Notice_Red", trNoticeAnchor);
	}

	public void ClearNotice()
	{
		if (trInGameNotice != null)
		{
			MasterPoolManager.ReturnToPool("Lobby", trInGameNotice);
			trInGameNotice = null;
		}
	}

	private void RefreshState()
	{
		switch (castleType)
		{
		case LobbyCastleType.Active:
			goActive.SetActive(value: true);
			goDeActive.SetActive(value: false);
			goClose.SetActive(value: false);
			goLock.SetActive(value: false);
			break;
		case LobbyCastleType.DeActive:
			goActive.SetActive(value: false);
			goDeActive.SetActive(value: true);
			goClose.SetActive(value: false);
			goLock.SetActive(value: false);
			break;
		case LobbyCastleType.Close:
			goActive.SetActive(value: false);
			goDeActive.SetActive(value: false);
			goClose.SetActive(value: true);
			goLock.SetActive(value: false);
			break;
		case LobbyCastleType.Lock:
			goActive.SetActive(value: false);
			goDeActive.SetActive(value: false);
			goClose.SetActive(value: false);
			goLock.SetActive(value: true);
			break;
		}
	}

	public void OnClickCastle()
	{
		if (castleType == LobbyCastleType.DeActive)
		{
			LobbyManager.MoveCastle(castleIdx);
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		}
	}

	private void OnDisable()
	{
		ClearNotice();
	}
}
