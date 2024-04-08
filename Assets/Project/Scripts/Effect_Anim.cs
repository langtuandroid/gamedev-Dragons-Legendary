using UnityEngine;

public class Effect_Anim : MonoBehaviour
{
	[SerializeField]
	private HunterLevelUp hunterLevelUp;

	[SerializeField]
	private HunterPromotionUp hunterPromotionUp;

	[SerializeField]
	private HunterPromotionEffect hunterPromotionEffect;

	[SerializeField]
	private ChestOpen chestOpen;

	[SerializeField]
	private ArenaChestOpen arenaChestOpen;

	[SerializeField]
	private BattleReward battleReward;

	public void End_LevelUp_Anim()
	{
		if (hunterLevelUp != null)
		{
			hunterLevelUp.End_LevelUp_Anim();
		}
	}

	public void End_PromotionUp_Anim()
	{
		if (hunterPromotionEffect != null)
		{
			hunterPromotionEffect.End_LevelUp_Anim();
		}
	}

	public void End_CardOpen_Anim()
	{
		if (chestOpen != null)
		{
			chestOpen.CardOpen_Anim_End();
		}
		if (arenaChestOpen != null)
		{
			arenaChestOpen.CardOpen_Anim_End();
		}
	}

	public void End_RewardOpen_Anim()
	{
		if (battleReward != null)
		{
			battleReward.End_BattleRewardOpen_Anim();
		}
	}
}
