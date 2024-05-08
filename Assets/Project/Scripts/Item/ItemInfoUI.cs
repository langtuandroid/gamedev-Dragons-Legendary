using UnityEngine;
using UnityEngine.UI;

public class ItemInfoUI : MonoBehaviour
{
	[SerializeField]
	private Transform trIconAnchor;

	[SerializeField]
	private Text textItemCount;

	private string spawnPoolName;

	private Transform trSpawnItem;

	public void Show(string poolName, string spawnName, int count)
	{
		spawnPoolName = poolName;
		trSpawnItem = MasterPoolManager.SpawnObject(spawnPoolName, spawnName, trIconAnchor);
		textItemCount.text = $"X{count}";
	}

	public void Clear()
	{
		if (trSpawnItem != null)
		{
			MasterPoolManager.ReturnToPool(spawnPoolName, trSpawnItem);
			trSpawnItem = null;
		}
	}
}
