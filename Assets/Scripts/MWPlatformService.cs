using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using UnityEngine;

public class MWPlatformService : MonoBehaviour
{
	public static Action<bool, string> LoginResult;

	private static PlayGamesClientConfiguration _GPGConfig;

	[SerializeField]
	private bool isAutoLogin = true;

	private static MWPlatformService instance;

	public static bool LoginState => Social.localUser.authenticated;

	public static string UserId => Social.localUser.id;

	public static string UserName => Social.localUser.userName;

	public static void Init()
	{
		_GPGConfig = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
		PlayGamesPlatform.InitializeInstance(_GPGConfig);
		PlayGamesPlatform.DebugLogEnabled = true;
		PlayGamesPlatform.Activate();
		if (instance.isAutoLogin)
		{
			Login(delegate(bool succeess, string errormessage)
			{
				if (LoginResult != null)
				{
					LoginResult(succeess, errormessage);
				}
				UnityEngine.Debug.Log("Playtform Login!! :: " + succeess + ", errormessage :: " + errormessage);
			});
		}
	}

	public static void Login(Action<bool, string> onResult = null)
	{
		Social.localUser.Authenticate(onResult);
	}

	public static void ShowLeaderboard()
	{
		if (Social.localUser.authenticated)
		{
			Social.ShowLeaderboardUI();
		}
	}

	public static void ShowArchivement()
	{
		if (Social.localUser.authenticated)
		{
			Social.ShowAchievementsUI();
		}
	}

	public static void ReportLeaderBoard(string leaderboardId, long score, Action<bool> OnResult = null)
	{
		Social.ReportScore(score, leaderboardId, OnResult);
	}

	public static void UnlockArchivement(string archivementId, int score, Action<bool> OnResult = null)
	{
		Social.ReportProgress(archivementId, score, OnResult);
	}

	public static string GetUniqueDeviceId()
	{
		return SystemInfo.deviceUniqueIdentifier;
	}

	private void Awake()
	{
		instance = this;
	}
}
