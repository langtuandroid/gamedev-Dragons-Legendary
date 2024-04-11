using System.Collections;
using UnityEngine;

[RequireComponent(typeof(InternetReachabilityVerifier))]
public class SkeletonIRVExample : MonoBehaviour
{
	private InternetReachabilityVerifier internetReachabilityVerifier;

	private bool isNetVerified()
	{
		return internetReachabilityVerifier.status == InternetReachabilityVerifier.Status.NetVerified;
	}

	private void forceReverification()
	{
		internetReachabilityVerifier.forceReverification();
	}

	private void netStatusChanged(InternetReachabilityVerifier.Status newStatus)
	{
		UnityEngine.Debug.Log("netStatusChanged: new InternetReachabilityVerifier.Status = " + newStatus);
	}

	private IEnumerator waitForNetwork()
	{
		yield return new WaitForEndOfFrame();
		yield return StartCoroutine(internetReachabilityVerifier.waitForNetVerifiedStatus());
		UnityEngine.Debug.Log("waitForNetwork coroutine succeeded and stopped.");
	}

	private void Start()
	{
		internetReachabilityVerifier = GetComponent<InternetReachabilityVerifier>();
		internetReachabilityVerifier.statusChangedDelegate += netStatusChanged;
		StartCoroutine(waitForNetwork());
	}
}
