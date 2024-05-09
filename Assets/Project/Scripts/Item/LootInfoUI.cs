using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LootInfoUI : MonoBehaviour
{
	[FormerlySerializedAs("trIconAnchor")] [SerializeField]
	private Transform _itemAnchor;

	[FormerlySerializedAs("textItemCount")] [SerializeField]
	private Text _itemCountText;

	private string _spawnPoolName;

	private Transform _itemSpawnTransform;

	public void Open(string poolName, string spawnName, int count)
	{
		_spawnPoolName = poolName;
		_itemSpawnTransform = MasterPoolManager.SpawnObject(_spawnPoolName, spawnName, _itemAnchor);
		_itemCountText.text = $"X{count}";
	}

	public void Remove()
	{
		if (_itemSpawnTransform != null)
		{
			MasterPoolManager.ReturnToPool(_spawnPoolName, _itemSpawnTransform);
			_itemSpawnTransform = null;
		}
	}
}
