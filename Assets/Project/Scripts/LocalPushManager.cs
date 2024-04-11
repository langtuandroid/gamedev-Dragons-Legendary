using UTNotifications;

public class LocalPushManager : GameObjectSingleton<LocalPushManager>
{
	public static void SetNotification(NotiType _type, int _delay = 0)
	{
		
		if (GamePreferenceManager.GetIsNotification())
		{
			UnityEngine.Debug.Log("######### SetNotification :: " + Manager.Instance);
			switch (_type)
			{
			case NotiType.Connect24:
				Manager.Instance.ScheduleNotification(86400, MWLocalize.GetData("local_push_install_h24_t"), MWLocalize.GetData("local_push_install_h24"), 1, null, "match_hero");
				break;
			case NotiType.Connect72:
				Manager.Instance.ScheduleNotification(259200, MWLocalize.GetData("local_push_install_h72_t"), MWLocalize.GetData("local_push_install_h72"), 2, null, "match_hero");
				break;
			case NotiType.DailyShopRefresh:
				Manager.Instance.ScheduleNotification(_delay, MWLocalize.GetData("local_push_reset_daily_shop_t"), MWLocalize.GetData("local_push_reset_daily_shop"), 5, null, "match_hero");
				break;
			case NotiType.FreeChest:
				Manager.Instance.ScheduleNotification(_delay, MWLocalize.GetData("local_push_comp_chest_t"), MWLocalize.GetData("local_push_comp_chest"), 4, null, "match_hero");
				break;
			case NotiType.FullEnergy:
				Manager.Instance.ScheduleNotification(_delay, MWLocalize.GetData("local_push_full_energy_t"), MWLocalize.GetData("local_push_full_energy"), 6, null, "match_hero");
				break;
			case NotiType.StoreReward:
				Manager.Instance.ScheduleNotification(_delay, MWLocalize.GetData("local_push_comp_store_t"), MWLocalize.GetData("local_push_comp_store"), 3, null, "match_hero");
				break;
			}
		}
	}

	public static void CancelNotification(NotiType _type)
	{
		Manager.Instance.CancelNotification((int)_type);
	}

	public static void CancelAllNotification()
	{
		Manager.Instance.CancelAllNotifications();
	}

	private void ClearNotification()
	{
		Manager.Instance.SetBadge(0);
	}

	private void ConnectNoti_Cancel()
	{
		CancelNotification(NotiType.Connect24);
		CancelNotification(NotiType.Connect72);
	}

	private void ConnectNoti_Setting()
	{
		SetNotification(NotiType.Connect24);
		SetNotification(NotiType.Connect72);
	}

	private void Initialize()
	{
		bool flag = Manager.Instance.Initialize(willHandleReceivedNotifications: true);
		UnityEngine.Debug.Log("UTNotifications Initialize: " + flag);
		if (flag)
		{
			Manager.Instance.SetBadge(0);
		}
	}

	private void Start()
	{
		Initialize();
		ClearNotification();
		ConnectNoti_Cancel();
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			UnityEngine.Debug.Log("PAUSE");
			ConnectNoti_Setting();
			return;
		}
		UnityEngine.Debug.Log("RESUME");
		if (Manager.Instance.Initialized)
		{
			ConnectNoti_Cancel();
		}
	}

	private void OnApplicationQuit()
	{
		ConnectNoti_Setting();
	}
}
