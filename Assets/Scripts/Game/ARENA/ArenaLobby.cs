using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaLobby : LobbyPopupBase
{
	public enum ArenaState
	{
		Open,
		Close
	}

	public Action GoBackEvent;

	private const string ARENA_OPEN_END_TIME_KEY = "ArenaOpenEndTime";

	private const string ARENA_REOPEN_TIME_KEY = "ArenaReOpenTime";

	[SerializeField]
	private Text textColorBuff;

	[SerializeField]
	private Text textTribeBuff;

	[SerializeField]
	private Text textEndTimer;

	[SerializeField]
	private Text textReOpenTiemr;

	[SerializeField]
	private Text textCurrentTicketCost;

	[SerializeField]
	private Text textLastLevel;

	[SerializeField]
	private Button btnPlay;

	[SerializeField]
	private Button btnShop;

	[SerializeField]
	private Transform trLevelCellParent;

	[SerializeField]
	private Transform trHunterSpawnAnchor;

	[SerializeField]
	private Transform trLevelContent;

	[SerializeField]
	private Transform trLevelContentDimmed;

	[SerializeField]
	private ScrollRect scrollRect;

	[SerializeField]
	private GameObject goPlayOnButton;

	[SerializeField]
	private GameObject goPlayOffButton;

	[SerializeField]
	private GameObject goShopOnButton;

	[SerializeField]
	private GameObject goShopOffButton;

	[SerializeField]
	private GameObject goInfoOn;

	[SerializeField]
	private GameObject goInfoOff;

	[SerializeField]
	private GameObject goEndContent;

	[SerializeField]
	private GameObject goInfo;

	[SerializeField]
	private GameObject goLevelAllClear;

	[SerializeField]
	private GameObject[] arrGoColorBuff = new GameObject[0];

	[SerializeField]
	private GameObject[] arrGoTribeBuff = new GameObject[0];

	[SerializeField]
	private ArenaLobbyTopUI topUI;

	private TimeSpan timeSpan;

	private Coroutine coroutineResume;

	private ArenaState state;

	private ARENA_INFO_DATA_RESULT infoDataResult;

	private ARENA_LEVEL_DATA activeLevelData;

	private List<Transform> listLastHunter = new List<Transform>();

	private float i;

	public bool HasOpen => base.gameObject.activeSelf;

	public Vector3 UserTicketPosition => topUI.UserTicketPosition;

	public Transform ArenaOpenTimer => textEndTimer.transform;

	public Transform LevelContent => trLevelContent;

	public Transform LevelContentDimmed => trLevelContentDimmed;

	public Transform ArenaShopButton => btnShop.transform;

	public override void Show()
	{
		UnityEngine.Debug.Log("Arena Lobby Show");
		LocalTimeCheckManager.InitType("ArenaOpenEndTime");
		LocalTimeCheckManager.InitType("ArenaReOpenTime");
		LocalTimeCheckManager.OnTimeTick = (Action<string, float>)Delegate.Combine(LocalTimeCheckManager.OnTimeTick, new Action<string, float>(OnTimeTickEvent));
		LocalTimeCheckManager.OnLocalTimeComplete = (Action<string>)Delegate.Combine(LocalTimeCheckManager.OnLocalTimeComplete, new Action<string>(OnLocalTimeComplete));
		Protocol_Set.Protocol_arena_info_Req(OnArenaInfoDataComplete);
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void HideProcessComplete()
	{
	}

	public void RefreshUserData()
	{
		topUI.RefreshData(isCount: true);
	}

	public void ShowHunters()
	{
		HideHunters();
		ShowLastWaveHunter();
	}

	public void HideHunters()
	{
		RemoveHunter();
	}

	public void ShowArenaTopUI()
	{
		topUI.Show();
		ShowHunters();
	}

	public void HideArenaTopUI()
	{
		topUI.Hide();
		HideHunters();
	}

	public void EndArenaTutorial()
	{
		ShowArenaSalePack();
	}

	private void Init()
	{
		if (!base.gameObject.activeSelf)
		{
			base.Show();
		}
		topUI.Show();
		LobbyManager.HideTopUI();
		if (infoDataResult.arenaInfo.endRemainTime > 0)
		{
			state = ArenaState.Open;
		}
		else
		{
			state = ArenaState.Close;
		}
		GameUtil.SetUseArenaHunterList();
		GameUtil.SetOwnHunterList(HUNTERLIST_TYPE.ARENA);
		RefreshData();
		LobbyManager.ShowArenaEventEnd(infoDataResult.rewardInfo);
		TutorialManager.CheckArenaLobbyEnter();
		ShowArenaSalePack();
	}

	private void ShowArenaSalePack()
	{
		if (!TutorialManager.DialogState)
		{
			if (GameInfo.isShowArenaSalePack)
			{
				LobbyManager.ShowSaleArenaPack();
			}
			GameInfo.isShowArenaSalePack = false;
		}
	}

	private void RefreshData()
	{
		LocalTimeCheckManager.TimeClear("ArenaOpenEndTime");
		LocalTimeCheckManager.TimeClear("ArenaReOpenTime");
		goPlayOnButton.SetActive(state == ArenaState.Open);
		goPlayOffButton.SetActive(state == ArenaState.Close);
		goShopOnButton.SetActive(state == ArenaState.Open);
		goShopOffButton.SetActive(state == ArenaState.Close);
		btnPlay.enabled = (state == ArenaState.Open);
		btnShop.enabled = (state == ArenaState.Open);
		ArenaLevelCell[] componentsInChildren = trLevelCellParent.GetComponentsInChildren<ArenaLevelCell>(includeInactive: true);
		foreach (ArenaLevelCell arenaLevelCell in componentsInChildren)
		{
			MWPoolManager.DeSpawn("Lobby", arenaLevelCell.transform);
		}
		Monster[] componentsInChildren2 = trHunterSpawnAnchor.GetComponentsInChildren<Monster>(includeInactive: true);
		foreach (Monster monster in componentsInChildren2)
		{
			MWPoolManager.DeSpawn("Monster", monster.transform);
		}
		topUI.RefreshData();
		switch (state)
		{
		case ArenaState.Open:
		{
			goInfoOn.SetActive(value: true);
			goInfoOff.SetActive(value: false);
			goEndContent.SetActive(value: false);
			textEndTimer.gameObject.SetActive(value: true);
			UnityEngine.Debug.Log("# :: " + infoDataResult.arenaInfo.color);
			string data = MWLocalize.GetData(GameDataManager.GetHunterColorName(infoDataResult.arenaInfo.color).colorOccupation);
			textColorBuff.text = $"{data} X{infoDataResult.arenaInfo.color_buff}";
			string data2 = MWLocalize.GetData(GameDataManager.GetHunterTribeName(infoDataResult.arenaInfo.tribe));
			textTribeBuff.text = $"{data2} X{infoDataResult.arenaInfo.tribe_buff}";
			timeSpan = TimeSpan.FromSeconds(infoDataResult.arenaInfo.endRemainTime);
			string arg = $"{timeSpan.Hours + timeSpan.Days * 24}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
			textEndTimer.text = string.Format("{0} {1}", MWLocalize.GetData("arena_lobby_text_01"), arg);
			LocalTimeCheckManager.AddTimer("ArenaOpenEndTime", infoDataResult.arenaInfo.endRemainTime);
			for (int k = 0; k < arrGoColorBuff.Length; k++)
			{
				arrGoColorBuff[k].SetActive(k != infoDataResult.arenaInfo.color);
			}
			for (int l = 0; l < arrGoTribeBuff.Length; l++)
			{
				arrGoTribeBuff[l].SetActive(l + 1 != infoDataResult.arenaInfo.tribe);
			}
			ShowLevelCell();
			ShowLastWaveHunter();
			break;
		}
		case ArenaState.Close:
			goInfoOn.SetActive(value: false);
			goInfoOff.SetActive(value: true);
			goEndContent.SetActive(value: true);
			textEndTimer.gameObject.SetActive(value: false);
			if (infoDataResult.userArenaInfo.arenaLevel > 0)
			{
				textLastLevel.text = string.Format(MWLocalize.GetData("arena_lobby_text_03"), infoDataResult.userArenaInfo.arenaLevel);
			}
			else
			{
				textLastLevel.text = MWLocalize.GetData("arena_lobby_text_08");
			}
			ShowReOpenTimer(infoDataResult.arenaInfo.nextOpenRemainTime);
			LocalTimeCheckManager.AddTimer("ArenaReOpenTime", infoDataResult.arenaInfo.nextOpenRemainTime);
			break;
		}
	}

	private void ShowReOpenTimer(float second)
	{
		timeSpan = TimeSpan.FromSeconds(second);
		string arg = $"{timeSpan.Hours + timeSpan.Days * 24}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
		textReOpenTiemr.text = $"{arg}";
	}

	private void ShowLevelCell()
	{
		int num = -1;
		for (int i = 0; i < infoDataResult.arenaLevelList.Length; i++)
		{
			ArenaLevelCell component = MWPoolManager.Spawn("Lobby", "Cell_Arena", trLevelCellParent).GetComponent<ArenaLevelCell>();
			UnityEngine.Debug.Log("i :: " + i + ", cell :: " + component);
			component.Init(infoDataResult.arenaLevelList[i]);
			if (infoDataResult.arenaLevelList[i].passYn == "today")
			{
				activeLevelData = infoDataResult.arenaLevelList[i];
				infoDataResult.activeLevelData = activeLevelData;
				num = i;
			}
		}
		scrollRect.horizontalNormalizedPosition = (float)num / (float)infoDataResult.arenaLevelList.Length;
		if (num < 0)
		{
			btnPlay.enabled = false;
			goLevelAllClear.SetActive(value: true);
			textCurrentTicketCost.text = string.Empty;
			goPlayOnButton.SetActive(value: false);
			goPlayOffButton.SetActive(value: true);
		}
		else
		{
			goLevelAllClear.SetActive(value: false);
			textCurrentTicketCost.text = $"{activeLevelData.costTicket}";
		}
	}

	private void ShowLastWaveHunter()
	{
		if (state == ArenaState.Close || !LobbyManager.CheckShowArenaLobbyHunter())
		{
			return;
		}
		for (int i = 0; i < infoDataResult.lastWaveMonsterList.Length; i++)
		{
			Monster component = MWPoolManager.Spawn("Monster", $"{GameDataManager.GetMonsterStatData(infoDataResult.lastWaveMonsterList[i]).mMonsterIdx}").GetComponent<Monster>();
			component.MonsterAnim.GetComponent<MeshRenderer>().sortingOrder = 1;
			component.MonsterUIOnOff(_onoff: false);
			component.transform.position = trHunterSpawnAnchor.position;
			component.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
			if (infoDataResult.lastWaveMonsterList.Length > 1)
			{
				Vector3 position = component.transform.position;
				if (i == 0)
				{
					position.x -= 1.8f;
				}
				else
				{
					position.x += 1.8f;
				}
				component.transform.position = position;
			}
			listLastHunter.Add(component.transform);
		}
	}

	private void OnTimeTickEvent(string type, float second)
	{
		if (type == "ArenaOpenEndTime")
		{
			if (second < 0f)
			{
				second = 0f;
			}
			float num = Mathf.Floor(second);
			TimeSpan timeSpan = TimeSpan.FromSeconds(num);
			string arg = $"{timeSpan.Hours + timeSpan.Days * 24}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
			textEndTimer.text = string.Format("{0} {1}", MWLocalize.GetData("arena_lobby_text_01"), arg);
		}
		else if (type == "ArenaReOpenTime")
		{
			if (second < 0f)
			{
				second = 0f;
			}
			float second2 = Mathf.Floor(second);
			ShowReOpenTimer(second2);
		}
	}

	private void OnLocalTimeComplete(string type)
	{
		if (type == "ArenaOpenEndTime" || type == "ArenaReOpenTime")
		{
			Protocol_Set.Protocol_arena_info_Req(OnArenaInfoDataComplete);
		}
	}

	private void OnArenaInfoDataComplete(ARENA_INFO_DATA_RESULT _result)
	{
		infoDataResult = _result;
		if (infoDataResult == null)
		{
			if (GoBackEvent != null)
			{
				GoBackEvent();
			}
			return;
		}
		ArenaLevelCell[] componentsInChildren = trLevelCellParent.GetComponentsInChildren<ArenaLevelCell>(includeInactive: true);
		foreach (ArenaLevelCell arenaLevelCell in componentsInChildren)
		{
			MWPoolManager.DeSpawn("Lobby", arenaLevelCell.transform);
		}
		RemoveHunter();
		Init();
	}

	private IEnumerator ProcessResumeCallArenaInfo()
	{
		yield return new WaitForSeconds(0.2f);
		Protocol_Set.Protocol_arena_info_Req(OnArenaInfoDataComplete);
		coroutineResume = null;
	}

	private void RemoveHunter()
	{
		foreach (Transform item in listLastHunter)
		{
			MWPoolManager.DeSpawn("Monster", item);
		}
		listLastHunter.Clear();
	}

	public void OnClickBack()
	{
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}

	public void OnClickPlay()
	{
		LobbyManager.ShowArenaLevelPlay(infoDataResult);
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickShowInfo()
	{
		goInfo.SetActive(value: true);
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickHideInfo()
	{
		goInfo.SetActive(value: false);
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}

	private void OnDisable()
	{
		LocalTimeCheckManager.TimeClear("ArenaOpenEndTime");
		LocalTimeCheckManager.TimeClear("ArenaReOpenTime");
		LocalTimeCheckManager.OnTimeTick = (Action<string, float>)Delegate.Remove(LocalTimeCheckManager.OnTimeTick, new Action<string, float>(OnTimeTickEvent));
		LocalTimeCheckManager.OnLocalTimeComplete = (Action<string>)Delegate.Remove(LocalTimeCheckManager.OnLocalTimeComplete, new Action<string>(OnLocalTimeComplete));
		LocalTimeCheckManager.SaveAndExit("ArenaOpenEndTime");
		LocalTimeCheckManager.SaveAndExit("ArenaReOpenTime");
		ArenaLevelCell[] componentsInChildren = trLevelCellParent.GetComponentsInChildren<ArenaLevelCell>(includeInactive: true);
		foreach (ArenaLevelCell arenaLevelCell in componentsInChildren)
		{
			MWPoolManager.DeSpawn("Lobby", arenaLevelCell.transform);
		}
		RemoveHunter();
		topUI.Hide();
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus)
		{
			if (coroutineResume != null)
			{
				StopCoroutine(coroutineResume);
				coroutineResume = null;
			}
			coroutineResume = StartCoroutine(ProcessResumeCallArenaInfo());
		}
	}
}
