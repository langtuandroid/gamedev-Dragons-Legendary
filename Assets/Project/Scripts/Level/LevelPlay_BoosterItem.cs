using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelPlay_BoosterItem : MonoBehaviour
{
	[SerializeField]
	private Transform booster_selected;

	[SerializeField]
	private Transform booster_deselected;

	[SerializeField]
	private Transform booster_icon;

	[SerializeField]
	private Transform booster_check;

	[SerializeField]
	private Transform booster_Free;

	[SerializeField]
	private Transform booster_Jewel;

	[SerializeField]
	private Text booster_Name;

	[SerializeField]
	private Text booster_Cost;

	private BoostItemDbData boosterData;

	private bool isSelected;

	private int itemType;

	public bool IsSelect => isSelected;

	public int BoostItemType => itemType;

	public BoostItemDbData BoosterData => boosterData;

	public void Init(int _itemType)
	{
		InitItem();
		itemType = _itemType;
		SetBoosterItem();
	}

	private void InitItem()
	{
		isSelected = false;
		booster_selected.gameObject.SetActive(value: false);
		booster_deselected.gameObject.SetActive(value: true);
		for (int i = 0; i < booster_icon.childCount; i++)
		{
			MWPoolManager.DeSpawn("Item", booster_icon.GetChild(0).transform);
		}
		booster_check.gameObject.SetActive(value: false);
	}

	private void SetBoosterItem()
	{
		Transform transform = MWPoolManager.Spawn("Item", "Booster" + itemType, booster_icon);
		boosterData = GameDataManager.GetBoostItemData(itemType);
		if (boosterData.costType == -1)
		{
			booster_Free.gameObject.SetActive(value: true);
			booster_Jewel.gameObject.SetActive(value: false);
			SetVideoBoostItem();
		}
		else
		{
			booster_Free.gameObject.SetActive(value: false);
			booster_Jewel.gameObject.SetActive(value: true);
			booster_Cost.text = boosterData.costCount.ToString();
		}
		booster_Name.text = MWLocalize.GetData(boosterData.boosterName);
	}

	private void SetVideoBoostItem()
	{
		if (GamePreferenceManager.GetIsBoostRewardVideo())
		{
			SelectBoosterItem();
		}
	}

	private void SelectBoosterItem()
	{
		isSelected = true;
		booster_selected.gameObject.SetActive(value: true);
		booster_deselected.gameObject.SetActive(value: false);
		booster_check.gameObject.SetActive(value: true);
	}

	private void DeselectBoosterItem()
	{
		isSelected = false;
		booster_selected.gameObject.SetActive(value: false);
		booster_deselected.gameObject.SetActive(value: true);
		booster_check.gameObject.SetActive(value: false);
	}

	private void SelectRewardVideo()
	{
		StartCoroutine(AdsComplete_Delay());
	}

	private IEnumerator AdsComplete_Delay()
	{
		yield return new WaitForSeconds(0.2f);
		if (true)
		{
			Protocol_Set.Protocol_shop_ad_start_Req(7);
			GamePreferenceManager.SetIsBoostRewardVideo(isBoostRewardVideo: true);
			SelectBoosterItem();
		}
	}

	public void OnClickBooster()
	{
		if (boosterData.costType == -1)
		{
			if (!isSelected)
			{
				
			}
		}
		else if (isSelected)
		{
			GameDataManager.AddUserJewel(boosterData.costCount);
			DeselectBoosterItem();
		}
		else if (GameDataManager.UseUserJewel(boosterData.costCount))
		{
			SelectBoosterItem();
		}
		else
		{
			LobbyManager.ShowNotEnoughJewel(boosterData.costCount - GameInfo.userData.userInfo.jewel);
		}
	}
}
