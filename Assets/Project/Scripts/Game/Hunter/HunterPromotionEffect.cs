using UnityEngine;
using UnityEngine.UI;

public class HunterPromotionEffect : MonoBehaviour
{
	[SerializeField]
	private Transform promotionUP_Character_Pos;

	[SerializeField]
	private Transform promotionUP_Character;

	[SerializeField]
	private Transform promotionUP_Tier1;

	[SerializeField]
	private Transform promotionUP_Tier2;

	[SerializeField]
	private Animator promotionUP_Anim;

	[SerializeField]
	private Image promotionUp_BG;

	[SerializeField]
	private HunterPromotionUp hunterPromotionUp;

	public Transform PromotionUP_Character_Pos => promotionUP_Character_Pos;

	public Transform PromotionUP_Character => promotionUP_Character;

	public Transform PromotionUP_Tier1 => promotionUP_Tier1;

	public Transform PromotionUP_Tier2 => promotionUP_Tier2;

	public Animator PromotionUP_Anim => promotionUP_Anim;

	public Image PromotionUp_BG => promotionUp_BG;

	public void SetHunterPromotionUp(HunterPromotionUp _hunterPromotionUp)
	{
		hunterPromotionUp = _hunterPromotionUp;
	}

	public void End_LevelUp_Anim()
	{
		hunterPromotionUp.End_LevelUp_Anim();
	}
}
