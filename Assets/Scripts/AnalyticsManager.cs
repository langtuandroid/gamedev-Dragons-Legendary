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
#if ENABLE_APPFLYER
        if (logEnabled)
        {
            AppsFlyer.trackRichEvent(_eventName, _params);
        }
#endif
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
        if (BuildSet.isDEV)
        {
            return;
        }
#if ENABLE_FIREBASE
        switch (_type)
        {
            case FBLog_Type.app_launch:
                if (CheckAnalytics("01_app_launch"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "01_app_launch"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.title:
                if (CheckAnalytics("02_title"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "02_title"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.lobby:
                if (CheckAnalytics("03_lobby"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "03_lobby"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.tuto1_battle:
                if (CheckAnalytics("05_1_tuto1_puzzle_move"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "05_1_tuto1_puzzle_move"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.tuto1_puzzle_move:
                if (CheckAnalytics("05_2_tuto1_puzzle_move"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "05_2_tuto1_puzzle_move"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.tuto1_wave2:
                if (CheckAnalytics("05_3_tuto1_puzzle_move"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "05_3_tuto1_puzzle_move"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.cleared_level_lv1:
                if (CheckAnalytics("06_cleared_level_lv1"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "06_cleared_level_lv1"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.open_reward_lv1:
                if (CheckAnalytics("07_open_reward_lv1"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "07_open_reward_lv1"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.claim_reward_lv1:
                if (CheckAnalytics("08_claim_reward_lv1"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "08_claim_reward_lv1"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.open_store:
                if (CheckAnalytics("09_open_store"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "09_open_store"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.battle_menu_click:
                if (CheckAnalytics("10_battle_menu_click"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "10_battle_menu_click"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.play_lv2:
                if (CheckAnalytics("11_play_lv2"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "11_play_lv2"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.cleared_level_lv2:
                if (CheckAnalytics("12_cleared_level_lv2"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "12_cleared_level_lv2"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.play_level_lv3:
                if (CheckAnalytics("13_play_level_lv3"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "13_play_level_lv3"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.cleared_level_lv3:
                if (CheckAnalytics("14_cleared_level_lv3"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "14_cleared_level_lv3"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.hunter_level_up:
                if (CheckAnalytics("15_hunter_level_up"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "15_hunter_level_up"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.popup_hunter_close:
                if (CheckAnalytics("16_popup_hunter_close"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "16_popup_hunter_close"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.cleared_level_lv4:
                if (CheckAnalytics("17_cleared_level_lv4"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "17_cleared_level_lv4"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.open_chest:
                if (CheckAnalytics("18_open_chest"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "18_open_chest"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.collect_chest:
                if (CheckAnalytics("19_collect_chest"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "19_collect_chest"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.cleared_level_lv5:
                if (CheckAnalytics("20_cleared_level_lv5"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "20_cleared_level_lv5"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.open_new_store:
                if (CheckAnalytics("21_open_new_store"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "21_open_new_store"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.play_level_lv6:
                if (CheckAnalytics("22_1_play_level_lv6"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "22_1_play_level_lv6"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.tuto12_puzzle_drag_1:
                if (CheckAnalytics("22_2_tuto12_puzzle_drag"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "22_2_tuto12_puzzle_drag"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.tuto12_puzzle_drag_2:
                if (CheckAnalytics("22_3_tuto12_puzzle_drag"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "22_3_tuto12_puzzle_drag"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.cleared_level_lv6:
                if (CheckAnalytics("23_cleared_level_lv6"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "23_cleared_level_lv6"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.cleared_level_lv12:
                if (CheckAnalytics("30_cleared_level_lv12"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "30_cleared_level_lv12"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.open_chest_2:
                if (CheckAnalytics("31_open_chest_2"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "31_open_chest_2"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.cleared_level_lv13:
                if (CheckAnalytics("32_Cleared_level_lv13"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "32_Cleared_level_lv13"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.cleared_level_lv19:
                if (CheckAnalytics("35_cleared_level_lv19"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "35_cleared_level_lv19"), new Parameter("parameter", "N/A"), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.stamin_under_4:
                if (CheckAnalytics("40_stamin_under_4"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "40_stamin_under_4"), new Parameter("parameter", LastLevel()), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.level_up_user:
                if (CheckAnalytics("90_level_up_user"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "90_level_up_user"), new Parameter("parameter", LastLevel()), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.store_open_by_self:
                if (CheckAnalytics("91_store_open_by_self"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "91_store_open_by_self"), new Parameter("parameter", LastLevel()), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.store_collect_by_self:
                if (CheckAnalytics("92_store_collect_by_self"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "92_store_collect_by_self"), new Parameter("parameter", LastLevel()), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.get_badge:
                if (CheckAnalytics("93_get_badge"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "93_get_badge"), new Parameter("parameter", LastLevel()), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.upgrade_store:
                if (CheckAnalytics("94_upgrade_store"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "94_upgrade_store"), new Parameter("parameter", LastLevel()), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
            case FBLog_Type.quick_loot_tuto:
                if (CheckAnalytics("95_Quick_loot_tuto"))
                {
                    FirebaseAnalytics.LogEvent("Initial_launch_funnel", new Parameter("STEP", "95_Quick_loot_tuto"), new Parameter("parameter", LastLevel()), new Parameter("ca_uid", GameInfo.caUid.ToString()), new Parameter("platfrom", GetDevice()));
                }
                break;
        }
#endif
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