using UnityEngine;

public class FloorOffice : MonoBehaviour
{
	[SerializeField]
	private Transform trOfficeAnchor;

	private Transform trOffice;

	public void SetOffice(string _officeName)
	{
		trOffice = MasterPoolManager.SpawnObject("Lobby", _officeName);
	}

	public void SyncOffice()
	{
		if (trOffice != null && trOfficeAnchor != null && trOffice.position != trOfficeAnchor.position)
		{
			trOffice.position = trOfficeAnchor.position;
		}
	}

	private void OnDestroy()
	{
		if (trOffice != null)
		{
			MasterPoolManager.ReturnToPool("Lobby", trOffice);
			trOffice = null;
		}
	}
}
