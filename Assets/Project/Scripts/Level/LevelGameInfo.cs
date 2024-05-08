using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelGameInfo : LobbyPopupBase
{
	public Action OnGoBackEvent;

	[FormerlySerializedAs("textUserLevel")] [SerializeField]
	private Text _textUserLevel;

	[FormerlySerializedAs("textUserExp")] [SerializeField]
	private Text _textUserExp;

	[FormerlySerializedAs("textUserAttackBoost")] [SerializeField]
	private Text _textUserAttackBoost;

	[FormerlySerializedAs("textMaxLevelUp")] [SerializeField]
	private Text _textMaxLevelUp;

	[FormerlySerializedAs("rtUserExpGauge")] [SerializeField]
	private RectTransform _rtUserExpGauge;

	[FormerlySerializedAs("trItemListAnchor")] [SerializeField]
	private Transform _trItemListAnchor;

	private Vector2 _userExpGaugeOffsetMax;

	private Vector3 _levelInfoItemSize = new Vector3(0.6f, 0.6f, 0.6f);

	private UserLevelDbData _currentLevelData;

	private UserLevelDbData _nextLevelData;

	public override void Open()
	{
		base.Open();
		Construct();
		LobbyManager.HideHunterLobby();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void CloseProcessComplete()
	{
	}

	private void Construct()
	{
		_currentLevelData = GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level);
		_nextLevelData = GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level + 1);
		_textUserLevel.text = $"{_currentLevelData.level}";
		_textUserExp.text = $"{GameInfo.userData.userInfo.exp}/{_currentLevelData.exp}";
		_textUserAttackBoost.text = string.Format(MasterLocalize.GetData("popup_level_text_2"), _currentLevelData.attackBonusAll * 100f);
		if (_nextLevelData == null)
		{
			_userExpGaugeOffsetMax = _rtUserExpGauge.offsetMax;
			_userExpGaugeOffsetMax.x = (float)(_currentLevelData.exp / _currentLevelData.exp) * 444f;
			_rtUserExpGauge.offsetMax = _userExpGaugeOffsetMax;
			_textUserExp.text = "max";
			_textMaxLevelUp.gameObject.SetActive(value: true);
			_textMaxLevelUp.text = string.Format("{0}", MasterLocalize.GetData("popup_level_up_text_5"));
		}
		else
		{
			_userExpGaugeOffsetMax = _rtUserExpGauge.offsetMax;
			_userExpGaugeOffsetMax.x = (float)GameInfo.userData.userInfo.exp / (float)_currentLevelData.exp * 444f;
			_rtUserExpGauge.offsetMax = _userExpGaugeOffsetMax;
			_textMaxLevelUp.gameObject.SetActive(value: false);
		}
		FindNextReward();
	}

	private void FindNextReward()
	{
		_nextLevelData = GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level + 1);
		if (_nextLevelData != null)
		{
			RequiredItem_Cell component = MasterPoolManager.SpawnObject("Grow", "cell_token", _trItemListAnchor).GetComponent<RequiredItem_Cell>();
			component.SetItemImg(50034, _levelInfoItemSize);
			component.SetCostText(MasterLocalize.GetData("common_text_2max"));
			component.SetClickType(ItemClickType.None);
			RequiredItem_Cell component2 = MasterPoolManager.SpawnObject("Grow", "cell_token", _trItemListAnchor).GetComponent<RequiredItem_Cell>();
			component2.SetItemImg(50034, _levelInfoItemSize);
			component2.SetCostText(MasterLocalize.GetData("common_text_refill"));
			component2.SetClickType(ItemClickType.None);
			RequiredItem_Cell component3 = MasterPoolManager.SpawnObject("Grow", "cell_token", _trItemListAnchor).GetComponent<RequiredItem_Cell>();
			component3.SetItemImg(50031, _levelInfoItemSize);
			component3.SetCostText(MasterLocalize.GetData("common_text_20jewel"));
			component3.SetClickType(ItemClickType.None);
			RequiredItem_Cell component4 = MasterPoolManager.SpawnObject("Grow", "cell_token", _trItemListAnchor).GetComponent<RequiredItem_Cell>();
			component4.SetItemImg(50033, _levelInfoItemSize);
			component4.SetCostText(MasterLocalize.GetData("common_text_1key"));
			component4.SetClickType(ItemClickType.None);
			RequiredItem_Cell component5 = MasterPoolManager.SpawnObject("Grow", "cell_token", _trItemListAnchor).GetComponent<RequiredItem_Cell>();
			component5.SetItemImg(50042, _levelInfoItemSize);
			component5.SetCostText(MasterLocalize.GetData("common_text_5percent"));
			component5.SetClickType(ItemClickType.None);
		}
	}

	public void OnClickGoBack()
	{
		if (OnGoBackEvent != null)
		{
			OnGoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}

	private void OnDisable()
	{
		RequiredItem_Cell[] componentsInChildren = _trItemListAnchor.GetComponentsInChildren<RequiredItem_Cell>();
		foreach (RequiredItem_Cell requiredItem_Cell in componentsInChildren)
		{
			MasterPoolManager.ReturnToPool("Grow", requiredItem_Cell.transform);
		}
	}
}
