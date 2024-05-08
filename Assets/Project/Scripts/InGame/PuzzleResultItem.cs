using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PuzzleResultItem : MonoBehaviour
{
	[FormerlySerializedAs("trItemParent")] [SerializeField]
	private Transform _itemParent;

	[FormerlySerializedAs("textItemName")] [SerializeField]
	private Text _textItemName;

	[FormerlySerializedAs("textItemMultiply")] [SerializeField]
	private Text _textItemMultiply;

	[FormerlySerializedAs("textItemAmount")] [SerializeField]
	private Text _textItemAmount;

	private Transform _trResultItem;

	private ResultItemData _resultData;

	public void OpenMenu(ResultItemData data)
	{
		ClearItems();
		_resultData = data;
		_trResultItem = MasterPoolManager.SpawnObject("Item", $"Item_{data.itemIdx}", _itemParent);
		_trResultItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		_textItemName.text = MasterLocalize.GetData(data.itemName);
		_textItemMultiply.text = $"X{data.itemMultiply}";
		_textItemAmount.text = $"{data.itemAmount}";
	}

	public void AddCount(int _count)
	{
		_textItemMultiply.text = $"X{_resultData.itemMultiply + _count}";
	}

	public void ClearItems()
	{
		if (_trResultItem != null)
		{
			MasterPoolManager.ReturnToPool("Item", _trResultItem);
			_trResultItem = null;
		}
	}

	private void OnDisable()
	{
		ClearItems();
	}
}
