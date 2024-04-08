using UnityEngine;
using UnityEngine.UI;

public class InGameResultItem : MonoBehaviour
{
	[SerializeField]
	private Transform trItemParent;

	[SerializeField]
	private Text textItemName;

	[SerializeField]
	private Text textItemMultiply;

	[SerializeField]
	private Text textItemAmount;

	private Transform trResultItem;

	private ResultItemData resultData;

	public void Show(ResultItemData data)
	{
		Clear();
		resultData = data;
		trResultItem = MWPoolManager.Spawn("Item", $"Item_{data.itemIdx}", trItemParent);
		trResultItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		textItemName.text = MWLocalize.GetData(data.itemName);
		textItemMultiply.text = $"X{data.itemMultiply}";
		textItemAmount.text = $"{data.itemAmount}";
	}

	public void AddCount(int _count)
	{
		textItemMultiply.text = $"X{resultData.itemMultiply + _count}";
	}

	public void Clear()
	{
		if (trResultItem != null)
		{
			MWPoolManager.DeSpawn("Item", trResultItem);
			trResultItem = null;
		}
	}

	private void OnDisable()
	{
		Clear();
	}
}
