using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LiftDetail : LobbyPopupBase
{
	public Action OnCloseEvent;

	[FormerlySerializedAs("textFloorName")] [SerializeField]
	private Text _floorNameText;

	[FormerlySerializedAs("textOpenInfo")] [SerializeField]
	private Text _infoOpenText;

	[FormerlySerializedAs("textStoreDuration")] [SerializeField]
	private Text _textStoreTime;

	[FormerlySerializedAs("textStoreEarnings")] [SerializeField]
	private Text _storeErning;

	[FormerlySerializedAs("textUserOwnBadge")] [SerializeField]
	private Text _badgeText;

	[FormerlySerializedAs("textStoreBadge")] [SerializeField]
	private Text _badgeStoredText;

	[FormerlySerializedAs("textGetBadgeCount")] [SerializeField]
	private Text _badgeNumberText;

	[FormerlySerializedAs("imageTitle")] [SerializeField]
	private Image _titlePicture;

	[FormerlySerializedAs("trBadgeAnchor")] [SerializeField]
	private Transform _badgeTransform;

	[FormerlySerializedAs("trUpgradeButton")] [SerializeField]
	private Transform _upgradeButton;

	[FormerlySerializedAs("trToOpenItemAnchor")] [SerializeField]
	private Transform _itemAnchor;

	[FormerlySerializedAs("goUpgradeButton")] [SerializeField]
	private GameObject _goUpgradeButton;

	[FormerlySerializedAs("arrGoLevelStar")] [SerializeField]
	private GameObject[] _goLevelStar = new GameObject[0];

	[FormerlySerializedAs("arrGoStoreOperating")] [SerializeField]
	private GameObject[] _storeOperating = new GameObject[0];

	private int _userCount;

	private Transform _transformBadge;

	private Transform _transformToOpen;

	private UserFloorData _userLift;

	private StoreProduceDbData _produceData;

	public Transform UpgradeButton => _upgradeButton;

	public void WowShow(UserFloorData _userData, StoreProduceDbData _produceData)
	{
		_userLift = _userData;
		this._produceData = _produceData;
		Open();
	}

	public override void Open()
	{
		base.Open();
		Construct();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void Complete()
	{
	}

	private void Construct()
	{
		_userCount = GameInfo.userData.GetItemCount(_produceData.snip1Type);
		if (_userCount < _produceData.snip1N)
		{
			_infoOpenText.text = $"<color=red>{_userCount}</color>/{_produceData.snip1N}";
		}
		else
		{
			_infoOpenText.text = $"{_userCount}/{_produceData.snip1N}";
		}
		UnityEngine.Debug.Log("produceTime :: " + _produceData.produceTime);
		TimeSpan timeSpan = TimeSpan.FromSeconds(_produceData.produceTime);
		string text = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}";
		_textStoreTime.text = text;
		_storeErning.text = $"{_produceData.getCoin}";
		_badgeText.text = string.Format("{0} {1}", MasterLocalize.GetData("common_text_you_have"), GameInfo.userData.GetItemCount(GameDataManager.GetStoreData(_produceData.storeIdx).spi));
		_badgeStoredText.text = $"{_userLift.operatingRatio}/{GameDataManager.GetGameConfigData(ConfigDataType.StoreGetbadgeCycle)}";
		_floorNameText.text = MasterLocalize.GetData(GameDataManager.GetStoreData(_produceData.storeIdx).storeName);
		_titlePicture.sprite = GameDataManager.GetFloorTitleSprite(LobbyManager.OpenChapterFloorId);
		_transformToOpen = MasterPoolManager.SpawnObject("Item", $"Item_{_produceData.snip1Type}", _itemAnchor);
		_transformBadge = MasterPoolManager.SpawnObject("Item", $"Item_{_produceData.spi}", _badgeTransform);
		_badgeNumberText.text = $"X{_produceData.spiN}";
		for (int i = 0; i < _goLevelStar.Length; i++)
		{
			_goLevelStar[i].SetActive(i + 1 <= _userLift.storeTier);
		}
		for (int j = 0; j < _storeOperating.Length; j++)
		{
			_storeOperating[j].SetActive(j + 1 <= _userLift.operatingRatio);
		}
		_goUpgradeButton.SetActive(GameDataManager.GetStoreProduceData(_userLift.storeIdx, _userLift.storeTier + 1) != null);
	}

	public void OnClickGotoUpgrade()
	{
		if (GameDataManager.GetStoreProduceData(_userLift.storeIdx, _userLift.storeTier + 1) != null)
		{
			LobbyManager.ShowFloorUpgrade(_userLift.storeIdx, _userLift.storeTier);
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		}
	}

	public void OnClickGoBack()
	{
		if (OnCloseEvent != null)
		{
			OnCloseEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}

	private void OnDisable()
	{
		if (_transformBadge != null)
		{
			MasterPoolManager.ReturnToPool("Item", _transformBadge);
			_transformBadge = null;
		}
		if (_transformToOpen != null)
		{
			MasterPoolManager.ReturnToPool("Item", _transformToOpen);
			_transformToOpen = null;
		}
	}
}
