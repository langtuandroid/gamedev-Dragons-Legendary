#if ENABLE_HTTP_BEST
using BestHTTP;
#endif
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Protocol_Set : GameObjectSingleton<Protocol_Set>
{
#if ENABLE_HTTP_BEST
	public List<HTTPRequest> Reservation_Protocol = new List<HTTPRequest>();
#endif

	private const string SOCIAL_LOGIN_STATE_KEY_NAME = "SocialLogin";

	[SerializeField]
	private Text textUserInfo;

	[SerializeField]
	private GameObject goDebugButton;

	[SerializeField]
	private LoginFailPopup loginFailPopup;

	[SerializeField]
	private NetworkError networkError;

	[SerializeField]
	private UpdateNotice updateNotice;

#if ENABLE_HTTP_BEST
	private HTTPRequest Sended_Protocol;
#endif

	private Action onCallBack_Default;

	private Action onCallBack_User_info;

	private Action onCallBack_User_Item_info;

	private Action<ChestListDbData[]> onCallBack_Chest;

	private Action<GAME_END_RESULT> OnCallBack_Game_End;

	private Action<string> OnCallBack_Floor_Upgrade;

	private Action<GAME_QUICK_LOOT> OnCallBack_Quick_Loot;

	private Action<GAME_LEVEL_REAMAIN_TIME_RESULT> OnCallBack_Level_Remain_Time;

	private Action<USER_DAILY_BONUS_RESULT> onCallBack_User_Daily_Bonus;

	private Action onCallBack_User_Daily_Bonus_Collect;

	private Action<ChestListDbData[]> _onCallBackBuyPackage;

	private Action<SHOP_PACKAGE_LIST_RESULT> OnCallBack_Shop_Package_List;

	private Action<ARENA_INFO_DATA_RESULT> OnCallBack_Arena_Info_Data;

	private Action<ARENA_GAME_START_RESULT> OnCallBack_Arena_Game_Start_Data;

	private Action<ARENA_GAME_END_RESULT> OnCallBack_Arena_Game_End_Data;

	private Action<ARENA_STORE_INFO[]> OnCallBack_Arena_Store_Info_Data;

	private Action OnCallBack_Arena_Ticket;

	private Action<int> OnCallBack_Ad_Start;

	private Action OnCallBack_Tutorial;

	private Coroutine coroutineNetworkDelay;

#if ENABLE_HTTP_BEST
	private List<HTTPRequest> listConnect = new List<HTTPRequest>();
#endif

	public static void CallSocialLoginConnect()
	{
		GameObjectSingleton<Protocol_Set>.Inst.InitSocialUser();
	}

	public static void CallGuestLoginConnect()
	{
		Protocol_check_auth_Req();
	}

	public static void Protocol_check_version_Req()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "check_version");
		JSONObject jSONObject2 = new JSONObject();
		switch (BuildSet.CurrentPlatformType)
		{
			case PlatformType.aos:
				jSONObject2.AddField("device", "aos");
				break;
			case PlatformType.ios:
				jSONObject2.AddField("device", "ios");
				break;
		}
		jSONObject2.AddField("version", BuildSet.AppVer);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set check_version = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_check_version_Res);
	}

	public static void Protocol_check_auth_Req()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "check_auth_new");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("sType", GameObjectSingleton<Protocol_Set>.Inst.GetPlatformType());
		jSONObject2.AddField("sUid", GameInfo.sUid);
		//jSONObject2.AddField("deviceId", MWPlatformService.GetUniqueDeviceId());
		jSONObject2.AddField("deviceId", MWPlatformService.GetUniqueDeviceId() + 1);
		switch (BuildSet.CurrentPlatformType)
		{
			case PlatformType.aos:
				jSONObject2.AddField("device", "aos");
				break;
			case PlatformType.ios:
				jSONObject2.AddField("device", "ios");
				break;
		}
		jSONObject2.AddField("adId", GameInfo.AD_ID);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set check_auth = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_check_auth_Res);
    }

    public static void Protocol_check_logout_Req()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "check_logout");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set check_logout = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, null, isLoading: false);
	}

	public static void Protocol_user_get_tutorial_Req(Action _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "user_get_tutorial");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set user_get_tutorial = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_user_get_tutorial_Res);
	}

	public static void Protocol_user_set_tutorial_Req(int sIdx, int seq, Action _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Tutorial = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "user_set_tutorial");
		JSONObject jSONObject2 = new JSONObject();
		string empty = string.Empty;
		empty = ((sIdx > 5) ? "e" : "m");
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("type", empty);
		jSONObject2.AddField("sIndex", sIdx);
		jSONObject2.AddField("seq", seq);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set user_set_tutorial = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_user_set_tutorial_Res, isLoading: false);
	}

	public static void Protocol_user_info_Req(Action _onCallBack = null, bool isLoading = true)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_info = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "user_info");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set user_info = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_user_info_Res, isLoading);
	}

	public static void Protocol_user_item_info_Req(Action _onCallBack = null, bool isLoading = true)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Item_info = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "user_item_info");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set user_item_info = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_user_item_info_Res, isLoading);
	}

	public static void Protocol_user_daily_bonus_Req(Action<USER_DAILY_BONUS_RESULT> _onCallBack = null, bool isLoading = true)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Daily_Bonus = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "user_daily_bonus");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set user_daily_bonus = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_user_daily_bonus_Res, isLoading);
	}

	public static void Protocol_user_get_daily_bonus_Req(int _dayCnt, Action _onCallBack = null, bool isLoading = true)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Daily_Bonus_Collect = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "user_get_daily_bonus");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("dayCnt", _dayCnt);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set user_get_daily_bonus = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_user_get_daily_bonus_Res, isLoading);
	}

	public static void Protocol_user_chapter_open_Req(int stage, int chapter, Action _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_info = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "user_chapter_open");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("stage", stage);
		jSONObject2.AddField("chapter", chapter);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set user_chapter_open = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_user_chapter_open_Res);
	}

	public static void Protocol_game_start_Req(int _levelIdx, List<int> _listBoostItem, Action _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "game_start");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("levelIdx", _levelIdx);
		JSONObject jSONObject3 = new JSONObject();
		if (_listBoostItem != null && _listBoostItem.Count > 0)
		{
			for (int i = 0; i < _listBoostItem.Count; i++)
			{
				jSONObject3.Add(_listBoostItem[i]);
			}
			jSONObject2.AddField("boosterIdxArray", jSONObject3);
		}
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set game_start = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_game_start_Res, isLoading: true, isSceneLoading: true);
	}

	public static void Protocol_game_end_Req(int _levelIdx, int _gameKey, int _result, int _resultReason, int _turns, int _monsterChestKey, List<List<int>> _monsterArray, int[] _waveArray, Action<GAME_END_RESULT> _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Game_End = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "game_end");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("levelIdx", _levelIdx);
		jSONObject2.AddField("gameKey", _gameKey);
		jSONObject2.AddField("result", _result);
		jSONObject2.AddField("resultReason", _resultReason);
		jSONObject2.AddField("turns", _turns);
		jSONObject2.AddField("monsterChestKey", _monsterChestKey);
		jSONObject2.AddField("continueCount", 0);
		JSONObject jSONObject3 = new JSONObject();
		for (int i = 0; i < _monsterArray.Count; i++)
		{
			JSONObject jSONObject4;
			if (jSONObject3.Count < i + 1)
			{
				jSONObject4 = new JSONObject();
				jSONObject3.Add(jSONObject4);
			}
			else
			{
				jSONObject4 = jSONObject3[i];
			}
			for (int j = 0; j < _monsterArray[i].Count; j++)
			{
				jSONObject4.Add(_monsterArray[i][j]);
			}
		}
		jSONObject2.AddField("waveMonsterArray", jSONObject3);
		JSONObject jSONObject5 = new JSONObject();
		for (int k = 0; k < _waveArray.Length; k++)
		{
			jSONObject5.Add(_waveArray[k]);
		}
		jSONObject2.AddField("waveArray", jSONObject5);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set game_end = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_game_end_Res, isLoading: true, _resultReason == 2);
	}

	public static void Protocol_game_continue_Req(Action _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "game_continue");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("gameKey", GameInfo.userPlayData.gameKey);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set game_continue = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_game_continue_Res);
	}

	public static void Protocol_game_quick_loot_Req(int levelIdx, int monsterChestKey, int adKey, Action<GAME_QUICK_LOOT> _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Quick_Loot = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "game_quick_loot");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("levelIdx", levelIdx);
		jSONObject2.AddField("monsterChestKey", monsterChestKey);
		jSONObject2.AddField("adKey", adKey);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set game_quick_loot = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_game_quick_loot_Res);
	}

	public static void Protocol_game_quick_loot_speed_up_Req(int levelIdx, Action _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "game_quick_loot_speed_up");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("levelIdx", levelIdx);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set game_quick_loot_speed_up = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_game_quick_loot_speed_up_Res);
	}

	public static void Protocol_game_chapter_collect_Req(int stage, int chapter, Action _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "game_chapter_collect");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("stage", stage);
		jSONObject2.AddField("chapter", chapter);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set game_chapter_collect = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_game_chapter_collect_Res);
	}

	public static void Protocol_game_level_remain_time_Req(int levelIdx, Action<GAME_LEVEL_REAMAIN_TIME_RESULT> _onCallBack = null)
	{
		UnityEngine.Debug.Log("@LOG Protocol_Set Protocol_game_level_remain_time_Req");
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Level_Remain_Time = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "game_level_remain_time");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("levelIdx", levelIdx);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set game_level_remain_time = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_game_level_remain_time_Res);
	}

	public static void Protocol_chest_popup_buy_coin_Req(int _needJewel, Action _onCallBack)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "chest_popup_buy_coin");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("needJewel", _needJewel);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set chest_popup_buy_coin = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_chest_popup_buy_coin_Res);
	}

	public static void Protocol_chest_collect_Req(int chestIdx, Action<ChestListDbData[]> _onCallBack, string isFree = "n", bool isTutorial = false, int tutorialNo = 0)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Chest = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "chest_collect");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("chestIdx", chestIdx);
		jSONObject2.AddField("freeYn", isFree);
		if (isTutorial)
		{
			jSONObject2.AddField("tutorialYn", "y");
		}
		else
		{
			jSONObject2.AddField("tutorialYn", "n");
		}
		jSONObject2.AddField("tutorialNo", tutorialNo);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_chest_collect_Res);
	}

	public static void Protocol_chest_req_reward_Req(int _probIdx, bool _isAd, Action _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "chest_req_reward");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("probIdx", _probIdx);
		if (_isAd)
		{
			jSONObject2.AddField("adYn", "y");
		}
		else
		{
			jSONObject2.AddField("adYn", "n");
		}
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_chest_req_reward_Res, isLoading: false);
	}

	public static void Protocol_hunter_change_Req(int[] _useHunter, HUNTERLIST_TYPE _listType, Action _onCallBack)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "hunter_change");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		JSONObject jSONObject3 = new JSONObject();
		for (int i = 0; i < _useHunter.Length; i++)
		{
			jSONObject3.Add(_useHunter[i]);
		}
		jSONObject2.AddField("hunterIdxArray", jSONObject3);
		jSONObject2.AddField("type", (int)_listType);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set hunter_change = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_hunter_change_Res);

	}

	public static void Protocol_hunter_is_not_new_Req(int _hunterIdx, Action _onCallBack)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "hunter_is_not_new");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("hunterIdx", _hunterIdx);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_hunter_is_not_new_Res, isLoading: false);

	}

	public static void Protocol_hunter_level_up_Req(int _hunterIdx, int _targetLevel, Action _onCallBack, bool isTutorial = false)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "hunter_level_up");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("hunterIdx", _hunterIdx);
		jSONObject2.AddField("level", _targetLevel);
		if (isTutorial)
		{
			jSONObject2.AddField("tutorialYn", "y");
		}
		else
		{
			jSONObject2.AddField("tutorialYn", "n");
		}
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_hunter_level_up_Res);

	}

	public static void Protocol_hunter_promotion_Req(int _hunterIdx, Action _onCallBack)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "hunter_promotion");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("hunterIdx", _hunterIdx);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_hunter_promotion_Res);

	}

	public static void Protocol_store_collect_Req(int _stage, int _storeIdx, Action _onCallBack)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "store_collect");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("stage", _stage);
		jSONObject2.AddField("storeIdx", _storeIdx);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set store_collect = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_store_collect_Res);

	}

	public static void Protocol_store_open_Req(int stageIdx, int storeIdx, Action _onCallBack)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "store_open");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("stage", stageIdx);
		jSONObject2.AddField("storeIdx", storeIdx);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_store_open_Res);

	}

	public static void Protocol_store_speed_up_Req(int _stage, int _storeIdx, int _jewel, Action _onCallBack)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "store_speed_up");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("stage", _stage);
		jSONObject2.AddField("storeIdx", _storeIdx);
		jSONObject2.AddField("jewel", _jewel);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_store_speed_up_Res);

	}

	public static void Protocol_store_upgrade_Req(int _storeIdx, int _storeTier, Action<string> _onCallBack)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Floor_Upgrade = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "store_upgrade");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("storeIdx", _storeIdx);
		jSONObject2.AddField("storeTier", _storeTier);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set store_upgrade = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_store_upgrade_Res);

	}

	public static void Protocol_shop_list_Req(Action _onCallBack)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "shop_list");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set shop_list = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_shop_list_Res);

	}

	public static void Protocol_shop_buy_daily_Req(int productIdx, int buyCount, Action _onCallBack)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "shop_buy_daily");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("productIdx", productIdx);
		jSONObject2.AddField("buyCount", buyCount);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set shop_buy_daily = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_shop_buy_daily_Res);

	}

	public static void Protocol_shop_buy_coin_Req(int productIdx, Action _onCallBack)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "shop_buy_coin");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("productIdx", productIdx);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set shop_buy_coin = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_shop_buy_coin_Res);

	}

	public static void Protocol_shop_buy_jewel_Req(int productIdx, Action _onCallBack)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "shop_buy_jewel");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("productIdx", productIdx);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set shop_buy_jewel = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_shop_buy_jewel_Res);

	}

	public static void Protocol_shop_popup_hunter_buy_coin_Req(int _hunterIdx, int _targetLevel, int _needJewel, Action _onCallBack)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "shop_popup_hunter_buy_coin");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("hunterIdx", _hunterIdx);
		jSONObject2.AddField("targetLevel", _targetLevel);
		jSONObject2.AddField("needJewel", _needJewel);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set shop_popup_hunter_buy_coin = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_shop_popup_hunter_buy_coin_Res);

	}

	public static void Protocol_shop_popup_hunter_promotion_buy_coin_Req(int _hunterIdx, int _needJewel, Action _onCallBack)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "shop_popup_hunter_promotion_buy_coin");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("hunterIdx", _hunterIdx);
		jSONObject2.AddField("needJewel", _needJewel);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set shop_popup_hunter_promotion_buy_coin = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_shop_popup_hunter_promotion_buy_coin_Res);

	}

	public static void Protocol_shop_popup_store_buy_coin_Req(int _storeIdx, int _needJewel, Action _onCallBack)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "shop_popup_store_buy_coin");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("storeIdx", _storeIdx);
		jSONObject2.AddField("needJewel", _needJewel);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set shop_popup_store_buy_coin = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_shop_popup_store_buy_coin_Res);

	}

	public static void Protocol_shop_package_list_Req(Action<SHOP_PACKAGE_LIST_RESULT> _onCallBack)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Shop_Package_List = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "shop_package_list");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set shop_package_list = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_shop_package_list_Res);

	}

	public static void Protocol_shop_ad_energy_Req(int energyCount, Action _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "shop_ad_energy");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("energyCount", energyCount);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set shop_ad_energy = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_shop_ad_energy_Res);

	}

	public static void Protocol_shop_ad_energy_start_Req()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "shop_ad_energy_start");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set shop_ad_energy_start = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, null, isLoading: false);

	}

	public static void Protocol_shop_buy_energy_Req(Action _onCallBack)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "shop_buy_energy");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set shop_buy_energy = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_shop_buy_energy_Res);

	}

	public static void Protocol_user_default_info_Req(Action _onCallBack = null, bool isLoading = true)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "user_default_info");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set user_default_info = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_user_default_info_Res, isLoading);

	}

	public static void Protocol_shop_buy_product_Req(int productId, string signature, string signedData, Action _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "shop_buy_product");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		switch (BuildSet.CurrentPlatformType)
		{
			case PlatformType.aos:
				jSONObject2.AddField("device", "aos");
				break;
			case PlatformType.ios:
				jSONObject2.AddField("device", "ios");
				break;
		}
		jSONObject2.AddField("purchaseType", "jewel");
		jSONObject2.AddField("productIdx", productId);
		jSONObject2.AddField("signature", signature);
		jSONObject2.AddField("signedData", signedData);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set shop_buy_product = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_shop_buy_product_Res);

	}

	public static void Protocol_shop_buy_package_Req(int productType, string signature, string signedData, Action<ChestListDbData[]> _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst._onCallBackBuyPackage = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "shop_buy_package");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		switch (BuildSet.CurrentPlatformType)
		{
			case PlatformType.aos:
				jSONObject2.AddField("device", "aos");
				break;
			case PlatformType.ios:
				jSONObject2.AddField("device", "ios");
				break;
		}
		jSONObject2.AddField("purchaseType", "package");
		jSONObject2.AddField("productType", productType);
		jSONObject2.AddField("signature", signature);
		jSONObject2.AddField("signedData", signedData);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set shop_buy_package = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_shop_buy_package_Res);

	}

	public static void Protocol_shop_ad_start_Req(int adType, Action<int> _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Ad_Start = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "shop_ad_start");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("adType", adType);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set shop_ad_start = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_shop_ad_start_Res);

	}

	public static void Protocol_chest_ad_start_Req(Action _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "chest_ad_start");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set chest_ad_start = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_chest_ad_start_Res, isLoading: false);

	}

	public static void Protocol_arena_info_Req(Action<ARENA_INFO_DATA_RESULT> _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Info_Data = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "arena_info");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set arena_info = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_arena_info_Res);
	}

	public static void Protocol_arena_game_start_Req(int levelIdx, List<int> _listBoostItem, Action<ARENA_GAME_START_RESULT> _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Game_Start_Data = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "arena_game_start");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("levelIdx", levelIdx);
		JSONObject jSONObject3 = new JSONObject();
		if (_listBoostItem != null && _listBoostItem.Count > 0)
		{
			for (int i = 0; i < _listBoostItem.Count; i++)
			{
				jSONObject3.Add(_listBoostItem[i]);
			}
			jSONObject2.AddField("boosterIdxArray", jSONObject3);
		}
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set arena_game_start = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_arena_game_start_Res, isLoading: true, isSceneLoading: true);
	}

	public static void Protocol_arena_game_end_Req(int levelIdx, int gameKey, int result, int resultReason, int wave, Action<ARENA_GAME_END_RESULT> _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Game_End_Data = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "arena_game_end");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("levelIdx", levelIdx);
		jSONObject2.AddField("gameKey", gameKey);
		jSONObject2.AddField("result", result);
		jSONObject2.AddField("resultReason", resultReason);
		jSONObject2.AddField("wave", wave);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set arena_game_end = " + jSONObject);

		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_arena_game_end_Res, isLoading: true, resultReason == 2);
	}

	public static void Protocol_arena_game_continue_Req(Action _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "arena_game_continue");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("gameKey", GameInfo.userPlayData.gameKey);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set arena_game_continue = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_arena_game_continue_Res);
	}

	public static void Protocol_arena_buy_ticket_Req(Action _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Ticket = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "arena_buy_ticket");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set arena_buy_ticket = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_arena_buy_ticket_Res);
	}

	public static void Protocol_arena_store_list_Req(Action<ARENA_STORE_INFO[]> _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Store_Info_Data = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "arena_store_list");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set arena_store_list = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_arena_store_list_Res);
	}

	public static void Protocol_arena_store_buy_product_Req(int productIdx, Action _onCallBack = null)
	{
		if (_onCallBack != null)
		{
			GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = _onCallBack;
		}
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("cmd", "arena_store_buy_product");
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.AddField("caUid", GameInfo.caUid);
		jSONObject2.AddField("productIdx", productIdx);
		jSONObject.AddField("params", jSONObject2);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("commands", jSONObject.ToString());
		UnityEngine.Debug.Log("@LOG Protocol_Set arena_store_buy_product = " + jSONObject);
		GameObjectSingleton<Protocol_Set>.Inst.CallforByBestHTTP(dictionary, GameObjectSingleton<Protocol_Set>.Inst.Protocol_arena_store_buy_product_Res);
	}

	public static void Send_Remain_Protocol()
	{
		GameObjectSingleton<Protocol_Set>.Inst.Send_Reservation_Protocol();
	}

