using UnityEngine;
using UnityEngine.UI;

public class DailyBonusItem : MonoBehaviour
{
	public const string ITEM_DATA_TYPE_HUNTER = "hunter";

	public const string ITEM_DATA_TYPE_COIN = "coin";

	public const string ITEM_DATA_TYPE_JEWEL = "jewel";

	public const string ITEM_DATA_TYPE_TOKEN = "token";

	public const string ITEM_DATA_TYPE_BADGE = "badge";

	[SerializeField]
	private Text textDayCnt;

	[SerializeField]
	private Text textItemDescription;

	[SerializeField]
	private Transform trItemAnchor;

	[SerializeField]
	private GameObject goAcceptCheck;

	[SerializeField]
	private GameObject goAcriveItem;

	private int daySeven;

	private Transform trDailyItem;

	private USER_DAILY_BONUS_DATA itemData;

	public int Day
	{
		set
		{
			daySeven = value;
		}
	}

	public void Init(USER_DAILY_BONUS_DATA _data)
	{
		itemData = _data;
		ShowItem();
	}

	private void ShowItem()
	{
		if (itemData != null)
		{
			textDayCnt.text = $"{daySeven}";
			if (itemData.type == "hunter")
			{
				textItemDescription.text = MasterLocalize.GetData(itemData.bonusText);
			}
			else
			{
				textItemDescription.text = $"{itemData.sum}";
			}
			goAcriveItem.SetActive(itemData.accepted == "today");
			if ((bool)goAcceptCheck)
			{
				goAcceptCheck.SetActive(itemData.accepted == "y");
			}
			trDailyItem = MasterPoolManager.SpawnObject("Item", GetItemPrefabName(itemData.type), trItemAnchor);
		}
	}

	private string GetItemPrefabName(string _type)
	{
		string result = "DailyItem_Coin";
		switch (_type)
		{
		case "hunter":
			result = "DailyItem_ChestBox";
			break;
		case "badge":
			result = "DailyItem_Decoration";
			break;
		case "jewel":
			result = "DailyItem_Jewel";
			break;
		case "token":
			result = "DailyItem_Token";
			break;
		}
		return result;
	}

	private void OnDisable()
	{
		if (trDailyItem != null)
		{
			MasterPoolManager.ReturnToPool("Item", trDailyItem);
			trDailyItem = null;
		}
	}
}
