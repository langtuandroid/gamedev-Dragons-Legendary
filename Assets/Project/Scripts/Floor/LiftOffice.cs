using UnityEngine;
using UnityEngine.Serialization;

public class LiftOffice : MonoBehaviour
{
	[FormerlySerializedAs("trOfficeAnchor")] [SerializeField]
	private Transform _officeTransform;

	private Transform _officeTransform2;

	public void PlaceOffice(string _officeName)
	{
		_officeTransform2 = MasterPoolManager.SpawnObject("Lobby", _officeName);
	}

	public void SaveOffice()
	{
		if (_officeTransform2 != null && _officeTransform != null && _officeTransform2.position != _officeTransform.position)
		{
			_officeTransform2.position = _officeTransform.position;
		}
	}

	private void OnDestroy()
	{
		if (_officeTransform2 != null)
		{
			MasterPoolManager.ReturnToPool("Lobby", _officeTransform2);
			_officeTransform2 = null;
		}
	}
}
