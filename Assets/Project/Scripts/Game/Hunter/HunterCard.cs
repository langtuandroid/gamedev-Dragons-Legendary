using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HunterCard : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public Action<HunterCard> Select_HunterCard;

	public Action<HunterCard> DeSelect_HunterCard;

	[SerializeField]
	private HunterInfo hunterInfo;

	[SerializeField]
	private Transform Tier_Tr;

	[SerializeField]
	private GameObject selectEff;

	[SerializeField]
	private int hunterIdx;

	[SerializeField]
	private int hunterSibiling;

	[SerializeField]
	private bool isUseHunter;

	[SerializeField]
	private bool isSelectHunter;

	[SerializeField]
	private HUNTERCARD_TYPE hunterCard_type;

	[SerializeField]
	private Image lock_Img;

	[SerializeField]
	private bool isOwn;

	[SerializeField]
	private Text hunterLevel;

	[SerializeField]
	private Transform newHunter;

	[SerializeField]
	private Transform enchantHunter;

	[SerializeField]
	private Text enchantHunter_text;

	[SerializeField]
	private Transform noticeIcon;

	[SerializeField]
	private int arenaBuff;

	[SerializeField]
	private Text arenaBuff_text;

	[SerializeField]
	private Transform hunterFace;

	public HunterInfo HunterInfo
	{
		get
		{
			return hunterInfo;
		}
		set
		{
			hunterInfo = value;
		}
	}

	public int HunterIdx
	{
		get
		{
			return hunterIdx;
		}
		set
		{
			hunterIdx = value;
		}
	}

	public int HunterSibiling
	{
		get
		{
			return hunterSibiling;
		}
		set
		{
			hunterSibiling = value;
		}
	}

	public bool IsUseHunter
	{
		get
		{
			return isUseHunter;
		}
		set
		{
			isUseHunter = value;
		}
	}

	public bool IsSelectHunter
	{
		get
		{
			return isSelectHunter;
		}
		set
		{
			isSelectHunter = value;
		}
	}

	public void Init(HUNTERCARD_TYPE _type, HunterInfo _hunterInfo, bool _isOwn, bool _isArena)
	{
		hunterInfo = _hunterInfo;
		hunterCard_type = _type;
		hunterIdx = 0;
		arenaBuff = 1;
		isSelectHunter = false;
		isUseHunter = false;
		Select_HunterCard = null;
		DeSelect_HunterCard = null;
		isOwn = _isOwn;
		SetHunter_Card(_hunterInfo);
		if (GameDataManager.HasUserHunterNew(hunterInfo.Hunter.hunterIdx))
		{
			newHunter.gameObject.SetActive(value: true);
			enchantHunter.gameObject.SetActive(value: false);
		}
		else
		{
			newHunter.gameObject.SetActive(value: false);
			if (GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx) > 1)
			{
				enchantHunter.gameObject.SetActive(value: true);
				enchantHunter_text.text = "x " + GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx);
			}
			else
			{
				enchantHunter.gameObject.SetActive(value: false);
			}
		}
		if (_isArena && GameInfo.inGamePlayData.arenaInfo != null)
		{
			arenaBuff_text.gameObject.SetActive(value: true);
			if (GameInfo.inGamePlayData.arenaInfo.color == hunterInfo.Hunter.color)
			{
				arenaBuff *= GameInfo.inGamePlayData.arenaInfo.color_buff;
			}
			if (GameInfo.inGamePlayData.arenaInfo.tribe == hunterInfo.Hunter.hunterTribe)
			{
				arenaBuff *= GameInfo.inGamePlayData.arenaInfo.tribe_buff;
			}
			if (arenaBuff == 1)
			{
				arenaBuff_text.gameObject.SetActive(value: false);
			}
			else
			{
				arenaBuff_text.text = "x" + arenaBuff.ToString();
			}
		}
		else
		{
			arenaBuff_text.gameObject.SetActive(value: false);
		}
		hunterLevel.text = "Lv " + hunterInfo.Stat.hunterLevel.ToString();
	}

	public void OnPointerClick(PointerEventData pointerEventData)
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		switch (hunterCard_type)
		{
		case HUNTERCARD_TYPE.DECK:
			if (isSelectHunter)
			{
				isSelectHunter = false;
				if (DeSelect_HunterCard != null)
				{
					DeSelect_HunterCard(this);
				}
			}
			else
			{
				isSelectHunter = true;
				if (Select_HunterCard != null)
				{
					Select_HunterCard(this);
				}
			}
			break;
		case HUNTERCARD_TYPE.LEVELPLAY:
			LobbyManager.ShowHunterView(hunterInfo, _isSpawn: true);
			break;
		case HUNTERCARD_TYPE.HUNTERLIST:
			if (isOwn)
			{
				LobbyManager.ShowHunterView(hunterInfo, _isSpawn: true);
			}
			else
			{
				LobbyManager.ShowHunterView(hunterInfo, _isSpawn: true, _isOwn: false);
			}
			break;
		}
	}

	public void ChangedUseStatus(bool _isUsetoUse)
	{
		if (!_isUsetoUse)
		{
			if (isUseHunter)
			{
				isUseHunter = false;
			}
			else
			{
				isUseHunter = true;
			}
		}
		isSelectHunter = false;
	}

	public void SelectCard_Cancel()
	{
		isSelectHunter = false;
	}

	private void SetHunter_Card(HunterInfo _hunterInfo)
	{
		SetHunterFace();
		for (int i = 0; i < Tier_Tr.childCount; i++)
		{
			Tier_Tr.GetChild(i).GetChild(0).gameObject.SetActive(value: false);
			Tier_Tr.GetChild(i).gameObject.SetActive(value: false);
		}
		for (int j = 0; j < _hunterInfo.Hunter.maxTier; j++)
		{
			Tier_Tr.GetChild(j).gameObject.SetActive(value: true);
			if (_hunterInfo.Stat.hunterTier >= j + 1 && isOwn)
			{
				Tier_Tr.GetChild(j).GetChild(0).gameObject.SetActive(value: true);
			}
		}
		if (isOwn)
		{
			lock_Img.gameObject.SetActive(value: false);
			hunterLevel.gameObject.SetActive(value: true);
			SetHunterNotice();
		}
		else
		{
			lock_Img.gameObject.SetActive(value: true);
			hunterLevel.gameObject.SetActive(value: false);
		}
	}

	private void SetHunterNotice()
	{
		if (noticeIcon.childCount > 0)
		{
			noticeIcon.GetChild(0).localPosition = Vector3.zero;
			noticeIcon.GetChild(0).localScale = Vector3.one;
			MWPoolManager.DeSpawn("Lobby", noticeIcon.GetChild(0));
		}
		if ((hunterInfo.Stat.hunterTier < hunterInfo.Hunter.maxTier || hunterInfo.Stat.hunterLevel < hunterInfo.Stat.hunterTier * 20) && CheckHunterAlert())
		{
			Transform transform = MWPoolManager.Spawn("Lobby", "Notice_Green", noticeIcon);
			transform.localPosition = Vector3.zero;
			transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		}
	}

	public bool CheckHunterAlert()
	{
		bool flag = false;
		if (CheckHunterLevelUp())
		{
			flag = true;
		}
		if (!flag && CheckHunterPromotion())
		{
			flag = true;
		}
		UnityEngine.Debug.Log("************* isLevelup = " + flag);
		return flag;
	}

	private bool CheckHunterLevelUp()
	{
		bool result = false;
		HunterLevelDbData hunterLevelDbData = null;
		if (GameDataManager.GetHunterLevelData(hunterInfo.Stat.hunterIdx, hunterInfo.Stat.hunterLevel, hunterInfo.Stat.hunterTier) == null)
		{
			return false;
		}
		hunterLevelDbData = GameDataManager.GetHunterLevelData(hunterInfo.Stat.hunterIdx, hunterInfo.Stat.hunterLevel, hunterInfo.Stat.hunterTier);
		if (CheckEneoughLevelUpItem(hunterLevelDbData.hnil, hunterLevelDbData.hnil_N) && hunterLevelDbData.needCoin <= GameInfo.userData.userInfo.coin && hunterInfo.Stat.hunterTier * 20 > hunterInfo.Stat.hunterLevel)
		{
			result = true;
		}
		return result;
	}

	private bool CheckHunterPromotion()
	{
		bool result = false;
		HunterPromotionDbData hunterPromotionDbData = null;
		if (GameDataManager.GetHunterPromotionData(hunterInfo.Hunter.color, hunterInfo.Hunter.maxTier, hunterInfo.Stat.hunterTier) == null)
		{
			return false;
		}
		hunterPromotionDbData = GameDataManager.GetHunterPromotionData(hunterInfo.Hunter.color, hunterInfo.Hunter.maxTier, hunterInfo.Stat.hunterTier);
		if (CheckEneoughPromotionItem(hunterPromotionDbData.hnip1, hunterPromotionDbData.hnip1_N) && hunterPromotionDbData.needCoin <= GameInfo.userData.userInfo.coin && hunterInfo.Stat.hunterTier * 20 == hunterInfo.Stat.hunterLevel && CheckEneoughPromotionItem(hunterPromotionDbData.hnip2, hunterPromotionDbData.hnip2_N) && CheckEneoughPromotionItem(hunterPromotionDbData.hnip3, hunterPromotionDbData.hnip3_N) && CheckEneoughPromotionItem(hunterPromotionDbData.hnip4, hunterPromotionDbData.hnip4_N))
		{
			result = true;
		}
		return result;
	}

	private bool CheckEneoughLevelUpItem(int _itemIdx, int _itemCount)
	{
		bool result = false;
		if (_itemCount <= GameInfo.userData.GetItemCount(_itemIdx))
		{
			result = true;
		}
		return result;
	}

	private bool CheckEneoughPromotionItem(int _itemIdx, int _itemCount)
	{
		bool result = false;
		if (_itemIdx == 0)
		{
			result = true;
		}
		else if (_itemCount <= GameInfo.userData.GetItemCount(_itemIdx))
		{
			result = true;
		}
		return result;
	}

	private void SetHunterFace()
	{
		for (int i = 0; i < hunterFace.childCount; i++)
		{
			hunterFace.GetChild(i).gameObject.SetActive(value: false);
		}
		switch (hunterInfo.Stat.hunterTier)
		{
		case 1:
			hunterFace.GetChild(int.Parse(hunterInfo.Hunter.hunterImg1) - 1).gameObject.SetActive(value: true);
			break;
		case 2:
			hunterFace.GetChild(int.Parse(hunterInfo.Hunter.hunterImg2) - 1).gameObject.SetActive(value: true);
			break;
		case 3:
			hunterFace.GetChild(int.Parse(hunterInfo.Hunter.hunterImg3) - 1).gameObject.SetActive(value: true);
			break;
		case 4:
			hunterFace.GetChild(int.Parse(hunterInfo.Hunter.hunterImg4) - 1).gameObject.SetActive(value: true);
			break;
		case 5:
			hunterFace.GetChild(int.Parse(hunterInfo.Hunter.hunterImg5) - 1).gameObject.SetActive(value: true);
			break;
		}
	}
}
