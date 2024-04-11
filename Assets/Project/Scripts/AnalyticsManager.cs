#if ENABLE_FIREBASE
using Firebase.Analytics;
#endif
using com.F4A.MobileThird;
using System.Collections.Generic;
using UnityEngine;


public class AnalyticsManager : MonoBehaviour
{
    public static bool logEnabled = true;

    public static void log(string _eventName, Dictionary<string, string> _params)
    {

    }

    public static void InAppPurchaseAppEnvent(System.Object e)
    {
    }

    public static void RewardRequestAppEnvent(string SceneName)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary.Add("ca_scene", SceneName);
        log("ca_ad_rv_requested", dictionary);
    }

    public static void RewardLoadAppEnvent(string SceneName)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary.Add("ca_scene", SceneName);
        log("ca_ad_rv_initiated", dictionary);
    }

    public static void RewardCompleteAppEnvent(string SceneName)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary.Add("ca_scene", SceneName);
        log("ca_ad_rv_completed", dictionary);
    }

    public static void RewardPromptAppEnvent(string SceneName)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary.Add("ca_scene", SceneName);
        log("ca_ad_rv_prompt", dictionary);
    }

    public static void FirebaseAnalyticsLogEvent(FBLog_Type _type)
    {
        
    }

    public static string GetDevice()
    {
        if (BuildSet.CurrentPlatformType == PlatformType.aos)
        {
            return "aos";
        }
        return "ios";
    }

    public static string LastLevel()
    {
        return GameInfo.userData.userStageState[GameInfo.userData.userStageState.Length - 1].chapterList[GameInfo.userData.userStageState[GameInfo.userData.userStageState.Length - 1].chapterList.Length - 1].levelList[GameInfo.userData.userStageState[GameInfo.userData.userStageState.Length - 1].chapterList[GameInfo.userData.userStageState[GameInfo.userData.userStageState.Length - 1].chapterList.Length - 1].levelList.Length - 1].levelIdx.ToString();
    }

    public static bool CheckAnalytics(string _type)
    {
        if (GamePreferenceManager.GetIsAnalytics(_type))
        {
            return false;
        }
        GamePreferenceManager.SetIsAnalytics(_type);
        return true;
    }
}