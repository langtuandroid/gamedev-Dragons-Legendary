using System;
using UnityEngine;
using UnityEngine.UI;

public class FloorDetail : LobbyPopupBase
{
	public Action GoBackEvent;

	[SerializeField]
	private Text textFloorName;

	[SerializeField]
	private Text textOpenInfo;

	[SerializeField]
	private Text textStoreDuration;

	[SerializeField]
	private Text textStoreEarnings;

	[SerializeField]
	private Text textUserOwnBadge;

	[SerializeField]
	private Text textStoreBadge;

	[SerializeField]
	private Text textGetBadgeCount;

	[SerializeField]
	private Image imageTitle;

	[SerializeField]
	private Transform trBadgeAnchor;

	[SerializeField]
	private Transform trUpgradeButton;

	[SerializeField]
	private Transform trToOpenItemAnchor;

	[SerializeField]
	private GameObject goUpgradeButton;

	[SerializeField]
	private GameObject[] arrGoLevelStar = new GameObject[0];

	[SerializeField]
	private GameObject[] arrGoStoreOperating = new GameObject[0];

	private int userItemCount;

	private Transform trBadge;

	private Transform trToOpenItem;

	private UserFloorData userFloorData;

	private StoreProduceDbData produceData;

	public Transform UpgradeButton => trUpgradeButton;

	public void Show(UserFloorData _userData, StoreProduceDbData _produceData)
	{
		userFloorData = _userData;
		produceData = _produceData;
		Open();
	}

	public override void Open()
	{
		base.Open();
		Init();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void CloseProcessComplete()
	{
	}

	private void Init()
	{
		userItemCount = GameInfo.userData.GetItemCount(produceData.snip1Type);
		if (userItemCount < produceData.snip1N)
		{
			textOpenInfo.text = $"<color=red>{userItemCount}</color>/{produceData.snip1N}";
		}
		else
		{
			textOpenInfo.text = $"{userItemCount}/{produceData.snip1N}";
		}
		UnityEngine.Debug.Log("produceTime :: " + produceData.produceTime);
		TimeSpan timeSpan = TimeSpan.FromSeconds(produceData.produceTime);
		string text = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}";
		textStoreDuration.text = text;
		textStoreEarnings.text = $"{produceData.getCoin}";
		textUserOwnBadge.text = string.Format("{0} {1}", MasterLocalize.GetData("common_text_you_have"), GameInfo.userData.GetItemCount(GameDataManager.GetStoreData(produceData.storeIdx).spi));
		textStoreBadge.text = $"{userFloorData.operatingRatio}/{GameDataManager.GetGameConfigData(ConfigDataType.StoreGetbadgeCycle)}";
		textFloorName.text = MasterLocalize.GetData(GameDataManager.GetStoreData(produceData.storeIdx).storeName);
		imageTitle.sprite = GameDataManager.GetFloorTitleSprite(LobbyManager.OpenChapterFloorId);
		trToOpenItem = MasterPoolManager.SpawnObject("Item", $"Item_{produceData.snip1Type}", trToOpenItemAnchor);
		trBadge = MasterPoolManager.SpawnObject("Item", $"Item_{produceData.spi}", trBadgeAnchor);
		textGetBadgeCount.text = $"X{produceData.spiN}";
		for (int i = 0; i < arrGoLevelStar.Length; i++)
		{
			arrGoLevelStar[i].SetActive(i + 1 <= userFloorData.storeTier);
		}
		for (int j = 0; j < arrGoStoreOperating.Length; j++)
		{
			arrGoStoreOperating[j].SetActive(j + 1 <= userFloorData.operatingRatio);
		}
		goUpgradeButton.SetActive(GameDataManager.GetStoreProduceData(userFloorData.storeIdx, userFloorData.storeTier + 1) != null);
	}

	public void OnClickGotoUpgrade()
	{
		if (GameDataManager.GetStoreProduceData(userFloorData.storeIdx, userFloorData.storeTier + 1) != null)
		{
			LobbyManager.ShowFloorUpgrade(userFloorData.storeIdx, userFloorData.storeTier);
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		}
	}

	public void OnClickGoBack()
	{
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}

	private void OnDisable()
	{
		if (trBadge != null)
		{
			MasterPoolManager.ReturnToPool("Item", trBadge);
			trBadge = null;
		}
		if (trToOpenItem != null)
		{
			MasterPoolManager.ReturnToPool("Item", trToOpenItem);
			trToOpenItem = null;
		}
	}
}
