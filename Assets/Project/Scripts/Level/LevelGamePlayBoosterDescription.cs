using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelGamePlayBoosterDescription : MonoBehaviour
{
	[FormerlySerializedAs("boostIcon1")] [SerializeField]
	private Transform _boostIcon1;

	[FormerlySerializedAs("boostIcon2")] [SerializeField]
	private Transform _boostIcon2;

	[FormerlySerializedAs("boostIcon3")] [SerializeField]
	private Transform _boostIcon3;

	[FormerlySerializedAs("boostExplain1")] [SerializeField]
	private Text _boostExplain1;

	[FormerlySerializedAs("boostExplain2")] [SerializeField]
	private Text _boostDescription2;

	[FormerlySerializedAs("boostExplain3")] [SerializeField]
	private Text boostDescription3;

	private int[] _boostType;

	private BoostItemDbData _boosterData;

	public void Construct(int[] _itemType)
	{
		base.gameObject.SetActive(value: true);
		_boostType = _itemType;
		RemoveBoosterIcon();
		CreateBoosterIcon();
	}

	private void RemoveBoosterIcon()
	{
		int childCount = _boostIcon1.childCount;
		for (int i = 0; i < childCount; i++)
		{
			MWPoolManager.DeSpawn("Item", _boostIcon1.GetChild(0).transform);
		}
		childCount = _boostIcon2.childCount;
		for (int j = 0; j < childCount; j++)
		{
			MWPoolManager.DeSpawn("Item", _boostIcon2.GetChild(0).transform);
		}
		childCount = _boostIcon3.childCount;
		for (int k = 0; k < childCount; k++)
		{
			MWPoolManager.DeSpawn("Item", _boostIcon3.GetChild(0).transform);
		}
	}

	private void CreateBoosterIcon()
	{
		Transform transform = MWPoolManager.Spawn("Item", "Booster" + _boostType[0], _boostIcon1);
		transform = MWPoolManager.Spawn("Item", "Booster" + _boostType[1], _boostIcon2);
		transform = MWPoolManager.Spawn("Item", "Booster" + _boostType[2], _boostIcon3);
		transform = null;
		_boostExplain1.text = MWLocalize.GetData(GameDataManager.GetBoostItemData(_boostType[0]).boosterExplain);
		_boostDescription2.text = MWLocalize.GetData(GameDataManager.GetBoostItemData(_boostType[1]).boosterExplain);
		boostDescription3.text = MWLocalize.GetData(GameDataManager.GetBoostItemData(_boostType[2]).boosterExplain);
	}

	public void OnClickDescription()
	{
		base.gameObject.SetActive(value: false);
	}
}
