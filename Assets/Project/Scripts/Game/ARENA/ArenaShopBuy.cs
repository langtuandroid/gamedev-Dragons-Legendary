using System;
using UnityEngine;
using UnityEngine.UI;

public class ArenaShopBuy : LobbyPopupBase
{
	public Action GoBackEvent;

	[SerializeField]
	private Transform item_Img;

	[SerializeField]
	private Transform item_Img_Tr;

	[SerializeField]
	private Text item_Name;

	[SerializeField]
	private Text item_Owned;

	[SerializeField]
	private Text item_Price;

	private int total_Price;

	private bool isbuyCondition;

	[SerializeField]
	private ARENA_STORE_INFO key;

	public void Show(ARENA_STORE_INFO _key)
	{
		base.Open();
		base.gameObject.SetActive(value: true);
		Init(_key);
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void CloseProcessComplete()
	{
	}

	public void Init(ARENA_STORE_INFO _key)
	{
		key = _key;
		SetItemForm();
	}

	private void SetItemForm()
	{
		if (item_Img != null)
		{
			MWPoolManager.DeSpawn("Item", item_Img);
			item_Img = null;
		}
		if (key.chestHunter > 0)
		{
			item_Img = MWPoolManager.Spawn("Item", "Item_" + key.chestHunter, item_Img_Tr);
		}
		else
		{
			item_Img = MWPoolManager.Spawn("Item", "Item_" + key.itemIdx, item_Img_Tr);
		}
		item_Name.text = MWLocalize.GetData(key.itemName);
		item_Owned.gameObject.SetActive(value: true);
		if (key.chestHunter > 0)
		{
			item_Owned.gameObject.SetActive(value: false);
		}
		else
		{
			item_Owned.text = string.Format(MWLocalize.GetData("common_text_owned"), GameInfo.userData.GetItemCount(key.itemIdx).ToString());
		}
		total_Price = key.costArenaPoint;
		Check_Coin_Enough();
	}

	private void Check_Coin_Enough()
	{
		if (total_Price <= GameInfo.userData.userInfo.arenaPoint)
		{
			isbuyCondition = true;
			item_Price.text = total_Price.ToString();
		}
		else
		{
			isbuyCondition = false;
			item_Price.text = total_Price.ToString();
		}
	}

	private void BuyArenaItemResponse()
	{
		key.soldOutYn = "y";
		LobbyManager.ShowArenaShop_Refresh();
		OnClickGoBack();
		SoundController.EffectSound_Play(EffectSoundType.UseCoin);
	}

	public void Click_Arena_Item()
	{
		if (isbuyCondition)
		{
			Protocol_Set.Protocol_arena_store_buy_product_Req(key.productIdx, BuyArenaItemResponse);
		}
	}

	public void OnClickGoBack()
	{
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
	}
}