#if ENABLE_HTTP_BEST
	private void Protocol_check_version_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set check_version : " + response.DataAsText.ToString());
			if(response != null && !string.IsNullOrEmpty(response.DataAsText))
			{
				DMCHelper.SaveFile(response.DataAsText, "data/check_version.json");
			}
			JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			CHECK_VERSION[] array = JsonConvert.DeserializeObject<CHECK_VERSION[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode))
			{
				if (array[0].result.force_update == 2)
				{
					InitSocialUser();
				}
				else
				{
					updateNotice.Show(array[0].result);
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_check_auth_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set check_auth : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/check_auth.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject != null)
			{
				CHECK_AUTH[] array = JsonConvert.DeserializeObject<CHECK_AUTH[]>(response.DataAsText.ToString());
				if (!CheckServerErrorCode(array[0].errorCode))
				{
					GameInfo.caUid = array[0].result.caUid;
					Protocol_user_get_tutorial_Req();
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_user_get_tutorial_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set user_get_tutorial : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/user_get_tutorial.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_GET_TUTORIAL_DATA[] array = JsonConvert.DeserializeObject<USER_GET_TUTORIAL_DATA[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				UnityEngine.Debug.Log("@LOG Protocol_Set Protocol_user_get_tutorial_Res :: " + array.Length);
				UnityEngine.Debug.Log("@LOG Protocol_Set Protocol_user_get_tutorial_Res ::: " + array[0].result);
				TutorialManager.SetTutorialData(array[0].result);
				GameDataManager.StartGame();
				if (onCallBack_Default != null)
				{
					onCallBack_Default();
					onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_user_set_tutorial_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set user_set_tutorial : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/user_set_tutorial.json");
            }
        }
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
		if (num == 0 && OnCallBack_Tutorial != null)
		{
			OnCallBack_Tutorial();
			OnCallBack_Tutorial = null;
		}
	}

	private void Protocol_user_info_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set user_info : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/user_info.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateUserData();
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_info != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_info();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_info = null;
				}
				LobbyManager.CheckHunterAlert();
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_user_item_info_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set user_item_info : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/user_item_info.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_ITEM_INFO[] array = JsonConvert.DeserializeObject<USER_ITEM_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result.userItemInfo);
				GameDataManager.UpdateUserData();
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Item_info != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Item_info();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Item_info = null;
				}
				LobbyManager.CheckHunterAlert();
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_user_daily_bonus_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set user_daily_bonus : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/user_daily_bonus.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject != null)
			{
				USER_DAILY_BONUS[] array = JsonConvert.DeserializeObject<USER_DAILY_BONUS[]>(response.DataAsText.ToString());
				if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update) && GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Daily_Bonus != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Daily_Bonus(array[0].result);
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Daily_Bonus = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_user_get_daily_bonus_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set user_get_daily_bonus : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/user_get_daily_bonus.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateUserData();
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Daily_Bonus_Collect != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Daily_Bonus_Collect();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Daily_Bonus_Collect = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_user_chapter_open_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set user_chapter_open : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/user_chapter_open.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateUserData();
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_info != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_info();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_info = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_game_start_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set game_start : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/game_start.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			GAME_START[] array = JsonConvert.DeserializeObject<GAME_START[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameInfo.userData.userInfo = array[0].result.userInfo;
				GameInfo.userPlayData.gameKey = array[0].result.gameKey;
				GameDataManager.UpdateUserData();
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_game_end_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set game_end : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/game_end.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject != null)
			{
				GAME_END[] array = JsonConvert.DeserializeObject<GAME_END[]>(response.DataAsText.ToString());
				if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update) && GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Game_End != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Game_End(array[0].result);
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Game_End = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_game_continue_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set game_continue : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/game_continue.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateUserData();
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_game_quick_loot_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set game_quick_loot : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/game_quick_loot.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject != null)
			{
				GAME_QUICK_LOOT[] array = JsonConvert.DeserializeObject<GAME_QUICK_LOOT[]>(response.DataAsText.ToString());
				if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update) && GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Quick_Loot != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Quick_Loot(array[0]);
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Quick_Loot = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_game_quick_loot_speed_up_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set game_quick_loot_speed_up : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/game_quick_loot_speed_up.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateUserData();
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_game_chapter_collect_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set game_chapter_collect : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/game_chapter_collect.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_game_level_remain_time_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set game_level_remain_time : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/game_level_remain_time.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject != null)
			{
				GAME_LEVEL_REAMAIN_TIME[] array = JsonConvert.DeserializeObject<GAME_LEVEL_REAMAIN_TIME[]>(response.DataAsText.ToString());
				if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update) && GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Level_Remain_Time != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Level_Remain_Time(array[0].result);
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Level_Remain_Time = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_chest_collect_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set chest_collect : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/chest_collect.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject != null)
			{
				CHEST_COLLECT[] array = JsonConvert.DeserializeObject<CHEST_COLLECT[]>(response.DataAsText.ToString());
				if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update) && GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Chest != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Chest(array[0].result.rewardList);
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Chest = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_chest_req_reward_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set chest_req_reward : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/chest_req_reward.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateUserData();
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_hunter_change_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set hunter_change : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/hunter_change.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateUserData();
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_hunter_is_not_new_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set hunter_is_not_new : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/hunter_is_not_new.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateUserData();
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_hunter_level_up_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set hunter_level_up : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/hunter_level_up.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateUserData();
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_hunter_promotion_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set hunter_promotion : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/hunter_promotion.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateUserData();
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_store_open_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set store_open : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/store_open.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateStoreData();
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_store_collect_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set store_collect : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/store_collect.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateStoreData();
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_store_speed_up_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set store_speed_up : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/store_speed_up.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateStoreData();
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_store_upgrade_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set store_upgrade : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/store_upgrade.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (CheckServerErrorCode(array[0].errorCode) || CheckForceUpdate(array[0].force_update))
			{
				return;
			}
			GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
			if (GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Floor_Upgrade != null)
			{
				GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Floor_Upgrade(array[0].result.forceCollectYn);
				GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Floor_Upgrade = null;
				if (array[0].result.forceCollectYn == "y")
				{
					return;
				}
			}
			GameDataManager.UpdateUserData();
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_shop_list_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set shop_list : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/shop_list.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			SHOP_LIST[] array = JsonConvert.DeserializeObject<SHOP_LIST[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameInfo.userData.userDailyItemList = array[0].result;
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
				GameDataManager.UpdateUserData();
				if (GameInfo.userData.userInfo.dailyShopNewYn.Equals("y"))
				{
					Protocol_user_default_info_Req();
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_shop_buy_daily_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set shop_buy_daily : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/shop_buy_daily.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			SHOP_BUY_DAILY[] array = JsonConvert.DeserializeObject<SHOP_BUY_DAILY[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameInfo.userData.userInfo = array[0].result.userInfo;
				GameInfo.userData.userItemList = array[0].result.userItemList;
				GameInfo.userData.userDailyItemList.dailyShopInfo.dailyShopList = array[0].result.dailyShopList;
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
				GameDataManager.UpdateUserData();
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_shop_buy_coin_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set shop_buy_coin : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/shop_buy_coin.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
				GameDataManager.UpdateUserData();
				LobbyManager.GetCoinEff(Vector3.zero);
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_shop_buy_jewel_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set shop_buy_jewel : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/shop_buy_jewel.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
				GameDataManager.UpdateUserData();
				LobbyManager.GetCoinEff(Vector3.zero);
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_shop_popup_hunter_buy_coin_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set shop_popup_hunter_buy_coin : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/shop_popup_hunter_buy_coin.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateUserData();
				LobbyManager.GetCoinEff(Vector3.zero);
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_shop_popup_hunter_promotion_buy_coin_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set shop_popup_hunter_promotion_buy_coin : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/shop_popup_hunter_promotion_buy_coin.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateUserData();
				LobbyManager.GetCoinEff(Vector3.zero);
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_shop_popup_store_buy_coin_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set shop_popup_store_buy_coin : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/shop_popup_store_buy_coin.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateUserData();
				LobbyManager.GetCoinEff(Vector3.zero);
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_shop_package_list_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set shop_package_list : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/shop_package_list.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject != null)
			{
				SHOP_PACKAGE_LIST[] array = JsonConvert.DeserializeObject<SHOP_PACKAGE_LIST[]>(response.DataAsText.ToString());
				if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update) && GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Shop_Package_List != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Shop_Package_List(array[0].result);
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Shop_Package_List = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_shop_ad_energy_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set shop_ad_energy : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/shop_ad_energy.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_shop_buy_energy_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set shop_buy_energy : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/shop_buy_energy.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
				GameDataManager.UpdateUserData();
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_user_default_info_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set user_default_info : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/user_default_info.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_DEFAULT_INFO[] array = JsonConvert.DeserializeObject<USER_DEFAULT_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode))
			{
				GameInfo.userData.userInfo = array[0].result.userDefaultInfo;
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
				GameDataManager.UpdateUserData();
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_shop_buy_product_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set shop_buy_product : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/shop_buy_product.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_DEFAULT_INFO[] array = JsonConvert.DeserializeObject<USER_DEFAULT_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameInfo.userData.userInfo = array[0].result.userDefaultInfo;
				if (array[0].success.Equals("true") && GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
				GameDataManager.UpdateUserData();
				LobbyManager.GetJewelEff(Vector3.zero);
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_shop_buy_package_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set shop_buy_package : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/shop_buy_package.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			SHOP_BUY_PACKAGE[] array = JsonConvert.DeserializeObject<SHOP_BUY_PACKAGE[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result.userData);
				if (array[0].success.Equals("true") && GameObjectSingleton<Protocol_Set>.Inst._onCallBackBuyPackage != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst._onCallBackBuyPackage(array[0].result.rewardList);
					GameObjectSingleton<Protocol_Set>.Inst._onCallBackBuyPackage = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_shop_ad_start_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set shop_ad_start : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/shop_ad_startjson");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject != null)
			{
				SHOP_AD_START[] array = JsonConvert.DeserializeObject<SHOP_AD_START[]>(response.DataAsText.ToString());
				if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update) && array[0].success.Equals("true") && GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Ad_Start != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Ad_Start(array[0].result.adKey);
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Ad_Start = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_chest_popup_buy_coin_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set chest_popup_buy_coin : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/chest_popup_buy_coin.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateUserData();
				LobbyManager.GetCoinEff(Vector3.zero);
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_chest_ad_start_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set chest_ad_start : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/chest_ad_start.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject != null)
			{
				CHECK_SUCCESS[] array = JsonConvert.DeserializeObject<CHECK_SUCCESS[]>(response.DataAsText.ToString());
				if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update) && GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_arena_info_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set arena_info : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/arena_info.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			ARENA_INFO_DATA[] array = JsonConvert.DeserializeObject<ARENA_INFO_DATA[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameInfo.userData.userInfo.arenaTicket = array[0].result.userArenaInfo.arenaTicket;
				GameInfo.userData.userInfo.arenaPoint = array[0].result.userArenaInfo.arenaPoint;
				GameInfo.userData.userHunterList = array[0].result.userHunterList;
				if (GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Info_Data != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Info_Data(array[0].result);
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Info_Data = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_arena_game_start_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set arena_game_start : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/arena_game_start.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			ARENA_GAME_START[] array = JsonConvert.DeserializeObject<ARENA_GAME_START[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameInfo.userPlayData.gameKey = array[0].result.gameKey;
				if (GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Game_Start_Data != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Game_Start_Data(array[0].result);
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Game_Start_Data = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_arena_game_end_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set arena_game_end : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/arena_game_end.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject != null)
			{
				ARENA_GAME_END[] array = JsonConvert.DeserializeObject<ARENA_GAME_END[]>(response.DataAsText.ToString());
				if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update) && GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Game_End_Data != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Game_End_Data(array[0].result);
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Game_End_Data = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_arena_game_continue_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set arena_game_continue : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/arena_game_continue.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateUserData();
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_arena_buy_ticket_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set arena_buy_ticket : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/arena_buy_ticket.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				if (GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Ticket != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Ticket();
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Ticket = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_arena_store_list_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set arena_store_list : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/arena_store_list.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject != null)
			{
				ARENA_STORE_LIST[] array = JsonConvert.DeserializeObject<ARENA_STORE_LIST[]>(response.DataAsText.ToString());
				if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update) && GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Store_Info_Data != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Store_Info_Data(array[0].result.arenaStoreInfo);
					GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Store_Info_Data = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private void Protocol_arena_store_buy_product_Res(HTTPRequest request, HTTPResponse response)
	{
		RemoveConnectRequset(request);
		int num = 0;
		UnityEngine.Debug.Log("@LOG Protocol_Set " + request.State.ToString());
		num = Get_ErrorCode(request);
		if (num == 0)
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set arena_store_buy_product : " + response.DataAsText.ToString());
            if (response != null && !string.IsNullOrEmpty(response.DataAsText))
            {
                DMCHelper.SaveFile(response.DataAsText, "data/arena_store_buy_product.json");
            }
            JSONObject jSONObject = new JSONObject(response.DataAsText);
			if (jSONObject == null)
			{
				return;
			}
			USER_INFO[] array = JsonConvert.DeserializeObject<USER_INFO[]>(response.DataAsText.ToString());
			if (!CheckServerErrorCode(array[0].errorCode) && !CheckForceUpdate(array[0].force_update))
			{
				GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
				GameDataManager.UpdateUserData();
				if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
				{
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
					GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
				}
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set error = " + num);
		}
	}

	private int Get_ErrorCode(HTTPRequest request, bool isLoadingCheck = true)
	{
		int num = 0;
		return num;
		switch (request.State)
		{
		case HTTPRequestStates.Finished:
			if (request.Response.IsSuccess)
			{
				num = 0;
				Sended_Protocol = null;
			}
			else
			{
				num = -1;
			}
			break;
		case HTTPRequestStates.Error:
			num = -1;
			break;
		case HTTPRequestStates.ConnectionTimedOut:
			num = 1;
			break;
		case HTTPRequestStates.TimedOut:
			num = 1;
			break;
		}
		if (num != 0)
		{
			networkError.Show();
		}
		return num;
	}

	private void CallforByBestHTTP(Dictionary<string, string> _obj, OnRequestFinishedDelegate callBack, bool isLoading = true, bool isSceneLoading = false)
	{
		HTTPRequest hTTPRequest = null;
		hTTPRequest = new HTTPRequest(new Uri(BuildSet.serverURL), HTTPMethods.Post, callBack);
		foreach (KeyValuePair<string, string> item in _obj)
		{
			if (item.Value == null)
			{
				UnityEngine.Debug.LogWarning(item.Key + " value is null");
			}
			else
			{
				hTTPRequest.AddField(item.Key, item.Value);
			}
		}
		if (IRVManager.CurrentNetStatus == InternetReachabilityVerifier.Status.NetVerified)
		{
			hTTPRequest.Send();
			GameObjectSingleton<Protocol_Set>.Inst.Sended_Protocol = hTTPRequest;
			if (isLoading)
			{
				listConnect.Add(hTTPRequest);
				GameDataManager.ShowNetworkLoading(isSceneLoading);
			}
			if (coroutineNetworkDelay != null)
			{
				StopCoroutine(coroutineNetworkDelay);
				coroutineNetworkDelay = null;
			}
		}
		else
		{
			UnityEngine.Debug.Log("@LOG Protocol_Set Network Error!!!!!!!");
			if (coroutineNetworkDelay != null)
			{
				coroutineNetworkDelay = StartCoroutine(CheckNetworkDelay());
			}
			GameObjectSingleton<Protocol_Set>.Inst.Reservation_Protocol.Add(hTTPRequest);
			listConnect.Clear();
		}
	}

	private void RemoveConnectRequset(HTTPRequest _request)
	{
		UnityEngine.Debug.Log("@LOG Protocol_Set RemoveConnectRequset :: start :: " + listConnect.Count);
		listConnect.Remove(_request);
		UnityEngine.Debug.Log("@LOG Protocol_Set RemoveConnectRequset :: end :: " + listConnect.Count);
		if (listConnect.Count == 0)
		{
			GameDataManager.HideNetworkLoading();
		}
	}
#else
    private void Protocol_check_version_Res(System.Object request, System.Object response)
    {
        RemoveConnectRequset(request);
        InitSocialUser();
    }


	private void Protocol_check_auth_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        Protocol_user_get_tutorial_Req();
    }

    private void Protocol_user_get_tutorial_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
		//TutorialManager.SetTutorialData(array[0].result);
		GameDataManager.StartGame();
        if (onCallBack_Default != null)
        {
            onCallBack_Default();
            onCallBack_Default = null;
        }
    }

	private void Protocol_user_set_tutorial_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        if (OnCallBack_Tutorial != null)
        {
            OnCallBack_Tutorial();
            OnCallBack_Tutorial = null;
        }
    }

	private void Protocol_user_info_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateUserData();
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_info != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_info();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_info = null;
        }
        LobbyManager.CheckHunterAlert();
    }

	private void Protocol_user_item_info_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result.userItemInfo);
        GameDataManager.UpdateUserData();
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Item_info != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Item_info();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Item_info = null;
        }
        LobbyManager.CheckHunterAlert();
    }

	private void Protocol_user_daily_bonus_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Daily_Bonus != null)
        {
            //GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Daily_Bonus(array[0].result);
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Daily_Bonus = null;
        }
    }

	private void Protocol_user_get_daily_bonus_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
		int num = 0;
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateUserData();
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Daily_Bonus_Collect != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Daily_Bonus_Collect();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_Daily_Bonus_Collect = null;
        }
    }

	private void Protocol_user_chapter_open_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateUserData();
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_info != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_info();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_User_info = null;
        }
    }

	private void Protocol_game_start_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameInfo.userData.userInfo = array[0].result.userInfo;
        //GameInfo.userPlayData.gameKey = array[0].result.gameKey;
        GameDataManager.UpdateUserData();
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_game_end_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        if (GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Game_End != null)
        {
            //GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Game_End(array[0].result);
            GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Game_End = null;
        }
    }

	private void Protocol_game_continue_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateUserData();
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_game_quick_loot_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        if (GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Quick_Loot != null)
        {
            //GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Quick_Loot(array[0]);
            GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Quick_Loot = null;
        }
    }

	private void Protocol_game_quick_loot_speed_up_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateUserData();
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_game_chapter_collect_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_game_level_remain_time_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        if (GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Level_Remain_Time != null)
        {
            //GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Level_Remain_Time(array[0].result);
            GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Level_Remain_Time = null;
        }
    }

	private void Protocol_chest_collect_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Chest != null)
        {
            //GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Chest(array[0].result.rewardList);
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Chest = null;
        }
    }

	private void Protocol_chest_req_reward_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateUserData();
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_hunter_change_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateUserData();
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_hunter_is_not_new_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateUserData();
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_hunter_level_up_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateUserData();
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_hunter_promotion_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateUserData();
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_store_open_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateStoreData();
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_store_collect_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateStoreData();
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_store_speed_up_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateStoreData();
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_store_upgrade_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        if (GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Floor_Upgrade != null)
        {
            //GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Floor_Upgrade(array[0].result.forceCollectYn);
            GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Floor_Upgrade = null;
            //if (array[0].result.forceCollectYn == "y")
            //{
            //    return;
            //}
        }
        GameDataManager.UpdateUserData();
    }

	private void Protocol_shop_list_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameInfo.userData.userDailyItemList = array[0].result;
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
        GameDataManager.UpdateUserData();
        if (GameInfo.userData.userInfo.dailyShopNewYn.Equals("y"))
        {
            Protocol_user_default_info_Req();
        }
    }

	private void Protocol_shop_buy_daily_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameInfo.userData.userInfo = array[0].result.userInfo;
        //GameInfo.userData.userItemList = array[0].result.userItemList;
        //GameInfo.userData.userDailyItemList.dailyShopInfo.dailyShopList = array[0].result.dailyShopList;
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
        GameDataManager.UpdateUserData();
    }

	private void Protocol_shop_buy_coin_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
        GameDataManager.UpdateUserData();
        LobbyManager.GetCoinEff(Vector3.zero);
    }

	private void Protocol_shop_buy_jewel_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
        GameDataManager.UpdateUserData();
        LobbyManager.GetCoinEff(Vector3.zero);
    }

	private void Protocol_shop_popup_hunter_buy_coin_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateUserData();
        LobbyManager.GetCoinEff(Vector3.zero);
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_shop_popup_hunter_promotion_buy_coin_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateUserData();
        LobbyManager.GetCoinEff(Vector3.zero);
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_shop_popup_store_buy_coin_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateUserData();
        LobbyManager.GetCoinEff(Vector3.zero);
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_shop_package_list_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
		if (GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Shop_Package_List != null)
        {
            //GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Shop_Package_List(array[0].result);
            GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Shop_Package_List = null;
        }
    }

	private void Protocol_shop_ad_energy_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_shop_buy_energy_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
        GameDataManager.UpdateUserData();
    }

	private void Protocol_user_default_info_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameInfo.userData.userInfo = array[0].result.userDefaultInfo;
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
        GameDataManager.UpdateUserData();
    }

	private void Protocol_shop_buy_product_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameInfo.userData.userInfo = array[0].result.userDefaultInfo;
        if (/*array[0].success.Equals("true") && */GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
        GameDataManager.UpdateUserData();
        LobbyManager.GetJewelEff(Vector3.zero);
    }

	private void Protocol_shop_buy_package_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result.userData);
        if (/*array[0].success.Equals("true") && */GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Buy_Package != null)
        {
            //GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Buy_Package(array[0].result.rewardList);
            GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Buy_Package = null;
        }
    }

	private void Protocol_shop_ad_start_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
		if (/*array[0].success.Equals("true") &&*/ GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Ad_Start != null)
        {
            //GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Ad_Start(array[0].result.adKey);
            GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Ad_Start = null;
        }
    }

	private void Protocol_chest_popup_buy_coin_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateUserData();
        LobbyManager.GetCoinEff(Vector3.zero);
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_chest_ad_start_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
		if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_arena_info_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameInfo.userData.userInfo.arenaTicket = array[0].result.userArenaInfo.arenaTicket;
        //GameInfo.userData.userInfo.arenaPoint = array[0].result.userArenaInfo.arenaPoint;
        //GameInfo.userData.userHunterList = array[0].result.userHunterList;
        if (GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Info_Data != null)
        {
            //GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Info_Data(array[0].result);
            GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Info_Data = null;
        }
    }

	private void Protocol_arena_game_start_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameInfo.userPlayData.gameKey = array[0].result.gameKey;
        if (GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Game_Start_Data != null)
        {
            //GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Game_Start_Data(array[0].result);
            GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Game_Start_Data = null;
        }
    }

	private void Protocol_arena_game_end_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
		if (GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Game_End_Data != null)
        {
            //GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Game_End_Data(array[0].result);
            GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Game_End_Data = null;
        }
    }

	private void Protocol_arena_game_continue_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateUserData();
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private void Protocol_arena_buy_ticket_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        if (GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Ticket != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Ticket();
            GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Ticket = null;
        }
    }

	private void Protocol_arena_store_list_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
		if (GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Store_Info_Data != null)
        {
            //GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Store_Info_Data(array[0].result.arenaStoreInfo);
            GameObjectSingleton<Protocol_Set>.Inst.OnCallBack_Arena_Store_Info_Data = null;
        }
    }

	private void Protocol_arena_store_buy_product_Res(System.Object request, System.Object response)
	{
		RemoveConnectRequset(request);
        //GameObjectSingleton<Protocol_Set>.Inst.InsertUserData(array[0].result);
        GameDataManager.UpdateUserData();
        if (GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default != null)
        {
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default();
            GameObjectSingleton<Protocol_Set>.Inst.onCallBack_Default = null;
        }
    }

	private int Get_ErrorCode(System.Object request, bool isLoadingCheck = true)
	{
		int num = 0;
		if (num != 0)
		{
			networkError.Show();
		}
		return num;
	}

    private void CallforByBestHTTP(Dictionary<string, string> _obj, System.Action<System.Object, System.Object> callBack,
		bool isLoading = true, bool isSceneLoading = false)
    {
		callBack?.Invoke(null, null);

	}

    private void RemoveConnectRequset(System.Object _request)
    {
		GameDataManager.HideNetworkLoading();
	}
