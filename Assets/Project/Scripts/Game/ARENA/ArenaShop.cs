using System;
using UnityEngine;

public class ArenaShop : LobbyPopupBase
{
	public Action GoBackEvent;

	[SerializeField]
	private Transform arenaList_Tr;

	private ARENA_STORE_INFO[] store_Info;

	public void Show(ARENA_STORE_INFO[] _info)
	{
		base.Open();
		base.gameObject.SetActive(value: true);
		Init(_info);
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void CloseProcessComplete()
	{
	}

	public void SetInit()
	{
		RefreshArenaShop_Buy();
	}

	private void Init(ARENA_STORE_INFO[] _info)
	{
		store_Info = _info;
		SetArenaShopItem();
	}

	private void RefreshArenaShop()
	{
		SetArenaShopItem();
	}

	private void RefreshArenaShop_Buy()
	{
		SetArenaShopItem();
	}

	private void SetArenaShopItem()
	{
		int childCount = arenaList_Tr.childCount;
		for (int i = 0; i < childCount; i++)
		{
			MWPoolManager.DeSpawn("Lobby", arenaList_Tr.GetChild(0));
		}
		if (store_Info.Length > 0)
		{
			for (int j = 0; j < store_Info.Length; j++)
			{
				ArenaShopItem arenaShopItem = null;
				arenaShopItem = MWPoolManager.Spawn("Lobby", "ArenaShopItem", arenaList_Tr).GetComponent<ArenaShopItem>();
				arenaShopItem.transform.localPosition = Vector3.zero;
				arenaShopItem.transform.localScale = Vector3.one;
				arenaShopItem.Init(store_Info[j]);
			}
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

	public void ShowArenaShopBuyPopup_Wealth_Jewel(int _type)
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		LobbyManager.ShowValueShopBuy(_type, "jewel");
	}

	public void ShowArenaShopBuyPopup_Wealth_Cash(int _type)
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		LobbyManager.ShowValueShopBuy(_type, "cash");
	}
}
