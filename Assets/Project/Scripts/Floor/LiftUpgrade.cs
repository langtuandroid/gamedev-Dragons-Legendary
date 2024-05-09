using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LiftUpgrade : LobbyPopupBase
{
	public Action<bool> OnBackEvent;

	[FormerlySerializedAs("textStoreName")] [SerializeField]
	private Text _storeNameText;

	[FormerlySerializedAs("textCurrentReward")] [SerializeField]
	private Text _currentRewardText;

	[FormerlySerializedAs("textNextReward")] [SerializeField]
	private Text _rewardText;

	[FormerlySerializedAs("textCurrentEarnings")] [SerializeField]
	private Text _currentEarninggsText;

	[FormerlySerializedAs("textNextEarnings")] [SerializeField]
	private Text _earningNextText;

	[FormerlySerializedAs("textCurrentToOpen")] [SerializeField]
	private Text _openCurrentText;

	[FormerlySerializedAs("textNextToOpen")] [SerializeField]
	private Text _openNextText;

	[FormerlySerializedAs("textCurrentDuration")] [SerializeField]
	private Text _durationText;

	[FormerlySerializedAs("textNextDuration")] [SerializeField]
	private Text _nextDuration;

	[FormerlySerializedAs("textDiscription")] [SerializeField]
	private Text _descriptionText;

	[FormerlySerializedAs("imageFloorTitle")] [SerializeField]
	private Image _floorTitle;

	[FormerlySerializedAs("trStoreAblility")] [SerializeField]
	private Transform _abilityStore;

	[FormerlySerializedAs("trStoreRequireItem")] [SerializeField]
	private Transform _storeRquire;

	[FormerlySerializedAs("trRequireItemAnchor")] [SerializeField]
	private Transform _requireItemAnchor;

	[FormerlySerializedAs("trUpgradeButton")] [SerializeField]
	private Transform _upgardeButtonTransform;

	[FormerlySerializedAs("goLockUpgrade")] [SerializeField]
	private GameObject _lockUpgrade;

	[FormerlySerializedAs("goRequireItemDimmed")] [SerializeField]
	private GameObject _requireItemDimmed;

	[FormerlySerializedAs("arrGoLevelStar")] [SerializeField]
	private GameObject[] _levelStar = new GameObject[0];

	private StoreUpgradeDbData _storeData;

	private StoreProduceDbData _produceStore;

	private StoreProduceDbData _nextProduceStore;

	private RequiredItem_Cell _requiredCoinCell;

	private List<UpgradeRequireItemData> _listRequireItemData = new List<UpgradeRequireItemData>();

	public Transform StoreAbility => _abilityStore;

	public Transform ReauireItemAnchor => _storeRquire;

	public Transform UpgradeButtonTransform => _upgardeButtonTransform;

	public void Openn(int storeIdx, int storeTier)
	{
		_storeData = GameDataManager.GetStoreUpgradeData(storeIdx, storeTier);
		_produceStore = GameDataManager.GetStoreProduceData(storeIdx, storeTier);
		_nextProduceStore = GameDataManager.GetStoreProduceData(storeIdx, storeTier + 1);
		Open();
	}

	public override void Open()
	{
		base.Open();
		GameDataManager.ChangeUserData = (Action)Delegate.Combine(GameDataManager.ChangeUserData, new Action(WhenUserDataChanged));
		Construct();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void Complete()
	{
	}

	public void ShowDimmedRequireItem()
	{
		_requireItemDimmed.SetActive(value: true);
	}

	private void Construct()
	{
		UserFloorData userFloorData = LobbyManager.GetUserFloorData();
		for (int i = 0; i < _levelStar.Length; i++)
		{
			_levelStar[i].SetActive(i <= userFloorData.storeTier);
		}
		_storeNameText.text = MasterLocalize.GetData(GameDataManager.GetStoreData(_produceStore.storeIdx).storeName);
		_currentRewardText.text = $"x{_produceStore.spiN}";
		_rewardText.text = $"x{_nextProduceStore.spiN}";
		_currentEarninggsText.text = $"{_produceStore.getCoin}";
		_earningNextText.text = $"{_nextProduceStore.getCoin}";
		_openCurrentText.text = $"{_produceStore.snip1N}";
		_openNextText.text = $"{_nextProduceStore.snip1N}";
		TimeSpan timeSpan = TimeSpan.FromSeconds(_produceStore.produceTime);
		string arg = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}";
		_durationText.text = $"{arg}";
		timeSpan = TimeSpan.FromSeconds(_nextProduceStore.produceTime);
		string arg2 = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}";
		_nextDuration.text = $"{arg2}";
		ShowDiscription();
		ShowNeededItem();
		_lockUpgrade.SetActive(!UpgradeCheck());
		_requireItemDimmed.SetActive(value: false);
		NotEnouchCoin.CallBuyCoin = CollCoinsEvent;
	}

	private void ShowDiscription()
	{
		_descriptionText.text = string.Format(MasterLocalize.GetData("popup_store_upgrade_text_1"), MasterLocalize.GetData(GameDataManager.GetItemListData(_nextProduceStore.spi).itemName), _nextProduceStore.spiN);
	}

	private void ShowNeededItem()
	{
		_requiredCoinCell = MasterPoolManager.SpawnObject("Grow", "cell_coin", _requireItemAnchor).GetComponent<RequiredItem_Cell>();
		_requiredCoinCell.SetItemImg(50032);
		if (GameInfo.userData.userInfo.coin < _storeData.needCoin)
		{
			_requiredCoinCell.SetCostText($"<color=red>{_storeData.needCoin}</color>");
			_requiredCoinCell.SetClickType(LootClickType.Coin, _storeData.needCoin - GameInfo.userData.userInfo.coin);
		}
		else
		{
			_requiredCoinCell.SetCostText($"{_storeData.needCoin}");
			_requiredCoinCell.SetClickType(LootClickType.None);
		}
		_listRequireItemData = GetNeededItems();
		foreach (UpgradeRequireItemData listRequireItemDatum in _listRequireItemData)
		{
			RequiredItem_Cell component = MasterPoolManager.SpawnObject("Grow", "cell_token", _requireItemAnchor).GetComponent<RequiredItem_Cell>();
			component.SetItemImg(listRequireItemDatum.itemIdx);
			component.SetCostText(StateText(GameInfo.userData.GetItemCount(listRequireItemDatum.itemIdx), listRequireItemDatum.itemCount));
		}
	}

	private string GetCoinStateText()
	{
		if (GameInfo.userData.userInfo.coin < _storeData.needCoin)
		{
			return $"<color=red>{_storeData.needCoin}</color>";
		}
		return $"{_storeData.needCoin}";
	}

	private string StateText(int currentCount, int needCound)
	{
		if (currentCount < needCound)
		{
			return $"<color=red>{currentCount}</color>/{needCound}";
		}
		return $"{currentCount}/{needCound}";
	}

	private bool UpgradeCheck()
	{
		if (GameInfo.userData.userInfo.coin < _storeData.needCoin)
		{
			return false;
		}
		foreach (UpgradeRequireItemData listRequireItemDatum in _listRequireItemData)
		{
			if (GameInfo.userData.GetItemCount(listRequireItemDatum.itemIdx) < listRequireItemDatum.itemCount)
			{
				return false;
			}
		}
		return true;
	}

	private List<UpgradeRequireItemData> GetNeededItems()
	{
		List<UpgradeRequireItemData> list = new List<UpgradeRequireItemData>();
		if (_storeData.sniu1 > 0)
		{
			UpgradeRequireItemData upgradeRequireItemData = new UpgradeRequireItemData();
			upgradeRequireItemData.itemIdx = _storeData.sniu1;
			upgradeRequireItemData.itemCount = _storeData.sniu1_N;
			list.Add(upgradeRequireItemData);
		}
		if (_storeData.sniu2 > 0)
		{
			UpgradeRequireItemData upgradeRequireItemData2 = new UpgradeRequireItemData();
			upgradeRequireItemData2.itemIdx = _storeData.sniu2;
			upgradeRequireItemData2.itemCount = _storeData.sniu2_N;
			list.Add(upgradeRequireItemData2);
		}
		if (_storeData.sniu3 > 0)
		{
			UpgradeRequireItemData upgradeRequireItemData3 = new UpgradeRequireItemData();
			upgradeRequireItemData3.itemIdx = _storeData.sniu3;
			upgradeRequireItemData3.itemCount = _storeData.sniu3_N;
			list.Add(upgradeRequireItemData3);
		}
		if (_storeData.sniu4 > 0)
		{
			UpgradeRequireItemData upgradeRequireItemData4 = new UpgradeRequireItemData();
			upgradeRequireItemData4.itemIdx = _storeData.sniu4;
			upgradeRequireItemData4.itemCount = _storeData.sniu4_N;
			list.Add(upgradeRequireItemData4);
		}
		return list;
	}

	private void WhenUserDataChanged()
	{
		RequiredItem_Cell[] componentsInChildren = _requireItemAnchor.GetComponentsInChildren<RequiredItem_Cell>();
		foreach (RequiredItem_Cell requiredItem_Cell in componentsInChildren)
		{
			MasterPoolManager.ReturnToPool("Grow", requiredItem_Cell.transform);
		}
		Construct();
	}

	private void UpgradeConnect(string forceCollect)
	{
		if (OnBackEvent != null)
		{
			OnBackEvent(forceCollect == "y");
		}
		SoundController.EffectSound_Play(EffectSoundType.UseCoin);
		SoundController.EffectSound_Play(EffectSoundType.StoreUpgrade);
	}

	private void CollCoinsEvent(int _needJewel)
	{
		Protocol_Set.Protocol_shop_popup_store_buy_coin_Req(_produceStore.storeIdx, _needJewel, OnComplete);
	}

	private void OnComplete()
	{
		if (GameInfo.userData.userInfo.coin < _storeData.needCoin)
		{
			_requiredCoinCell.SetCostText($"<color=red>{_storeData.needCoin}</color>");
			_requiredCoinCell.SetClickType(LootClickType.Coin, _storeData.needCoin - GameInfo.userData.userInfo.coin);
		}
		else
		{
			_requiredCoinCell.SetCostText($"{_storeData.needCoin}");
			_requiredCoinCell.SetClickType(LootClickType.None);
		}
	}

	public void OnClickGoBack()
	{
		if (OnBackEvent != null)
		{
			OnBackEvent(obj: false);
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}

	public void OnClickUpgrade()
	{
		if (UpgradeCheck())
		{
			Protocol_Set.Protocol_store_upgrade_Req(_produceStore.storeIdx, _produceStore.storeTier, UpgradeConnect);
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		}
	}

	private void OnDisable()
	{
		RequiredItem_Cell[] componentsInChildren = _requireItemAnchor.GetComponentsInChildren<RequiredItem_Cell>();
		foreach (RequiredItem_Cell requiredItem_Cell in componentsInChildren)
		{
			MasterPoolManager.ReturnToPool("Grow", requiredItem_Cell.transform);
		}
		GameDataManager.ChangeUserData = (Action)Delegate.Remove(GameDataManager.ChangeUserData, new Action(WhenUserDataChanged));
		NotEnouchCoin.CallBuyCoin = null;
	}
}
