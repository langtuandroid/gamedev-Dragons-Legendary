using System;

[Serializable]
public class UserInfoData
{
	public string iosUid;

	public string aosUid;

	public string awsUid;

	public int jewel;

	public int coin;

	public int level = 1;

	public int exp;

	public int energy;

	public int chestKey;

	public int maxEnergy;

	public int energyRemainTime;

	public int chestRemainTime;

	public int freeChestRemainTime;

	public int dailyShopRemainTime;

	public string levelUpYn = "n";

	public string dailyShopNewYn = "n";

	public int ad_energy_limit;

	public int ad_chest_limit;

	public int arenaPoint;

	public string arenaAlarmYn;

	public int arenaLevel;

	public int arenaTicket;

	public string oldUserYn = string.Empty;
}
