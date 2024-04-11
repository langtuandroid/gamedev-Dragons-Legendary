using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestOpen : LobbyPopupBase
{
	public enum ChestOpenState
	{
		None = 1,
		Open
	}

	public Action GoBackEvent;

	private List<ChestListDbData> chestListDbData = new List<ChestListDbData>();

	private ChestType chestType = ChestType.Free;

	private ChestOpenState chestOpenState = ChestOpenState.None;

	private int currentIdx;

	private int total_boxCount;

	private int current_boxCount;

	private int totalIdx;

	private int remainOpenCount;

	[SerializeField]
	private Animator chestAnim;

	[SerializeField]
	private Transform hunterCard_tr;

	private HunterCard hunterCard;

	[SerializeField]
	private Text hunterName_Text;

	[SerializeField]
	private Transform itemImg_tr;

	[SerializeField]
	private Transform itemImg;

	[SerializeField]
	private Text itemCount_Text;

	[SerializeField]
	private Text itemName_Text;

	[SerializeField]
	private Text itemType_Text;

	[SerializeField]
	private Text itemProperty_Text;

	[SerializeField]
	private Text itemYouHave_Text;

	[SerializeField]
	private Transform convertCard_tr;

	[SerializeField]
	private HunterCard convertCard;

	[SerializeField]
	private Transform chestResult;

	[SerializeField]
	private Transform chestResult_tr;

	[SerializeField]
	private Transform newRibbon;

	[SerializeField]
	private ScrollRect scrollResult;

	public void Show(List<ChestListDbData> _chestList, ChestType _chestType)
	{
		UnityEngine.Debug.Log("ChestOpen count :: " + _chestList.Count);
		base.Show();
		base.gameObject.SetActive(value: true);
		chestListDbData = _chestList;
		chestType = _chestType;
		currentIdx = 0;
		current_boxCount = 1;
		if (chestListDbData.Count >= 25)
		{
			total_boxCount = 5;
		}
		else
		{
			total_boxCount = 1;
		}
		Init(_isFirst: true);
		switch (chestType)
		{
		case ChestType.Free:
			PlayAnim("Mysterious_Idle");
			break;
		case ChestType.Worn:
			PlayAnim("Key_Idle");
			break;
		case ChestType.Mysterious:
			PlayAnim("Mysterious_Idle");
			break;
		case ChestType.CoinChest:
			PlayAnim("Coin_Idle");
			break;
		}
		for (int i = 0; i < _chestList.Count; i++)
		{
			UnityEngine.Debug.Log("_chestList :: " + i + " :: " + _chestList[i].chestItem);
		}
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void HideProcessComplete()
	{
	}

	public void CardOpen_Anim_End()
	{
		UnityEngine.Debug.Log("this.currentIdx = " + currentIdx);
		currentIdx++;
		chestOpenState = ChestOpenState.None;
		if (remainOpenCount > 0)
		{
			return;
		}
		int childCount = chestResult_tr.childCount;
		for (int i = 0; i < childCount; i++)
		{
			MWPoolManager.DeSpawn("Item", chestResult_tr.GetChild(0));
		}
		if (total_boxCount == 1)
		{
			for (int j = 0; j < chestListDbData.Count; j++)
			{
				CardResult_Setting(chestListDbData[j]);
			}
		}
		else
		{
			for (int k = current_boxCount * 5 - 5; k < current_boxCount * 5; k++)
			{
				CardResult_Setting(chestListDbData[k]);
			}
		}
		StartCoroutine(ShowChestResult());
	}

	public IEnumerator ShowChestResult()
	{
		chestOpenState = ChestOpenState.Open;
		yield return new WaitForSeconds(0.5f);
		chestResult.gameObject.SetActive(value: true);
		scrollResult.horizontalNormalizedPosition = 0f;
		if (LobbyManager.OpenChestOpen != null)
		{
			LobbyManager.OpenChestOpen();
		}
		if (LobbyManager.OpenChestOpenEnchant != null)
		{
			LobbyManager.OpenChestOpenEnchant();
		}
	}

	public void SetRemainOpenCount(int _count)
	{
		remainOpenCount = _count;
	}

	private void Init(bool _isFirst)
	{
		SetRemainCount();
		chestOpenState = ChestOpenState.None;
		if (_isFirst)
		{
			_isFirst = false;
		}
	}

	private void SetRemainCount()
	{
		switch (chestType)
		{
		case ChestType.Free:
			totalIdx = GameDataManager.GetChestData()[3].pickTimes;
			break;
		case ChestType.Worn:
			totalIdx = GameDataManager.GetChestData()[2].pickTimes;
			break;
		case ChestType.Mysterious:
			totalIdx = GameDataManager.GetChestData()[3].pickTimes;
			break;
		case ChestType.CoinChest:
			totalIdx = GameDataManager.GetChestData()[8].pickTimes;
			break;
		}
		UnityEngine.Debug.Log("## totalIdx :: " + totalIdx);
		remainOpenCount = totalIdx;
	}

	private void SetAnim(int _idx)
	{
		switch (chestType)
		{
		case ChestType.Reward:
		case ChestType.QuickLoot:
		case (ChestType)6:
		case (ChestType)7:
			break;
		case ChestType.Free:
			SoundController.EffectSound_Play(EffectSoundType.FreeChestOpen);
			switch (GameDataManager.GetHunterData(chestListDbData[_idx].chestHunter).maxTier)
			{
			case 3:
				PlayAnim("Mysterious_3Star");
				break;
			case 4:
				PlayAnim("Mysterious_4Star");
				break;
			case 5:
				PlayAnim("Mysterious_5Star");
				break;
			}
			break;
		case ChestType.Worn:
			SoundController.EffectSound_Play(EffectSoundType.WornChestOpen);
			PlayAnim("Key_Open");
			break;
		case ChestType.CoinChest:
			SoundController.EffectSound_Play(EffectSoundType.MysteriousChestOpen);
			switch (GameDataManager.GetHunterData(chestListDbData[_idx].chestHunter).maxTier)
			{
			case 3:
				PlayAnim("Coin_3Star");
				break;
			case 4:
				PlayAnim("Coin_4Star");
				break;
			case 5:
				PlayAnim("Coin_5Star");
				break;
			}
			break;
		case ChestType.Mysterious:
			SoundController.EffectSound_Play(EffectSoundType.MysteriousChestOpen);
			switch (GameDataManager.GetHunterData(chestListDbData[_idx].chestHunter).maxTier)
			{
			case 3:
				PlayAnim("Mysterious_3Star");
				break;
			case 4:
				PlayAnim("Mysterious_4Star");
				break;
			case 5:
				PlayAnim("Mysterious_5Star");
				break;
			}
			break;
		}
	}

	private void CardOpen_Setting(int _idx)
	{
		switch (chestType)
		{
		case ChestType.Reward:
		case ChestType.QuickLoot:
		case (ChestType)6:
		case (ChestType)7:
			break;
		case ChestType.Free:
			if (GameUtil.GetHunterExist(chestListDbData[_idx].chestHunter))
			{
				newRibbon.gameObject.SetActive(value: false);
			}
			CardOpen_Setting_Hunter(_idx);
			break;
		case ChestType.Worn:
			CardOpen_Setting_Item(_idx);
			break;
		case ChestType.Mysterious:
		case ChestType.CoinChest:
			if (GameUtil.GetHunterExist(chestListDbData[_idx].chestHunter))
			{
				newRibbon.gameObject.SetActive(value: false);
			}
			CardOpen_Setting_Hunter(_idx);
			break;
		}
	}

	private void CardOpen_Setting_Hunter(int _idx)
	{
		if (hunterCard != null)
		{
			MWPoolManager.DeSpawn("Hunter", hunterCard.transform);
			hunterCard = null;
		}
		hunterCard = MWPoolManager.Spawn("Hunter", "HunterCard_" + chestListDbData[_idx].chestHunter, hunterCard_tr).GetComponent<HunterCard>();
		hunterCard.Init(HUNTERCARD_TYPE.CHESTOPEN, GameDataManager.GetHunterInfo(chestListDbData[_idx].chestHunter, chestListDbData[_idx].hunterLevel, chestListDbData[_idx].hunterTier), _isOwn: true, _isArena: false);
		hunterCard.HunterIdx = 0;
		hunterCard.transform.localPosition = Vector3.zero;
		hunterCard.transform.localScale = Vector3.one;
		UnityEngine.Debug.Log("Set HunterCard AnchoredPosition !!");
		hunterCard.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(175f, -145f);
		hunterCard.transform.SetSiblingIndex(1);
		hunterName_Text.text = MWLocalize.GetData(GameDataManager.GetHunterInfo(chestListDbData[_idx].chestHunter, 1, 1).Hunter.hunterName);
	}

	private void CardOpen_Setting_Item(int _idx)
	{
		if (itemImg != null)
		{
			MWPoolManager.DeSpawn("Item", itemImg);
			itemImg = null;
		}
		itemImg = MWPoolManager.Spawn("Item", "Item_" + chestListDbData[_idx].chestItem, itemImg_tr);
		itemImg.transform.localPosition = Vector3.zero;
		itemImg.transform.localScale = Vector3.one;
		itemImg.transform.SetAsFirstSibling();
		itemCount_Text.text = "x" + chestListDbData[_idx].chestItemN.ToString();
		itemName_Text.text = MWLocalize.GetData(GameDataManager.GetItemListData(chestListDbData[_idx].chestItem).itemName);
		itemType_Text.text = MWLocalize.GetData(GameDataManager.GetItemListData(chestListDbData[_idx].chestItem).itemType);
		itemYouHave_Text.text = MWLocalize.GetData("common_text_you_have") + GameInfo.userData.GetItemCount(chestListDbData[_idx].chestItem).ToString();
	}

	private void CardResult_Setting(ChestListDbData _data)
	{
		bool flag = false;
		for (int i = 0; i < chestResult_tr.childCount; i++)
		{
			if (chestResult_tr.GetChild(i).GetComponent<ChestResultItem>().ChestListDbData.chestItem == _data.chestItem && _data.chestItem != 0)
			{
				chestResult_tr.GetChild(i).GetComponent<ChestResultItem>().AddAmount(_data.chestItemN);
				flag = true;
			}
		}
		if (!flag)
		{
			ChestResultItem component = MWPoolManager.Spawn("Item", "Item", chestResult_tr).GetComponent<ChestResultItem>();
			component.Init(_data, new Vector3(1.2f, 1.2f, 1.2f));
			component.transform.localPosition = Vector3.zero;
			component.transform.localScale = Vector3.one;
		}
	}

	private void PlayAnim(string _anim)
	{
		chestAnim.ResetTrigger(_anim);
		chestAnim.SetTrigger(_anim);
	}

	public void Click_CardOpen()
	{
		UnityEngine.Debug.Log("CardOpen 11 !!");
		if (chestOpenState != ChestOpenState.Open && currentIdx < chestListDbData.Count)
		{
			UnityEngine.Debug.Log("CardOpen 22 !!");
			chestOpenState = ChestOpenState.Open;
			CardOpen_Setting(currentIdx);
			SetAnim(currentIdx);
			remainOpenCount--;
		}
	}

	public void Click_CardResult_OK_BT()
	{
		if (current_boxCount != total_boxCount)
		{
			UnityEngine.Debug.Log("this.current_boxCount 11 = " + current_boxCount);
			chestResult.gameObject.SetActive(value: false);
			current_boxCount++;
			Init(_isFirst: false);
			switch (chestType)
			{
			case ChestType.Free:
				PlayAnim("Mysterious_Idle");
				break;
			case ChestType.Worn:
				PlayAnim("Key_Idle");
				break;
			case ChestType.Mysterious:
				PlayAnim("Mysterious_Idle");
				break;
			case ChestType.CoinChest:
				PlayAnim("Coin_Idle");
				break;
			}
		}
		else
		{
			chestResult.gameObject.SetActive(value: false);
			OnClickGoBack();
		}
		if (LobbyManager.OpenChestOpenEnchant != null)
		{
			LobbyManager.OpenChestOpenEnchant();
		}
		if (LobbyManager.OpenChestResultDone != null)
		{
			LobbyManager.OpenChestResultDone();
		}
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void Click_Skip_BT()
	{
		chestOpenState = ChestOpenState.None;
		remainOpenCount = 0;
		if (remainOpenCount > 0)
		{
			return;
		}
		int childCount = chestResult_tr.childCount;
		for (int i = 0; i < childCount; i++)
		{
			MWPoolManager.DeSpawn("Item", chestResult_tr.GetChild(0));
		}
		if (total_boxCount == 1)
		{
			for (int j = 0; j < chestListDbData.Count; j++)
			{
				CardResult_Setting(chestListDbData[j]);
			}
		}
		else
		{
			currentIdx = current_boxCount * 5;
			UnityEngine.Debug.Log("this.current_boxCount 22 = " + current_boxCount);
			for (int k = current_boxCount * 5 - 5; k < current_boxCount * 5; k++)
			{
				CardResult_Setting(chestListDbData[k]);
			}
		}
		if (LobbyManager.OpenChestOpen != null)
		{
			LobbyManager.OpenChestOpen();
		}
		else
		{
			StartCoroutine(ShowChestResult());
		}
	}

	public void OnClickGoBack()
	{
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
	}
}
