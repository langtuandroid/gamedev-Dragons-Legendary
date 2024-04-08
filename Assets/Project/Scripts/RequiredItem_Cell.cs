using UnityEngine;
using UnityEngine.UI;

public class RequiredItem_Cell : MonoBehaviour
{
	[SerializeField]
	private Transform Item_Img;

	[SerializeField]
	private Text costText;

	private int lackCoin;

	private ItemClickType clickType = ItemClickType.Item;

	private int itemIdx;

	public void SetItemImg(int _idx, Vector3 size = default(Vector3))
	{
		if (Item_Img != null)
		{
			MWPoolManager.DeSpawn("Item", Item_Img);
			Item_Img = null;
		}
		Item_Img = MWPoolManager.Spawn("Item", "Item_" + _idx, base.transform);
		Item_Img.SetAsFirstSibling();
		if (size == Vector3.zero)
		{
			Item_Img.localScale = Vector3.one;
		}
		else
		{
			Item_Img.localScale = size;
		}
		itemIdx = _idx;
		clickType = ItemClickType.Item;
	}

	public void SetCostText(string _str)
	{
		SetCostText_Setting(_str);
	}

	public void SetClickType(ItemClickType type, int _coin = 1)
	{
		UnityEngine.Debug.Log("SetClickType :: " + type);
		clickType = type;
		if (clickType == ItemClickType.Coin)
		{
			lackCoin = _coin;
		}
	}

	public void OnClickItemSortList()
	{
		UnityEngine.Debug.Log("OnClickItemSortList - " + clickType);
		switch (clickType)
		{
		case ItemClickType.Item:
			if (GameDataManager.GetItemListData(itemIdx).itemType == "Badge")
			{
				LobbyManager.MoveToBadgeFloor(itemIdx);
			}
			else
			{
				LobbyManager.ShowItemSortList(itemIdx);
			}
			break;
		case ItemClickType.Coin:
			LobbyManager.ShowNotEnoughCoin(lackCoin);
			break;
		}
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	private void SetCostText_Setting(string _str)
	{
		costText.text = _str;
	}
}
