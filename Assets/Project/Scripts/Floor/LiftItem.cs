using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LiftItem : MonoBehaviour
{
	private enum LiftState
	{
		Standby = 1,
		Progress,
		Complete
	}

	public Action OnStoreOpen;
	
	public Action OnCompleteCollect;

	[FormerlySerializedAs("storePrefabName")] [SerializeField]
	private string _storePrefab;

	[FormerlySerializedAs("type")] [SerializeField]
	private BlockType _blockType;

	[FormerlySerializedAs("btnOpen")] [SerializeField]
	private Button _openButton;

	[FormerlySerializedAs("btnCollect")] [SerializeField]
	private Button _collectButton;

	[FormerlySerializedAs("btnSpeedUp")] [SerializeField]
	private Button _speedUpButton;

	[FormerlySerializedAs("textLockMessage")] [SerializeField]
	private Text _lockMessage;

	[FormerlySerializedAs("textCost")] [SerializeField]
	private Text _costText;

	[FormerlySerializedAs("textSpeedUpCost")] [SerializeField]
	private Text _speedUpCost;

	[FormerlySerializedAs("textProduceTime")] [SerializeField]
	private Text _produceTimeText;

	[FormerlySerializedAs("textCollectCoin")] [SerializeField]
	private Text _collectCoinsText;

	[FormerlySerializedAs("textFloorName")] [SerializeField]
	private Text _floorNameText;

	[FormerlySerializedAs("textFloorNameLock")] [SerializeField]
	private Text _nameFloorText;

	[FormerlySerializedAs("textFloorLock")] [SerializeField]
	private Text _lockFloorText;

	[FormerlySerializedAs("textBadgeName")] [SerializeField]
	private Text _badgeNameText;
	
	private Image blendImage;

	[FormerlySerializedAs("rectProduceTime")] [SerializeField]
	private RectTransform _produceTimeRectTransform;

	[FormerlySerializedAs("goLock")] [SerializeField]
	private GameObject _lockObject;

	[FormerlySerializedAs("goTimer")] [SerializeField]
	private GameObject _timerObject;

	[FormerlySerializedAs("goProduceCompleteEffect")] [SerializeField]
	private GameObject _completeEffect;

	[FormerlySerializedAs("listFloorLevel")] [SerializeField]
	private List<GameObject> _listFloorLevel = new List<GameObject>();

	[FormerlySerializedAs("trFloorTestAnchor")] [SerializeField]
	private Transform _testAnchorTransform;

	[FormerlySerializedAs("trRequireItemAnchor")] [SerializeField]
	private Transform _requireItemTransform;

	[FormerlySerializedAs("trBadgeAnchor")] [SerializeField]
	private Transform _badgeTransform;

	[FormerlySerializedAs("trUserGetBadgeAnchor")] [SerializeField]
	private Transform _userBadgeAnchor;

	[FormerlySerializedAs("trGaugeBadgeAnchor")] [SerializeField]
	private Transform _userGuageTransform;

	[FormerlySerializedAs("trUpgradeNoticeAnchor")] [SerializeField]
	private Transform _upgradeNoticeTransform;

	[FormerlySerializedAs("trOwnBadgeAnchor")] [SerializeField]
	private Transform _badgeOwnAnchor;

	[FormerlySerializedAs("trFloorHead")] [SerializeField]
	private Transform _floorHeadTransform;

	[FormerlySerializedAs("trFloorTouchCollect")] [SerializeField]
	private Transform _floorTouchCollectTransform;

	[FormerlySerializedAs("animControl")] [SerializeField]
	private LiftAnimControl _liftAnimator;

	[FormerlySerializedAs("animatorBadgeGauge")] [SerializeField]
	private Animator _animatorBadge;

	private int _index;

	private int _stageIndex;

	private int _userItemsNumber;

	private float _remainDuration;

	private float _prevSecond = -1f;

	private string _keyType;

	private bool _isEnabledButton = true;

	private RectTransform _floorItemRectTransform;

	private StoreProduceDbData _produceDbData;

	private LiftState _floorStage;

	[FormerlySerializedAs("floorData")] [SerializeField]
	private UserFloorData _floorData;

	private Transform _floorTransform;

	private Transform _badgeTrans;

	private Transform _upgradeNoticeTrans;

	private Transform _requireTrans;

	private Transform _gaugeBadgeTransform;

	private Transform _ownBadgeTransform;

	private Transform _userBadge;

	private Coroutine _badgeCoroutine;

	public int BadgeIndex
	{
		get
		{
			if (_produceDbData == null)
			{
				return 0;
			}
			return _produceDbData.spi;
		}
	}

	public bool IsLocked
	{
		get
		{
			if (_floorData == null)
			{
				return false;
			}
			return _floorData.isOpen;
		}
	}

	public bool IsHasUpgrade => CheckIsUpgradeAvailable();

	public BlockType FloorBlockType => _blockType;

	public Transform TrOpenButton => _openButton.gameObject.transform;

	public Transform TrCollectButton => _collectButton.gameObject.transform;

	public Transform Store => _floorTransform;

	public Transform Badge => _badgeTransform;

	public Transform TouchCollectTransform => _floorTouchCollectTransform;

	public void Construct(UserFloorData _data)
	{
		_floorData = _data;
		_keyType = _floorData.storeIdx.ToString();
		_produceDbData = GameDataManager.GetStoreProduceData(_floorData.storeIdx, _floorData.storeTier);
		_floorNameText.text = MasterLocalize.GetData(GameDataManager.GetStoreData(_floorData.storeIdx).storeName);
		_badgeNameText.text = MasterLocalize.GetData(GameDataManager.GetItemListData(_produceDbData.spi).itemName);
		_badgeTrans = MasterPoolManager.SpawnObject("Item", $"Item_{_produceDbData.spi}", _badgeTransform);
		_badgeTrans.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		_userBadge = MasterPoolManager.SpawnObject("Item", $"Item_{_produceDbData.spi}", _userBadgeAnchor);
		_userBadge.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		_liftAnimator.Construct();
		LiftAnimControl floorAnimControl = _liftAnimator;
		floorAnimControl.openBadgeGauge = (Action)Delegate.Combine(floorAnimControl.openBadgeGauge, new Action(OnShowBadgeGauge));
		LiftAnimControl floorAnimControl2 = _liftAnimator;
		floorAnimControl2.begingBadgeAnimation = (Action)Delegate.Combine(floorAnimControl2.begingBadgeAnimation, new Action(BadgeStartTween));
		LiftAnimControl floorAnimControl3 = _liftAnimator;
		floorAnimControl3.OnReward = (Action)Delegate.Combine(floorAnimControl3.OnReward, new Action(OnRewardTaken));
		LiftAnimControl floorAnimControl4 = _liftAnimator;
		floorAnimControl4.OnEffectsReady = (Action)Delegate.Combine(floorAnimControl4.OnEffectsReady, new Action(OnAllCompleted));
		_completeEffect.SetActive(value: false);
		_lockObject.SetActive(value: false);
		_floorStage = (LiftState)_floorData.state;
		UnityEngine.Debug.Log("typeKey :: " + _floorData.storeRemainTime);
		LocalTimeCheckManager.InitType(_keyType);
		LocalTimeCheckManager.OnTimeTick = (Action<string, float>)Delegate.Combine(LocalTimeCheckManager.OnTimeTick, new Action<string, float>(OnTimeEvent));
		LocalTimeCheckManager.OnLocalTimeComplete = (Action<string>)Delegate.Combine(LocalTimeCheckManager.OnLocalTimeComplete, new Action<string>(LocalTimeOver));
		if (_floorData.state == 2 && _floorData.storeRemainTime > 0)
		{
			LocalTimeCheckManager.AddTimer(_keyType, _floorData.storeRemainTime);
		}
		if (_testAnchorTransform != null && _storePrefab != string.Empty)
		{
			_testAnchorTransform.gameObject.SetActive(value: true);
			_floorTransform = MasterPoolManager.SpawnObject("Lobby", _storePrefab);
		}
		if (_requireTrans == null)
		{
			_requireTrans = MasterPoolManager.SpawnObject("Item", $"Item_{_produceDbData.snip1Type}", _requireItemTransform);
		}
		if (_gaugeBadgeTransform == null)
		{
			_gaugeBadgeTransform = MasterPoolManager.SpawnObject("Item", $"Item_{_produceDbData.spi}", _userGuageTransform);
		}
		if (_ownBadgeTransform == null)
		{
			_ownBadgeTransform = MasterPoolManager.SpawnObject("Item", $"Item_{_produceDbData.spi}", _badgeOwnAnchor);
			_ownBadgeTransform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		}
		ResetLiftState();
	}

	public void ResetProdictData(int storeIdx)
	{
		_produceDbData = GameDataManager.GetStoreProduceData(storeIdx, 1);
	}

	public void StafeIdSet(int _stageId)
	{
		_stageIndex = _stageId;
		if (_floorTransform != null)
		{
			_floorTransform.GetComponent<LiftStore>().BlendSet(_stageIndex);
		}
	}

	public void FloorSet(int floorId)
	{
		_index = floorId;
	}

	public void ChangeType(BlockType _type)
	{
		_blockType = _type;
	}

	public void ChangeBlend(bool isActive)
	{
	}

	public void ChangeIndex(int index)
	{
		_floorItemRectTransform.SetSiblingIndex(index + 2);
	}

	public void LockLift()
	{
		_lockObject.SetActive(value: true);
		_completeEffect.SetActive(value: false);
		_lockMessage.text = string.Format(MasterLocalize.GetData("common_text_clear_chapter"), _index);
		_nameFloorText.text = MasterLocalize.GetData(GameDataManager.GetStoreData(_produceDbData.storeIdx).storeName);
		_lockFloorText.text = MasterLocalize.GetData("common_text_locked");
	}

	public void RefreshLift(UserFloorData _data)
	{
		_floorData = _data;
		_keyType = _floorData.storeIdx.ToString();
		_produceDbData = GameDataManager.GetStoreProduceData(_floorData.storeIdx, _floorData.storeTier);
		_floorStage = (LiftState)_floorData.state;
		if (_floorData.state == 2 && _floorData.storeRemainTime > 0 && LocalTimeCheckManager.GetSecond(_keyType) <= 0.0)
		{
			LocalTimeCheckManager.AddTimer(_keyType, _floorData.storeRemainTime);
		}
		_floorTransform.GetComponent<LiftStore>().Refresh();
		ResetLiftState();
	}

	public void LockAllUI()
	{
		_isEnabledButton = false;
	}

	public void Upgrade()
	{
		switch (_floorStage)
		{
		case LiftState.Progress:
			LocalTimeCheckManager.TimeComplete(_keyType);
			OnClickCollect();
			break;
		case LiftState.Complete:
			OnClickCollect();
			break;
		}
		LocalTimeCheckManager.AddTimer(_keyType, _produceDbData.produceTime);
		_floorStage = LiftState.Progress;
		ResetLiftState();
	}

	public void RunCollection()
	{
		OnCollectFromShop();
	}

	public void LockEffectOpen()
	{
		if (_floorTransform != null)
		{
			LiftStore component = _floorTransform.GetComponent<LiftStore>();
			component.UnlockEffectSpawn();
		}
		SoundController.EffectSound_Play(EffectSoundType.StoreUnlock);
	}

	public void OpenDetails()
	{
		LobbyManager.OpenStageFloorId = _stageIndex;
		LobbyManager.OpenChapterFloorId = _index;
		LobbyManager.ShowFloorDetail(_floorData, _produceDbData);
	}

	public void StoreOpen()
	{
		if (_floorTransform != null)
		{
			_floorTransform.GetComponent<LiftStore>().WowShow();
		}
	}

	public void CloseStore()
	{
		if (_floorTransform != null)
		{
			_floorTransform.GetComponent<LiftStore>().SkibidiHide();
		}
	}

	private void LevelStarsShow()
	{
		if (_floorData != null)
		{
			for (int i = 0; i < _listFloorLevel.Count; i++)
			{
				_listFloorLevel[i].SetActive(i + 1 <= _floorData.storeTier);
			}
		}
	}

	private void ResetLiftState()
	{
		if (_floorData == null)
		{
			return;
		}
		switch (_floorStage)
		{
		case LiftState.Standby:
			_openButton.gameObject.SetActive(value: true);
			_collectButton.gameObject.SetActive(value: false);
			_speedUpButton.gameObject.SetActive(value: false);
			_timerObject.SetActive(value: false);
			SettingStand();
			if ((bool)_floorTransform)
			{
				_floorTransform.GetComponent<LiftStore>().Deactivate();
			}
			break;
		case LiftState.Progress:
			_openButton.gameObject.SetActive(value: false);
			_collectButton.gameObject.SetActive(value: false);
			_speedUpButton.gameObject.SetActive(value: true);
			_speedUpCost.text = $"{GameUtil.GetConvertTimeToJewel((float)LocalTimeCheckManager.GetSecond(_keyType))}";
			_timerObject.SetActive(value: true);
			if ((bool)_floorTransform)
			{
				_floorTransform.GetComponent<LiftStore>().Activate();
			}
			break;
		case LiftState.Complete:
			_openButton.gameObject.SetActive(value: false);
			_collectButton.gameObject.SetActive(value: true);
			_speedUpButton.gameObject.SetActive(value: false);
			_collectCoinsText.text = $"{_produceDbData.getCoin:#,##0}";
			_timerObject.SetActive(value: false);
			if ((bool)_floorTransform)
			{
				_floorTransform.GetComponent<LiftStore>().Deactivate();
			}
			break;
		}
		LevelStarsShow();
		ReloadUpgrade();
	}

	private void SettingStand()
	{
		if (_produceDbData != null)
		{
			_userItemsNumber = GameInfo.userData.GetItemCount(_produceDbData.snip1Type);
			if (_userItemsNumber < _produceDbData.snip1N)
			{
				_costText.text = $"<color=red>{_userItemsNumber}</color>/{_produceDbData.snip1N}";
			}
			else
			{
				_costText.text = $"{_userItemsNumber}/{_produceDbData.snip1N}";
			}
		}
	}

	private void ReloadUpgrade()
	{
		if (!(_upgradeNoticeTransform == null))
		{
			ClearNotice();
			if (CheckIsUpgradeAvailable())
			{
				_upgradeNoticeTrans = MasterPoolManager.SpawnObject("Lobby", "Notice_Upgrade", _upgradeNoticeTransform);
			}
		}
	}

	private void ClearNotice()
	{
		if (_upgradeNoticeTrans != null)
		{
			MasterPoolManager.ReturnToPool("Lobby", _upgradeNoticeTrans);
			_upgradeNoticeTrans = null;
		}
	}

	private bool CheckIsUpgradeAvailable()
	{
		StoreUpgradeDbData storeUpgradeData = GameDataManager.GetStoreUpgradeData(_floorData.storeIdx, _floorData.storeTier);
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

	private void OnTimeEvent(string type, float second)
	{
		if (type == _keyType)
		{
			if (second < 0f)
			{
				second = 0f;
			}
			float num = Mathf.Floor(second);
			TimeSpan timeSpan = TimeSpan.FromSeconds(num);
			string text = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
			_produceTimeText.text = text;
			float num2 = ((float)_produceDbData.produceTime - second) / (float)_produceDbData.produceTime;
			Vector2 sizeDelta = _produceTimeRectTransform.sizeDelta;
			sizeDelta.x = num2 * 446f;
			_produceTimeRectTransform.sizeDelta = sizeDelta;
			_remainDuration = second;
			_speedUpCost.text = $"{GameUtil.GetConvertTimeToJewel(second)}";
		}
	}

	private void LocalTimeOver(string type)
	{
		if (type == _keyType)
		{
			Protocol_Set.Protocol_user_info_Req(null, isLoading: false);
			LobbyManager.HideSpeedUpCollect();
		}
	}

	private void OnShowBadgeGauge()
	{
		int num = _floorData.operatingRatio;
		if (num == 0)
		{
			num = 5;
		}
		UnityEngine.Debug.Log("OnShowBadgeGauge :: " + num);
		_animatorBadge.Play("Badgegauge_light0" + num + "_stop");
		SoundController.EffectSound_Play(EffectSoundType.FillGauge);
	}

	private void BadgeStartTween()
	{
		StopBadgeAnimating();
		_badgeCoroutine = StartCoroutine(LoadingAnimation());
	}

	private void OnRewardTaken()
	{
		if (_floorData.operatingRatio == 0)
		{
			_liftAnimator.Next();
		}
		else
		{
			_completeEffect.SetActive(value: false);
		}
	}

	private void OnAllCompleted()
	{
		_liftAnimator.RemoveAll();
		_completeEffect.SetActive(value: false);
	}

	private void StopBadgeAnimating()
	{
		if (_badgeCoroutine != null)
		{
			StopCoroutine(_badgeCoroutine);
			_badgeCoroutine = null;
		}
	}

	private IEnumerator LoadingAnimation()
	{
		_animatorBadge.SetTrigger("ContinueTrigger");
		yield return new WaitForSeconds(2.5f);
		_liftAnimator.Next();
	}

	private IEnumerator LevelUpRoutine(int _addCoin, int _addExp)
	{
		int expCount = UnityEngine.Random.Range(4, 8);
		for (int i = 0; i < expCount; i++)
		{
			float num = UnityEngine.Random.Range(-0.2f, -0.1f);
			Transform transform = MasterPoolManager.SpawnObject("Effect", "FX_Exp_get", null, 1.2f + num + 0.4f);
			transform.localScale = new Vector2(0.12f, 0.12f);
			transform.position = _collectButton.transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
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
			Transform transform2 = MasterPoolManager.SpawnObject("Effect", "FX_Coin_get", null, 1.2f + num2 + 0.4f);
			transform2.localScale = new Vector2(0.12f, 0.12f);
			transform2.position = _collectButton.transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
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

	private void OnStoreShow()
	{
		SoundController.EffectSound_Play(EffectSoundType.StoreOpen);
		if (OnStoreOpen != null)
		{
			OnStoreOpen();
		}
		if (_userItemsNumber >= _produceDbData.snip1N)
		{
			StopBadgeAnimating();
			_liftAnimator.RemoveAll();
			_completeEffect.SetActive(value: false);
		}
	}

	private void OnCollectFromShop()
	{
		if (OnCompleteCollect != null)
		{
			OnCompleteCollect();
		}
		StartCoroutine(LevelUpRoutine(_produceDbData.getCoin, _produceDbData.getExp));
		LocalTimeCheckManager.TimeClear(_keyType);
		_completeEffect.SetActive(value: true);
		_liftAnimator.PlaceCoin(_produceDbData.getCoin);
		_liftAnimator.PlaceBudges(_produceDbData.spiN);
		_liftAnimator.CountRaio(_floorData.operatingRatio);
		_liftAnimator.BadgeIndex(_produceDbData.spi);
		_liftAnimator.StartPlay();
		if (_floorData.operatingRatio == 0)
		{
			LobbyManager.CallBadgeAcquireEvent(_stageIndex, _index);
		}
	}

	private void OnStoreSpeedUpComplete()
	{
	}

	private void SpeedCollect(bool isSuccess)
	{
		if (isSuccess)
		{
			Protocol_Set.Protocol_store_speed_up_Req(_stageIndex + 1, _floorData.storeIdx, GameUtil.GetConvertTimeToJewel(_remainDuration), OnCollectFromShop);
		}
	}

	private void ListResponse()
	{
		LobbyManager.ShowValueShop(ValueShopType.Jewel);
	}

	public void OnClickOpen()
	{
		if (!_isEnabledButton)
		{
			return;
		}
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		if (_userItemsNumber < _produceDbData.snip1N)
		{
			LobbyManager.ShowItemSortList(_produceDbData.snip1Type, isWaveItemSort: true);
			return;
		}
		if (GamePreferenceManager.GetIsAnalytics("09_open_store"))
		{
			
		}
		Protocol_Set.Protocol_store_open_Req(_stageIndex + 1, _floorData.storeIdx, OnStoreShow);
	}

	public void OnClickCollect()
	{
		if (_isEnabledButton)
		{
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
			if (GamePreferenceManager.GetIsAnalytics("91_store_open_by_self"))
			{
				
			}
			Protocol_Set.Protocol_store_collect_Req(_stageIndex + 1, _floorData.storeIdx, OnCollectFromShop);
		}
	}

	public void OnClickSpeedUp()
	{
		if (_isEnabledButton)
		{
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
			if (GameInfo.userData.userInfo.jewel < GameUtil.GetConvertTimeToJewel(_remainDuration))
			{
				Protocol_Set.Protocol_shop_list_Req(ListResponse);
			}
			else
			{
				LobbyManager.ShowSpeedUpCollect(_remainDuration, GameUtil.GetConvertTimeToJewel(_remainDuration), _produceDbData.getCoin, SpeedCollect);
			}
		}
	}

	public void OnClickDetail()
	{
		if (_isEnabledButton)
		{
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
			LobbyManager.OpenStageFloorId = _stageIndex;
			LobbyManager.OpenChapterFloorId = _index;
			LobbyManager.ShowFloorDetail(_floorData, _produceDbData);
		}
	}

	public void SyncronizeStore()
	{
		if (_floorTransform != null && _testAnchorTransform != null && _floorTransform.position != _testAnchorTransform.position)
		{
			_floorTransform.position = _testAnchorTransform.position;
		}
	}

	private void Awake()
	{
		_floorItemRectTransform = base.gameObject.GetComponent<RectTransform>();
	}

	private void OnDisable()
	{
		if (_floorTransform != null)
		{
			MasterPoolManager.ReturnToPool("Lobby", _floorTransform);
			_floorTransform = null;
		}
		LocalTimeCheckManager.OnTimeTick = (Action<string, float>)Delegate.Remove(LocalTimeCheckManager.OnTimeTick, new Action<string, float>(OnTimeEvent));
		LocalTimeCheckManager.OnLocalTimeComplete = (Action<string>)Delegate.Remove(LocalTimeCheckManager.OnLocalTimeComplete, new Action<string>(LocalTimeOver));
		if (_keyType != null)
		{
			LocalTimeCheckManager.TimeClear(_keyType);
		}
		LocalTimeCheckManager.SaveAndExit(_keyType);
		if (_liftAnimator != null)
		{
			LiftAnimControl floorAnimControl = _liftAnimator;
			floorAnimControl.begingBadgeAnimation = (Action)Delegate.Remove(floorAnimControl.begingBadgeAnimation, new Action(BadgeStartTween));
			LiftAnimControl floorAnimControl2 = _liftAnimator;
			floorAnimControl2.OnReward = (Action)Delegate.Remove(floorAnimControl2.OnReward, new Action(OnRewardTaken));
			LiftAnimControl floorAnimControl3 = _liftAnimator;
			floorAnimControl3.openBadgeGauge = (Action)Delegate.Remove(floorAnimControl3.openBadgeGauge, new Action(OnShowBadgeGauge));
			LiftAnimControl floorAnimControl4 = _liftAnimator;
			floorAnimControl4.OnEffectsReady = (Action)Delegate.Remove(floorAnimControl4.OnEffectsReady, new Action(OnAllCompleted));
		}
		if (_badgeTrans != null)
		{
			MasterPoolManager.ReturnToPool("Item", _badgeTrans);
			_badgeTrans = null;
		}
		if (_userBadge != null)
		{
			MasterPoolManager.ReturnToPool("Item", _userBadge);
			_userBadge = null;
		}
		if (_requireTrans != null)
		{
			MasterPoolManager.ReturnToPool("Item", _requireTrans);
			_requireTrans = null;
		}
		if (_gaugeBadgeTransform != null)
		{
			MasterPoolManager.ReturnToPool("Item", _gaugeBadgeTransform);
			_gaugeBadgeTransform = null;
		}
		if (_ownBadgeTransform != null)
		{
			MasterPoolManager.ReturnToPool("Item", _ownBadgeTransform);
			_ownBadgeTransform = null;
		}
		ClearNotice();
	}
}
