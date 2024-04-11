#if ENABLE_FIREBASE
using Firebase;
using System.Threading.Tasks;
#endif
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
	private void Start()
	{
#if ENABLE_FIREBASE
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(delegate(Task<DependencyStatus> task)
		{
			DependencyStatus result = task.Result;
			if (result == DependencyStatus.Available)
			{
				UnityEngine.Debug.Log("Firebase Available !!!!");
			}
			else
			{
				UnityEngine.Debug.LogError($"Could not resolve all Firebase dependencies: {result}");
			}
		});
#endif
	}
}
