using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest : LobbyPopupBase
{
	public enum FreeChestState
	{
		Nothing = -1,
		Progress,
		Complete
	}

	public Action GoBackEvent;

	[SerializeField]
	private Text textProduceTime;

	[SerializeField]
	private Text textWornChest_bt;

	[SerializeField]
	private Text textCoinChest_bt;

	[SerializeField]
	private Text textMysteriousChest_btx1;

	[SerializeField]
	private Transform trFreeChestButton;

	[SerializeField]
	private Transform trMysteriousChestButton;

	[SerializeField]
	private Transform timer_tr;

	[SerializeField]
	private Transform free_collect_BT_Img;

	[SerializeField]
	private Transform boxProbability;

	[SerializeField]
	private List<ChestListDbData> chestListDbData = new List<ChestListDbData>();

	private FreeChestState freeChestState;

	public Transform FreeChestButton => trFreeChestButton;

	public Transform MysteriousChestButton => trMysteriousChestButton;

	public override void Show()
	{
		base.Show();
		base.gameObject.SetActive(value: true);
		Init();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void HideProcessComplete()
	{
	}

	public void Init()
	{
		GameUtil.SetUseHunterList();
		GameUtil.SetOwnHunterList(HUNTERLIST_TYPE.NORMAL);
		if (chestListDbData != null)
		{
			chestListDbData.Clear();
		}
		LocalTimeCheckManager.OnTimeTick = (Action<string, float>)Delegate.Combine(LocalTimeCheckManager.OnTimeTick, new Action<string, float>(OnTimeTickEvent));
		LocalTimeCheckManager.OnLocalTimeComplete = (Action<string>)Delegate.Combine(LocalTimeCheckManager.OnLocalTimeComplete, new Action<string>(OnLocalTimeComplete));
		Init_WornChest();
		Init_CoinChest();
		Init_MysteriousChest();
	}

	private void Init_WornChest()
	{
		if (GameInfo.userData.userInfo.chestKey >= GameDataManager.GetChestData()[2].needItemNx1)
		{
			textWornChest_bt.text = "<color=#ffffff>" + GameInfo.userData.userInfo.chestKey + "/" + GameDataManager.GetChestData()[2].needItemNx1 + "</color>";
		}
		else
		{
			textWornChest_bt.text = "<color=#ff0000>" + GameInfo.userData.userInfo.chestKey + "/" + GameDataManager.GetChestData()[2].needItemNx1 + "</color>";
		}
	}

	private void Init_CoinChest()
	{
		if (GameInfo.userData.userInfo.coin >= GameDataManager.GetChestData()[8].needItemNx1)
		{
			textCoinChest_bt.text = $"<color=#ffffff>{GameDataManager.GetChestData()[8].needItemNx1:#,###}</color>";
		}
		else
		{
			textCoinChest_bt.text = $"<color=#ff0000>{GameDataManager.GetChestData()[8].needItemNx1:#,###}</color>";
		}
	}

	private void Init_MysteriousChest()
	{
		switch (LocalTimeCheckManager.GetTimeState("mysteriousChestOpenTime"))
		{
		case TimeCheckState.Nothing:
			freeChestState = FreeChestState.Nothing;
			UnityEngine.Debug.Log("Free Chest State = Nothing");
			break;
		case TimeCheckState.Progress:
			freeChestState = FreeChestState.Progress;
			UnityEngine.Debug.Log("Free Chest State = Progress");
			break;
		case TimeCheckState.Complete:
			freeChestState = FreeChestState.Complete;
			UnityEngine.Debug.Log("Free Chest State = Complete");
			break;
		}
		RefreshFreeChestState();
		if (GameInfo.userData.userInfo.jewel >= GameDataManager.GetChestData()[3].needItemNx1)
		{
			textMysteriousChest_btx1.text = "<color=#ffffff>" + GameDataManager.GetChestData()[3].needItemNx1 + "</color>";
		}
		else
		{
			textMysteriousChest_btx1.text = "<color=#ff0000>" + GameDataManager.GetChestData()[3].needItemNx1 + "</color>";
		}
	}

	private void Open_Chest_Free(ChestListDbData[] _item)
	{
		LobbyManager.CallUserData(Refresh_Chest);
		GetChestItemList(ChestType.Free, _item);
	}

	private void Open_Chest_Worn(ChestListDbData[] _item)
	{
		if (GameInfo.userData.userStageState[0].chapterList.Length < 3)
		{
			for (int i = 0; i < _item.Length; i++)
			{
				if (_item[i].chestHunter == 20025)
				{
					GamePreferenceManager.SetIsGetHunter20025(isGet: true);
				}
			}
		}
		LobbyManager.CallUserData(Refresh_Chest);
		GetChestItemList(ChestType.Worn, _item);
	}

	private void Open_Chest_Mysterious(ChestListDbData[] _item)
	{
		if (GameInfo.userData.userStageState[0].chapterList.Length < 3)
		{
			for (int i = 0; i < _item.Length; i++)
			{
				if (_item[i].chestHunter == 20025)
				{
					GamePreferenceManager.SetIsGetHunter20025(isGet: true);
				}
			}
		}
		else if (GameInfo.userData.userStageState[0].chapterList.Length >= 3 && GamePreferenceManager.GetIsGetHunter20025() && LobbyManager.JewelChestOpen != null)
		{
			Protocol_Set.Protocol_hunter_change_Req(new int[5]
			{
				20001,
				20007,
				20011,
				20019,
				20025
			}, HUNTERLIST_TYPE.NORMAL, null);
			GamePreferenceManager.SetIsGetHunter20025(isGet: false);
		}
		if (LobbyManager.OpenChestOpenResult != null)
		{
			TutorialManager.SaveTutorial(5, 1);
		}
		if (LobbyManager.OpenChestOpenEnchant != null)
		{
			TutorialManager.SaveTutorial(13, 1);
		}
		LobbyManager.CallUserData(Refresh_Chest);
		GetChestItemList(ChestType.Mysterious, _item);
		if (LobbyManager.JewelChestOpen != null)
		{
			LobbyManager.JewelChestOpen();
		}
	}

	private void Open_Chest_Coin(ChestListDbData[] _item)
	{
		if (GameInfo.userData.userStageState[0].chapterList.Length < 3)
		{
			for (int i = 0; i < _item.Length; i++)
			{
				if (_item[i].chestHunter == 20025)
				{
					GamePreferenceManager.SetIsGetHunter20025(isGet: true);
				}
			}
		}
		LobbyManager.CallUserData(Refresh_Chest);
		GetChestItemList(ChestType.CoinChest, _item);
	}

	private void Refresh_Chest()
	{
		Init_WornChest();
		Init_CoinChest();
		Init_MysteriousChest();
	}

	private void GetChestItemList(ChestType _type, ChestListDbData[] _item)
	{
		bool flag = false;
		if (chestListDbData != null)
		{
			chestListDbData.Clear();
		}
		for (int i = 0; i < _item.Length; i++)
		{
			chestListDbData.Add(_item[i]);
		}
		LobbyManager.ShowChestOpen(chestListDbData, _type);
		RefreshFreeChestState();
	}

	private void RefreshFreeChestState()
	{
		switch (freeChestState)
		{
		case FreeChestState.Nothing:
			UnityEngine.Debug.Log("Free Chest State = Nothing");
			free_collect_BT_Img.gameObject.SetActive(value: true);
			timer_tr.gameObject.SetActive(value: false);
			break;
		case FreeChestState.Progress:
			UnityEngine.Debug.Log("Free Chest State = Progress");
			free_collect_BT_Img.gameObject.SetActive(value: false);
			timer_tr.gameObject.SetActive(value: true);
			break;
		case FreeChestState.Complete:
			UnityEngine.Debug.Log("Free Chest State = Complete");
			free_collect_BT_Img.gameObject.SetActive(value: true);
			timer_tr.gameObject.SetActive(value: false);
			break;
		}
	}

	private void OnTimeTickEvent(string type, float second)
	{
		if (type == "mysteriousChestOpenTime")
		{
			float num = Mathf.Floor(second);
			TimeSpan timeSpan = TimeSpan.FromSeconds(num);
			string text = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
			textProduceTime.text = text;
		}
	}

	private void OnLocalTimeComplete(string type)
	{
		if (type == "mysteriousChestOpenTime")
		{
			freeChestState = FreeChestState.Complete;
			RefreshFreeChestState();
		}
	}

	private void OnCallBuyCoinEvent(int _needJewel)
	{
		Protocol_Set.Protocol_chest_popup_buy_coin_Req(_needJewel, OnBuyCoinComplete);
	}

	private void OnBuyCoinComplete()
	{
		Refresh_Chest();
	}

	public void OnClickOpen_FreeChest()
	{
		if (freeChestState != 0)
		{
			freeChestState = FreeChestState.Progress;
			Protocol_Set.Protocol_chest_collect_Req(1, Open_Chest_Free);
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		}
	}

	public void OnClickOpen_WornChest()
	{
		if (GameInfo.userData.userInfo.chestKey >= GameDataManager.GetChestData()[2].needItemNx1)
		{
			Protocol_Set.Protocol_chest_collect_Req(2, Open_Chest_Worn);
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		}
	}

	public void OnClickOpen_CoinChest()
	{
		if (GameInfo.userData.userInfo.coin < GameDataManager.GetChestData()[8].needItemNx1)
		{
			NotEnouchCoin.CallBuyCoin = OnCallBuyCoinEvent;
			LobbyManager.ShowNotEnoughCoin(GameDataManager.GetChestData()[8].needItemNx1 - GameInfo.userData.userInfo.coin);
		}
		else
		{
			Protocol_Set.Protocol_chest_collect_Req(8, Open_Chest_Coin);
		}
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickOpen_MysteriousChestx1()
	{
		UnityEngine.Debug.Log("ENCHANT TUTORIAL 22");
		if (freeChestState == FreeChestState.Progress)
		{
			if (GameInfo.userData.userInfo.jewel < GameDataManager.GetChestData()[3].needItemNx1 && LobbyManager.JewelChestOpen == null && LobbyManager.OpenChestOpenEnchant == null && LobbyManager.OpenChestOpenResult == null)
			{
				LobbyManager.ShowNotEnoughJewel(GameDataManager.GetChestData()[3].needItemNx1 - GameInfo.userData.userInfo.jewel);
				return;
			}
			if (LobbyManager.JewelChestOpen == null && LobbyManager.OpenChestOpenEnchant == null && LobbyManager.OpenChestOpenResult == null)
			{
				Protocol_Set.Protocol_chest_collect_Req(3, Open_Chest_Mysterious);
			}
			else if (LobbyManager.OpenChestOpenResult != null)
			{
				Protocol_Set.Protocol_chest_collect_Req(3, Open_Chest_Mysterious, "y", isTutorial: true, 4);
			}
			else if (LobbyManager.OpenChestOpenEnchant != null)
			{
				Protocol_Set.Protocol_chest_collect_Req(3, Open_Chest_Mysterious, "n", isTutorial: true, 13);
			}
			else
			{
				Protocol_Set.Protocol_chest_collect_Req(3, Open_Chest_Mysterious, "n", isTutorial: true, 11);
			}
		}
		else
		{
			freeChestState = FreeChestState.Progress;
			if (LobbyManager.JewelChestOpen == null && LobbyManager.OpenChestOpenEnchant == null && LobbyManager.OpenChestOpenResult == null)
			{
				Protocol_Set.Protocol_chest_collect_Req(3, Open_Chest_Mysterious, "y");
			}
			else if (LobbyManager.OpenChestOpenResult != null)
			{
				Protocol_Set.Protocol_chest_collect_Req(3, Open_Chest_Mysterious, "y", isTutorial: true, 4);
			}
			else if (LobbyManager.OpenChestOpenEnchant != null)
			{
				Protocol_Set.Protocol_chest_collect_Req(3, Open_Chest_Mysterious, "n", isTutorial: true, 13);
			}
			else
			{
				Protocol_Set.Protocol_chest_collect_Req(3, Open_Chest_Mysterious, "n", isTutorial: true, 11);
			}
		}
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickGoBack()
	{
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		if (LobbyManager.OpenChestOpenEnchant != null)
		{
			LobbyManager.OpenChestOpenEnchant();
		}
	}

	public void ClickBoxProbability()
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		boxProbability.gameObject.SetActive(value: true);
	}

	public void CloseBoxProbability()
	{
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
		boxProbability.gameObject.SetActive(value: false);
	}

	private void OnDisable()
	{
		NotEnouchCoin.CallBuyCoin = null;
		LocalTimeCheckManager.OnTimeTick = (Action<string, float>)Delegate.Remove(LocalTimeCheckManager.OnTimeTick, new Action<string, float>(OnTimeTickEvent));
		LocalTimeCheckManager.OnLocalTimeComplete = (Action<string>)Delegate.Remove(LocalTimeCheckManager.OnLocalTimeComplete, new Action<string>(OnLocalTimeComplete));
	}
}
