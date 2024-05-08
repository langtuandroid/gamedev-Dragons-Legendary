using System;
using UnityEngine;
using UnityEngine.Serialization;

public class MasterPlatformService : MonoBehaviour
{
	[FormerlySerializedAs("isAutoLogin")] [SerializeField]
	private bool _isAutoLogin = true;
	
	public static bool LoginState => Social.localUser.authenticated;

	public static string UserId => Social.localUser.id;
	
	public static string GetUniqueDeviceId()
	{
		return SystemInfo.deviceUniqueIdentifier;
	}
	
}
