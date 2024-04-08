using UnityEngine;
using UnityEngine.UI;

public class ArenaShopItem : MonoBehaviour
{
	[SerializeField]
	private ARENA_STORE_INFO data;

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
	private Text item_Amount;

	public void Init(ARENA_STORE_INFO _data)
	{
		data = _data;
		if (item_Img != null)
		{
			MWPoolManager.DeSpawn("Item", item_Img);
		}
		soldOut.gameObject.SetActive(value: false);
		item_Price.gameObject.SetActive(value: true);
		item_Icon.gameObject.SetActive(value: true);
		if (data.chestHunter > 0)
		{
			item_Img = MWPoolManager.Spawn("Item", "Item_" + data.chestHunter, item_Img_tr);
		}
		else
		{
			item_Img = MWPoolManager.Spawn("Item", "Item_" + data.itemIdx, item_Img_tr);
		}
		if (data.soldOutYn.Equals("n"))
		{
			item_Price.text = GameUtil.InsertCommaInt(data.costArenaPoint);
		}
		else
		{
			soldOut.gameObject.SetActive(value: true);
			item_Price.gameObject.SetActive(value: false);
			item_Icon.gameObject.SetActive(value: false);
			item_Price.text = GameUtil.InsertCommaInt(data.costArenaPoint);
		}
		item_Name.text = MWLocalize.GetData(data.itemName);
		item_Amount.text = "x" + GameUtil.InsertCommaInt(data.itemN);
	}

	public void ShowArenaShopBuyPopup_Item()
	{
		if (!data.soldOutYn.Equals("y"))
		{
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
			LobbyManager.ShowArenaShopBuy(data);
		}
	}
}
