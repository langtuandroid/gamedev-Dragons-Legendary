using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelGamePlayBoosterItem : MonoBehaviour
{
	[FormerlySerializedAs("booster_selected")] [SerializeField]
	private Transform _boosterSelected;

	[FormerlySerializedAs("booster_deselected")] [SerializeField]
	private Transform _boosterDeselected;

	[FormerlySerializedAs("booster_icon")] [SerializeField]
	private Transform _boosterIcon;

	[FormerlySerializedAs("booster_check")] [SerializeField]
	private Transform _boosterCheck;

	[FormerlySerializedAs("booster_Free")] [SerializeField]
	private Transform _boosterFree;

	[FormerlySerializedAs("booster_Jewel")] [SerializeField]
	private Transform _boosterJewel;

	[FormerlySerializedAs("booster_Name")] [SerializeField]
	private Text _boosterName;

	[FormerlySerializedAs("booster_Cost")] [SerializeField]
	private Text _boosterCost;

	private BoostItemDbData _boosterData;

	private bool _isSelected;

	private int _itemType;

	public bool IsSelect => _isSelected;

	public int BoostItemType => _itemType;

	public BoostItemDbData BoosterData => _boosterData;

	public void Construct(int _itemType)
	{
		ConstructItem();
		this._itemType = _itemType;
		ConfigureBoosterItem();
	}

	private void ConstructItem()
	{
		_isSelected = false;
		_boosterSelected.gameObject.SetActive(value: false);
		_boosterDeselected.gameObject.SetActive(value: true);
		for (int i = 0; i < _boosterIcon.childCount; i++)
		{
			MWPoolManager.DeSpawn("Item", _boosterIcon.GetChild(0).transform);
		}
		_boosterCheck.gameObject.SetActive(value: false);
	}

	private void ConfigureBoosterItem()
	{
		Transform transform = MWPoolManager.Spawn("Item", "Booster" + _itemType, _boosterIcon);
		_boosterData = GameDataManager.GetBoostItemData(_itemType);
		if (_boosterData.costType == -1)
		{
			_boosterFree.gameObject.SetActive(value: true);
			_boosterJewel.gameObject.SetActive(value: false);
			VideoBoostItem();
		}
		else
		{
			_boosterFree.gameObject.SetActive(value: false);
			_boosterJewel.gameObject.SetActive(value: true);
			_boosterCost.text = _boosterData.costCount.ToString();
		}
		_boosterName.text = MWLocalize.GetData(_boosterData.boosterName);
	}

	private void VideoBoostItem()
	{
		if (GamePreferenceManager.GetIsBoostRewardVideo())
		{
			EquipBoosterItem();
		}
	}

	private void EquipBoosterItem()
	{
		_isSelected = true;
		_boosterSelected.gameObject.SetActive(value: true);
		_boosterDeselected.gameObject.SetActive(value: false);
		_boosterCheck.gameObject.SetActive(value: true);
	}

	private void UnequipBoosterItem()
	{
		_isSelected = false;
		_boosterSelected.gameObject.SetActive(value: false);
		_boosterDeselected.gameObject.SetActive(value: true);
		_boosterCheck.gameObject.SetActive(value: false);
	}

	public void OnClickBooster()
	{
		if (_boosterData.costType == -1)
		{
			if (!_isSelected)
			{
				
			}
		}
		else if (_isSelected)
		{
			GameDataManager.AddUserJewel(_boosterData.costCount);
			UnequipBoosterItem();
		}
		else if (GameDataManager.UseUserJewel(_boosterData.costCount))
		{
			EquipBoosterItem();
		}
		else
		{
			LobbyManager.ShowNotEnoughJewel(_boosterData.costCount - GameInfo.userData.userInfo.jewel);
		}
	}
}
