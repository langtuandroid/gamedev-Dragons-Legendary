using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HeroCard : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public Action<HeroCard> OnSelect;
	public Action<HeroCard> OnDeselect;

	[FormerlySerializedAs("Tier_Tr")] [SerializeField]
	private Transform _tierTr;
	
	private GameObject _selectEffect;
	private int _heroSibiling;
	private bool _isSelectHero;
	private int _arenaBuff;
	private bool _isOwn;
	
	[FormerlySerializedAs("hunterCard_type")] [SerializeField]
	private HerocardType _heroType;

	[FormerlySerializedAs("lock_Img")] [SerializeField]
	private Image _lockImage;
	
	[FormerlySerializedAs("hunterLevel")] [SerializeField]
	private Text _heroLevelText;

	[FormerlySerializedAs("newHunter")] [SerializeField]
	private Transform _newHero;

	[FormerlySerializedAs("_enchantedHanter")] [FormerlySerializedAs("enchantHunter")] [SerializeField]
	private Transform _enchantedHunter;

	[FormerlySerializedAs("enchantHunter_text")] [SerializeField]
	private Text _enchantedText;

	[FormerlySerializedAs("noticeIcon")] [SerializeField]
	private Transform _noticeIcon;

	[FormerlySerializedAs("arenaBuff_text")] [SerializeField]
	private Text _arenaBuffText;

	[FormerlySerializedAs("hunterFace")] [SerializeField]
	private Transform _hunterFace;

	public HunterInfo HunterInfo { get; set; }

	public int HeroIdx { get; set; }

	public bool IsUseHero { get; set; }

	public void Construct(HerocardType _type, HunterInfo _hunterInfo, bool _isOwn, bool _isArena)
	{
		this.HunterInfo = _hunterInfo;
		_heroType = _type;
		HeroIdx = 0;
		_arenaBuff = 1;
		_isSelectHero = false;
		IsUseHero = false;
		OnSelect = null;
		OnDeselect = null;
		this._isOwn = _isOwn;
		SetHunterCard(_hunterInfo);
		if (GameDataManager.HasUserHunterNew(this.HunterInfo.Hunter.hunterIdx))
		{
			_newHero.gameObject.SetActive(value: true);
			_enchantedHunter.gameObject.SetActive(value: false);
		}
		else
		{
			_newHero.gameObject.SetActive(value: false);
			if (GameDataManager.HasUserHunterEnchant(this.HunterInfo.Hunter.hunterIdx) > 1)
			{
				_enchantedHunter.gameObject.SetActive(value: true);
				_enchantedText.text = "x " + GameDataManager.HasUserHunterEnchant(this.HunterInfo.Hunter.hunterIdx);
			}
			else
			{
				_enchantedHunter.gameObject.SetActive(value: false);
			}
		}
		if (_isArena && GameInfo.inGamePlayData.arenaInfo != null)
		{
			_arenaBuffText.gameObject.SetActive(value: true);
			if (GameInfo.inGamePlayData.arenaInfo.color == this.HunterInfo.Hunter.color)
			{
				_arenaBuff *= GameInfo.inGamePlayData.arenaInfo.color_buff;
			}
			if (GameInfo.inGamePlayData.arenaInfo.tribe == this.HunterInfo.Hunter.hunterTribe)
			{
				_arenaBuff *= GameInfo.inGamePlayData.arenaInfo.tribe_buff;
			}
			if (_arenaBuff == 1)
			{
				_arenaBuffText.gameObject.SetActive(value: false);
			}
			else
			{
				_arenaBuffText.text = "x" + _arenaBuff.ToString();
			}
		}
		else
		{
			_arenaBuffText.gameObject.SetActive(value: false);
		}
		_heroLevelText.text = "Lv " + this.HunterInfo.Stat.hunterLevel.ToString();
	}

	public void OnPointerClick(PointerEventData pointerEventData)
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		switch (_heroType)
		{
		case HerocardType.Deck:
			if (_isSelectHero)
			{
				_isSelectHero = false;
				if (OnDeselect != null)
				{
					OnDeselect(this);
				}
			}
			else
			{
				_isSelectHero = true;
				if (OnSelect != null)
				{
					OnSelect(this);
				}
			}
			break;
		case HerocardType.Levelplay:
			LobbyManager.ShowHunterView(HunterInfo, _isSpawn: true);
			break;
		case HerocardType.Hunterlist:
			if (_isOwn)
			{
				LobbyManager.ShowHunterView(HunterInfo, _isSpawn: true);
			}
			else
			{
				LobbyManager.ShowHunterView(HunterInfo, _isSpawn: true, _isOwn: false);
			}
			break;
		}
	}

	public void ChangeStatus(bool _isUsetoUse)
	{
		if (!_isUsetoUse)
		{
			if (IsUseHero)
			{
				IsUseHero = false;
			}
			else
			{
				IsUseHero = true;
			}
		}
		_isSelectHero = false;
	}

	public void CancelSelection()
	{
		_isSelectHero = false;
	}

	private void SetHunterCard(HunterInfo _hunterInfo)
	{
		ChangeFace();
		for (int i = 0; i < _tierTr.childCount; i++)
		{
			_tierTr.GetChild(i).GetChild(0).gameObject.SetActive(value: false);
			_tierTr.GetChild(i).gameObject.SetActive(value: false);
		}
		for (int j = 0; j < _hunterInfo.Hunter.maxTier; j++)
		{
			_tierTr.GetChild(j).gameObject.SetActive(value: true);
			if (_hunterInfo.Stat.hunterTier >= j + 1 && _isOwn)
			{
				_tierTr.GetChild(j).GetChild(0).gameObject.SetActive(value: true);
			}
		}
		if (_isOwn)
		{
			_lockImage.gameObject.SetActive(value: false);
			_heroLevelText.gameObject.SetActive(value: true);
			SetNotice();
		}
		else
		{
			_lockImage.gameObject.SetActive(value: true);
			_heroLevelText.gameObject.SetActive(value: false);
		}
	}

	private void SetNotice()
	{
		if (_noticeIcon.childCount > 0)
		{
			_noticeIcon.GetChild(0).localPosition = Vector3.zero;
			_noticeIcon.GetChild(0).localScale = Vector3.one;
			MasterPoolManager.ReturnToPool("Lobby", _noticeIcon.GetChild(0));
		}
		if ((HunterInfo.Stat.hunterTier < HunterInfo.Hunter.maxTier || HunterInfo.Stat.hunterLevel < HunterInfo.Stat.hunterTier * 20) && CheckAlert())
		{
			Transform transform = MasterPoolManager.SpawnObject("Lobby", "Notice_Green", _noticeIcon);
			transform.localPosition = Vector3.zero;
			transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		}
	}

	private bool CheckAlert()
	{
		bool flag = false;
		if (CheckIsLevelUp())
		{
			flag = true;
		}
		if (!flag && CheckPromotion())
		{
			flag = true;
		}
		UnityEngine.Debug.Log("************* isLevelup = " + flag);
		return flag;
	}

	private bool CheckIsLevelUp()
	{
		bool result = false;
		HunterLevelDbData hunterLevelDbData = null;
		if (GameDataManager.GetHunterLevelData(HunterInfo.Stat.hunterIdx, HunterInfo.Stat.hunterLevel, HunterInfo.Stat.hunterTier) == null)
		{
			return false;
		}
		hunterLevelDbData = GameDataManager.GetHunterLevelData(HunterInfo.Stat.hunterIdx, HunterInfo.Stat.hunterLevel, HunterInfo.Stat.hunterTier);
		if (IsEnoughToUpgrade(hunterLevelDbData.hnil, hunterLevelDbData.hnil_N) && hunterLevelDbData.needCoin <= GameInfo.userData.userInfo.coin && HunterInfo.Stat.hunterTier * 20 > HunterInfo.Stat.hunterLevel)
		{
			result = true;
		}
		return result;
	}

	private bool CheckPromotion()
	{
		bool result = false;
		HunterPromotionDbData hunterPromotionDbData = null;
		if (GameDataManager.GetHunterPromotionData(HunterInfo.Hunter.color, HunterInfo.Hunter.maxTier, HunterInfo.Stat.hunterTier) == null)
		{
			return false;
		}
		hunterPromotionDbData = GameDataManager.GetHunterPromotionData(HunterInfo.Hunter.color, HunterInfo.Hunter.maxTier, HunterInfo.Stat.hunterTier);
		if (IsEnouhgToPromotion(hunterPromotionDbData.hnip1, hunterPromotionDbData.hnip1_N) && hunterPromotionDbData.needCoin <= GameInfo.userData.userInfo.coin && HunterInfo.Stat.hunterTier * 20 == HunterInfo.Stat.hunterLevel && IsEnouhgToPromotion(hunterPromotionDbData.hnip2, hunterPromotionDbData.hnip2_N) && IsEnouhgToPromotion(hunterPromotionDbData.hnip3, hunterPromotionDbData.hnip3_N) && IsEnouhgToPromotion(hunterPromotionDbData.hnip4, hunterPromotionDbData.hnip4_N))
		{
			result = true;
		}
		return result;
	}

	private bool IsEnoughToUpgrade(int _itemIdx, int _itemCount)
	{
		bool result = false;
		if (_itemCount <= GameInfo.userData.GetItemCount(_itemIdx))
		{
			result = true;
		}
		return result;
	}

	private bool IsEnouhgToPromotion(int _itemIdx, int _itemCount)
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

	private void ChangeFace()
	{
		for (int i = 0; i < _hunterFace.childCount; i++)
		{
			_hunterFace.GetChild(i).gameObject.SetActive(value: false);
		}
		switch (HunterInfo.Stat.hunterTier)
		{
		case 1:
			_hunterFace.GetChild(int.Parse(HunterInfo.Hunter.hunterImg1) - 1).gameObject.SetActive(value: true);
			break;
		case 2:
			_hunterFace.GetChild(int.Parse(HunterInfo.Hunter.hunterImg2) - 1).gameObject.SetActive(value: true);
			break;
		case 3:
			_hunterFace.GetChild(int.Parse(HunterInfo.Hunter.hunterImg3) - 1).gameObject.SetActive(value: true);
			break;
		case 4:
			_hunterFace.GetChild(int.Parse(HunterInfo.Hunter.hunterImg4) - 1).gameObject.SetActive(value: true);
			break;
		case 5:
			_hunterFace.GetChild(int.Parse(HunterInfo.Hunter.hunterImg5) - 1).gameObject.SetActive(value: true);
			break;
		}
	}
}
