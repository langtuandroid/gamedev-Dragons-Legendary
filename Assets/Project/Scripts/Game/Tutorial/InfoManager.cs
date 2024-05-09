using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InfoManager : GameObjectSingleton<InfoManager>
{
	[FormerlySerializedAs("dialogue")] [SerializeField]
	private InfoDialogue _dialogueInfo;

	[FormerlySerializedAs("imageDimmed")] [SerializeField]
	private Image _imageDimm;

	[FormerlySerializedAs("trTutorialUI")] [SerializeField]
	private Transform _uiTutorial;

	private bool _isActiveIsTutorial;

	private bool _isDimmendClick = true;

	private int _origDepth = -1;

	private int _idMust;

	private int _seqMust;
	
	private int _currentID;
	
	private int _thisSequence;

	private Button _dimmedButton;

	private InfoType _tutorialType = InfoType.None;

	private Transform _highLight;

	private Transform _origParent;

	private Transform _copyObject;

	private Transform _handTransform;

	private USER_GET_TUTORIAL_RESULT _tutorialData;

	private Color _colorDimmed = new Color(0f, 0f, 0f, 0.7f);

	private Color _colorTransp = new Color(0f, 0f, 0f, 0f);

	private InfoIntro _infoTutor;

	private InfoBattleProgress _battleProgress;

	private InfoStoreManagement _storeManger;

	private InfoNewStoreOpen _newStore;

	private InfoBadgeAcquire _bughAcquire;

	private InfoChest _chestTutor;

	private InfoHunterLevelUp _hunterLevel;

	private InfoStoreUpgrade _upgradeStore;

	private InfoHunterSkill _hunterSkill;

	private InfoDailyShop _dailyShop;

	private InfoJewelChest ChestJevel;

	private InfoDragMatch _dragMatch;

	private InfoHunterEnchant _hunterEnchant;

	private InfoLeaderSkill _leaderSkill;

	private InfoArenaOpen _arenaOpen;

	private InfoArenaEnter _arenaEnter;

	private List<InfoDepthData> _dethData = new List<InfoDepthData>();

	public static int ID => Inst._currentID;

	public static int Seq => Inst._thisSequence;

	public static bool Intro => Inst._currentID == 0;

	public static bool DialogState => Inst._dialogueInfo.gameObject.activeSelf;

	public static bool CurrentProgress
	{
		get
		{
			if (Inst._currentID == 0)
			{
				return true;
			}
			if (Inst._currentID == 1 && (Inst._thisSequence == 8 || Inst._thisSequence == 9))
			{
				return true;
			}
			return false;
		}
	}

	public static void ConfgureData(USER_GET_TUTORIAL_RESULT _data)
	{
		Inst._tutorialData = _data;
		Inst._isActiveIsTutorial = Inst.CheckTutorialState();
		Inst._currentID = _data.mustData.sIndex;
		Inst._thisSequence = _data.mustData.seq;
		Inst._idMust = _data.mustData.sIndex;
		Inst._seqMust = _data.mustData.seq;
		Debug.Log("SetTutorialData - currentSIdx :: " + Inst._thisSequence + ", currentSeq :: " + Inst._thisSequence);
		GameInfo.isTutorial = Inst._isActiveIsTutorial;
		if (GameInfo.isTutorial)
		{
		}
	}

	public static void LookTutorial()
	{
		if (!Inst._isActiveIsTutorial)
		{
			return;
		}
		if (Inst._currentID > -1)
		{
			Inst.OpenTutorial(Inst._currentID, Inst._thisSequence);
		}
		if (Inst._tutorialData.eventData[1].passYn == "n")
		{
			Debug.Log("CheckTutorial OnStoreBadgeAcquire");
			LobbyManager.StoreBadgeAcquire = Inst.StoreBack;
		}
		if (Inst._tutorialData.eventData[2].passYn == "n" && LobbyManager.HasFirstFloorItemUpgrade)
		{
			Inst.OpenTutorial(8, 1);
			SaveAllData();
			Inst._tutorialData.eventData[2].passYn = "y";
		}
		if (Inst._tutorialData.eventData[3].passYn == "n")
		{
			Debug.Log("CheckTutorial DAILY SHOP 11");
			if (LobbyManager.DailyShopOpen == null)
			{
				Debug.Log("CheckTutorial DAILY SHOP 22");
				LobbyManager.DailyShopOpen = Inst.DailyShopOpen;
			}
		}
		if (Inst._tutorialData.eventData[4].passYn == "n")
		{
			LobbyManager.OpenQuickLoot = Inst.EnterQuick;
			LobbyManager.QuickLootRefresh = Inst.RefreshEvent;
		}
		if (Inst._tutorialData.eventData[5].passYn == "n")
		{
			if (LobbyManager.Chapter2Clear == null)
			{
				LobbyManager.Chapter2Clear = Inst.OnTutorialBlockClear;
			}
			if (LobbyManager.Chapter2Clear != null)
			{
				LobbyManager.Chapter2Clear();
			}
		}
		if (Inst._tutorialData.eventData[7].passYn == "n")
		{
			if (LobbyManager.Chapter4Clear == null)
			{
				LobbyManager.Chapter4Clear = Inst.ChapterCleared;
			}
			if (LobbyManager.Chapter4Clear != null)
			{
				LobbyManager.Chapter4Clear();
			}
		}
		if (Inst._tutorialData.eventData[9].passYn == "n" && GameInfo.userData.userStageState[0].chapterList.Length == 4)
		{
			Inst.OpenTutorial(15, 1);
			Inst._tutorialData.eventData[9].passYn = "y";
			SaveAllData(15, 0);
		}
	}

	public static void IsGamerTurn()
	{
		if (!Inst._isActiveIsTutorial)
		{
			return;
		}
		if (Inst._tutorialData.eventData[0].passYn == "n")
		{
			Debug.Log("USERSKILL TUTORIAL 11");
			if (GameInfo.inGamePlayData.level >= 4 && PuzzlePlayManager.CheckIsUseHunterSkill() != null)
			{
				Debug.Log("USERSKILL TUTORIAL 22");
				PuzzlePlayManager.OnHunterSkill = Inst.SkillHunter;
				PuzzlePlayManager.OnHunterSkill();
			}
		}
		else
		{
			Debug.Log("USERSKILL TUTORIAL 33");
		}
		if (Inst._currentID == 0)
		{
			int num = Inst._thisSequence;
			if (num == 2 || num == 7)
			{
				OpenTutorial();
			}
		}
		if (Inst._currentID == 1)
		{
			int num2 = Inst._thisSequence;
			if (num2 == 2 || num2 == 8 || num2 == 9)
			{
				OpenTutorial();
			}
		}
		else if (Inst._currentID == 2)
		{
			int num3 = Inst._thisSequence;
			if (num3 == 9 || num3 == 11)
			{
				OpenTutorial();
			}
		}
		else if (Inst._currentID == 3)
		{
			int num4 = Inst._thisSequence;
			if (num4 == 1)
			{
				OpenTutorial();
			}
		}
	}

	public static void ArenaEnterCheck()
	{
		if (Inst._isActiveIsTutorial)
		{
			if (Inst._tutorialData.eventData[10].passYn == "n" && GameInfo.userData.userInfo.arenaAlarmYn == "y")
			{
				Inst.OpenTutorial(16, 1);
				Inst._tutorialData.eventData[10].passYn = "y";
				SaveAllData(16, 0);
			}
			if (Inst._tutorialData.eventData[11].passYn == "n" && GameInfo.userData.userInfo.arenaAlarmYn == "n")
			{
				Inst.OpenTutorial(17, 1);
				Inst._tutorialData.eventData[11].passYn = "y";
				SaveAllData(17, 0);
				LobbyManager.HideDailyAndPackage();
			}
		}
	}

	public static void HunterSkillClear()
	{
		if (!(Inst._tutorialData.eventData[0].passYn == "y"))
		{
			SaveAllData(6, 1);
			Inst._tutorialData.eventData[0].passYn = "y";
		}
	}

	public static void RewardButtle()
	{
		if (Inst._isActiveIsTutorial && Inst._currentID == 3 && Inst._thisSequence == 1)
		{
			OpenTutorial();
		}
	}

	public static void DeckTutorial()
	{
		if (Inst._tutorialData.eventData[8].passYn == "n" && GameInfo.userData.userStageState[0].chapterList.Length >= 3)
		{
			if (GameInfo.userData.userStageState[0].chapterList[2].levelList.Length > 1)
			{
				Inst._tutorialData.eventData[8].passYn = "y";
				SaveAllData(14, 1);
			}
			else if (LobbyManager.OpenDeckEdit == null)
			{
				LobbyManager.OpenDeckEdit = Inst.PlayLevel13;
			}
		}
	}

	public static void OpenTutorial()
	{
		Inst.OpenTutorial(Inst._currentID, Inst._thisSequence);
	}

	public static void StartSequence(int _seq)
	{
		Inst._thisSequence = _seq;
	}

	public static void CloseTutorial()
	{
		Debug.Log("Tutorial - HideTutorial");
		Inst._imageDimm.gameObject.SetActive(value: false);
		Inst._dialogueInfo.gameObject.SetActive(value: false);
	}

	public static void SetClickDimmed(bool isClick)
	{
		Inst._isDimmendClick = isClick;
		Inst._dimmedButton.enabled = isClick;
		Inst._dialogueInfo.ClickNext(isClick);
	}

	public static void HighLightUI(Transform target, bool isScaleOne = true)
	{
		Inst._highLight = target;
		Inst._origParent = target.parent;
		Inst._origDepth = Inst._highLight.GetComponent<RectTransform>().GetSiblingIndex();
		target.SetParent(Inst._uiTutorial);
		if (isScaleOne)
		{
			target.localScale = Vector3.one;
		}
	}

	public static void ReturnUILight(Vector3 size = default(Vector3))
	{
		if (!(Inst._highLight == null))
		{
			Inst._highLight.SetParent(Inst._origParent);
			Inst._highLight.GetComponent<RectTransform>().SetSiblingIndex(Inst._origDepth);
			if (size == Vector3.zero)
			{
				Inst._highLight.localScale = Vector3.one;
			}
			else
			{
				Inst._highLight.localScale = size;
			}
		}
	}

	public static Transform ShowCopyHighLight(Transform target)
	{
		Inst._copyObject = Instantiate(target);
		Inst._copyObject.SetParent(Inst._uiTutorial);
		return Inst._copyObject;
	}

	public static void ReturnCopy()
	{
		if (Inst._copyObject != null)
		{
			Destroy(Inst._copyObject.gameObject);
		}
	}

	public static void SortLightSprite(Transform target)
	{
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		SpriteRenderer[] componentsInChildren = target.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
		foreach (SpriteRenderer item in componentsInChildren)
		{
			list.Add(item);
		}
		ShowListOfSprites(list);
	}

	public static void ShowListOfSprites(List<SpriteRenderer> _list)
	{
		if (Inst._dethData.Count <= 0)
		{
			foreach (SpriteRenderer item in _list)
			{
				InfoDepthData tutorialDepthData = new InfoDepthData();
				tutorialDepthData.srObject = item;
				tutorialDepthData.originalDepth = tutorialDepthData.srObject.sortingOrder;
				Inst._dethData.Add(tutorialDepthData);
				item.sortingOrder += 2000;
				item.gameObject.layer = LayerMask.NameToLayer("Tutorial");
			}
		}
	}

	public static void ReturnBackAll()
	{
		foreach (InfoDepthData listDepthDatum in Inst._dethData)
		{
			listDepthDatum.srObject.gameObject.layer = LayerMask.NameToLayer("Default");
			listDepthDatum.srObject.sortingOrder = listDepthDatum.originalDepth;
		}
		Inst._dethData.Clear();
	}

	public static void ContinueStep()
	{
		Inst.ContinueTutorial();
	}

	public static void NextStepIndex()
	{
		Inst._currentID++;
		Inst._thisSequence = 1;
	}

	public static void SaveAllData(Action _callBack = null)
	{
		Debug.Log("@@@@@@@@@@@@ SaveTutorial :: " + Inst._currentID + " / " + Inst._thisSequence);
		Protocol_Set.Protocol_user_set_tutorial_Req(Inst._currentID, Inst._thisSequence, _callBack);
	}

	public static void SaveAllData(int sIdx, int seq, Action _callBack = null)
	{
		Debug.Log("@@@@@@@@@@@@ SaveTutorial :: " + sIdx + " / " + seq);
		Protocol_Set.Protocol_user_set_tutorial_Req(sIdx, seq, _callBack);
	}

	public static void TutorialEnd()
	{
		Inst._currentID = -1;
		Inst._thisSequence = 0;
		SaveAllData();
	}

	public static void EventTutorialEbd()
	{
		Debug.Log("%%%%%%%%%%%%% Current Must SIdx = " + Inst._idMust);
		Debug.Log("%%%%%%%%%%%%% Current Must SSeq = " + Inst._seqMust);
		Inst._currentID = Inst._idMust;
		Inst._thisSequence = Inst._seqMust;
	}

	public static void HandAppear(Transform _trTarget, Vector3 _addPosition)
	{
		Inst.StartCoroutine(Inst.HandDelay(_trTarget, _addPosition));
	}

	public static void RemoveHand()
	{
		if (Inst._handTransform != null)
		{
			MasterPoolManager.ReturnToPool("Tutorial", Inst._handTransform);
			Inst._handTransform = null;
		}
	}

	private IEnumerator HandDelay(Transform _trTarget, Vector3 _addPosition)
	{
		yield return null;
		Inst._handTransform = MasterPoolManager.SpawnObject("Tutorial", "Tutorial_Hand");
		Inst._handTransform.position = _trTarget.position + _addPosition;
		Inst._handTransform.GetComponent<InfoHand>().DownHand();
	}

	private bool CheckTutorialState()
	{
		Debug.Log("tutorialData :: " + _tutorialData.mustData);
		if (_tutorialData.mustData.sIndex > 0)
		{
			return true;
		}
		USER_GET_TUTORIAL_EVENT_INFO[] eventData = _tutorialData.eventData;
		foreach (USER_GET_TUTORIAL_EVENT_INFO uSER_GET_TUTORIAL_EVENT_INFO in eventData)
		{
			if (uSER_GET_TUTORIAL_EVENT_INFO.passYn == "n")
			{
				return true;
			}
		}
		return false;
	}

	private void OpenTutorial(int sidex, int seq)
	{
		Debug.Log("Tutorial - ShowTutorial ::: " + sidex + " / " + seq);
		if (seq == 0)
		{
			seq = 1;
		}
		if (sidex > 5 && seq == 1)
		{
			_idMust = _currentID;
			_seqMust = _thisSequence;
		}
		_currentID = sidex;
		_thisSequence = seq;
		InfoDbData tutorialDbData = GameDataManager.GetDicTutorialDbData()[sidex][seq];
		SetClickDimmed(isClick: true);
		switch (sidex)
		{
		case 0:
			ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
			BGShow(tutorialDbData.background == 1);
			_infoTutor.Open(seq);
			break;
		case 1:
			ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
			BGShow(tutorialDbData.background == 1);
			_battleProgress.Open(seq);
			break;
		case 2:
			ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
			BGShow(tutorialDbData.background == 1);
			_storeManger.Open(seq);
			break;
		case 5:
			Debug.Log("NewStoreOpen :: " + GameInfo.userData.userStageState[0].chapterList[0].levelList.Length);
			if (GameInfo.userData.userStageState[0].chapterList[0].levelList.Length == 5 && GameInfo.userData.userStageState[0].chapterList[0].levelList[4].starCount != 0)
			{
				_newStore.Open(seq);
				ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
				BGShow(tutorialDbData.background == 1);
			}
			break;
		case 10:
			ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
			BGShow(tutorialDbData.background == 1);
			break;
		case 4:
			if (GameInfo.userData.userStageState[0].chapterList[0].levelList.Length >= 4 && GameInfo.userData.userStageState[0].chapterList[0].levelList[3].starCount > 0)
			{
				_chestTutor.Open(seq);
				ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
				BGShow(tutorialDbData.background == 1);
			}
			break;
		case 7:
			_bughAcquire.Open(seq);
			ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
			BGShow(tutorialDbData.background == 1);
			break;
		case 3:
			if (GameInfo.currentSceneType == SceneType.Lobby)
			{
				if (GameInfo.userData.userStageState[0].chapterList[0].levelList.Length >= 4 && GameInfo.userData.userStageState[0].chapterList[0].levelList[2].starCount > 0)
				{
					BGShow(tutorialDbData.background == 1);
					_hunterLevel.Open(seq);
					ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
				}
			}
			else
			{
				BGShow(tutorialDbData.background == 1);
				_hunterLevel.Open(seq);
				ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
			}
			break;
		case 8:
			_upgradeStore.Open(seq);
			ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
			BGShow(tutorialDbData.background == 1);
			break;
		case 6:
			ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
			BGShow(tutorialDbData.background == 1);
			_hunterSkill.Open(seq);
			break;
		case 9:
			_dailyShop.Open(seq);
			ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
			BGShow(tutorialDbData.background == 1);
			break;
		case 11:
			ChestJevel.Open(seq);
			ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
			BGShow(tutorialDbData.background == 1);
			break;
		case 12:
			ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
			BGShow(tutorialDbData.background == 1);
			_dragMatch.Open(seq);
			break;
		case 13:
			_hunterEnchant.Open(seq);
			ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
			BGShow(tutorialDbData.background == 1);
			break;
		case 14:
			_leaderSkill.Open(seq);
			ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
			BGShow(tutorialDbData.background == 1);
			break;
		case 15:
			ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
			BGShow(tutorialDbData.background == 1);
			_arenaOpen.Open(seq);
			break;
		case 16:
			ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
			BGShow(tutorialDbData.background == 1);
			_arenaEnter.Open(seq);
			break;
		case 17:
			ShowTutroialDialoge(tutorialDbData.TutoMessage, (InfoDialogue.DialogPos)tutorialDbData.descPosition, tutorialDbData.characterImg);
			BGShow(tutorialDbData.background == 1);
			break;
		}
	}

	private void ShowTutroialDialoge(string message, InfoDialogue.DialogPos type, string characterImg)
	{
		Debug.Log("ShowDialog - " + message);
		if (message == "0")
		{
			_dialogueInfo.gameObject.SetActive(value: false);
			return;
		}
		_dialogueInfo.gameObject.SetActive(value: true);
		_dialogueInfo.OpenPhrase(message, type, characterImg != "0" && !characterImg.Contains("hunter"));
	}

	private void BGShow(bool isDimmed)
	{
		_imageDimm.gameObject.SetActive(value: true);
		if (isDimmed)
		{
			_imageDimm.color = _colorDimmed;
		}
		else
		{
			_imageDimm.color = _colorTransp;
		}
	}

	private void ContinueTutorial()
	{
		_thisSequence++;
		if (_currentID == 3 && _thisSequence == 4 && GameInfo.currentSceneType == SceneType.InGame)
		{
			PuzzlePlayManager.BattleRewardCompleteEvent();
		}
		else if (!GameDataManager.GetDicTutorialDbData()[_currentID].ContainsKey(_thisSequence))
		{
			switch (_currentID)
			{
			case 4:
				LobbyManager.OpenChestOpenResult();
				break;
			case 7:
				ReturnUILight();
				CloseTutorial();
				EventTutorialEbd();
				return;
			case 9:
				LobbyManager.OpenValueShop();
				return;
			case 10:
				CloseTutorial();
				EventTutorialEbd();
				return;
			case 12:
				PuzzlePlayManager.ContinueTimer();
				CloseTutorial();
				EventTutorialEbd();
				return;
			case 13:
			case 14:
				CloseTutorial();
				EventTutorialEbd();
				return;
			case 16:
			case 17:
				CloseTutorial();
				EventTutorialEbd();
				LobbyManager.EndArenaTutorial();
				return;
			}
			if (_currentID >= 5)
			{
				_currentID = _idMust;
				_thisSequence = _seqMust;
				return;
			}
			_currentID++;
			_thisSequence = 1;
			CloseTutorial();
			if (GameDataManager.GetDicTutorialDbData().ContainsKey(_currentID) && _currentID <= 5)
			{
			}
		}
		else
		{
			OpenTutorial(_currentID, _thisSequence);
		}
	}

	private void DailyShopOpen()
	{
		LobbyManager.DailyShopOpen = null;
		OpenTutorial(9, 1);
		_tutorialData.eventData[3].passYn = "y";
	}

	private void EnterQuick()
	{
		LobbyManager.QuickLootRefresh = null;
		LobbyManager.OpenQuickLoot = null;
		OpenTutorial(10, 1);
		_tutorialData.eventData[4].passYn = "y";
		SaveAllData(10, 0);
	}

	private void OnTutorialBlockClear()
	{
		if (GameInfo.userData.userStageState[0].chapterList.Length >= 2 && GameInfo.userData.userStageState[0].chapterList[1].levelList.Length >= 7 && GameInfo.userData.userStageState[0].chapterList[1].levelList[GameInfo.userData.userStageState[0].chapterList[1].levelList.Length - 1].starCount > 0)
		{
			LobbyManager.Chapter2Clear = null;
			OpenTutorial(11, 1);
			_tutorialData.eventData[5].passYn = "y";
		}
	}

	private void ChapterCleared()
	{
		if (GameInfo.userData.userStageState[0].chapterList.Length >= 4 && GameInfo.userData.userStageState[0].chapterList[3].levelList.Length >= 1 && !_tutorialData.eventData[0].passYn.Equals("n") && !_tutorialData.eventData[5].passYn.Equals("n") && GameInfo.userData.userStageState[0].chapterList[3].levelList[0].starCount > 0)
		{
			LobbyManager.Chapter4Clear = null;
			OpenTutorial(13, 1);
			_tutorialData.eventData[7].passYn = "y";
		}
	}

	private void PlayLevel13()
	{
		LobbyManager.OpenDeckEdit = null;
		OpenTutorial(14, 1);
		_tutorialData.eventData[8].passYn = "y";
	}

	private void StoreBack(int castleId, int floorId)
	{
		LobbyManager.StoreBadgeAcquire = null;
		_bughAcquire.SetStoreID(castleId, floorId);
		OpenTutorial(7, 1);
		SaveAllData();
		_tutorialData.eventData[1].passYn = "y";
	}

	private void SkillHunter()
	{
		OpenTutorial(6, 1);
		PuzzlePlayManager.OnHunterSkill = null;
		_tutorialData.eventData[0].passYn = "y";
	}

	private void RefreshEvent()
	{
		LobbyManager.QuickLootRefresh = null;
		LobbyManager.OpenQuickLoot = null;
		_tutorialData.eventData[4].passYn = "y";
		SaveAllData(10, 1);
	}

	public void OnClickNextTutorial()
	{
		if (_isDimmendClick)
		{
			ContinueTutorial();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		_dimmedButton = _imageDimm.GetComponent<Button>();
		_infoTutor = gameObject.GetComponent<InfoIntro>();
		_battleProgress = gameObject.GetComponent<InfoBattleProgress>();
		_storeManger = gameObject.GetComponent<InfoStoreManagement>();
		_newStore = gameObject.GetComponent<InfoNewStoreOpen>();
		_bughAcquire = gameObject.GetComponent<InfoBadgeAcquire>();
		_hunterLevel = gameObject.GetComponent<InfoHunterLevelUp>();
		_chestTutor = gameObject.GetComponent<InfoChest>();
		_upgradeStore = gameObject.GetComponent<InfoStoreUpgrade>();
		_hunterSkill = gameObject.GetComponent<InfoHunterSkill>();
		_dailyShop = gameObject.GetComponent<InfoDailyShop>();
		ChestJevel = gameObject.GetComponent<InfoJewelChest>();
		_dragMatch = gameObject.GetComponent<InfoDragMatch>();
		_hunterEnchant = gameObject.GetComponent<InfoHunterEnchant>();
		_leaderSkill = gameObject.GetComponent<InfoLeaderSkill>();
		_arenaOpen = gameObject.GetComponent<InfoArenaOpen>();
		_arenaEnter = gameObject.GetComponent<InfoArenaEnter>();
	}
}
