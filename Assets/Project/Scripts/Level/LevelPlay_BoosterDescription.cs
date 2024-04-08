using UnityEngine;
using UnityEngine.UI;

public class LevelPlay_BoosterDescription : MonoBehaviour
{
	[SerializeField]
	private Transform boostIcon1;

	[SerializeField]
	private Transform boostIcon2;

	[SerializeField]
	private Transform boostIcon3;

	[SerializeField]
	private Text boostExplain1;

	[SerializeField]
	private Text boostExplain2;

	[SerializeField]
	private Text boostExplain3;

	private int[] boostType;

	private BoostItemDbData boosterData;

	public void Init(int[] _itemType)
	{
		base.gameObject.SetActive(value: true);
		boostType = _itemType;
		DespawnBoosterIcon();
		SpawnBoosterIcon();
	}

	private void DespawnBoosterIcon()
	{
		int childCount = boostIcon1.childCount;
		for (int i = 0; i < childCount; i++)
		{
			MWPoolManager.DeSpawn("Item", boostIcon1.GetChild(0).transform);
		}
		childCount = boostIcon2.childCount;
		for (int j = 0; j < childCount; j++)
		{
			MWPoolManager.DeSpawn("Item", boostIcon2.GetChild(0).transform);
		}
		childCount = boostIcon3.childCount;
		for (int k = 0; k < childCount; k++)
		{
			MWPoolManager.DeSpawn("Item", boostIcon3.GetChild(0).transform);
		}
	}

	private void SpawnBoosterIcon()
	{
		Transform transform = MWPoolManager.Spawn("Item", "Booster" + boostType[0], boostIcon1);
		transform = MWPoolManager.Spawn("Item", "Booster" + boostType[1], boostIcon2);
		transform = MWPoolManager.Spawn("Item", "Booster" + boostType[2], boostIcon3);
		transform = null;
		boostExplain1.text = MWLocalize.GetData(GameDataManager.GetBoostItemData(boostType[0]).boosterExplain);
		boostExplain2.text = MWLocalize.GetData(GameDataManager.GetBoostItemData(boostType[1]).boosterExplain);
		boostExplain3.text = MWLocalize.GetData(GameDataManager.GetBoostItemData(boostType[2]).boosterExplain);
	}

	public void OnClickDescription()
	{
		base.gameObject.SetActive(value: false);
	}
}
