public class GameInfo
{
	public static bool isTutorial = false;

	public static bool isShowArenaSalePack = true;

	public static bool isResumeUserDataConnect = true;

	public static string sUid = string.Empty;

	public static int chargeEnergyAdsValue;

	public static int saleStartPackCheck = 0;

	public static int saleSpecialOfferCheck = 0;

	public static float inGameBattleSpeedRate = 1f;

	public static UserData userData = new UserData();

	public static UserPlayData userPlayData = new UserPlayData();

	public static PuzzlePlayData inGamePlayData = new PuzzlePlayData();

	public static SHOP_PACKAGE_LIST_DATA arenaPackageData;

	public static bool IS_MUSIC_SOUND = GamePreferenceManager.GetIsMusicSound();

	public static bool IS_EFFECT_SOUND = GamePreferenceManager.GetIsEffectSound();

	public static bool IS_VIBRATION = GamePreferenceManager.GetIsVibration();

	public static bool IS_NOTIFICATIONS = GamePreferenceManager.GetIsNotification();

	public static Language_Type CURRENTLANGUAGE = GamePreferenceManager.GetLanguage();

	public static bool IS_OS_LOGIN = false;

	public static int caUid = 11;

	public static SceneType currentSceneType;

	public static bool isForceRandomBlockPattern = false;

	public static bool isDirectBattleReward = false;

	public static bool isRate = false;

	public static string AD_ID = string.Empty;
}
