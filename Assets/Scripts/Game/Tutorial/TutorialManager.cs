using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : GameObjectSingleton<TutorialManager>
{
	[SerializeField]
	private TutorialDialogue dialogue;

	[SerializeField]
	private Image imageDimmed;

	[SerializeField]
	private Transform trTutorialUI;

	private bool isTutorial;

	private bool isDimmedClick = true;

	private int originalDepth = -1;

	private int mustSIdx;

	private int mustSeq;

	[SerializeField]
	private int currentSIdx;

	[SerializeField]
	private int currentSeq;

	private Button btnDimmed;

	private TutorialType currentTutorialType = TutorialType.None;

	private Transform trHighLight;

	private Transform trOriginalParent;

	private Transform trCopyHighLightObject;

	private Transform trHand;

	private USER_GET_TUTORIAL_RESULT tutorialData;

	private Color colorDimmed = new Color(0f, 0f, 0f, 0.7f);

	private Color colorTransparent = new Color(0f, 0f, 0f, 0f);

	private TutorialIntro tutorialIntro;

	private TutorialBattleProgress tutorialBattleProgress;

	private TutorialStoreManagement tutorialStoreManagement;

	private TutorialNewStoreOpen tutorialNewStoreOpen;

	private TutorialBadgeAcquire tutorialBadgeAcquire;

	private TutorialChest tutorialChest;

	private TutorialHunterLevelUp tutorialHunterLevelUp;

	private TutorialStoreUpgrade tutorialStoreUpgrade;

	private TutorialHunterSkill tutorialHunterSkill;

	private TutorialDailyShop tutorialDailyShop;

	private TutorialJewelChest tutorialJewelChest;

	private TutorialDragMatch tutorialDragMatch;

	private TutorialHunterEnchant tutorialHunterEnchant;

	private TutorialLeaderSkill tutorialLeaderSkill;

	private TutorialArenaOpen tutorialArenaOpen;

	private TutorialArenaEnter tutorialArenaEnter;

	private List<TutorialDepthData> listDepthData = new List<TutorialDepthData>();

	public static int SIdx => GameObjectSingleton<TutorialManager>.Inst.currentSIdx;

	public static int Seq => GameObjectSingleton<TutorialManager>.Inst.currentSeq;

	public static bool Intro => GameObjectSingleton<TutorialManager>.Inst.currentSIdx == 0;

	public static bool DialogState => GameObjectSingleton<TutorialManager>.Inst.dialogue.gameObject.activeSelf;

	public static bool ProgressTutorial
	{
		get
		{
			if (GameObjectSingleton<TutorialManager>.Inst.currentSIdx == 0)
			{
				return true;
			}
			if (GameObjectSingleton<TutorialManager>.Inst.currentSIdx == 1 && (GameObjectSingleton<TutorialManager>.Inst.currentSeq == 8 || GameObjectSingleton<TutorialManager>.Inst.currentSeq == 9))
			{
				return true;
			}
			return false;
		}
	}

	public static void SetTutorialData(USER_GET_TUTORIAL_RESULT _data)
	{
		GameObjectSingleton<TutorialManager>.Inst.tutorialData = _data;
		GameObjectSingleton<TutorialManager>.Inst.isTutorial = GameObjectSingleton<TutorialManager>.Inst.CheckTutorialState();
		GameObjectSingleton<TutorialManager>.Inst.currentSIdx = _data.mustData.sIndex;
		GameObjectSingleton<TutorialManager>.Inst.currentSeq = _data.mustData.seq;
		GameObjectSingleton<TutorialManager>.Inst.mustSIdx = _data.mustData.sIndex;
		GameObjectSingleton<TutorialManager>.Inst.mustSeq = _data.mustData.seq;
		UnityEngine.Debug.Log("SetTutorialData - currentSIdx :: " + GameObjectSingleton<TutorialManager>.Inst.currentSeq + ", currentSeq :: " + GameObjectSingleton<TutorialManager>.Inst.currentSeq);
		GameInfo.isTutorial = GameObjectSingleton<TutorialManager>.Inst.isTutorial;
		if (GameInfo.isTutorial)
		{
		}
	}

	public static void CheckTutorial()
	{
		if (!GameObjectSingleton<TutorialManager>.Inst.isTutorial)
		{
			return;
		}
		if (GameObjectSingleton<TutorialManager>.Inst.currentSIdx > -1)
		{
			GameObjectSingleton<TutorialManager>.Inst.ShowTutorial(GameObjectSingleton<TutorialManager>.Inst.currentSIdx, GameObjectSingleton<TutorialManager>.Inst.currentSeq);
		}
		if (GameObjectSingleton<TutorialManager>.Inst.tutorialData.eventData[1].passYn == "n")
		{
			UnityEngine.Debug.Log("CheckTutorial OnStoreBadgeAcquire");
			LobbyManager.StoreBadgeAcquire = GameObjectSingleton<TutorialManager>.Inst.OnStoreBadgeAcquire;
		}
		if (GameObjectSingleton<TutorialManager>.Inst.tutorialData.eventData[2].passYn == "n" && LobbyManager.HasFirstFloorItemUpgrade)
		{
			GameObjectSingleton<TutorialManager>.Inst.ShowTutorial(8, 1);
			SaveTutorial();
			GameObjectSingleton<TutorialManager>.Inst.tutorialData.eventData[2].passYn = "y";
		}
		if (GameObjectSingleton<TutorialManager>.Inst.tutorialData.eventData[3].passYn == "n")
		{
			UnityEngine.Debug.Log("CheckTutorial DAILY SHOP 11");
			if (LobbyManager.DailyShopOpen == null)
			{
				UnityEngine.Debug.Log("CheckTutorial DAILY SHOP 22");
				LobbyManager.DailyShopOpen = GameObjectSingleton<TutorialManager>.Inst.OnDailyShopOpen;
			}
		}
		if (GameObjectSingleton<TutorialManager>.Inst.tutorialData.eventData[4].passYn == "n")
		{
			LobbyManager.OpenQuickLoot = GameObjectSingleton<TutorialManager>.Inst.OnQuickLootEnter;
			LobbyManager.QuickLootRefresh = GameObjectSingleton<TutorialManager>.Inst.OnQuickLootRefreshEvent;
		}
		if (GameObjectSingleton<TutorialManager>.Inst.tutorialData.eventData[5].passYn == "n")
		{
			if (LobbyManager.Chapter2Clear == null)
			{
				LobbyManager.Chapter2Clear = GameObjectSingleton<TutorialManager>.Inst.OnChapter2Clear;
			}
			if (LobbyManager.Chapter2Clear != null)
			{
				LobbyManager.Chapter2Clear();
			}
		}
		if (GameObjectSingleton<TutorialManager>.Inst.tutorialData.eventData[7].passYn == "n")
		{
			if (LobbyManager.Chapter4Clear == null)
			{
				LobbyManager.Chapter4Clear = GameObjectSingleton<TutorialManager>.Inst.OnChapter4Clear;
			}
			if (LobbyManager.Chapter4Clear != null)
			{
				LobbyManager.Chapter4Clear();
			}
		}
		if (GameObjectSingleton<TutorialManager>.Inst.tutorialData.eventData[9].passYn == "n" && GameInfo.userData.userStageState[0].chapterList.Length == 4)
		{
			GameObjectSingleton<TutorialManager>.Inst.ShowTutorial(15, 1);
			GameObjectSingleton<TutorialManager>.Inst.tutorialData.eventData[9].passYn = "y";
			SaveTutorial(15, 0);
		}
	}

	public static void CheckInGameUserTurn()
	{
		if (!GameObjectSingleton<TutorialManager>.Inst.isTutorial)
		{
			return;
		}
		if (GameObjectSingleton<TutorialManager>.Inst.tutorialData.eventData[0].passYn == "n")
		{
			UnityEngine.Debug.Log("USERSKILL TUTORIAL 11");
			if (GameInfo.inGamePlayData.level >= 4 && InGamePlayManager.CheckIsUseHunterSkill() != null)
			{
				UnityEngine.Debug.Log("USERSKILL TUTORIAL 22");
				InGamePlayManager.HunterSkillEvent = GameObjectSingleton<TutorialManager>.Inst.HunterSkillEvent;
				InGamePlayManager.HunterSkillEvent();
			}
		}
		else
		{
			UnityEngine.Debug.Log("USERSKILL TUTORIAL 33");
		}
		if (GameObjectSingleton<TutorialManager>.Inst.currentSIdx == 0)
		{
			int num = GameObjectSingleton<TutorialManager>.Inst.currentSeq;
			if (num == 2 || num == 7)
			{
				ShowTutorial();
			}
		}
		if (GameObjectSingleton<TutorialManager>.Inst.currentSIdx == 1)
		{
			int num2 = GameObjectSingleton<TutorialManager>.Inst.currentSeq;
			if (num2 == 2 || num2 == 8 || num2 == 9)
			{
				ShowTutorial();
			}
		}
		else if (GameObjectSingleton<TutorialManager>.Inst.currentSIdx == 2)
		{
			int num3 = GameObjectSingleton<TutorialManager>.Inst.currentSeq;
			if (num3 == 9 || num3 == 11)
			{
				ShowTutorial();
			}
		}
		else if (GameObjectSingleton<TutorialManager>.Inst.currentSIdx == 3)
		{
			int num4 = GameObjectSingleton<TutorialManager>.Inst.currentSeq;
			if (num4 == 1)
			{
				ShowTutorial();
			}
		}
	}

	public static void CheckArenaLobbyEnter()
	{
		if (GameObjectSingleton<TutorialManager>.Inst.isTutorial)
		{
			if (GameObjectSingleton<TutorialManager>.Inst.tutorialData.eventData[10].passYn == "n" && GameInfo.userData.userInfo.arenaAlarmYn == "y")
			{
				GameObjectSingleton<TutorialManager>.Inst.ShowTutorial(16, 1);
				GameObjectSingleton<TutorialManager>.Inst.tutorialData.eventData[10].passYn = "y";
				SaveTutorial(16, 0);
			}
			if (GameObjectSingleton<TutorialManager>.Inst.tutorialData.eventData[11].passYn == "n" && GameInfo.userData.userInfo.arenaAlarmYn == "n")
			{
				GameObjectSingleton<TutorialManager>.Inst.ShowTutorial(17, 1);
				GameObjectSingleton<TutorialManager>.Inst.tutorialData.eventData[11].passYn = "y";
				SaveTutorial(17, 0);
				LobbyManager.HideDailyAndPackage();
			}
		}
	}

	public static void HunterSkillTutorialForceClear()
	{
		if (!(GameObjectSingleton<TutorialManager>.Inst.tutorialData.eventData[0].passYn == "y"))
		{
			SaveTutorial(6, 1);
			GameObjectSingleton<TutorialManager>.Inst.tutorialData.eventData[0].passYn = "y";
		}
	}

	public static void CheckBattleReward()
	{
		if (GameObjectSingleton<TutorialManager>.Inst.isTutorial && GameObjectSingleton<TutorialManager>.Inst.currentSIdx == 3 && GameObjectSingleton<TutorialManager>.Inst.currentSeq == 1)
		{
			ShowTutorial();
		}
	}

	public static void CheckHunterDeckTutorial()
	{
		if (GameObjectSingleton<TutorialManager>.Inst.tutorialData.eventData[8].passYn == "n" && GameInfo.userData.userStageState[0].chapterList.Length >= 3)
		{
			if (GameInfo.userData.userStageState[0].chapterList[2].levelList.Length > 1)
			{
				GameObjectSingleton<TutorialManager>.Inst.tutorialData.eventData[8].passYn = "y";
				SaveTutorial(14, 1);
			}
			else if (LobbyManager.OpenDeckEdit == null)
			{
				LobbyManager.OpenDeckEdit = GameObjectSingleton<TutorialManager>.Inst.OnLevelPlay13;
			}
		}
	}

	public static void ShowTutorial()
	{
		GameObjectSingleton<TutorialManager>.Inst.ShowTutorial(GameObjectSingleton<TutorialManager>.Inst.currentSIdx, GameObjectSingleton<TutorialManager>.Inst.currentSeq);
	}

	public static void SetSeq(int _seq)
	{
		GameObjectSingleton<TutorialManager>.Inst.currentSeq = _seq;
	}

	public static void HideTutorial()
	{
		UnityEngine.Debug.Log("Tutorial - HideTutorial");
		GameObjectSingleton<TutorialManager>.Inst.imageDimmed.gameObject.SetActive(value: false);
		GameObjectSingleton<TutorialManager>.Inst.dialogue.gameObject.SetActive(value: false);
	}

	public static void SetDimmedClick(bool isClick)
	{
		GameObjectSingleton<TutorialManager>.Inst.isDimmedClick = isClick;
		GameObjectSingleton<TutorialManager>.Inst.btnDimmed.enabled = isClick;
		GameObjectSingleton<TutorialManager>.Inst.dialogue.SetNextClickPossible(isClick);
	}

	public static void ShowHighLightUI(Transform target, bool isScaleOne = true)
	{
		GameObjectSingleton<TutorialManager>.Inst.trHighLight = target;
		GameObjectSingleton<TutorialManager>.Inst.trOriginalParent = target.parent;
		GameObjectSingleton<TutorialManager>.Inst.originalDepth = GameObjectSingleton<TutorialManager>.Inst.trHighLight.GetComponent<RectTransform>().GetSiblingIndex();
		target.SetParent(GameObjectSingleton<TutorialManager>.Inst.trTutorialUI);
		if (isScaleOne)
		{
			target.localScale = Vector3.one;
		}
	}

	public static void ReturnHighLightUI(Vector3 size = default(Vector3))
	{
		if (!(GameObjectSingleton<TutorialManager>.Inst.trHighLight == null))
		{
			GameObjectSingleton<TutorialManager>.Inst.trHighLight.SetParent(GameObjectSingleton<TutorialManager>.Inst.trOriginalParent);
			GameObjectSingleton<TutorialManager>.Inst.trHighLight.GetComponent<RectTransform>().SetSiblingIndex(GameObjectSingleton<TutorialManager>.Inst.originalDepth);
			if (size == Vector3.zero)
			{
				GameObjectSingleton<TutorialManager>.Inst.trHighLight.localScale = Vector3.one;
			}
			else
			{
				GameObjectSingleton<TutorialManager>.Inst.trHighLight.localScale = size;
			}
		}
	}

	public static Transform ShowCopyHighLightUI(Transform target)
	{
		GameObjectSingleton<TutorialManager>.Inst.trCopyHighLightObject = UnityEngine.Object.Instantiate(target);
		GameObjectSingleton<TutorialManager>.Inst.trCopyHighLightObject.SetParent(GameObjectSingleton<TutorialManager>.Inst.trTutorialUI);
		return GameObjectSingleton<TutorialManager>.Inst.trCopyHighLightObject;
	}

	public static void ReturnCopyHighLightUI()
	{
		if (GameObjectSingleton<TutorialManager>.Inst.trCopyHighLightObject != null)
		{
			UnityEngine.Object.Destroy(GameObjectSingleton<TutorialManager>.Inst.trCopyHighLightObject.gameObject);
		}
	}

	public static void ShowAndSortHighLightSprite(Transform target)
	{
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		SpriteRenderer[] componentsInChildren = target.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
		foreach (SpriteRenderer item in componentsInChildren)
		{
			list.Add(item);
		}
		ShowHighLightSpriteList(list);
	}

	public static void ShowHighLightSpriteList(List<SpriteRenderer> _list)
	{
		if (GameObjectSingleton<TutorialManager>.Inst.listDepthData.Count <= 0)
		{
			foreach (SpriteRenderer item in _list)
			{
				TutorialDepthData tutorialDepthData = new TutorialDepthData();
				tutorialDepthData.srObject = item;
				tutorialDepthData.originalDepth = tutorialDepthData.srObject.sortingOrder;
				GameObjectSingleton<TutorialManager>.Inst.listDepthData.Add(tutorialDepthData);
				item.sortingOrder += 2000;
				item.gameObject.layer = LayerMask.NameToLayer("Tutorial");
			}
		}
	}

	public static void ReturnHighLightSpriteList()
	{
		foreach (TutorialDepthData listDepthDatum in GameObjectSingleton<TutorialManager>.Inst.listDepthData)
		{
			listDepthDatum.srObject.gameObject.layer = LayerMask.NameToLayer("Default");
			listDepthDatum.srObject.sortingOrder = listDepthDatum.originalDepth;
		}
		GameObjectSingleton<TutorialManager>.Inst.listDepthData.Clear();
	}

	public static void NextSep()
	{
		GameObjectSingleton<TutorialManager>.Inst.NextTutorial();
	}

	public static void NextTutorialindex()
	{
		GameObjectSingleton<TutorialManager>.Inst.currentSIdx++;
		GameObjectSingleton<TutorialManager>.Inst.currentSeq = 1;
	}

	public static void SaveTutorial(Action _callBack = null)
	{
		UnityEngine.Debug.Log("@@@@@@@@@@@@ SaveTutorial :: " + GameObjectSingleton<TutorialManager>.Inst.currentSIdx + " / " + GameObjectSingleton<TutorialManager>.Inst.currentSeq);
		Protocol_Set.Protocol_user_set_tutorial_Req(GameObjectSingleton<TutorialManager>.Inst.currentSIdx, GameObjectSingleton<TutorialManager>.Inst.currentSeq, _callBack);
	}

	public static void SaveTutorial(int sIdx, int seq, Action _callBack = null)
	{
		UnityEngine.Debug.Log("@@@@@@@@@@@@ SaveTutorial :: " + sIdx + " / " + seq);
		Protocol_Set.Protocol_user_set_tutorial_Req(sIdx, seq, _callBack);
	}

	public static void EndMustTutorial()
	{
		GameObjectSingleton<TutorialManager>.Inst.currentSIdx = -1;
		GameObjectSingleton<TutorialManager>.Inst.currentSeq = 0;
		SaveTutorial();
	}

	public static void EndEventTutorial()
	{
		UnityEngine.Debug.Log("%%%%%%%%%%%%% Current Must SIdx = " + GameObjectSingleton<TutorialManager>.Inst.mustSIdx);
		UnityEngine.Debug.Log("%%%%%%%%%%%%% Current Must SSeq = " + GameObjectSingleton<TutorialManager>.Inst.mustSeq);
		GameObjectSingleton<TutorialManager>.Inst.currentSIdx = GameObjectSingleton<TutorialManager>.Inst.mustSIdx;
		GameObjectSingleton<TutorialManager>.Inst.currentSeq = GameObjectSingleton<TutorialManager>.Inst.mustSeq;
	}

	public static void ShowHand(Transform _trTarget, Vector3 _addPosition)
	{
		GameObjectSingleton<TutorialManager>.Inst.StartCoroutine(GameObjectSingleton<TutorialManager>.Inst.ProcessShowHandDelay(_trTarget, _addPosition));
	}

	public static void ClearHand()
	{
		if (GameObjectSingleton<TutorialManager>.Inst.trHand != null)
		{
			MWPoolManager.DeSpawn("Tutorial", GameObjectSingleton<TutorialManager>.Inst.trHand);
			GameObjectSingleton<TutorialManager>.Inst.trHand = null;
		}
	}

	private IEnumerator ProcessShowHandDelay(Transform _trTarget, Vector3 _addPosition)
	{
		yield return null;
		GameObjectSingleton<TutorialManager>.Inst.trHand = MWPoolManager.Spawn("Tutorial", "Tutorial_Hand");
		GameObjectSingleton<TutorialManager>.Inst.trHand.position = _trTarget.position + _addPosition;
		GameObjectSingleton<TutorialManager>.Inst.trHand.GetComponent<TutorialHand>().ShowHandDown();
	}

	private bool CheckTutorialState()
	{
		UnityEngine.Debug.Log("tutorialData :: " + tutorialData.mustData);
		if (tutorialData.mustData.sIndex > 0)
		{
			return true;
		}
		USER_GET_TUTORIAL_EVENT_INFO[] eventData = tutorialData.eventData;
		foreach (USER_GET_TUTORIAL_EVENT_INFO uSER_GET_TUTORIAL_EVENT_INFO in eventData)
		{
			if (uSER_GET_TUTORIAL_EVENT_INFO.passYn == "n")
			{
				return true;
			}
		}
		return false;
	}

	private void ShowTutorial(int sidex, int seq)
	{
		UnityEngine.Debug.Log("Tutorial - ShowTutorial ::: " + sidex + " / " + seq);
		if (seq == 0)
		{
			seq = 1;
		}
		if (sidex > 5 && seq == 1)
		{
			mustSIdx = currentSIdx;
			mustSeq = currentSeq;
		}
		currentSIdx = sidex;
		currentSeq = seq;
		TutorialDbData tutorialDbData = GameDataManager.GetDicTutorialDbData()[sidex][seq];
		SetDimmedClick(isClick: true);
		switch (sidex)
		{
		case 0:
			ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
			ShowBackground(tutorialDbData.background == 1);
			tutorialIntro.Show(seq);
			break;
		case 1:
			ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
			ShowBackground(tutorialDbData.background == 1);
			tutorialBattleProgress.Show(seq);
			break;
		case 2:
			ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
			ShowBackground(tutorialDbData.background == 1);
			tutorialStoreManagement.Show(seq);
			break;
		case 5:
			UnityEngine.Debug.Log("NewStoreOpen :: " + GameInfo.userData.userStageState[0].chapterList[0].levelList.Length);
			if (GameInfo.userData.userStageState[0].chapterList[0].levelList.Length == 5 && GameInfo.userData.userStageState[0].chapterList[0].levelList[4].starCount != 0)
			{
				tutorialNewStoreOpen.Show(seq);
				ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
				ShowBackground(tutorialDbData.background == 1);
			}
			break;
		case 10:
			ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
			ShowBackground(tutorialDbData.background == 1);
			break;
		case 4:
			if (GameInfo.userData.userStageState[0].chapterList[0].levelList.Length >= 4 && GameInfo.userData.userStageState[0].chapterList[0].levelList[3].starCount > 0)
			{
				tutorialChest.Show(seq);
				ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
				ShowBackground(tutorialDbData.background == 1);
			}
			break;
		case 7:
			tutorialBadgeAcquire.Show(seq);
			ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
			ShowBackground(tutorialDbData.background == 1);
			break;
		case 3:
			if (GameInfo.currentSceneType == SceneType.Lobby)
			{
				if (GameInfo.userData.userStageState[0].chapterList[0].levelList.Length >= 4 && GameInfo.userData.userStageState[0].chapterList[0].levelList[2].starCount > 0)
				{
					ShowBackground(tutorialDbData.background == 1);
					tutorialHunterLevelUp.Show(seq);
					ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
				}
			}
			else
			{
				ShowBackground(tutorialDbData.background == 1);
				tutorialHunterLevelUp.Show(seq);
				ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
			}
			break;
		case 8:
			tutorialStoreUpgrade.Show(seq);
			ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
			ShowBackground(tutorialDbData.background == 1);
			break;
		case 6:
			ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
			ShowBackground(tutorialDbData.background == 1);
			tutorialHunterSkill.Show(seq);
			break;
		case 9:
			tutorialDailyShop.Show(seq);
			ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
			ShowBackground(tutorialDbData.background == 1);
			break;
		case 11:
			tutorialJewelChest.Show(seq);
			ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
			ShowBackground(tutorialDbData.background == 1);
			break;
		case 12:
			ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
			ShowBackground(tutorialDbData.background == 1);
			tutorialDragMatch.Show(seq);
			break;
		case 13:
			tutorialHunterEnchant.Show(seq);
			ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
			ShowBackground(tutorialDbData.background == 1);
			break;
		case 14:
			tutorialLeaderSkill.Show(seq);
			ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
			ShowBackground(tutorialDbData.background == 1);
			break;
		case 15:
			ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
			ShowBackground(tutorialDbData.background == 1);
			tutorialArenaOpen.Show(seq);
			break;
		case 16:
			ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
			ShowBackground(tutorialDbData.background == 1);
			tutorialArenaEnter.Show(seq);
			break;
		case 17:
			ShowDialog(tutorialDbData.TutoMessage, (TutorialDialogue.DialoguePosType)tutorialDbData.descPosition, tutorialDbData.characterImg);
			ShowBackground(tutorialDbData.background == 1);
			break;
		}
	}

	private void ShowDialog(string message, TutorialDialogue.DialoguePosType type, string characterImg)
	{
		UnityEngine.Debug.Log("ShowDialog - " + message);
		if (message == "0")
		{
			dialogue.gameObject.SetActive(value: false);
			return;
		}
		dialogue.gameObject.SetActive(value: true);
		dialogue.ShowDialogue(message, type, characterImg != "0" && !characterImg.Contains("hunter"));
	}

	private void ShowBackground(bool isDimmed)
	{
		imageDimmed.gameObject.SetActive(value: true);
		if (isDimmed)
		{
			imageDimmed.color = colorDimmed;
		}
		else
		{
			imageDimmed.color = colorTransparent;
		}
	}

	private void NextTutorial()
	{
		currentSeq++;
		if (currentSIdx == 3 && currentSeq == 4 && GameInfo.currentSceneType == SceneType.InGame)
		{
			InGamePlayManager.BattleRewardCompleteEvent();
		}
		else if (!GameDataManager.GetDicTutorialDbData()[currentSIdx].ContainsKey(currentSeq))
		{
			switch (currentSIdx)
			{
			case 4:
				LobbyManager.OpenChestOpenResult();
				break;
			case 7:
				ReturnHighLightUI();
				HideTutorial();
				EndEventTutorial();
				return;
			case 9:
				LobbyManager.OpenValueShop();
				return;
			case 10:
				HideTutorial();
				EndEventTutorial();
				AnalyticsManager.FirebaseAnalyticsLogEvent(FBLog_Type.quick_loot_tuto);
				return;
			case 12:
				InGamePlayManager.ResumeMatchTimer();
				HideTutorial();
				EndEventTutorial();
				return;
			case 13:
			case 14:
				HideTutorial();
				EndEventTutorial();
				return;
			case 16:
			case 17:
				HideTutorial();
				EndEventTutorial();
				LobbyManager.EndArenaTutorial();
				return;
			}
			if (currentSIdx >= 5)
			{
				currentSIdx = mustSIdx;
				currentSeq = mustSeq;
				return;
			}
			currentSIdx++;
			currentSeq = 1;
			HideTutorial();
			if (GameDataManager.GetDicTutorialDbData().ContainsKey(currentSIdx) && currentSIdx <= 5)
			{
			}
		}
		else
		{
			ShowTutorial(currentSIdx, currentSeq);
		}
	}

	private void OnDailyShopOpen()
	{
		LobbyManager.DailyShopOpen = null;
		ShowTutorial(9, 1);
		tutorialData.eventData[3].passYn = "y";
	}

	private void OnQuickLootEnter()
	{
		LobbyManager.QuickLootRefresh = null;
		LobbyManager.OpenQuickLoot = null;
		ShowTutorial(10, 1);
		tutorialData.eventData[4].passYn = "y";
		SaveTutorial(10, 0);
	}

	private void OnChapter2Clear()
	{
		if (GameInfo.userData.userStageState[0].chapterList.Length >= 2 && GameInfo.userData.userStageState[0].chapterList[1].levelList.Length >= 7 && GameInfo.userData.userStageState[0].chapterList[1].levelList[GameInfo.userData.userStageState[0].chapterList[1].levelList.Length - 1].starCount > 0)
		{
			LobbyManager.Chapter2Clear = null;
			ShowTutorial(11, 1);
			tutorialData.eventData[5].passYn = "y";
		}
	}

	private void OnChapter4Clear()
	{
		if (GameInfo.userData.userStageState[0].chapterList.Length >= 4 && GameInfo.userData.userStageState[0].chapterList[3].levelList.Length >= 1 && !tutorialData.eventData[0].passYn.Equals("n") && !tutorialData.eventData[5].passYn.Equals("n") && GameInfo.userData.userStageState[0].chapterList[3].levelList[0].starCount > 0)
		{
			LobbyManager.Chapter4Clear = null;
			ShowTutorial(13, 1);
			tutorialData.eventData[7].passYn = "y";
		}
	}

	private void OnLevelPlay13()
	{
		LobbyManager.OpenDeckEdit = null;
		ShowTutorial(14, 1);
		tutorialData.eventData[8].passYn = "y";
	}

	private void OnStoreBadgeAcquire(int castleId, int floorId)
	{
		LobbyManager.StoreBadgeAcquire = null;
		tutorialBadgeAcquire.SetCastleAndStoreId(castleId, floorId);
		ShowTutorial(7, 1);
		SaveTutorial();
		tutorialData.eventData[1].passYn = "y";
	}

	private void HunterSkillEvent()
	{
		ShowTutorial(6, 1);
		InGamePlayManager.HunterSkillEvent = null;
		tutorialData.eventData[0].passYn = "y";
	}

	private void OnQuickLootRefreshEvent()
	{
		LobbyManager.QuickLootRefresh = null;
		LobbyManager.OpenQuickLoot = null;
		tutorialData.eventData[4].passYn = "y";
		SaveTutorial(10, 1);
	}

	public void OnClickNextTutorial()
	{
		if (isDimmedClick)
		{
			NextTutorial();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		btnDimmed = imageDimmed.GetComponent<Button>();
		tutorialIntro = base.gameObject.GetComponent<TutorialIntro>();
		tutorialBattleProgress = base.gameObject.GetComponent<TutorialBattleProgress>();
		tutorialStoreManagement = base.gameObject.GetComponent<TutorialStoreManagement>();
		tutorialNewStoreOpen = base.gameObject.GetComponent<TutorialNewStoreOpen>();
		tutorialBadgeAcquire = base.gameObject.GetComponent<TutorialBadgeAcquire>();
		tutorialHunterLevelUp = base.gameObject.GetComponent<TutorialHunterLevelUp>();
		tutorialChest = base.gameObject.GetComponent<TutorialChest>();
		tutorialStoreUpgrade = base.gameObject.GetComponent<TutorialStoreUpgrade>();
		tutorialHunterSkill = base.gameObject.GetComponent<TutorialHunterSkill>();
		tutorialDailyShop = base.gameObject.GetComponent<TutorialDailyShop>();
		tutorialJewelChest = base.gameObject.GetComponent<TutorialJewelChest>();
		tutorialDragMatch = base.gameObject.GetComponent<TutorialDragMatch>();
		tutorialHunterEnchant = base.gameObject.GetComponent<TutorialHunterEnchant>();
		tutorialLeaderSkill = base.gameObject.GetComponent<TutorialLeaderSkill>();
		tutorialArenaOpen = base.gameObject.GetComponent<TutorialArenaOpen>();
		tutorialArenaEnter = base.gameObject.GetComponent<TutorialArenaEnter>();
	}
}
