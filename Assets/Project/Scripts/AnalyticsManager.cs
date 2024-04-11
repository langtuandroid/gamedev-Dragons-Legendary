#if ENABLE_FIREBASE
using Firebase.Analytics;
#endif
using com.F4A.MobileThird;
using System.Collections.Generic;
using UnityEngine;


public class AnalyticsManager : MonoBehaviour
{
    
    public static void log(string _eventName, Dictionary<string, string> _params)
    {

    }

    public static void InAppPurchaseAppEnvent(System.Object e)
    {
    }


    public static void RewardPromptAppEnvent(string SceneName)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary.Add("ca_scene", SceneName);
        log("ca_ad_rv_prompt", dictionary);
    }

 
}