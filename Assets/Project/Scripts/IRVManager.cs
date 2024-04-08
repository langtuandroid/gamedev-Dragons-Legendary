using System.Collections;
using UnityEngine;

public class IRVManager : MonoBehaviour
{
	public InternetReachabilityVerifier internetReachabilityVerifier;

	public static InternetReachabilityVerifier.Status CurrentNetStatus = InternetReachabilityVerifier.Status.Error;

	private static IRVManager instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		internetReachabilityVerifier.statusChangedDelegate += netStatusChanged;
	}

	public void CheckCurrentNetStatus()
	{
		if (CurrentNetStatus != InternetReachabilityVerifier.Status.NetVerified)
		{
		}
	}

	private IEnumerator NetCheck()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			UnityEngine.Debug.Log("******** CurrentNetStatus = " + CurrentNetStatus);
		}
	}

	private void netStatusChanged(InternetReachabilityVerifier.Status newStatus)
	{
		MonoBehaviour.print("InternetReachabilityVerifier.Status  = " + newStatus);
		CurrentNetStatus = newStatus;
		if (CurrentNetStatus == InternetReachabilityVerifier.Status.NetVerified)
		{
			Protocol_Set.Send_Remain_Protocol();
		}
		if (newStatus == InternetReachabilityVerifier.Status.Error)
		{
			string lastError = internetReachabilityVerifier.lastError;
		}
		CheckCurrentNetStatus();
	}
}
