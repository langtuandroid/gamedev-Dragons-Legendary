using UnityEngine;
using UnityEngine.UI;

public class ArenaChestResultItem : MonoBehaviour
{
	[SerializeField]
	private ChestListDbData chestListDbData;

	[SerializeField]
	private HeroCard hunterCard;

	[SerializeField]
	private Transform itemCard;

	[SerializeField]
	private Text itemCount_Text;

	[SerializeField]
	private Text itemName_Text;

	[SerializeField]
	private Text itemAmount_Text;

	public ChestListDbData ChestListDbData => chestListDbData;

	public void Init(ChestListDbData _data, Vector3 size = default(Vector3))
	{
		chestListDbData = _data;
		if (hunterCard != null)
		{
			MWPoolManager.DeSpawn("Hunter", hunterCard.transform);
			hunterCard = null;
		}
		if (itemCard != null)
		{
			MWPoolManager.DeSpawn("Item", itemCard);
			itemCard = null;
		}
		if (size == Vector3.zero)
		{
			size = Vector3.one;
		}
		if (_data.chestHunter > 0 && _data.chestItem == 0)
		{
			hunterCard = MWPoolManager.Spawn("Hunter", "HunterCard_" + _data.chestHunter, base.transform).GetComponent<HeroCard>();
			hunterCard.Construct(HerocardType.Chestopen, GameDataManager.GetHunterInfo(_data.chestHunter, _data.hunterLevel, _data.hunterTier), _isOwn: true, _isArena: false);
			hunterCard.HeroIdx = 0;
			hunterCard.transform.localPosition = Vector3.zero;
			hunterCard.transform.localScale = size;
			hunterCard.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(65f, -35f);
			hunterCard.transform.SetAsFirstSibling();
			itemCount_Text.text = "x1";
			itemName_Text.text = MWLocalize.GetData(GameDataManager.GetHunterInfo(_data.chestHunter, 1, 1).Hunter.hunterName);
			if ((bool)itemAmount_Text)
			{
				itemAmount_Text.text = _data.chestItemN.ToString();
			}
		}
		else if (_data.chestHunter == 0 && _data.chestItem > 0)
		{
			if (itemCard != null)
			{
				MWPoolManager.DeSpawn("Item", itemCard);
				itemCard = null;
			}
			itemCard = MWPoolManager.Spawn("Item", "Item_" + _data.chestItem, base.transform);
			itemCard.transform.localPosition = Vector3.zero;
			itemCard.transform.localScale = size;
			itemCard.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 35f);
			itemCard.transform.SetAsFirstSibling();
			itemCount_Text.text = "x" + _data.chestItemN.ToString();
			itemName_Text.text = MWLocalize.GetData(GameDataManager.GetItemListData(_data.chestItem).itemName);
			if ((bool)itemAmount_Text)
			{
				itemAmount_Text.text = GameInfo.userData.GetItemCount(_data.chestItem).ToString();
			}
		}
		else if (_data.chestHunter > 0 && _data.chestItem > 0)
		{
			if (itemCard != null)
			{
				MWPoolManager.DeSpawn("Item", itemCard);
				itemCard = null;
			}
			itemCard = MWPoolManager.Spawn("Item", "Item_" + _data.chestItem, base.transform);
			itemCard.transform.localPosition = Vector3.zero;
			itemCard.transform.localScale = Vector3.one;
			itemCard.transform.SetAsFirstSibling();
			itemCount_Text.text = "x" + _data.chestItemN.ToString();
			itemName_Text.text = MWLocalize.GetData(GameDataManager.GetItemListData(_data.chestItem).itemName);
			if ((bool)itemAmount_Text)
			{
				itemAmount_Text.text = GameInfo.userData.GetItemCount(_data.chestItem).ToString();
			}
		}
	}

	public void AddAmount(int _amount)
	{
		itemCount_Text.text = "x" + (chestListDbData.chestItemN + _amount);
	}
}
