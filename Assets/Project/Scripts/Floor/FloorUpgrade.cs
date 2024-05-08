using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorUpgrade : LobbyPopupBase
{
	public Action<bool> GoBackEvent;

	[SerializeField]
	private Text textStoreName;

	[SerializeField]
	private Text textCurrentReward;

	[SerializeField]
	private Text textNextReward;

	[SerializeField]
	private Text textCurrentEarnings;

	[SerializeField]
	private Text textNextEarnings;

	[SerializeField]
	private Text textCurrentToOpen;

	[SerializeField]
	private Text textNextToOpen;

	[SerializeField]
	private Text textCurrentDuration;

	[SerializeField]
	private Text textNextDuration;

	[SerializeField]
	private Text textDiscription;

	[SerializeField]
	private Image imageFloorTitle;

	[SerializeField]
	private Transform trStoreAblility;

	[SerializeField]
	private Transform trStoreRequireItem;

	[SerializeField]
	private Transform trRequireItemAnchor;

	[SerializeField]
	private Transform trUpgradeButton;

	[SerializeField]
	private GameObject goLockUpgrade;

	[SerializeField]
	private GameObject goRequireItemDimmed;

	[SerializeField]
	private GameObject[] arrGoLevelStar = new GameObject[0];

	private StoreUpgradeDbData upgradeStoreData;

	private StoreProduceDbData currentProduceStoreData;

	private StoreProduceDbData nextProduceStoreData;

	private RequiredItem_Cell requiredCoinCell;

	private List<UpgradeRequireItemData> listRequireItemData = new List<UpgradeRequireItemData>();

	public Transform StoreAbility => trStoreAblility;

	public Transform ReauireItemAnchor => trStoreRequireItem;

	public Transform UpgradeButton => trUpgradeButton;

	public void Show(int storeIdx, int storeTier)
	{
		upgradeStoreData = GameDataManager.GetStoreUpgradeData(storeIdx, storeTier);
		currentProduceStoreData = GameDataManager.GetStoreProduceData(storeIdx, storeTier);
		nextProduceStoreData = GameDataManager.GetStoreProduceData(storeIdx, storeTier + 1);
		Open();
	}

	public override void Open()
	{
		base.Open();
		GameDataManager.ChangeUserData = (Action)Delegate.Combine(GameDataManager.ChangeUserData, new Action(OnUserDataChangeEvent));
		Init();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void CloseProcessComplete()
	{
	}

	public void ShowDimmedRequireItem()
	{
		goRequireItemDimmed.SetActive(value: true);
	}

	private void Init()
	{
		UserFloorData userFloorData = LobbyManager.GetUserFloorData();
		for (int i = 0; i < arrGoLevelStar.Length; i++)
		{
			arrGoLevelStar[i].SetActive(i <= userFloorData.storeTier);
		}
		textStoreName.text = MasterLocalize.GetData(GameDataManager.GetStoreData(currentProduceStoreData.storeIdx).storeName);
		textCurrentReward.text = $"x{currentProduceStoreData.spiN}";
		textNextReward.text = $"x{nextProduceStoreData.spiN}";
		textCurrentEarnings.text = $"{currentProduceStoreData.getCoin}";
		textNextEarnings.text = $"{nextProduceStoreData.getCoin}";
		textCurrentToOpen.text = $"{currentProduceStoreData.snip1N}";
		textNextToOpen.text = $"{nextProduceStoreData.snip1N}";
		TimeSpan timeSpan = TimeSpan.FromSeconds(currentProduceStoreData.produceTime);
		string arg = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}";
		textCurrentDuration.text = $"{arg}";
		timeSpan = TimeSpan.FromSeconds(nextProduceStoreData.produceTime);
		string arg2 = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}";
		textNextDuration.text = $"{arg2}";
		ShowDiscription();
		ShowRequireItem();
		goLockUpgrade.SetActive(!CheckUpgradeState());
		goRequireItemDimmed.SetActive(value: false);
		NotEnouchCoin.CallBuyCoin = OnCallBuyCoinEvent;
	}

	private void ShowDiscription()
	{
		textDiscription.text = string.Format(MasterLocalize.GetData("popup_store_upgrade_text_1"), MasterLocalize.GetData(GameDataManager.GetItemListData(nextProduceStoreData.spi).itemName), nextProduceStoreData.spiN);
	}

	private void ShowRequireItem()
	{
		requiredCoinCell = MasterPoolManager.SpawnObject("Grow", "cell_coin", trRequireItemAnchor).GetComponent<RequiredItem_Cell>();
		requiredCoinCell.SetItemImg(50032);
		if (GameInfo.userData.userInfo.coin < upgradeStoreData.needCoin)
		{
			requiredCoinCell.SetCostText($"<color=red>{upgradeStoreData.needCoin}</color>");
			requiredCoinCell.SetClickType(ItemClickType.Coin, upgradeStoreData.needCoin - GameInfo.userData.userInfo.coin);
		}
		else
		{
			requiredCoinCell.SetCostText($"{upgradeStoreData.needCoin}");
			requiredCoinCell.SetClickType(ItemClickType.None);
		}
		listRequireItemData = GetRequireItemData();
		foreach (UpgradeRequireItemData listRequireItemDatum in listRequireItemData)
		{
			RequiredItem_Cell component = MasterPoolManager.SpawnObject("Grow", "cell_token", trRequireItemAnchor).GetComponent<RequiredItem_Cell>();
			component.SetItemImg(listRequireItemDatum.itemIdx);
			component.SetCostText(GetItemStateText(GameInfo.userData.GetItemCount(listRequireItemDatum.itemIdx), listRequireItemDatum.itemCount));
		}
	}

	private string GetCoinStateText()
	{
		if (GameInfo.userData.userInfo.coin < upgradeStoreData.needCoin)
		{
			return $"<color=red>{upgradeStoreData.needCoin}</color>";
		}
		return $"{upgradeStoreData.needCoin}";
	}

	private string GetItemStateText(int currentCount, int needCound)
	{
		if (currentCount < needCound)
		{
			return $"<color=red>{currentCount}</color>/{needCound}";
		}
		return $"{currentCount}/{needCound}";
	}

	private bool CheckUpgradeState()
	{
		if (GameInfo.userData.userInfo.coin < upgradeStoreData.needCoin)
		{
			return false;
		}
		foreach (UpgradeRequireItemData listRequireItemDatum in listRequireItemData)
		{
			if (GameInfo.userData.GetItemCount(listRequireItemDatum.itemIdx) < listRequireItemDatum.itemCount)
			{
				return false;
			}
		}
		return true;
	}

	private List<UpgradeRequireItemData> GetRequireItemData()
	{
		List<UpgradeRequireItemData> list = new List<UpgradeRequireItemData>();
		if (upgradeStoreData.sniu1 > 0)
		{
			UpgradeRequireItemData upgradeRequireItemData = new UpgradeRequireItemData();
			upgradeRequireItemData.itemIdx = upgradeStoreData.sniu1;
			upgradeRequireItemData.itemCount = upgradeStoreData.sniu1_N;
			list.Add(upgradeRequireItemData);
		}
		if (upgradeStoreData.sniu2 > 0)
		{
			UpgradeRequireItemData upgradeRequireItemData2 = new UpgradeRequireItemData();
			upgradeRequireItemData2.itemIdx = upgradeStoreData.sniu2;
			upgradeRequireItemData2.itemCount = upgradeStoreData.sniu2_N;
			list.Add(upgradeRequireItemData2);
		}
		if (upgradeStoreData.sniu3 > 0)
		{
			UpgradeRequireItemData upgradeRequireItemData3 = new UpgradeRequireItemData();
			upgradeRequireItemData3.itemIdx = upgradeStoreData.sniu3;
			upgradeRequireItemData3.itemCount = upgradeStoreData.sniu3_N;
			list.Add(upgradeRequireItemData3);
		}
		if (upgradeStoreData.sniu4 > 0)
		{
			UpgradeRequireItemData upgradeRequireItemData4 = new UpgradeRequireItemData();
			upgradeRequireItemData4.itemIdx = upgradeStoreData.sniu4;
			upgradeRequireItemData4.itemCount = upgradeStoreData.sniu4_N;
			list.Add(upgradeRequireItemData4);
		}
		return list;
	}

	private void OnUserDataChangeEvent()
	{
		RequiredItem_Cell[] componentsInChildren = trRequireItemAnchor.GetComponentsInChildren<RequiredItem_Cell>();
		foreach (RequiredItem_Cell requiredItem_Cell in componentsInChildren)
		{
			MasterPoolManager.ReturnToPool("Grow", requiredItem_Cell.transform);
		}
		Init();
	}

	private void OnUpgradeConnectComplete(string forceCollect)
	{
		if (GoBackEvent != null)
		{
			GoBackEvent(forceCollect == "y");
		}
		SoundController.EffectSound_Play(EffectSoundType.UseCoin);
		SoundController.EffectSound_Play(EffectSoundType.StoreUpgrade);
	}

	private void OnCallBuyCoinEvent(int _needJewel)
	{
		Protocol_Set.Protocol_shop_popup_store_buy_coin_Req(currentProduceStoreData.storeIdx, _needJewel, OnBuyCoinComplete);
	}

	private void OnBuyCoinComplete()
	{
		if (GameInfo.userData.userInfo.coin < upgradeStoreData.needCoin)
		{
			requiredCoinCell.SetCostText($"<color=red>{upgradeStoreData.needCoin}</color>");
			requiredCoinCell.SetClickType(ItemClickType.Coin, upgradeStoreData.needCoin - GameInfo.userData.userInfo.coin);
		}
		else
		{
			requiredCoinCell.SetCostText($"{upgradeStoreData.needCoin}");
			requiredCoinCell.SetClickType(ItemClickType.None);
		}
	}

	public void OnClickGoBack()
	{
		if (GoBackEvent != null)
		{
			GoBackEvent(obj: false);
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}

	public void OnClickUpgrade()
	{
		if (CheckUpgradeState())
		{
			Protocol_Set.Protocol_store_upgrade_Req(currentProduceStoreData.storeIdx, currentProduceStoreData.storeTier, OnUpgradeConnectComplete);
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		}
	}

	private void OnDisable()
	{
		RequiredItem_Cell[] componentsInChildren = trRequireItemAnchor.GetComponentsInChildren<RequiredItem_Cell>();
		foreach (RequiredItem_Cell requiredItem_Cell in componentsInChildren)
		{
			MasterPoolManager.ReturnToPool("Grow", requiredItem_Cell.transform);
		}
		GameDataManager.ChangeUserData = (Action)Delegate.Remove(GameDataManager.ChangeUserData, new Action(OnUserDataChangeEvent));
		NotEnouchCoin.CallBuyCoin = null;
	}
}