#endif

    private IEnumerator CheckNetworkDelay()
	{
		yield return new WaitForSeconds(2f);
		if (IRVManager.CurrentNetStatus != InternetReachabilityVerifier.Status.NetVerified)
		{
			//networkError.Show();
		}
	}

	private void Send_Reservation_Protocol()
	{
		networkError.Hide();
#if ENABLE_HTTP_BEST
		if (IRVManager.CurrentNetStatus == InternetReachabilityVerifier.Status.NetVerified)
		{
			if (Sended_Protocol != null)
			{
				Sended_Protocol.Send();
				Sended_Protocol = Sended_Protocol;
			}
			else if (Reservation_Protocol.Count > 0)
			{
				StartCoroutine(Send_Reservation_Protocol_Coroutine());
			}
		}
#endif
	}

	private IEnumerator Send_Reservation_Protocol_Coroutine()
	{
#if ENABLE_HTTP_BEST
		for (int i = 0; i < Reservation_Protocol.Count; i++)
		{
			Reservation_Protocol[i].Send();
			yield return new WaitForSeconds(0.1f);
		}
		Reservation_Protocol.Clear();
#else
		return null;
#endif
	}

	private bool CheckServerErrorCode(int _errorCode)
	{
		return false;//TODO remove
		if (_errorCode == 0)
		{
			return false;
		}
		networkError.Show();
		return true;
	}

	private bool CheckForceUpdate(CHECK_VERSION_RESULT[] data)
	{
		if (data != null && data.Length > 0)
		{
			updateNotice.Show(data[0]);
			return true;
		}
		return false;
	}

	private void InsertUserData(UserData _userData)
	{
		if (_userData.userInfo != null)
		{
			GameInfo.userData.userInfo = _userData.userInfo;
		}
		if (_userData.userHunterList != null)
		{
			GameInfo.userData.userHunterList = _userData.userHunterList;
		}
		if (_userData.userItemList != null)
		{
			GameInfo.userData.userItemList = _userData.userItemList;
		}
		if (_userData.userStageState != null)
		{
			GameInfo.userData.userStageState = _userData.userStageState;
		}
		if (_userData.userFloorState != null)
		{
			GameInfo.userData.userFloorState = _userData.userFloorState;
		}
		if (_userData.userDailyItemList != null)
		{
			GameInfo.userData.userDailyItemList = _userData.userDailyItemList;
		}
		GameUtil.SetUseHunterList();
		GameUtil.SetUseArenaHunterList();
	}

	private void InitSocialUser()
	{
		//MWPlatformService.LoginResult = OnSocialLoginResultEvent;
		//MWPlatformService.Init();
		OnSocialLoginResultEvent(false, "START");
	}

	private void OnSocialLoginResultEvent(bool isSuccess, string errorMessage)
	{
		if (isSuccess)
		{
			GameInfo.sUid = MWPlatformService.UserId;
			PlayerPrefs.SetString("SocialLogin", "y");
		}
		else if (PlayerPrefs.GetString("SocialLogin") == "y")
		{
			loginFailPopup.Show();
			return;
		}
		Protocol_check_auth_Req();
		UnityEngine.Debug.Log("@LOG Protocol_Set OnSocialLoginResultEvent - isSuccess :: " + isSuccess + ", errorMessage :: " + errorMessage);
		UnityEngine.Debug.Log("suid :: " + GameInfo.sUid);
	}

	private int GetPlatformType()
	{
		UnityEngine.Debug.Log("@LOG Protocol_Set GetPlatformType - " + GameInfo.sUid);
		if (GameInfo.sUid != null && GameInfo.sUid != string.Empty)
		{
			switch (BuildSet.CurrentPlatformType)
			{
				case PlatformType.aos:
					return 2;
				case PlatformType.ios:
					return 3;
				default:
					return 1;
			}
		}
		return 1;
	}

	public void OnClickShowUserInfo()
	{
		textUserInfo.gameObject.SetActive(!textUserInfo.gameObject.activeSelf);
	}

	protected override void Awake()
	{
		base.Awake();
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus && coroutineNetworkDelay != null)
		{
			StopCoroutine(coroutineNetworkDelay);
			coroutineNetworkDelay = null;
		}
	}

	private void OnApplicationQuit()
	{
		UnityEngine.Debug.Log("@LOG Protocol_Set Session out");
		Protocol_check_logout_Req();
	}
}