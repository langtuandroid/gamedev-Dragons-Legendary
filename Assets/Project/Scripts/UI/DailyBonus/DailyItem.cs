using UnityEngine;
using UnityEngine.UI;

public class DailyItem : MonoBehaviour
{
	[SerializeField]
	private ShopDailyDbData data;

	[SerializeField]
	private Transform item_Img;

	[SerializeField]
	private Transform item_Img_tr;

	[SerializeField]
	private Transform soldOut;

	[SerializeField]
	private Text item_Name;

	[SerializeField]
	private Transform item_Icon;

	[SerializeField]
	private Text item_Price;

	[SerializeField]
	private Text item_Left;

	[SerializeField]
	private Text item_Owned;

	[SerializeField]
	private Text item_Type;

	[SerializeField]
	private ValueShop valueShop;

	public void Init(ShopDailyDbData _data, ValueShop _valueShop)
	{
		data = _data;
		if (item_Img != null)
		{
			MasterPoolManager.ReturnToPool("Item", item_Img);
		}
		soldOut.gameObject.SetActive(value: false);
		item_Price.gameObject.SetActive(value: true);
		item_Icon.gameObject.SetActive(value: true);
		item_Img = MasterPoolManager.SpawnObject("Item", "Item_" + data.itemIdx, item_Img_tr);
		item_Name.text = MasterLocalize.GetData(data.itemName);
		if (data.left < 0)
		{
			data.left = 0;
		}
		item_Left.text = string.Format(MasterLocalize.GetData("shop_daily_text_3"), data.left.ToString());
		if (data.left > 0)
		{
			item_Price.text = GameUtil.InsertCommaInt(data.resultPrice);
		}
		else
		{
			soldOut.gameObject.SetActive(value: true);
			item_Price.gameObject.SetActive(value: false);
			item_Icon.gameObject.SetActive(value: false);
		}
		item_Owned.text = string.Format(MasterLocalize.GetData("common_text_owned"), GameInfo.userData.GetItemCount(data.itemIdx).ToString());
		item_Type.text = MasterLocalize.GetData(GameDataManager.GetItemListData(data.itemIdx).itemType);
		valueShop = _valueShop;
		valueShop.valueShopState = ValueShopType.Daily;
	}

	public void ShowValueShopBuyPopup_Item()
	{
		if (data.left > 0)
		{
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
			LobbyManager.ShowValueShopBuy(data.productIdx, "item");
		}
	}
}
