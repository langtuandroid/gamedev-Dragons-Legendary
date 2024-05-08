using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PromotionEffect : MonoBehaviour
{
	[FormerlySerializedAs("promotionUP_Character_Pos")] [SerializeField]
	private Transform _promotionUpCharacterPos;

	[FormerlySerializedAs("promotionUP_Character")] [SerializeField]
	private Transform _promotionUpCharacter;

	[FormerlySerializedAs("promotionUP_Tier1")] [SerializeField]
	private Transform _promotionUpTier1;

	[FormerlySerializedAs("promotionUP_Tier2")] [SerializeField]
	private Transform _promotionUpTier2;

	[FormerlySerializedAs("promotionUP_Anim")] [SerializeField]
	private Animator _promotionUpAnim;

	[FormerlySerializedAs("promotionUp_BG")] [SerializeField]
	private Image _promotionUpBg;

	[FormerlySerializedAs("hunterPromotionUp")] [SerializeField]
	private HunterPromotionUp _hunterPromotionUp;

	public Transform PromotionUP_Character_Pos => _promotionUpCharacterPos;

	public Transform PromotionUP_Character => _promotionUpCharacter;

	public Transform PromotionUP_Tier1 => _promotionUpTier1;

	public Transform PromotionUP_Tier2 => _promotionUpTier2;

	public Animator PromotionUP_Anim => _promotionUpAnim;

	public Image PromotionUp_BG => _promotionUpBg;

	public void SetHunterPromotionUp(HunterPromotionUp _hunterPromotionUp)
	{
		this._hunterPromotionUp = _hunterPromotionUp;
	}

	public void End_LevelUp_Anim()
	{
		_hunterPromotionUp.End_LevelUp_Anim();
	}
}
