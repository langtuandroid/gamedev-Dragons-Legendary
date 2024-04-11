using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorItem : MonoBehaviour
{
	public enum FloorState
	{
		Standby = 1,
		Progress,
		Complete
	}

	public Action StartStoreOpen;

	public Action CollectComplete;

	private const float PRODUCE_TIME_GAUGE_BEGIN_VALUE = 676f;

	private const float PRODUCE_TIME_GAUGE_END_VALUE = 116f;

	private const float PRODUCE_TIME_MAX_GAUGE = 446f;

	private const float STORE_LEVEL_UP_EFFECT_TIME = 1.2f;

	private const float STORE_LEVEL_UP_EFFECT_DELAY_SPAWN_DURATION = 0.4f;

	private const string FLOOR_LOCK_MESSAGE = "Clear Chapter ";

	[SerializeField]
	private string storePrefabName;

	[SerializeField]
	private BlockType type;

	[SerializeField]
	private Button btnOpen;

	[SerializeField]
	private Button btnCollect;

	[SerializeField]
	private Button btnSpeedUp;

	[SerializeField]
	private Text textLockMessage;

	[SerializeField]
	private Text textCost;

	[SerializeField]
	private Text textSpeedUpCost;

	[SerializeField]
	private Text textProduceTime;

	[SerializeField]
	private Text textCollectCoin;

	[SerializeField]
	private Text textFloorName;

	[SerializeField]
	private Text textFloorNameLock;

	[SerializeField]
	private Text textFloorLock;

	[SerializeField]
	private Text textBadgeName;

	[SerializeField]
	private Image imageBlend;

	[SerializeField]
	private RectTransform rectProduceTime;

	[SerializeField]
	private GameObject goLock;

	[SerializeField]
	private GameObject goTimer;

	[SerializeField]
	private GameObject goProduceCompleteEffect;

	[SerializeField]
	private List<GameObject> listFloorLevel = new List<GameObject>();

	[SerializeField]
	private Transform trFloorTestAnchor;

	[SerializeField]
	private Transform trRequireItemAnchor;

	[SerializeField]
	private Transform trBadgeAnchor;

	[SerializeField]
	private Transform trUserGetBadgeAnchor;

	[SerializeField]
	private Transform trGaugeBadgeAnchor;

	[SerializeField]
	private Transform trUpgradeNoticeAnchor;

	[SerializeField]
	private Transform trOwnBadgeAnchor;

	[SerializeField]
	private Transform trFloorHead;

	[SerializeField]
	private Transform trFloorTouchCollect;

	[SerializeField]
	private FloorAnimControl animControl;

	[SerializeField]
	private Animator animatorBadgeGauge;

	private int id;

	private int stageId;

	private int userItemCount;

	private float remainDuration;

	private float prevSecond = -1f;

	private string typeKey;

	private bool isButtonEnable = true;

	private RectTransform rtFloorItem;

	private StoreProduceDbData produceData;

	private FloorState floorState;

	[SerializeField]
	private UserFloorData floorData;

	private Transform trFloor;

	private Transform trBadge;

	private Transform trUpgradeNotice;

	private Transform trRequireItem;

	private Transform trGaugeBadge;

	private Transform trOwnBadge;

	private Transform trUserGetBadge;

	private Coroutine coroutineBadge;

	public int BadgeIdx
	{
		get
		{
			if (produceData == null)
			{
				return 0;
			}
			return produceData.spi;
		}
	}

	public bool UnLock
	{
		get
		{
			if (floorData == null)
			{
				return false;
			}
			return floorData.isOpen;
		}
	}

	public bool HasUpgrade => CheckStoreUpgrade();

	public BlockType FloorType => type;

	public Transform TrOpenButton => btnOpen.gameObject.transform;

	public Transform TrCollectButton => btnCollect.gameObject.transform;

	public Transform TrStore => trFloor;

	public Transform TrBadge => trBadgeAnchor;

	public Transform TrTouchCollect => trFloorTouchCollect;

	public void Init(UserFloorData _data)
	{
		floorData = _data;
		typeKey = floorData.storeIdx.ToString();
		produceData = GameDataManager.GetStoreProduceData(floorData.storeIdx, floorData.storeTier);
		textFloorName.text = MWLocalize.GetData(GameDataManager.GetStoreData(floorData.storeIdx).storeName);
		textBadgeName.text = MWLocalize.GetData(GameDataManager.GetItemListData(produceData.spi).itemName);
		trBadge = MWPoolManager.Spawn("Item", $"Item_{produceData.spi}", trBadgeAnchor);
		trBadge.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		trUserGetBadge = MWPoolManager.Spawn("Item", $"Item_{produceData.spi}", trUserGetBadgeAnchor);
		trUserGetBadge.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		animControl.Init();
		FloorAnimControl floorAnimControl = animControl;
		floorAnimControl.ShowBadgeGauge = (Action)Delegate.Combine(floorAnimControl.ShowBadgeGauge, new Action(OnShowBadgeGauge));
		FloorAnimControl floorAnimControl2 = animControl;
		floorAnimControl2.StartBadgeAnimation = (Action)Delegate.Combine(floorAnimControl2.StartBadgeAnimation, new Action(OnStartBadgeAnimation));
		FloorAnimControl floorAnimControl3 = animControl;
		floorAnimControl3.ProduceRewardComplete = (Action)Delegate.Combine(floorAnimControl3.ProduceRewardComplete, new Action(OnProduceRewardComplete));
		FloorAnimControl floorAnimControl4 = animControl;
		floorAnimControl4.AllEffectComplete = (Action)Delegate.Combine(floorAnimControl4.AllEffectComplete, new Action(OnAllEffectComplete));
		goProduceCompleteEffect.SetActive(value: false);
		goLock.SetActive(value: false);
		floorState = (FloorState)floorData.state;
		UnityEngine.Debug.Log("typeKey :: " + floorData.storeRemainTime);
		LocalTimeCheckManager.InitType(typeKey);
		LocalTimeCheckManager.OnTimeTick = (Action<string, float>)Delegate.Combine(LocalTimeCheckManager.OnTimeTick, new Action<string, float>(OnTimeTickEvent));
		LocalTimeCheckManager.OnLocalTimeComplete = (Action<string>)Delegate.Combine(LocalTimeCheckManager.OnLocalTimeComplete, new Action<string>(OnLocalTimeComplete));
		if (floorData.state == 2 && floorData.storeRemainTime > 0)
		{
			LocalTimeCheckManager.AddTimer(typeKey, floorData.storeRemainTime);
		}
		if (trFloorTestAnchor != null && storePrefabName != string.Empty)
		{
			trFloorTestAnchor.gameObject.SetActive(value: true);
			trFloor = MWPoolManager.Spawn("Lobby", storePrefabName);
		}
		if (trRequireItem == null)
		{
			trRequireItem = MWPoolManager.Spawn("Item", $"Item_{produceData.snip1Type}", trRequireItemAnchor);
		}
		if (trGaugeBadge == null)
		{
			trGaugeBadge = MWPoolManager.Spawn("Item", $"Item_{produceData.spi}", trGaugeBadgeAnchor);
		}
		if (trOwnBadge == null)
		{
			trOwnBadge = MWPoolManager.Spawn("Item", $"Item_{produceData.spi}", trOwnBadgeAnchor);
			trOwnBadge.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		}
		RefreshFloorState();
	}

	public void SetDefaultProductData(int storeIdx)
	{
		produceData = GameDataManager.GetStoreProduceData(storeIdx, 1);
	}

	public void SetStageId(int _stageId)
	{
		stageId = _stageId;
		if (trFloor != null)
		{
			trFloor.GetComponent<FloorStore>().SetBlend(stageId);
		}
	}

	public void SetFloorId(int floorId)
	{
		id = floorId;
	}

	public void SetType(BlockType _type)
	{
		type = _type;
	}

	public void SetBlend(bool isActive)
	{
	}

	public void ChangeIndex(int index)
	{
		rtFloorItem.SetSiblingIndex(index + 2);
	}

	public void Lock()
	{
		goLock.SetActive(value: true);
		goProduceCompleteEffect.SetActive(value: false);
		textLockMessage.text = string.Format(MWLocalize.GetData("common_text_clear_chapter"), id);
		textFloorNameLock.text = MWLocalize.GetData(GameDataManager.GetStoreData(produceData.storeIdx).storeName);
		textFloorLock.text = MWLocalize.GetData("common_text_locked");
	}

	public void Refresh(UserFloorData _data)
	{
		floorData = _data;
		typeKey = floorData.storeIdx.ToString();
		produceData = GameDataManager.GetStoreProduceData(floorData.storeIdx, floorData.storeTier);
		floorState = (FloorState)floorData.state;
		if (floorData.state == 2 && floorData.storeRemainTime > 0 && LocalTimeCheckManager.GetSecond(typeKey) <= 0.0)
		{
			LocalTimeCheckManager.AddTimer(typeKey, floorData.storeRemainTime);
		}
		trFloor.GetComponent<FloorStore>().Refresh();
		RefreshFloorState();
	}

	public void AllButtonLock()
	{
		isButtonEnable = false;
	}

	public void LevelUp()
	{
		switch (floorState)
		{
		case FloorState.Progress:
			LocalTimeCheckManager.TimeComplete(typeKey);
			OnClickCollect();
			break;
		case FloorState.Complete:
			OnClickCollect();
			break;
		}
		LocalTimeCheckManager.AddTimer(typeKey, produceData.produceTime);
		floorState = FloorState.Progress;
		RefreshFloorState();
	}

	public void ForceCollectEffect()
	{
		OnStoreCollectComplete();
	}

	public void ShowUnLockEffect()
	{
		if (trFloor != null)
		{
			FloorStore component = trFloor.GetComponent<FloorStore>();
			component.ShowUnLockEffect();
		}
		SoundController.EffectSound_Play(EffectSoundType.StoreUnlock);
	}

	public void ShowDetail()
	{
		LobbyManager.OpenStageFloorId = stageId;
		LobbyManager.OpenChapterFloorId = id;
		LobbyManager.ShowFloorDetail(floorData, produceData);
	}

	public void ShowStore()
	{
		if (trFloor != null)
		{
			trFloor.GetComponent<FloorStore>().Show();
		}
	}

	public void HideStore()
	{
		if (trFloor != null)
		{
			trFloor.GetComponent<FloorStore>().Hide();
		}
	}

	private void ShowFloorLevelStar()
	{
		if (floorData != null)
		{
			for (int i = 0; i < listFloorLevel.Count; i++)
			{
				listFloorLevel[i].SetActive(i + 1 <= floorData.storeTier);
			}
		}
	}

	private void RefreshFloorState()
	{
		if (floorData == null)
		{
			return;
		}
		switch (floorState)
		{
		case FloorState.Standby:
			btnOpen.gameObject.SetActive(value: true);
			btnCollect.gameObject.SetActive(value: false);
			btnSpeedUp.gameObject.SetActive(value: false);
			goTimer.SetActive(value: false);
			StandBySetting();
			if ((bool)trFloor)
			{
				trFloor.GetComponent<FloorStore>().Close();
			}
			break;
		case FloorState.Progress:
			btnOpen.gameObject.SetActive(value: false);
			btnCollect.gameObject.SetActive(value: false);
			btnSpeedUp.gameObject.SetActive(value: true);
			textSpeedUpCost.text = $"{GameUtil.GetConvertTimeToJewel((float)LocalTimeCheckManager.GetSecond(typeKey))}";
			goTimer.SetActive(value: true);
			if ((bool)trFloor)
			{
				trFloor.GetComponent<FloorStore>().Open();
			}
			break;
		case FloorState.Complete:
			btnOpen.gameObject.SetActive(value: false);
			btnCollect.gameObject.SetActive(value: true);
			btnSpeedUp.gameObject.SetActive(value: false);
			textCollectCoin.text = $"{produceData.getCoin:#,##0}";
			goTimer.SetActive(value: false);
			if ((bool)trFloor)
			{
				trFloor.GetComponent<FloorStore>().Close();
			}
			break;
		}
		ShowFloorLevelStar();
		RefreshUpgradeNotice();
	}

	private void StandBySetting()
	{
		if (produceData != null)
		{
			userItemCount = GameInfo.userData.GetItemCount(produceData.snip1Type);
			if (userItemCount < produceData.snip1N)
			{
				textCost.text = $"<color=red>{userItemCount}</color>/{produceData.snip1N}";
			}
			else
			{
				textCost.text = $"{userItemCount}/{produceData.snip1N}";
			}
		}
	}

	private void RefreshUpgradeNotice()
	{
		if (!(trUpgradeNoticeAnchor == null))
		{
			ClearUpgradeNotice();
			if (CheckStoreUpgrade())
			{
				trUpgradeNotice = MWPoolManager.Spawn("Lobby", "Notice_Upgrade", trUpgradeNoticeAnchor);
			}
		}
	}

	private void ClearUpgradeNotice()
	{
		if (trUpgradeNotice != null)
		{
			MWPoolManager.DeSpawn("Lobby", trUpgradeNotice);
			trUpgradeNotice = null;
		}
	}

	private bool CheckStoreUpgrade()
	{
		StoreUpgradeDbData storeUpgradeData = GameDataManager.GetStoreUpgradeData(floorData.storeIdx, floorData.storeTier);
		if (storeUpgradeData == null)
		{
			return false;
		}
		if (GameInfo.userData.userInfo.coin < storeUpgradeData.needCoin)
		{
			return false;
		}
		if (storeUpgradeData.sniu1 > 0 && GameInfo.userData.GetItemCount(storeUpgradeData.sniu1) < storeUpgradeData.sniu1_N)
		{
			return false;
		}
		if (storeUpgradeData.sniu2 > 0 && GameInfo.userData.GetItemCount(storeUpgradeData.sniu2) < storeUpgradeData.sniu2_N)
		{
			return false;
		}
		if (storeUpgradeData.sniu3 > 0 && GameInfo.userData.GetItemCount(storeUpgradeData.sniu3) < storeUpgradeData.sniu3_N)
		{
			return false;
		}
		if (storeUpgradeData.sniu4 > 0 && GameInfo.userData.GetItemCount(storeUpgradeData.sniu4) < storeUpgradeData.sniu4_N)
		{
			return false;
		}
		return true;
	}

	private void OnTimeTickEvent(string type, float second)
	{
		if (type == typeKey)
		{
			if (second < 0f)
			{
				second = 0f;
			}
			float num = Mathf.Floor(second);
			TimeSpan timeSpan = TimeSpan.FromSeconds(num);
			string text = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
			textProduceTime.text = text;
			float num2 = ((float)produceData.produceTime - second) / (float)produceData.produceTime;
			Vector2 sizeDelta = rectProduceTime.sizeDelta;
			sizeDelta.x = num2 * 446f;
			rectProduceTime.sizeDelta = sizeDelta;
			remainDuration = second;
			textSpeedUpCost.text = $"{GameUtil.GetConvertTimeToJewel(second)}";
		}
	}

	private void OnLocalTimeComplete(string type)
	{
		if (type == typeKey)
		{
			Protocol_Set.Protocol_user_info_Req(null, isLoading: false);
			LobbyManager.HideSpeedUpCollect();
		}
	}

	private void OnShowBadgeGauge()
	{
		int num = floorData.operatingRatio;
		if (num == 0)
		{
			num = 5;
		}
		UnityEngine.Debug.Log("OnShowBadgeGauge :: " + num);
		animatorBadgeGauge.Play("Badgegauge_light0" + num + "_stop");
		SoundController.EffectSound_Play(EffectSoundType.FillGauge);
	}

	private void OnStartBadgeAnimation()
	{
		StopBadgeAnimation();
		coroutineBadge = StartCoroutine(ProcessBadgeAnimation());
	}

	private void OnProduceRewardComplete()
	{
		if (floorData.operatingRatio == 0)
		{
			animControl.Continue();
		}
		else
		{
			goProduceCompleteEffect.SetActive(value: false);
		}
	}

	private void OnAllEffectComplete()
	{
		animControl.Clear();
		goProduceCompleteEffect.SetActive(value: false);
	}

	private void StopBadgeAnimation()
	{
		if (coroutineBadge != null)
		{
			StopCoroutine(coroutineBadge);
			coroutineBadge = null;
		}
	}

	private IEnumerator ProcessBadgeAnimation()
	{
		animatorBadgeGauge.SetTrigger("ContinueTrigger");
		yield return new WaitForSeconds(2.5f);
		animControl.Continue();
	}

	private IEnumerator ShowStoreLevelUpEffect(int _addCoin, int _addExp)
	{
		int expCount = UnityEngine.Random.Range(4, 8);
		for (int i = 0; i < expCount; i++)
		{
			float num = UnityEngine.Random.Range(-0.2f, -0.1f);
			Transform transform = MWPoolManager.Spawn("Effect", "FX_Exp_get", null, 1.2f + num + 0.4f);
			transform.localScale = new Vector2(0.12f, 0.12f);
			transform.position = btnCollect.transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
			GameObject gameObject = transform.gameObject;
			Vector3 userLevelPosition = LobbyManager.UserLevelPosition;
			LeanTween.moveX(gameObject, userLevelPosition.x, 1.2f + num);
			GameObject gameObject2 = transform.gameObject;
			Vector3 userLevelPosition2 = LobbyManager.UserLevelPosition;
			LeanTween.moveY(gameObject2, userLevelPosition2.y, 1.2f + num).setEaseInCubic();
		}
		int coinCount = UnityEngine.Random.Range(4, 8);
		for (int j = 0; j < coinCount; j++)
		{
			float num2 = UnityEngine.Random.Range(-0.2f, -0.1f);
			Transform transform2 = MWPoolManager.Spawn("Effect", "FX_Coin_get", null, 1.2f + num2 + 0.4f);
			transform2.localScale = new Vector2(0.12f, 0.12f);
			transform2.position = btnCollect.transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
			GameObject gameObject3 = transform2.gameObject;
			Vector3 userCoinPosition = LobbyManager.UserCoinPosition;
			LeanTween.moveX(gameObject3, userCoinPosition.x, 1.2f + num2).setEaseInCubic();
			GameObject gameObject4 = transform2.gameObject;
			Vector3 userCoinPosition2 = LobbyManager.UserCoinPosition;
			LeanTween.moveY(gameObject4, userCoinPosition2.y, 1.2f + num2);
		}
		yield return new WaitForSeconds(1.2f);
		GameDataManager.UpdateUserData();
		SoundController.EffectSound_Play(EffectSoundType.GetExp);
		SoundController.EffectSound_Play(EffectSoundType.GetCoin);
		SoundController.EffectSound_Play(EffectSoundType.FillGauge);
	}

	private void OnStoreOpenComplete()
	{
		SoundController.EffectSound_Play(EffectSoundType.StoreOpen);
		if (StartStoreOpen != null)
		{
			StartStoreOpen();
		}
		if (userItemCount >= produceData.snip1N)
		{
			StopBadgeAnimation();
			animControl.Clear();
			goProduceCompleteEffect.SetActive(value: false);
		}
	}

	private void OnStoreCollectComplete()
	{
		if (CollectComplete != null)
		{
			CollectComplete();
		}
		StartCoroutine(ShowStoreLevelUpEffect(produceData.getCoin, produceData.getExp));
		LocalTimeCheckManager.TimeClear(typeKey);
		goProduceCompleteEffect.SetActive(value: true);
		animControl.SetCoin(produceData.getCoin);
		animControl.SetBadgeCount(produceData.spiN);
		animControl.SetRatioCount(floorData.operatingRatio);
		animControl.SetBadgeIdx(produceData.spi);
		animControl.Play();
		if (floorData.operatingRatio == 0)
		{
			LobbyManager.CallBadgeAcquireEvent(stageId, id);
		}
	}

	private void OnStoreSpeedUpComplete()
	{
	}

	private void OnSpeedUpCollect(bool isSuccess)
	{
		if (isSuccess)
		{
			Protocol_Set.Protocol_store_speed_up_Req(stageId + 1, floorData.storeIdx, GameUtil.GetConvertTimeToJewel(remainDuration), OnStoreCollectComplete);
		}
	}

	private void OnShopListResponse()
	{
		LobbyManager.ShowValueShop(ValueShopType.Jewel);
	}

	public void OnClickOpen()
	{
		if (!isButtonEnable)
		{
			return;
		}
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		if (userItemCount < produceData.snip1N)
		{
			LobbyManager.ShowItemSortList(produceData.snip1Type, isWaveItemSort: true);
			return;
		}
		if (GamePreferenceManager.GetIsAnalytics("09_open_store"))
		{
			AnalyticsManager.FirebaseAnalyticsLogEvent(FBLog_Type.open_store);
		}
		Protocol_Set.Protocol_store_open_Req(stageId + 1, floorData.storeIdx, OnStoreOpenComplete);
	}

	public void OnClickCollect()
	{
		if (isButtonEnable)
		{
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
			if (GamePreferenceManager.GetIsAnalytics("91_store_open_by_self"))
			{
				AnalyticsManager.FirebaseAnalyticsLogEvent(FBLog_Type.store_collect_by_self);
			}
			Protocol_Set.Protocol_store_collect_Req(stageId + 1, floorData.storeIdx, OnStoreCollectComplete);
		}
	}

	public void OnClickSpeedUp()
	{
		if (isButtonEnable)
		{
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
			if (GameInfo.userData.userInfo.jewel < GameUtil.GetConvertTimeToJewel(remainDuration))
			{
				Protocol_Set.Protocol_shop_list_Req(OnShopListResponse);
			}
			else
			{
				LobbyManager.ShowSpeedUpCollect(remainDuration, GameUtil.GetConvertTimeToJewel(remainDuration), produceData.getCoin, OnSpeedUpCollect);
			}
		}
	}

	public void OnClickDetail()
	{
		if (isButtonEnable)
		{
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
			LobbyManager.OpenStageFloorId = stageId;
			LobbyManager.OpenChapterFloorId = id;
			LobbyManager.ShowFloorDetail(floorData, produceData);
		}
	}

	public void SyncStore()
	{
		if (trFloor != null && trFloorTestAnchor != null && trFloor.position != trFloorTestAnchor.position)
		{
			trFloor.position = trFloorTestAnchor.position;
		}
	}

	private void Awake()
	{
		rtFloorItem = base.gameObject.GetComponent<RectTransform>();
	}

	private void OnDisable()
	{
		if (trFloor != null)
		{
			MWPoolManager.DeSpawn("Lobby", trFloor);
			trFloor = null;
		}
		LocalTimeCheckManager.OnTimeTick = (Action<string, float>)Delegate.Remove(LocalTimeCheckManager.OnTimeTick, new Action<string, float>(OnTimeTickEvent));
		LocalTimeCheckManager.OnLocalTimeComplete = (Action<string>)Delegate.Remove(LocalTimeCheckManager.OnLocalTimeComplete, new Action<string>(OnLocalTimeComplete));
		if (typeKey != null)
		{
			LocalTimeCheckManager.TimeClear(typeKey);
		}
		LocalTimeCheckManager.SaveAndExit(typeKey);
		if (animControl != null)
		{
			FloorAnimControl floorAnimControl = animControl;
			floorAnimControl.StartBadgeAnimation = (Action)Delegate.Remove(floorAnimControl.StartBadgeAnimation, new Action(OnStartBadgeAnimation));
			FloorAnimControl floorAnimControl2 = animControl;
			floorAnimControl2.ProduceRewardComplete = (Action)Delegate.Remove(floorAnimControl2.ProduceRewardComplete, new Action(OnProduceRewardComplete));
			FloorAnimControl floorAnimControl3 = animControl;
			floorAnimControl3.ShowBadgeGauge = (Action)Delegate.Remove(floorAnimControl3.ShowBadgeGauge, new Action(OnShowBadgeGauge));
			FloorAnimControl floorAnimControl4 = animControl;
			floorAnimControl4.AllEffectComplete = (Action)Delegate.Remove(floorAnimControl4.AllEffectComplete, new Action(OnAllEffectComplete));
		}
		if (trBadge != null)
		{
			MWPoolManager.DeSpawn("Item", trBadge);
			trBadge = null;
		}
		if (trUserGetBadge != null)
		{
			MWPoolManager.DeSpawn("Item", trUserGetBadge);
			trUserGetBadge = null;
		}
		if (trRequireItem != null)
		{
			MWPoolManager.DeSpawn("Item", trRequireItem);
			trRequireItem = null;
		}
		if (trGaugeBadge != null)
		{
			MWPoolManager.DeSpawn("Item", trGaugeBadge);
			trGaugeBadge = null;
		}
		if (trOwnBadge != null)
		{
			MWPoolManager.DeSpawn("Item", trOwnBadge);
			trOwnBadge = null;
		}
		ClearUpgradeNotice();
	}
}
