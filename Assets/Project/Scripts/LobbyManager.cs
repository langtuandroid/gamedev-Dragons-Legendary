using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : GameObjectSingleton<LobbyManager>
{
	public static Action StartStoreOpen;

	public static Action StoreCollectComplete;

	public static Action OpenStageSelect;

	public static Action<int> SelectStageCell;

	public static Action OpenQuickLoot;

	public static Action OpenChest;

	public static Action OpenChestOpen;

	public static Action OpenChestOpenResult;

	public static Action OpenChestOpenEnchant;

	public static Action OpenChestResultDone;

	public static Action OpenValueShop;

	public static Action OpenHunterList;

	public static Action OpenHunterInfo;

	public static Action OpenHunterLevel;

	public static Action OpenHunterLevelUp;

	public static Action<int, int> StoreBadgeAcquire;

	public static Action StoreDetailEnter;

	public static Action StoreUpgradeEnter;

	public static Action StoreUpgradeComplete;

	public static Action QuickLootRefresh;

	public static Action DailyShopOpen;

	public static Action Chapter2Clear;

	public static Action Chapter4Clear;

	public static Action JewelChestOpen;

	public static Action StoreTouchCollect;

	public static Action LevelPlayShow;

	public static Action OpenDeckEdit;

	public static Action ArenaMenuEnter;

	[SerializeField]
	private Transform trFloorStageContent;

	[SerializeField]
	private ScrollRect srcollRectFloor;

	[SerializeField]
	private LobbyTopUI topUI;

	[SerializeField]
	private ScrollSnap castleScrollSnap;

	[SerializeField]
	private LobbyCastleUI castleUI;

	[SerializeField]
	private LobbyBottomUI bottomUI;

	[SerializeField]
	private StageSelect stageSelect;

	[SerializeField]
	private LevelSelect levelSelect;

	[SerializeField]
	private LevelPlay levelPlay;

	[SerializeField]
	private QuickLoot quickLoot;

	[SerializeField]
	private Chest chest;

	[SerializeField]
	private DeckEdit deckEdit;

	[SerializeField]
	private ChestOpen chestOpen;

	[SerializeField]
	private ValueShop valueShop;

	[SerializeField]
	private ValueShopBuy valueShopBuy;

	[SerializeField]
	private HunterList hunterList;

	[SerializeField]
	private HunterView hunterView;

	[SerializeField]
	private HunterLevel hunterLevel;

	[SerializeField]
	private HunterLevelUp hunterLevelUp;

	[SerializeField]
	private HunterPromotion hunterPromotion;

	[SerializeField]
	private HunterPromotionUp hunterPromotionUp;

	[SerializeField]
	private Setting setting;

	[SerializeField]
	private FloorDetail floorDetail;

	[SerializeField]
	private FloorUpgrade floorUpgrade;

	[SerializeField]
	private LevelInfo userLevelInfo;

	[SerializeField]
	private EnergyInfo userEnergyInfo;

	[SerializeField]
	private ItemSortList itemSortList;

	[SerializeField]
	private NotEnouchCoin notEnoughCoin;

	[SerializeField]
	private NotEnoughJewel notEnoughJewel;

	[SerializeField]
	private SpeedUpCollect speedUpCollect;

	[SerializeField]
	private GameQuitPopup gameQuitPopup;

	[SerializeField]
	private DailyBonus dailyBonus;

	[SerializeField]
	private RewardResult rewardResult;

	[SerializeField]
	private SaleStartPack saleStartPack;

	[SerializeField]
	private SaleSpecialOffer saleSpecialOffer;

	[SerializeField]
	private SaleArenaPack saleArenaPack;

	[SerializeField]
	private ArenaLobby arenaLobby;

	[SerializeField]
	private ArenaLevelPlay arenaLevelPlay;

	[SerializeField]
	private ArenaEventEnd arenaEventEnd;

	[SerializeField]
	private ArenaTicketNone arenaTicketNone;

	[SerializeField]
	private ArenaShop arenaShop;

	[SerializeField]
	private ArenaShopBuy arenaShopBuy;

	[SerializeField]
	private Rate rate;

	[SerializeField]
	private Transform trLobbyPopupFront;

	[SerializeField]
	private Transform trLobbyPopupBack;

	[SerializeField]
	private Transform trChestLock;

	private int openStageFloorIdx;

	private int openChapterFloorIdx;

	private List<FloorController> listFloorController = new List<FloorController>();

	private Action userData_onCallBack;

	private Coroutine coroutineResume;

	private SHOP_PACKAGE_LIST_RESULT shopPackageListResult;

	public static SHOP_PACKAGE_LIST_RESULT PackageList => GameObjectSingleton<LobbyManager>.Inst.shopPackageListResult;

	public static int OpenStageFloorId
	{
		set
		{
			GameObjectSingleton<LobbyManager>.Inst.openStageFloorIdx = value;
		}
	}

	public static int OpenChapterFloorId
	{
		get
		{
			return GameObjectSingleton<LobbyManager>.Inst.openChapterFloorIdx;
		}
		set
		{
			GameObjectSingleton<LobbyManager>.Inst.openChapterFloorIdx = value;
		}
	}

	public static bool HasFirstFloorItemUpgrade => GameObjectSingleton<LobbyManager>.Inst.listFloorController[0].ListFloorItem[0].HasUpgrade;

	public static Vector3 UserArenaTicketPosition => GameObjectSingleton<LobbyManager>.Inst.arenaLobby.UserTicketPosition;

	public static Transform ArenaOpenTimer => GameObjectSingleton<LobbyManager>.Inst.arenaLobby.ArenaOpenTimer;

	public static Transform ArenaLevelContent => GameObjectSingleton<LobbyManager>.Inst.arenaLobby.LevelContent;

	public static Transform ArenaLevelContentDimmed => GameObjectSingleton<LobbyManager>.Inst.arenaLobby.LevelContentDimmed;

	public static Transform ArenaShopButton => GameObjectSingleton<LobbyManager>.Inst.arenaLobby.ArenaShopButton;

	public static Transform ArenaMenuButton => GameObjectSingleton<LobbyManager>.Inst.bottomUI.ArenaButton;

	public static Transform BattleButton => GameObjectSingleton<LobbyManager>.Inst.bottomUI.BattleButton;

	public static Transform FloorDetailUpgradeButton => GameObjectSingleton<LobbyManager>.Inst.floorDetail.UpgradeButton;

	public static Transform FloorUpgradeAbility => GameObjectSingleton<LobbyManager>.Inst.floorUpgrade.StoreAbility;

	public static Transform FloorUpgradeRequireItemAnchor => GameObjectSingleton<LobbyManager>.Inst.floorUpgrade.ReauireItemAnchor;

	public static Transform FloorUpgradeConfimButton => GameObjectSingleton<LobbyManager>.Inst.floorUpgrade.UpgradeButton;

	public static Transform FirstStageCell => GameObjectSingleton<LobbyManager>.Inst.stageSelect.FirstStageCell;

	public static Transform FirstFloorOpenButton => GameObjectSingleton<LobbyManager>.Inst.listFloorController[0].ListFloorItem[0].TrOpenButton;

	public static Transform FirstFloorCollectButton => GameObjectSingleton<LobbyManager>.Inst.listFloorController[0].ListFloorItem[0].TrCollectButton;

	public static FloorItem FirstFloorItem => GameObjectSingleton<LobbyManager>.Inst.listFloorController[0].ListFloorItem[0];

	public static FloorItem SecondFloorItem => GameObjectSingleton<LobbyManager>.Inst.listFloorController[0].ListFloorItem[1];

	public static Vector3 FloorStorePosition => GameObjectSingleton<LobbyManager>.Inst.listFloorController[0].ListFloorItem[0].TrStore.position;

	public static LevelCell SecondLevelCell => GameObjectSingleton<LobbyManager>.Inst.levelSelect.SecondLevelCell;

	public static Transform LevelPlayButton => GameObjectSingleton<LobbyManager>.Inst.levelPlay.PlayButton;

	public static TimeCheckState ChestTimeState => LocalTimeCheckManager.GetTimeState("mysteriousChestOpenTime");

	public static Vector3 UserCoinPosition => GameObjectSingleton<LobbyManager>.Inst.topUI.UserCoinPosition;

	public static Vector3 UserLevelPosition => GameObjectSingleton<LobbyManager>.Inst.topUI.UserLevelPosition;

	public static Vector3 UserEnergyPosition => GameObjectSingleton<LobbyManager>.Inst.topUI.UserEnergyPosition;

	public static Vector3 UserJewelPosition => GameObjectSingleton<LobbyManager>.Inst.topUI.UserJewelPosition;

	public static Transform GetChestButton => GameObjectSingleton<LobbyManager>.Inst.bottomUI.transform.GetChild(3);

	public static Transform GetWornChestButton => GameObjectSingleton<LobbyManager>.Inst.chest.transform.GetChild(6).GetChild(0).GetChild(4);

	public static Transform GetMysteriousChestButton => GameObjectSingleton<LobbyManager>.Inst.chest.MysteriousChestButton;

	public static Transform GetMysteriousFreeChestButton => GameObjectSingleton<LobbyManager>.Inst.chest.FreeChestButton;

	public static Transform GetHunterListButton => GameObjectSingleton<LobbyManager>.Inst.bottomUI.transform.GetChild(2);

	public static Transform GetChestHunterUI => GameObjectSingleton<LobbyManager>.Inst.chestOpen.transform.GetChild(0).GetChild(5);

	public static Transform GetHunter => GameObjectSingleton<LobbyManager>.Inst.hunterList.GetHunter;

	public static Transform GetHunterDeckEdit1 => GameObjectSingleton<LobbyManager>.Inst.deckEdit.GetHunter1;

	public static Transform GetHunterDeckEdit2 => GameObjectSingleton<LobbyManager>.Inst.deckEdit.GetHunter2;

	public static Transform GetDeckEditBackButton => GameObjectSingleton<LobbyManager>.Inst.deckEdit.transform.GetChild(3);

	public static Transform GetDeckEditButton => GameObjectSingleton<LobbyManager>.Inst.levelPlay.transform.GetChild(6).GetChild(6);

	public static Transform GetChestResultOkButton => GameObjectSingleton<LobbyManager>.Inst.chestOpen.transform.GetChild(3).GetChild(0).GetChild(2);

	public static Transform GetChestBackButton => GameObjectSingleton<LobbyManager>.Inst.chest.transform.GetChild(7);

	public static Transform GetHunterEnchantCard => GameObjectSingleton<LobbyManager>.Inst.hunterView.transform.GetChild(1).GetChild(1).GetChild(6);

	public static Transform LevelUpBT => GameObjectSingleton<LobbyManager>.Inst.hunterView.transform.GetChild(2).GetChild(1);

	public static Transform LevelUpPlayBT => GameObjectSingleton<LobbyManager>.Inst.hunterLevel.transform.GetChild(3);

	public static Transform LevelUpRequiredItem => GameObjectSingleton<LobbyManager>.Inst.hunterLevel.transform.GetChild(1).GetChild(1);

	public static Transform GetHunterList => GameObjectSingleton<LobbyManager>.Inst.hunterList.transform;

	public static Transform GetHunterLevel => GameObjectSingleton<LobbyManager>.Inst.hunterLevel.transform;

	public static HunterInfo HunterLevelHunterInfo => GameObjectSingleton<LobbyManager>.Inst.hunterLevel.HunterInfo;

	public static HunterInfo HunterPromotionHunterInfo => GameObjectSingleton<LobbyManager>.Inst.hunterPromotion.HunterInfo;

	public static UserFloorData GetUserFloorData()
	{
		return GameInfo.userData.userFloorState[GameObjectSingleton<LobbyManager>.Inst.openStageFloorIdx].floorList[GameObjectSingleton<LobbyManager>.Inst.openChapterFloorIdx];
	}

	public static Transform GetFloorBadge(int castleId, int floorId)
	{
		return GameObjectSingleton<LobbyManager>.Inst.listFloorController[castleId].ListFloorItem[floorId].TrBadge;
	}

	public static Transform GetFloorTouchCollect(int castleId, int floorId)
	{
		return GameObjectSingleton<LobbyManager>.Inst.listFloorController[castleId].ListFloorItem[floorId].TrTouchCollect;
	}

	public static void CallUserData(Action _onCallBack)
	{
		GameObjectSingleton<LobbyManager>.Inst.userData_onCallBack = _onCallBack;
		Protocol_Set.Protocol_user_info_Req(GameObjectSingleton<LobbyManager>.Inst.ResponseUserData);
	}

	public static void ShowChapterList(int stageid)
	{
		if (!GameObjectSingleton<LobbyManager>.Inst.levelSelect.gameObject.activeSelf)
		{
			GameInfo.inGamePlayData.stage = stageid;
			GameObjectSingleton<LobbyManager>.Inst.levelSelect.Show(stageid);
		}
	}

	public static void ShowLevelPlay(int levelIndex)
	{
		if (!GameObjectSingleton<LobbyManager>.Inst.levelPlay.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.levelPlay.Show(levelIndex);
		}
	}

	public static void ShowQuickLoot(int levelIndex)
	{
		if (!GameObjectSingleton<LobbyManager>.Inst.quickLoot.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.quickLoot.Show(levelIndex);
			if (OpenQuickLoot != null)
			{
				OpenQuickLoot();
			}
		}
	}

	public static void ShowChestOpen(List<ChestListDbData> _chestList, ChestType _chestType, int _openCount = -1)
	{
		if (!GameObjectSingleton<LobbyManager>.Inst.chestOpen.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.chestOpen.Show(_chestList, _chestType);
			if (_openCount > 0)
			{
				GameObjectSingleton<LobbyManager>.Inst.chestOpen.SetRemainOpenCount(_openCount);
			}
		}
	}

	public static void ShowChestOpenResult()
	{
		if (GameObjectSingleton<LobbyManager>.Inst.chestOpen.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.chestOpen.ShowChestResult();
		}
	}

	public static void ShowValueShopBuy(int _key, string _type)
	{
		if (!GameObjectSingleton<LobbyManager>.Inst.valueShopBuy.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.valueShopBuy.Show(_key, _type);
		}
	}

	public static void ShowValueShop_Refresh()
	{
		GameObjectSingleton<LobbyManager>.Inst.valueShop.SetInit();
	}

	public static void ShowValueShop(ValueShopType type = ValueShopType.Daily)
	{
		GameObjectSingleton<LobbyManager>.Inst.valueShop.Show(type);
		if (GameObjectSingleton<LobbyManager>.Inst.arenaLobby.HasOpen)
		{
			GameObjectSingleton<LobbyManager>.Inst.topUI.Show();
			GameObjectSingleton<LobbyManager>.Inst.arenaLobby.HideArenaTopUI();
		}
		GameObjectSingleton<LobbyManager>.Inst.HideReadyMonsterList();
	}

	public static void CloseValueShopBuy()
	{
		if (GameObjectSingleton<LobbyManager>.Inst.valueShopBuy.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.valueShopBuy.OnClickGoBack();
		}
	}

	public static void ShowHunterList()
	{
		GameObjectSingleton<LobbyManager>.Inst.hunterList.SetInit();
	}

	public static void ShowLevelPlay()
	{
		GameObjectSingleton<LobbyManager>.Inst.levelPlay.RefreshDeck();
	}

	public static void ShowArenaLevelPlay()
	{
		GameObjectSingleton<LobbyManager>.Inst.arenaLevelPlay.RefreshDeck();
	}

	public static void ShowQuickLoot()
	{
		GameObjectSingleton<LobbyManager>.Inst.quickLoot.RefreshDeck();
	}

	public static void ShowHunterView(HunterInfo _hunterInfo, bool _isSpawn, bool _isOwn = true)
	{
		if (_isSpawn)
		{
			GameObjectSingleton<LobbyManager>.Inst.hunterView.Show(_hunterInfo, _isOwn);
		}
		else
		{
			GameObjectSingleton<LobbyManager>.Inst.hunterView.SetInit(_hunterInfo, _isOwn);
		}
		if (OpenHunterInfo != null)
		{
			OpenHunterInfo();
		}
		GameObjectSingleton<LobbyManager>.Inst.HideReadyMonsterList();
	}

	public static void ShowHunterLevel(HunterInfo _hunterInfo, bool _isSpawn)
	{
		if (_isSpawn)
		{
			GameObjectSingleton<LobbyManager>.Inst.hunterLevel.Show(_hunterInfo);
		}
		else
		{
			GameObjectSingleton<LobbyManager>.Inst.hunterLevel.SetInit(_hunterInfo);
		}
		if (OpenHunterLevel != null)
		{
			OpenHunterLevel();
		}
	}

	public static void ShowHunterLevelUp(HunterInfo _hunterInfo_before, HunterInfo _hunterInfo_after, bool _isSpawn)
	{
		if (_isSpawn)
		{
			GameObjectSingleton<LobbyManager>.Inst.hunterLevelUp.Show(_hunterInfo_before, _hunterInfo_after);
		}
		else
		{
			GameObjectSingleton<LobbyManager>.Inst.hunterLevelUp.SetInit(_hunterInfo_before, _hunterInfo_after);
		}
		if (OpenHunterLevelUp != null)
		{
			OpenHunterLevelUp();
		}
	}

	public static void ShowHunterPromotion(HunterInfo _hunterInfo, bool _isSpawn)
	{
		if (_isSpawn)
		{
			GameObjectSingleton<LobbyManager>.Inst.hunterPromotion.Show(_hunterInfo);
		}
		else
		{
			GameObjectSingleton<LobbyManager>.Inst.hunterPromotion.SetInit(_hunterInfo);
		}
	}

	public static void ShowHunterPromotionUp(HunterInfo _hunterInfo, bool _isSpawn)
	{
		if (_isSpawn)
		{
			GameObjectSingleton<LobbyManager>.Inst.hunterPromotionUp.Show(_hunterInfo);
		}
		else
		{
			GameObjectSingleton<LobbyManager>.Inst.hunterPromotionUp.SetInit(_hunterInfo);
		}
	}

	public static void ShowFloorDetail(UserFloorData _userFloorData, StoreProduceDbData _produceData)
	{
		if (!GameObjectSingleton<LobbyManager>.Inst.floorDetail.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.floorDetail.Show(_userFloorData, _produceData);
			if (StoreDetailEnter != null)
			{
				StoreDetailEnter();
			}
		}
	}

	public static void ShowFloorUpgrade(int storeIdx, int storeTier)
	{
		if (!GameObjectSingleton<LobbyManager>.Inst.floorUpgrade.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.floorUpgrade.Show(storeIdx, storeTier);
			if (StoreUpgradeEnter != null)
			{
				StoreUpgradeEnter();
			}
		}
	}

	public static void ShowFloorUpgradeItemDimmed()
	{
		GameObjectSingleton<LobbyManager>.Inst.floorUpgrade.ShowDimmedRequireItem();
	}

	public static void ShowUserLevelInfo()
	{
		if (!GameObjectSingleton<LobbyManager>.Inst.userLevelInfo.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.userLevelInfo.Show();
		}
	}

	public static void ShowUserEnergyInfo()
	{
		if (!GameObjectSingleton<LobbyManager>.Inst.userEnergyInfo.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.userEnergyInfo.Show();
			GameObjectSingleton<LobbyManager>.Inst.HideReadyMonsterList();
		}
	}

	public static void ShowItemSortList(int itemIdx, bool isWaveItemSort = false)
	{
		UnityEngine.Debug.Log("!!!!");
		if (!GameObjectSingleton<LobbyManager>.Inst.itemSortList.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.itemSortList.Show(itemIdx, isWaveItemSort);
		}
	}

	public static void ShowNotEnoughCoin(int lackCoin)
	{
		if (!GameObjectSingleton<LobbyManager>.Inst.notEnoughCoin.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.notEnoughCoin.Show(lackCoin);
		}
	}

	public static void ShowNotEnoughJewel(int lackJewel)
	{
		if (!GameObjectSingleton<LobbyManager>.Inst.notEnoughJewel.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.notEnoughJewel.Show(lackJewel);
		}
	}

	public static void ShowSpeedUpCollect(float _second, int _needJewel, int _getCoin, Action<bool> _callBack)
	{
		if (!GameObjectSingleton<LobbyManager>.Inst.speedUpCollect.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.speedUpCollect.Show(_second, _needJewel, _getCoin, _callBack);
		}
	}

	public static void ShowRewardResult(ChestListDbData[] _arrData)
	{
		if (!GameObjectSingleton<LobbyManager>.Inst.rewardResult.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.rewardResult.Show(RewardResult.RewardResultType.DailyBonus, _arrData);
		}
	}

	public static void ShowUserLevelUp()
	{
		GameDataManager.UserLevelUp();
	}

	public static void ShowArenaLevelPlay(ARENA_INFO_DATA_RESULT _data)
	{
		GameObjectSingleton<LobbyManager>.Inst.arenaLevelPlay.Show(_data);
		GameObjectSingleton<LobbyManager>.Inst.arenaLobby.HideHunters();
	}

	public static void ShowArenaEventEnd(ARENA_REWARD_INFO _data)
	{
		GameObjectSingleton<LobbyManager>.Inst.arenaEventEnd.Show(_data);
	}

	public static void ShowArenaTicketNone()
	{
		GameObjectSingleton<LobbyManager>.Inst.arenaTicketNone.Show();
	}

	public static void HideSpeedUpCollect()
	{
		GameObjectSingleton<LobbyManager>.Inst.speedUpCollect.Hide();
	}

	public static void ShowSaleArenaPack()
	{
		GameObjectSingleton<LobbyManager>.Inst.saleArenaPack.Show();
	}

	public static bool CheckShowArenaLobbyHunter()
	{
		return !GameObjectSingleton<LobbyManager>.Inst.arenaLevelPlay.gameObject.activeSelf;
	}

	public static void EndArenaTutorial()
	{
		GameObjectSingleton<LobbyManager>.Inst.arenaLobby.EndArenaTutorial();
	}

	public static void MoveCastle(int index)
	{
		if (GameObjectSingleton<LobbyManager>.Inst.castleUI.HasCastleActive(index))
		{
			GameObjectSingleton<LobbyManager>.Inst.castleUI.MoveCastle(index);
			GameObjectSingleton<LobbyManager>.Inst.castleScrollSnap.SnapToIndex(index);
		}
	}

	public static void MoveStore(int castleIdx, int floorIdx)
	{
		MoveCastle(castleIdx);
		GameObjectSingleton<LobbyManager>.Inst.listFloorController[castleIdx].MoveToFloor(floorIdx);
	}

	public static void OnGoBackLevel()
	{
		GameObjectSingleton<LobbyManager>.Inst.hunterLevel.Hide();
	}

	public static void OnGoBackPromotion()
	{
		GameObjectSingleton<LobbyManager>.Inst.hunterPromotion.Hide();
	}

	public static void HideHunterLobby()
	{
		if (GameObjectSingleton<LobbyManager>.Inst.hunterPromotion.HunterCheckNull() && GameObjectSingleton<LobbyManager>.Inst.hunterPromotion.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.hunterPromotion.HunterTransform.gameObject.SetActive(value: false);
		}
		else if (GameObjectSingleton<LobbyManager>.Inst.hunterLevel.HunterCheckNull() && GameObjectSingleton<LobbyManager>.Inst.hunterLevel.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.hunterLevel.HunterTransform.gameObject.SetActive(value: false);
		}
		else if (GameObjectSingleton<LobbyManager>.Inst.hunterView.HunterCheckNull() && GameObjectSingleton<LobbyManager>.Inst.hunterView.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.hunterView.HunterTransform.gameObject.SetActive(value: false);
		}
	}

	public static void ShowHunterLobby()
	{
		UnityEngine.Debug.Log("@@@@@ 11 ");
		if (GameObjectSingleton<LobbyManager>.Inst.hunterPromotion.HunterCheckNull() && GameObjectSingleton<LobbyManager>.Inst.hunterPromotion.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.hunterPromotion.HunterTransform.gameObject.SetActive(value: true);
		}
		else if (GameObjectSingleton<LobbyManager>.Inst.hunterLevel.HunterCheckNull() && GameObjectSingleton<LobbyManager>.Inst.hunterLevel.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.hunterLevel.HunterTransform.gameObject.SetActive(value: true);
		}
		else if (GameObjectSingleton<LobbyManager>.Inst.hunterView.HunterCheckNull() && GameObjectSingleton<LobbyManager>.Inst.hunterView.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.hunterView.HunterTransform.gameObject.SetActive(value: true);
		}
		UnityEngine.Debug.Log("@@@@@ 22 ");
	}

	public static void MoveToBadgeFloor(int itemIdx)
	{
		UnityEngine.Debug.Log("@@@@@");
		GameObjectSingleton<LobbyManager>.Inst.hunterList.Hide();
		GameObjectSingleton<LobbyManager>.Inst.hunterPromotion.Hide();
		GameObjectSingleton<LobbyManager>.Inst.hunterPromotionUp.Hide();
		GameObjectSingleton<LobbyManager>.Inst.hunterView.Hide();
		for (int i = 0; i < GameObjectSingleton<LobbyManager>.Inst.listFloorController.Count; i++)
		{
			FloorController floorController = GameObjectSingleton<LobbyManager>.Inst.listFloorController[i];
			for (int j = 0; j < floorController.ListFloorItem.Count; j++)
			{
				FloorItem floorItem = floorController.ListFloorItem[j];
				if (floorItem.BadgeIdx == itemIdx)
				{
					GameObjectSingleton<LobbyManager>.Inst.srcollRectFloor.horizontalNormalizedPosition = 1 - i / GameObjectSingleton<LobbyManager>.Inst.listFloorController.Count;
					floorController.MoveToFloor(j);
				}
			}
		}
	}

	public static void GotoInGame(int levelIndex)
	{
		GameObjectSingleton<LobbyManager>.Inst.topUI.Exit();
		GameObjectSingleton<LobbyManager>.Inst.bottomUI.Exit();
		GameObjectSingleton<LobbyManager>.Inst.castleUI.Exit();
		GameObjectSingleton<LobbyManager>.Inst.levelSelect.Hide();
		LobbyPopupBase[] componentsInChildren = GameObjectSingleton<LobbyManager>.Inst.trLobbyPopupFront.GetComponentsInChildren<LobbyPopupBase>();
		foreach (LobbyPopupBase lobbyPopupBase in componentsInChildren)
		{
			lobbyPopupBase.Hide();
		}
		LobbyPopupBase[] componentsInChildren2 = GameObjectSingleton<LobbyManager>.Inst.trLobbyPopupBack.GetComponentsInChildren<LobbyPopupBase>();
		foreach (LobbyPopupBase lobbyPopupBase2 in componentsInChildren2)
		{
			lobbyPopupBase2.Hide();
		}
		GameInfo.inGamePlayData.inGameType = InGameType.Stage;
		GameInfo.inGamePlayData.star2ClearTurn = GameDataManager.GetLevelIndexDbData(levelIndex).star2;
		GameInfo.inGamePlayData.star3ClearTurn = GameDataManager.GetLevelIndexDbData(levelIndex).star3;
		GameInfo.inGamePlayData.matchTime = GameDataManager.GetGameConfigData(ConfigDataType.TurnTimeTotal);
		GameInfo.inGamePlayData.matchTimeBonus = GameDataManager.GetGameConfigData(ConfigDataType.TurnTimeBonus);
		GameInfo.inGamePlayData.matchTimeRatio = GameDataManager.GetGameConfigData(ConfigDataType.TurnTimeRatio);
		LevelDbData levelIndexDbData = GameDataManager.GetLevelIndexDbData(levelIndex);
		GameInfo.inGamePlayData.puzzleR = levelIndexDbData.puzzleR;
		GameInfo.inGamePlayData.puzzleY = levelIndexDbData.puzzleY;
		GameInfo.inGamePlayData.puzzleG = levelIndexDbData.puzzleG;
		GameInfo.inGamePlayData.puzzleB = levelIndexDbData.puzzleB;
		GameInfo.inGamePlayData.puzzleP = levelIndexDbData.puzzleP;
		GameInfo.inGamePlayData.puzzleH = levelIndexDbData.puzzleH;
		GameInfo.inGamePlayData.isDragon = levelIndexDbData.isDragon;
		if (GameDataManager.GetDicScenarioInGameDbData().ContainsKey(levelIndex))
		{
			GameInfo.inGamePlayData.isShowScenario = GameDataManager.LoadScenarioInGameShow(levelIndex);
		}
		else
		{
			GameInfo.inGamePlayData.isShowScenario = false;
		}
		GameInfo.inGamePlayData.dicMonsterStatData.Clear();
		GameInfo.inGamePlayData.dicWaveDbData = new Dictionary<int, WaveDbData>();
		foreach (KeyValuePair<int, WaveDbData> dicWaveDbDatum in GameDataManager.GetDicWaveDbData(levelIndex))
		{
			GameInfo.inGamePlayData.dicWaveDbData.Add(dicWaveDbDatum.Key, dicWaveDbDatum.Value);
		}
		foreach (KeyValuePair<int, WaveDbData> dicWaveDbDatum2 in GameInfo.inGamePlayData.dicWaveDbData)
		{
			if (!GameInfo.inGamePlayData.dicMonsterStatData.ContainsKey(dicWaveDbDatum2.Value.wave))
			{
				GameInfo.inGamePlayData.dicMonsterStatData.Add(dicWaveDbDatum2.Value.wave, new List<MonsterStatDbData>());
			}
			if (dicWaveDbDatum2.Value.spawnM1 > 0)
			{
				GameInfo.inGamePlayData.dicMonsterStatData[dicWaveDbDatum2.Value.wave].Add(GameDataManager.GetMonsterStatData(dicWaveDbDatum2.Value.spawnM1));
			}
			if (dicWaveDbDatum2.Value.spawnM2 > 0)
			{
				GameInfo.inGamePlayData.dicMonsterStatData[dicWaveDbDatum2.Value.wave].Add(GameDataManager.GetMonsterStatData(dicWaveDbDatum2.Value.spawnM2));
			}
			if (dicWaveDbDatum2.Value.spawnM3 > 0)
			{
				GameInfo.inGamePlayData.dicMonsterStatData[dicWaveDbDatum2.Value.wave].Add(GameDataManager.GetMonsterStatData(dicWaveDbDatum2.Value.spawnM3));
			}
			if (dicWaveDbDatum2.Value.spawnM4 > 0)
			{
				GameInfo.inGamePlayData.dicMonsterStatData[dicWaveDbDatum2.Value.wave].Add(GameDataManager.GetMonsterStatData(dicWaveDbDatum2.Value.spawnM4));
			}
		}
		GameInfo.inGamePlayData.dicHunterInfo.Clear();
		UserHunterData[] huntersUseInfo = GameInfo.userData.huntersUseInfo;
		foreach (UserHunterData userHunterData in huntersUseInfo)
		{
			HunterInfo hunterInfo = GameDataManager.GetHunterInfo(userHunterData.hunterIdx, userHunterData.hunterLevel, userHunterData.hunterTier);
			GameInfo.inGamePlayData.dicHunterInfo.Add(userHunterData.hunterIdx, hunterInfo);
		}
		LocalTimeCheckManager.Clear();
		if (GameInfo.inGamePlayData.stage > 0)
		{
			UserLevelState userLevelState = GameInfo.userData.GetUserLevelState(GameInfo.inGamePlayData.stage - 1, GameInfo.inGamePlayData.chapter - 1, GameInfo.inGamePlayData.level);
			if (!TutorialManager.Intro && GameInfo.inGamePlayData.isShowScenario && userLevelState != null && userLevelState.starCount > 0)
			{
				GameDataManager.SaveScenarioInGameShow(levelIndex);
				GameInfo.inGamePlayData.isShowScenario = false;
			}
		}
		GameDataManager.MoveScene(SceneType.InGame);
	}

	public static void GotoArena(ARENA_GAME_START_RESULT _arenaResult)
	{
		GameObjectSingleton<LobbyManager>.Inst.topUI.Exit();
		GameObjectSingleton<LobbyManager>.Inst.bottomUI.Exit();
		GameObjectSingleton<LobbyManager>.Inst.castleUI.Exit();
		LobbyPopupBase[] componentsInChildren = GameObjectSingleton<LobbyManager>.Inst.trLobbyPopupFront.GetComponentsInChildren<LobbyPopupBase>();
		foreach (LobbyPopupBase lobbyPopupBase in componentsInChildren)
		{
			lobbyPopupBase.Hide();
		}
		LobbyPopupBase[] componentsInChildren2 = GameObjectSingleton<LobbyManager>.Inst.trLobbyPopupBack.GetComponentsInChildren<LobbyPopupBase>();
		foreach (LobbyPopupBase lobbyPopupBase2 in componentsInChildren2)
		{
			lobbyPopupBase2.Hide();
		}
		GameInfo.inGamePlayData.inGameType = InGameType.Arena;
		GameInfo.inGamePlayData.matchTime = GameDataManager.GetGameConfigData(ConfigDataType.TurnTimeTotal);
		GameInfo.inGamePlayData.matchTimeBonus = GameDataManager.GetGameConfigData(ConfigDataType.TurnTimeBonus);
		GameInfo.inGamePlayData.matchTimeRatio = GameDataManager.GetGameConfigData(ConfigDataType.TurnTimeRatio);
		GameInfo.inGamePlayData.puzzleR = _arenaResult.arena_level_info.puzzleR;
		GameInfo.inGamePlayData.puzzleY = _arenaResult.arena_level_info.puzzleY;
		GameInfo.inGamePlayData.puzzleG = _arenaResult.arena_level_info.puzzleG;
		GameInfo.inGamePlayData.puzzleB = _arenaResult.arena_level_info.puzzleB;
		GameInfo.inGamePlayData.puzzleP = _arenaResult.arena_level_info.puzzleP;
		GameInfo.inGamePlayData.puzzleH = _arenaResult.arena_level_info.puzzleH;
		GameInfo.inGamePlayData.isDragon = 0;
		GameInfo.inGamePlayData.levelIdx = 0;
		GameInfo.inGamePlayData.isShowScenario = false;
		GameInfo.inGamePlayData.dicWaveDbData = new Dictionary<int, WaveDbData>();
		GameInfo.inGamePlayData.dicMonsterStatData.Clear();
		for (int k = 0; k < _arenaResult.spec_arena_wave.Length; k++)
		{
			WaveDbData waveDbData = _arenaResult.spec_arena_wave[k];
			GameInfo.inGamePlayData.dicWaveDbData.Add(waveDbData.wave, waveDbData);
			if (!GameInfo.inGamePlayData.dicMonsterStatData.ContainsKey(waveDbData.wave))
			{
				GameInfo.inGamePlayData.dicMonsterStatData.Add(waveDbData.wave, new List<MonsterStatDbData>());
			}
			MonsterStatDbData[] array = _arenaResult.spec_monster_stat[k];
			foreach (MonsterStatDbData item in array)
			{
				GameInfo.inGamePlayData.dicMonsterStatData[waveDbData.wave].Add(item);
			}
		}
		GameInfo.inGamePlayData.dicHunterInfo.Clear();
		UserHunterData[] huntersArenaUseInfo = GameInfo.userData.huntersArenaUseInfo;
		foreach (UserHunterData userHunterData in huntersArenaUseInfo)
		{
			HunterInfo hunterInfo = GameDataManager.GetHunterInfo(userHunterData.hunterIdx, userHunterData.hunterLevel, userHunterData.hunterTier);
			GameInfo.inGamePlayData.dicHunterInfo.Add(userHunterData.hunterIdx, hunterInfo);
		}
		LocalTimeCheckManager.Clear();
		GameDataManager.MoveScene(SceneType.InGame);
	}

	public static bool CheckHunterAlert()
	{
		bool flag = false;
		if (GameObjectSingleton<LobbyManager>.Inst.CheckHunterLevelUp())
		{
			flag = true;
		}
		if (!flag && GameObjectSingleton<LobbyManager>.Inst.CheckHunterPromotion())
		{
			flag = true;
		}
		return flag;
	}

	public static bool CheckChestAlert()
	{
		bool flag = false;
		if (GameInfo.userData.userStageState[0].chapterList[0].levelList.Length < 5)
		{
			return false;
		}
		if (ChestTimeState != TimeCheckState.Progress)
		{
			flag = true;
		}
		if (!flag && GameInfo.userData.userInfo.chestKey >= 1)
		{
			flag = true;
		}
		return flag;
	}

	public static void CallBadgeAcquireEvent(int castleId, int floorId)
	{
		if (StoreBadgeAcquire != null)
		{
			StoreBadgeAcquire(castleId, floorId);
		}
	}

	public static void CallQuickLootRefrechEvent()
	{
		if (QuickLootRefresh != null)
		{
			QuickLootRefresh();
		}
	}

	public static void ShowHunterListForce()
	{
		GameObjectSingleton<LobbyManager>.Inst.hunterList.Show();
		GameObjectSingleton<LobbyManager>.Inst.HideAllStore();
		if (OpenHunterList != null)
		{
			OpenHunterList();
		}
	}

	public static void StoreBadgeCollect()
	{
		if (StoreTouchCollect != null)
		{
			StoreTouchCollect();
		}
	}

	public static void ReadyLevelPlay()
	{
		if (LevelPlayShow != null)
		{
			LevelPlayShow();
		}
	}

	public static void RefreshBottomNotice()
	{
		GameObjectSingleton<LobbyManager>.Inst.bottomUI.RefreshNotice();
	}

	public static void GetExpEff(Vector3 _pos)
	{
		GameObjectSingleton<LobbyManager>.Inst.GetExpEffStart(_pos);
	}

	public static void GetCoinEff(Vector3 _pos)
	{
		GameObjectSingleton<LobbyManager>.Inst.GetCoinEffStart(_pos);
	}

	public static void GetJewelEff(Vector3 _pos)
	{
		GameObjectSingleton<LobbyManager>.Inst.GetJewelEffStart(_pos);
	}

	public static void PurchaseStartPackComplete()
	{
		GameObjectSingleton<LobbyManager>.Inst.shopPackageListResult.starterPackYn = "y";
	}

	public static void PurchaseSpecialOfferComplete()
	{
		GameObjectSingleton<LobbyManager>.Inst.shopPackageListResult.specialOfferYn = "y";
	}

	public static void HunterCardClickForTUtorial(HunterCard _card)
	{
		GameObjectSingleton<LobbyManager>.Inst.deckEdit.OnSelect_HunterCardForTutorial(_card);
	}

	public static void CheckDailyBonusConnect()
	{
		GameObjectSingleton<LobbyManager>.Inst.ShowDailyBonus();
	}

	public static void HideTopUI()
	{
		GameObjectSingleton<LobbyManager>.Inst.topUI.Hide();
	}

	public static void HideDailyAndPackage()
	{
		GameObjectSingleton<LobbyManager>.Inst.dailyBonus.Hide();
		GameObjectSingleton<LobbyManager>.Inst.saleStartPack.Hide();
		GameObjectSingleton<LobbyManager>.Inst.saleSpecialOffer.Hide();
	}

	private void Init()
	{
		GameInfo.isForceRandomBlockPattern = false;
		GameInfo.isDirectBattleReward = false;
		base.gameObject.SetActive(value: true);
		GameUtil.SetUseHunterList();
		GameUtil.SetOwnHunterList(HUNTERLIST_TYPE.NORMAL);
		topUI.Init();
		castleUI.Init();
		topUI.RefreshData();
		castleUI.RefreshNotice();
		bottomUI.RefreshNotice();
		stageSelect.GoBackEvent = OnStageSelectGoBackEvent;
		levelSelect.GoBackEvent = OnLevelSelectGoBackEvent;
		levelPlay.GoBackEvent = OnLevelPlayGoBackEvent;
		quickLoot.GoBackEvent = OnQuickLootGoBackEvent;
		chest.GoBackEvent = OnChestGoBackEvent;
		deckEdit.GoBackEvent = OnDeckEditGoBackEvent;
		chestOpen.GoBackEvent = OnChestOpenGoBackEvent;
		valueShop.GoBackEvent = OnValueShopGoBackEvent;
		valueShopBuy.GoBackEvent = OnValueShopBuyGoBackEvent;
		hunterList.GoBackEvent = OnHunterListGoBackEvent;
		setting.GoBackEvent = OnSettingGoBackEvent;
		hunterView.GoBackEvent = OnHunterViewGoBackEvent;
		hunterLevel.GoBackEvent = OnHunterLevelGoBackEvent;
		hunterLevelUp.GoBackEvent = OnHunterLevelUpGoBackEvent;
		hunterPromotion.GoBackEvent = OnHunterPromotionGoBackEvent;
		hunterPromotionUp.GoBackEvent = OnHunterPromotionUpGoBackEvent;
		floorDetail.GoBackEvent = OnFloorDetailGoBackEvent;
		floorUpgrade.GoBackEvent = OnFloorUpgradeGoBackEvent;
		userLevelInfo.GoBackEvent = OnUserLevelInfoGoBackEvent;
		userEnergyInfo.GoBackEvent = OnUserEnergyInfoGoBackEvent;
		itemSortList.GoBackEvent = OnItemSortListGoBackEvent;
		notEnoughCoin.GoBackEvent = OnNotEnoughCoinGoBackEvent;
		notEnoughJewel.GoBackEvent = OnNotEnoughJewelGoBackEvent;
		speedUpCollect.GoBackEvent = OnSpeedUpCollectGoBackEvent;
		gameQuitPopup.GoBackEvent = OnGamqQuitGoBackEvent;
		dailyBonus.GoBackEvent = OnDailyBonusGoBackEvent;
		rewardResult.GoBackEvent = OnRewardResultGoBackEvent;
		dailyBonus.RewardResultComplete = OnDailyBonusComplete;
		saleStartPack.GoBackEvent = OnSaleStartPackGoBackEvent;
		saleSpecialOffer.GoBackEvent = OnSaleSpecialOfferGoBackEvent;
		saleArenaPack.GoBackEvent = OnSaleArenaPackGoBackEvent;
		arenaLobby.GoBackEvent = OnArenaLobbyGoBackEvent;
		arenaShop.GoBackEvent = OnArenaShopGoBackEvent;
		arenaShopBuy.GoBackEvent = OnArenaShopBuyGoBackEvent;
		arenaLevelPlay.GoBackEvent = OnArenaLevelPlayGoBackEvent;
		arenaEventEnd.GoBackEvent = OnArenaEventEndGoBackEvent;
		arenaTicketNone.GoBackEvent = OnArenaTicketNoneGoBackEvent;
		stageSelect.SelectStageEvent = OnSelectStageCell;
		castleScrollSnap.ScrollPageSnapEvent = OnCastleSnapComplete;
		GameDataManager.ChangeUserData = (Action)Delegate.Combine(GameDataManager.ChangeUserData, new Action(OnChangeUserDataEvent));
		GameDataManager.ChangeStoreData = (Action)Delegate.Combine(GameDataManager.ChangeStoreData, new Action(OnChangeStoreDataEvent));
		InitForFloor();
		CheckFreeChestTimer();
		CheckEnergyLocalPush();
		CheckFloorLocalPush();
		CheckDailyShopLocalPush();
		CheckFreeChestLocalPush();
		GameInfo.inGamePlayData.dicActiveBoostItem.Clear();
		switch (GameInfo.inGamePlayData.inGameType)
		{
		case InGameType.Stage:
			SoundController.BGM_Stop(MusicSoundType.IngameBGM);
			SoundController.BGM_Stop(MusicSoundType.InGameDragonBgm);
			break;
		case InGameType.Arena:
			SoundController.BGM_Stop(MusicSoundType.ArenaBGM);
			break;
		}
		SoundController.BGM_Play(MusicSoundType.LobbyBGM);
		TutorialManager.CheckTutorial();
		CheckChestLock();
		
		if (GameInfo.inGamePlayData.inGameType == InGameType.Arena)
		{
			arenaLobby.Show();
		}
		if (!TutorialManager.Intro)
		{
			if (!TutorialManager.DialogState && !TutorialManager.ProgressTutorial)
			{
				ShowDailyBonus();
			}
			else
			{
				CheckLobbyState();
			}
		}
		GameDataManager.SaveInGameBattleSpeed(GameInfo.inGameBattleSpeedRate);
	}

	private void CheckRate()
	{
		if (!GamePreferenceManager.GetIsRate() && GameInfo.isRate)
		{
			OnShowRate();
		}
	}

	private void CheckLobbyState()
	{
		MoveCaslteUnLock();
		StartCoroutine(MoveToUnLockStore());
		if (GameInfo.userData.userInfo.levelUpYn == "y")
		{
			ShowUserLevelUp();
		}
	}

	private void CheckChestLock()
	{
		if (GameInfo.userData.userStageState[0].chapterList[0].levelList.Length < 5)
		{
			trChestLock.gameObject.SetActive(value: true);
		}
	}

	private void InitForFloor()
	{
		listFloorController.Clear();
		UserFloorStage[] userFloorState = GameInfo.userData.userFloorState;
		for (int i = 0; i < userFloorState.Length; i++)
		{
			FloorController component = MWPoolManager.Spawn("Lobby", "Floor Scroll View2", trFloorStageContent).GetComponent<FloorController>();
			component.Init(i);
			FloorController floorController = component;
			floorController.StartStoreOpen = (Action)Delegate.Combine(floorController.StartStoreOpen, new Action(OnStartStoreOpen));
			FloorController floorController2 = component;
			floorController2.CollectComplete = (Action)Delegate.Combine(floorController2.CollectComplete, new Action(OnStoreCollectComplete));
			listFloorController.Add(component);
		}
	}

	private void RefreshFloor()
	{
		FloorController[] componentsInChildren = trFloorStageContent.GetComponentsInChildren<FloorController>();
		foreach (FloorController floorController in componentsInChildren)
		{
			floorController.Refresh();
		}
	}

	private void ShowDailyBonus()
	{
		dailyBonus.Show();
	}

	private void CallShopPackageList()
	{
		Protocol_Set.Protocol_shop_package_list_Req(OnShopPackageListComplete);
	}

	private void OnStageSelectGoBackEvent()
	{
		stageSelect.Hide();
		ShowAllStore();
	}

	private void OnLevelSelectGoBackEvent()
	{
		levelSelect.Hide();
	}

	private void OnLevelPlayGoBackEvent()
	{
		levelPlay.Hide();
	}

	private void OnQuickLootGoBackEvent()
	{
		quickLoot.Hide();
		RefreshFloor();
	}

	private void OnChestGoBackEvent()
	{
		chest.Hide();
		RefreshFloor();
		ShowAllStore();
	}

	private void OnDeckEditGoBackEvent()
	{
		if (deckEdit.EditType == HUNTERLIST_TYPE.NORMAL)
		{
			levelPlay.RefreshDeck();
			hunterList.RefreshDeck();
			quickLoot.RefreshDeck();
		}
		else
		{
			arenaLevelPlay.RefreshDeck();
		}
		deckEdit.Hide();
		RefreshFloor();
		ShowReadyMonsterList();
	}

	private void OnValueShopGoBackEvent()
	{
		valueShop.Hide();
		RefreshFloor();
		if (arenaLobby.HasOpen)
		{
			topUI.Hide();
			arenaLobby.ShowArenaTopUI();
		}
		CheckLobbyHunterShow("valueShop");
		ShowReadyMonsterList();
	}

	private void OnValueShopBuyGoBackEvent()
	{
		valueShopBuy.Hide();
		RefreshFloor();
	}

	private void OnChestOpenGoBackEvent()
	{
		chestOpen.Hide();
		RefreshFloor();
	}

	private void OnHunterListGoBackEvent()
	{
		hunterList.Hide();
		ShowAllStore();
		RefreshFloor();
	}

	private void OnHunterViewGoBackEvent()
	{
		hunterView.Hide();
		RefreshFloor();
		ShowReadyMonsterList();
	}

	private void OnHunterLevelGoBackEvent()
	{
		hunterLevel.Hide();
		RefreshFloor();
	}

	private void OnSettingGoBackEvent()
	{
		setting.Hide();
		RefreshFloor();
		if (arenaLobby.HasOpen)
		{
			topUI.Hide();
			arenaLobby.ShowArenaTopUI();
		}
		CheckLobbyHunterShow("setting");
	}

	private void OnHunterLevelUpGoBackEvent()
	{
		hunterLevelUp.Hide();
		RefreshFloor();
		ShowHunterLobby();
	}

	private void OnHunterPromotionGoBackEvent()
	{
		hunterPromotion.Hide();
		RefreshFloor();
	}

	private void OnHunterPromotionUpGoBackEvent()
	{
		hunterPromotionUp.Hide();
		RefreshFloor();
	}

	private void OnFloorDetailGoBackEvent()
	{
		floorDetail.Hide();
		RefreshFloor();
	}

	private void OnFloorUpgradeGoBackEvent(bool isForceCollect)
	{
		floorUpgrade.Hide();
		floorDetail.Hide();
		if (isForceCollect)
		{
			listFloorController[openStageFloorIdx].FloorForceCollect(openChapterFloorIdx);
		}
		if (StoreUpgradeComplete != null)
		{
			StoreUpgradeComplete();
		}
	}

	private void OnUserLevelInfoGoBackEvent()
	{
		userLevelInfo.Hide();
		CheckLobbyHunterShow("levelInfo");
	}

	private void OnUserEnergyInfoGoBackEvent()
	{
		userEnergyInfo.Hide();
		RefreshFloor();
		CheckLobbyHunterShow("energyInfo");
		ShowReadyMonsterList();
	}

	private void OnItemSortListGoBackEvent()
	{
		itemSortList.Hide();
		CheckLobbyHunterShow("itemSortList");
	}

	private void OnNotEnoughCoinGoBackEvent()
	{
		notEnoughCoin.Hide();
	}

	private void OnNotEnoughJewelGoBackEvent()
	{
		UnityEngine.Debug.Log("OnNotEnoughJewelGoBackEvent");
		notEnoughJewel.Hide();
	}

	private void OnSpeedUpCollectGoBackEvent()
	{
		speedUpCollect.Hide();
		RefreshFloor();
	}

	private void OnGamqQuitGoBackEvent()
	{
		gameQuitPopup.Hide();
	}

	private void OnDailyBonusGoBackEvent()
	{
		dailyBonus.Hide();
	}

	private void OnRewardResultGoBackEvent(RewardResult.RewardResultType _type)
	{
		rewardResult.Hide();
		if (_type == RewardResult.RewardResultType.DailyBonus)
		{
			dailyBonus.RewardResultEnd();
		}
	}

	private void OnSaleStartPackGoBackEvent()
	{
		CheckRate();
		saleStartPack.Hide();
		CheckLobbyState();
	}

	private void OnSaleSpecialOfferGoBackEvent()
	{
		CheckRate();
		saleSpecialOffer.Hide();
		CheckLobbyState();
	}

	private void OnSaleArenaPackGoBackEvent()
	{
		saleArenaPack.Hide();
	}

	private void OnArenaLobbyGoBackEvent()
	{
		arenaLobby.Hide();
		topUI.Show();
	}

	private void OnArenaShopGoBackEvent()
	{
		arenaShop.Hide();
		arenaLobby.ShowHunters();
	}

	private void OnArenaShopBuyGoBackEvent()
	{
		arenaShopBuy.Hide();
	}

	private void OnArenaLevelPlayGoBackEvent()
	{
		arenaLevelPlay.Hide();
		arenaLobby.ShowHunters();
	}

	private void OnArenaEventEndGoBackEvent()
	{
		arenaEventEnd.Hide();
	}

	private void OnArenaTicketNoneGoBackEvent()
	{
		arenaTicketNone.Hide();
		arenaLevelPlay.ForceGoArena();
	}

	private void OnShopPackageListComplete(SHOP_PACKAGE_LIST_RESULT _result)
	{
		shopPackageListResult = _result;
		if (_result.packageList != null && _result.packageList.Length != 0)
		{
			if (_result.starterPackYn == "n")
			{
				saleStartPack.Show(_result.packageList[0]);
			}
			else if (_result.specialOfferYn == "n")
			{
				saleSpecialOffer.Show(_result.packageList[1]);
			}
			else
			{
				CheckRate();
				CheckLobbyState();
			}
			GameInfo.arenaPackageData = _result.packageList[2];
		}
	}

	private void OnDailyBonusComplete()
	{
		UnityEngine.Debug.Log("OnDailyBonusComplete");
		RefreshFloor();
		CallShopPackageList();
	}

	private void OnChangeUserDataEvent()
	{
		topUI.RefreshData(isCount: true);
		castleUI.RefreshNotice();
		bottomUI.RefreshNotice();
		arenaLobby.RefreshUserData();
		RefreshFloor();
		CheckFreeChestTimer();
		CheckEnergyLocalPush();
		CheckDailyShopLocalPush();
		CheckFreeChestLocalPush();
		CheckFloorLocalPush();
		if (itemSortList.gameObject.activeSelf)
		{
			itemSortList.Refresh();
		}
		if (levelSelect.gameObject.activeSelf)
		{
			levelSelect.Refresh();
		}
	}

	private void OnChangeStoreDataEvent()
	{
		castleUI.RefreshNotice();
		RefreshFloor();
		CheckFloorLocalPush();
	}

	private void CheckEnergyLocalPush()
	{
		if (GameInfo.userData.userInfo.energy < GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level).maxEnergy)
		{
			int value = GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level).maxEnergy - GameInfo.userData.userInfo.energy - 1;
			int num = GameInfo.userData.userInfo.energyRemainTime + Mathf.Clamp(value, 0, GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level).maxEnergy) * GameDataManager.GetGameConfigData(ConfigDataType.EnergyRechargeTime) * 60;
			LocalPushManager.SetNotification(NotiType.FullEnergy, num);
			UnityEngine.Debug.Log("CheckEnergyLocalPush :: " + num);
		}
	}

	private void CheckDailyShopLocalPush()
	{
		if (GameInfo.userData.userInfo.dailyShopRemainTime > 0)
		{
			int dailyShopRemainTime = GameInfo.userData.userInfo.dailyShopRemainTime;
			LocalPushManager.SetNotification(NotiType.DailyShopRefresh, GameInfo.userData.userInfo.dailyShopRemainTime);
			UnityEngine.Debug.Log("CheckDailyShopLocalPush :: " + GameInfo.userData.userInfo.dailyShopRemainTime);
		}
	}

	private void CheckFreeChestLocalPush()
	{
		if (GameInfo.userData.userInfo.chestRemainTime > 0)
		{
			int chestRemainTime = GameInfo.userData.userInfo.chestRemainTime;
			LocalPushManager.SetNotification(NotiType.FreeChest, GameInfo.userData.userInfo.chestRemainTime);
			UnityEngine.Debug.Log("CheckFreeChestLocalPush :: " + GameInfo.userData.userInfo.chestRemainTime);
		}
	}

	private void CheckFloorLocalPush()
	{
		int num = -1;
		UserFloorStage[] userFloorState = GameInfo.userData.userFloorState;
		foreach (UserFloorStage userFloorStage in userFloorState)
		{
			UserFloorData[] floorList = userFloorStage.floorList;
			foreach (UserFloorData userFloorData in floorList)
			{
				if (userFloorData.storeRemainTime > 0 && userFloorData.state == 2 && userFloorData.storeRemainTime > num)
				{
					num = userFloorData.storeRemainTime;
				}
			}
		}
		UnityEngine.Debug.Log("CheckFloorLocalPush :: " + num);
		if (num > 0)
		{
			LocalPushManager.SetNotification(NotiType.StoreReward, num);
		}
		else
		{
			LocalPushManager.CancelNotification(NotiType.StoreReward);
		}
	}

	private void OnUserInfoCommectComplete()
	{
		Init();
	}

	private void CheckFreeChestTimer()
	{
		if (GameInfo.userData.userInfo.chestRemainTime > 0)
		{
			LocalTimeCheckManager.InitType("mysteriousChestOpenTime");
			LocalTimeCheckManager.TimeClear("mysteriousChestOpenTime");
			LocalTimeCheckManager.AddTimer("mysteriousChestOpenTime", GameInfo.userData.userInfo.chestRemainTime);
			LocalTimeCheckManager.OnLocalTimeComplete = (Action<string>)Delegate.Combine(LocalTimeCheckManager.OnLocalTimeComplete, new Action<string>(OnFreeChestComplete));
		}
		else
		{
			LocalTimeCheckManager.TimeComplete("mysteriousChestOpenTime");
		}
		UnityEngine.Debug.Log("CheckFreeChestTimer :: " + GameInfo.userData.userInfo.chestRemainTime);
		bottomUI.RefreshNotice();
	}

	private void OnFreeChestComplete(string type)
	{
		if (type == "mysteriousChestOpenTime")
		{
			bottomUI.RefreshNotice();
		}
	}

	private void ResponseUserData()
	{
		if (GameObjectSingleton<LobbyManager>.Inst.userData_onCallBack != null)
		{
			GameObjectSingleton<LobbyManager>.Inst.userData_onCallBack();
			GameObjectSingleton<LobbyManager>.Inst.userData_onCallBack = null;
		}
	}

	private void ShopListResponse()
	{
		GameObjectSingleton<LobbyManager>.Inst.valueShop.Show();
	}

	private void ArenaShopListResponse(ARENA_STORE_INFO[] _info)
	{
		GameObjectSingleton<LobbyManager>.Inst.arenaLobby.HideHunters();
		arenaShop.Show(_info);
	}

	private bool CheckHunterLevelUp()
	{
		bool flag = false;
		UserHunterData userHunterData = null;
		for (int i = 0; i < GameInfo.userData.huntersUseInfo.Length; i++)
		{
			userHunterData = GameInfo.userData.huntersUseInfo[i];
			flag = GameObjectSingleton<LobbyManager>.Inst.CheckHunterLevelUpDataCheck(userHunterData);
			if (flag)
			{
				break;
			}
		}
		if (!flag && GameInfo.userData.huntersOwnInfo != null)
		{
			for (int j = 0; j < GameInfo.userData.huntersOwnInfo.Length; j++)
			{
				userHunterData = GameInfo.userData.huntersOwnInfo[j];
				flag = GameObjectSingleton<LobbyManager>.Inst.CheckHunterLevelUpDataCheck(userHunterData);
				if (flag)
				{
					break;
				}
			}
		}
		return flag;
	}

	private bool CheckHunterLevelUpDataCheck(UserHunterData hunterdata)
	{
		bool result = false;
		HunterLevelDbData hunterLevelDbData = null;
		UnityEngine.Debug.Log("CheckHunterLevelUpDataCheck :: " + hunterdata);
		HunterInfo hunterInfo = GameDataManager.GetHunterInfo(hunterdata.hunterIdx, hunterdata.hunterLevel, hunterdata.hunterTier);
		if (hunterInfo.Stat.hunterTier * 20 == hunterInfo.Stat.hunterLevel)
		{
			return false;
		}
		hunterLevelDbData = GameDataManager.GetHunterLevelData(hunterdata.hunterIdx, hunterdata.hunterLevel, hunterdata.hunterTier);
		if (GameObjectSingleton<LobbyManager>.Inst.CheckEneoughLevelUpItem(hunterLevelDbData.hnil, hunterLevelDbData.hnil_N) && hunterLevelDbData.needCoin <= GameInfo.userData.userInfo.coin && hunterdata.hunterTier * 20 > hunterdata.hunterLevel)
		{
			result = true;
		}
		return result;
	}

	private bool CheckHunterPromotion()
	{
		bool flag = false;
		UserHunterData userHunterData = null;
		for (int i = 0; i < GameInfo.userData.huntersUseInfo.Length; i++)
		{
			userHunterData = GameInfo.userData.huntersUseInfo[i];
			flag = GameObjectSingleton<LobbyManager>.Inst.CheckHunterPromotionDataCheck(userHunterData);
			if (flag)
			{
				break;
			}
		}
		if (!flag && GameInfo.userData.huntersOwnInfo != null)
		{
			for (int j = 0; j < GameInfo.userData.huntersOwnInfo.Length; j++)
			{
				userHunterData = GameInfo.userData.huntersOwnInfo[j];
				flag = GameObjectSingleton<LobbyManager>.Inst.CheckHunterPromotionDataCheck(userHunterData);
				if (flag)
				{
					break;
				}
			}
		}
		return flag;
	}

	private bool CheckHunterPromotionDataCheck(UserHunterData hunterdata)
	{
		bool result = false;
		HunterInfo hunterInfo = null;
		HunterPromotionDbData hunterPromotionDbData = null;
		hunterInfo = GameDataManager.GetHunterInfo(hunterdata.hunterIdx, hunterdata.hunterLevel, hunterdata.hunterTier);
		if (hunterInfo.Hunter.maxTier == hunterInfo.Stat.hunterTier)
		{
			return false;
		}
		hunterPromotionDbData = GameDataManager.GetHunterPromotionData(hunterInfo.Hunter.color, hunterInfo.Hunter.maxTier, hunterInfo.Stat.hunterTier);
		if (GameObjectSingleton<LobbyManager>.Inst.CheckEneoughPromotionItem(hunterPromotionDbData.hnip1, hunterPromotionDbData.hnip1_N) && hunterPromotionDbData.needCoin <= GameInfo.userData.userInfo.coin && hunterdata.hunterTier * 20 == hunterdata.hunterLevel && GameObjectSingleton<LobbyManager>.Inst.CheckEneoughPromotionItem(hunterPromotionDbData.hnip2, hunterPromotionDbData.hnip2_N) && GameObjectSingleton<LobbyManager>.Inst.CheckEneoughPromotionItem(hunterPromotionDbData.hnip3, hunterPromotionDbData.hnip3_N) && GameObjectSingleton<LobbyManager>.Inst.CheckEneoughPromotionItem(hunterPromotionDbData.hnip4, hunterPromotionDbData.hnip4_N))
		{
			result = true;
		}
		return result;
	}

	private bool CheckEneoughLevelUpItem(int _itemIdx, int _itemCount)
	{
		bool result = false;
		if (_itemCount <= GameInfo.userData.GetItemCount(_itemIdx))
		{
			result = true;
		}
		return result;
	}

	private bool CheckEneoughPromotionItem(int _itemIdx, int _itemCount)
	{
		bool result = false;
		if (_itemIdx == 0)
		{
			result = true;
		}
		else if (_itemCount <= GameInfo.userData.GetItemCount(_itemIdx))
		{
			result = true;
		}
		return result;
	}

	private void MoveCaslteUnLock()
	{
		for (int i = 0; i < GameInfo.userData.userFloorState.Length; i++)
		{
			if (GameInfo.userData.userFloorState[i].isOpen)
			{
				MoveCastle(i);
				Transform transform = MWPoolManager.Spawn("Effect", "FX_castle_unlock", null, 5f);
				transform.position = Vector3.zero;
				transform.localScale = Vector3.one;
			}
		}
	}

	private IEnumerator MoveToUnLockStore()
	{
		yield return null;
		for (int i = 0; i < GameObjectSingleton<LobbyManager>.Inst.listFloorController.Count; i++)
		{
			FloorController floorController = GameObjectSingleton<LobbyManager>.Inst.listFloorController[i];
			for (int j = 0; j < floorController.ListFloorItem.Count; j++)
			{
				FloorItem floorItem = floorController.ListFloorItem[j];
				if (floorItem.UnLock)
				{
					MoveCastle(i);
					floorController.MoveToFloor(j);
					floorItem.ShowUnLockEffect();
					yield break;
				}
			}
		}
	}

	private void OnStartStoreOpen()
	{
		if (StartStoreOpen != null)
		{
			StartStoreOpen();
		}
	}

	private void OnStoreCollectComplete()
	{
		if (StoreCollectComplete != null)
		{
			StoreCollectComplete();
		}
	}

	private void OnSelectStageCell(int index)
	{
		if (SelectStageCell != null)
		{
			SelectStageCell(index);
		}
	}

	private void OnCastleSnapComplete(int _caslteId)
	{
		castleUI.MoveCastle(_caslteId);
	}

	private bool CheckLobbyPopupActive()
	{
		LobbyPopupBase[] componentsInChildren = trLobbyPopupFront.GetComponentsInChildren<LobbyPopupBase>();
		foreach (LobbyPopupBase lobbyPopupBase in componentsInChildren)
		{
			if (lobbyPopupBase.gameObject.activeSelf)
			{
				return true;
			}
		}
		LobbyPopupBase[] componentsInChildren2 = trLobbyPopupBack.GetComponentsInChildren<LobbyPopupBase>();
		foreach (LobbyPopupBase lobbyPopupBase2 in componentsInChildren2)
		{
			if (lobbyPopupBase2.gameObject.activeSelf)
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator ProcessResumeCallUserInfo()
	{
		yield return new WaitForSeconds(0.2f);
		Protocol_Set.Protocol_user_info_Req();
		coroutineResume = null;
	}

	private void GetExpEffStart(Vector3 _pos)
	{
		StartCoroutine(ShowGetExpEffect(_pos));
	}

	private void GetCoinEffStart(Vector3 _pos)
	{
		StartCoroutine(ShowGetCoinEffect(_pos));
	}

	private void GetJewelEffStart(Vector3 _pos)
	{
		StartCoroutine(ShowGetJewelEffect(_pos));
	}

	private IEnumerator ShowGetExpEffect(Vector3 _pos)
	{
		int expCount = UnityEngine.Random.Range(4, 8);
		for (int i = 0; i < expCount; i++)
		{
			float num = UnityEngine.Random.Range(-0.2f, -0.1f);
			Transform transform = MWPoolManager.Spawn("Effect", "FX_Exp_get", null, 1.2f + num + 0.4f);
			transform.localScale = new Vector2(0.12f, 0.12f);
			transform.position = _pos + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
			GameObject gameObject = transform.gameObject;
			Vector3 userLevelPosition = UserLevelPosition;
			LeanTween.moveX(gameObject, userLevelPosition.x, 1.2f + num);
			GameObject gameObject2 = transform.gameObject;
			Vector3 userLevelPosition2 = UserLevelPosition;
			LeanTween.moveY(gameObject2, userLevelPosition2.y, 1.2f + num).setEaseInCubic();
		}
		yield return new WaitForSeconds(1.2f);
		GameDataManager.UpdateUserData();
		SoundController.EffectSound_Play(EffectSoundType.GetExp);
	}

	private IEnumerator ShowGetCoinEffect(Vector3 _pos)
	{
		int coinCount = UnityEngine.Random.Range(4, 8);
		for (int i = 0; i < coinCount; i++)
		{
			float num = UnityEngine.Random.Range(-0.2f, -0.1f);
			Transform transform = MWPoolManager.Spawn("Effect", "FX_Coin_get", null, 1.2f + num + 0.4f);
			transform.localScale = new Vector2(0.12f, 0.12f);
			transform.position = _pos + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
			GameObject gameObject = transform.gameObject;
			Vector3 userCoinPosition = UserCoinPosition;
			LeanTween.moveX(gameObject, userCoinPosition.x, 1.2f + num).setEaseInCubic();
			GameObject gameObject2 = transform.gameObject;
			Vector3 userCoinPosition2 = UserCoinPosition;
			LeanTween.moveY(gameObject2, userCoinPosition2.y, 1.2f + num);
		}
		yield return new WaitForSeconds(1.2f);
		GameDataManager.UpdateUserData();
		SoundController.EffectSound_Play(EffectSoundType.GetCoin);
	}

	private IEnumerator ShowGetJewelEffect(Vector3 _pos)
	{
		int jewelCount = UnityEngine.Random.Range(4, 8);
		for (int i = 0; i < jewelCount; i++)
		{
			float num = UnityEngine.Random.Range(-0.2f, -0.1f);
			Transform transform = MWPoolManager.Spawn("Effect", "FX_jewel_get", null, 1.2f + num + 0.4f);
			transform.localScale = new Vector2(0.12f, 0.12f);
			transform.position = _pos + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
			GameObject gameObject = transform.gameObject;
			Vector3 userJewelPosition = UserJewelPosition;
			LeanTween.moveX(gameObject, userJewelPosition.x, 1.2f + num).setEaseInCubic();
			GameObject gameObject2 = transform.gameObject;
			Vector3 userJewelPosition2 = UserJewelPosition;
			LeanTween.moveY(gameObject2, userJewelPosition2.y, 1.2f + num);
		}
		yield return new WaitForSeconds(1.2f);
		GameDataManager.UpdateUserData();
		SoundController.EffectSound_Play(EffectSoundType.GetJewel);
	}

	private void ShowAllStore()
	{
		for (int i = 0; i < GameObjectSingleton<LobbyManager>.Inst.listFloorController.Count; i++)
		{
			FloorController floorController = GameObjectSingleton<LobbyManager>.Inst.listFloorController[i];
			floorController.ShowStore();
		}
	}

	private void HideAllStore()
	{
		for (int i = 0; i < GameObjectSingleton<LobbyManager>.Inst.listFloorController.Count; i++)
		{
			FloorController floorController = GameObjectSingleton<LobbyManager>.Inst.listFloorController[i];
			floorController.HideStore();
		}
	}

	private void CheckLobbyHunterShow(string _type)
	{
		if (_type == null)
		{
			return;
		}
		if (!(_type == "levelInfo"))
		{
			if (!(_type == "energyInfo"))
			{
				if (!(_type == "valueShop"))
				{
					if (!(_type == "setting"))
					{
						if (_type == "itemSortList" && !userEnergyInfo.gameObject.activeSelf && !valueShop.gameObject.activeSelf && !userLevelInfo.gameObject.activeSelf && !userLevelInfo.gameObject.activeSelf)
						{
							ShowHunterLobby();
						}
					}
					else if (!userEnergyInfo.gameObject.activeSelf && !valueShop.gameObject.activeSelf && !userLevelInfo.gameObject.activeSelf && !itemSortList.gameObject.activeSelf)
					{
						ShowHunterLobby();
					}
				}
				else if (!userEnergyInfo.gameObject.activeSelf && !userLevelInfo.gameObject.activeSelf && !setting.gameObject.activeSelf && !itemSortList.gameObject.activeSelf)
				{
					ShowHunterLobby();
				}
			}
			else if (!userLevelInfo.gameObject.activeSelf && !valueShop.gameObject.activeSelf && !setting.gameObject.activeSelf && !itemSortList.gameObject.activeSelf)
			{
				ShowHunterLobby();
			}
		}
		else if (!userEnergyInfo.gameObject.activeSelf && !valueShop.gameObject.activeSelf && !setting.gameObject.activeSelf && !itemSortList.gameObject.activeSelf)
		{
			ShowHunterLobby();
		}
	}

	private void ShowReadyMonsterList()
	{
		levelPlay.ShowMonster();
		quickLoot.ShowMonster();
		arenaLevelPlay.ShowMonster();
	}

	private void HideReadyMonsterList()
	{
		levelPlay.HideMonster();
		quickLoot.HideMonster();
		arenaLevelPlay.HideMonster();
	}

	public void OnShowStageSelect()
	{
		stageSelect.Show();
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		if (OpenStageSelect != null)
		{
			OpenStageSelect();
		}
		HideAllStore();
	}

	public void OnClickStageSelect()
	{
	}

	public void OnShowDeckEditNormal()
	{
		deckEdit.Show(HUNTERLIST_TYPE.NORMAL);
		HideReadyMonsterList();
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnShowDeckEditArena()
	{
		deckEdit.Show(HUNTERLIST_TYPE.ARENA);
		HideReadyMonsterList();
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnShowHunterList()
	{
		hunterList.Show();
		HideAllStore();
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		if (OpenHunterList != null)
		{
			OpenHunterList();
		}
	}

	public void OnShowChest()
	{
		GameObjectSingleton<LobbyManager>.Inst.chest.Show();
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		if (OpenChest != null)
		{
			OpenChest();
		}
		HideAllStore();
	}

	public void OnShowValueShop()
	{
		Protocol_Set.Protocol_shop_list_Req(GameObjectSingleton<LobbyManager>.Inst.ShopListResponse);
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnShowArena()
	{
		arenaLobby.Show();
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		if (ArenaMenuEnter != null)
		{
			ArenaMenuEnter();
		}
	}

	public static void ShowArenaShopBuy(ARENA_STORE_INFO data)
	{
		if (!GameObjectSingleton<LobbyManager>.Inst.arenaShopBuy.gameObject.activeSelf)
		{
			GameObjectSingleton<LobbyManager>.Inst.arenaShopBuy.Show(data);
		}
	}

	public static void ShowArenaShop_Refresh()
	{
		GameObjectSingleton<LobbyManager>.Inst.arenaShop.SetInit();
	}

	public void OnShowArenaShop()
	{
		Protocol_Set.Protocol_arena_store_list_Req(GameObjectSingleton<LobbyManager>.Inst.ArenaShopListResponse);
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnShowSetting()
	{
		GameObjectSingleton<LobbyManager>.Inst.setting.Show();
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		if (arenaLobby.HasOpen)
		{
			topUI.Show();
			arenaLobby.HideArenaTopUI();
		}
	}

	public static void OnShowRate()
	{
		GameInfo.isRate = false;
		GameObjectSingleton<LobbyManager>.Inst.rate.Init();
	}

	protected override void Awake()
	{
		base.Awake();
		base.gameObject.SetActive(value: false);
		Protocol_Set.Protocol_user_info_Req(OnUserInfoCommectComplete);
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKey(KeyCode.Escape) && !gameQuitPopup.gameObject.activeSelf && !CheckLobbyPopupActive())
		{
			gameQuitPopup.Show();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		GameDataManager.ChangeUserData = (Action)Delegate.Remove(GameDataManager.ChangeUserData, new Action(OnChangeUserDataEvent));
		GameDataManager.ChangeStoreData = (Action)Delegate.Remove(GameDataManager.ChangeStoreData, new Action(OnChangeStoreDataEvent));
		LocalTimeCheckManager.OnLocalTimeComplete = (Action<string>)Delegate.Remove(LocalTimeCheckManager.OnLocalTimeComplete, new Action<string>(OnFreeChestComplete));
		LocalTimeCheckManager.TimeClear("mysteriousChestOpenTime");
		LocalTimeCheckManager.SaveAndExit("mysteriousChestOpenTime");
		FloorController[] componentsInChildren = trFloorStageContent.GetComponentsInChildren<FloorController>();
		foreach (FloorController floorController in componentsInChildren)
		{
			FloorController floorController2 = floorController;
			floorController2.StartStoreOpen = (Action)Delegate.Remove(floorController2.StartStoreOpen, new Action(OnStartStoreOpen));
			FloorController floorController3 = floorController;
			floorController3.CollectComplete = (Action)Delegate.Remove(floorController3.CollectComplete, new Action(OnStoreCollectComplete));
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus && GameInfo.isResumeUserDataConnect)
		{
			if (coroutineResume != null)
			{
				StopCoroutine(coroutineResume);
				coroutineResume = null;
			}
			coroutineResume = StartCoroutine(ProcessResumeCallUserInfo());
		}
	}
}
