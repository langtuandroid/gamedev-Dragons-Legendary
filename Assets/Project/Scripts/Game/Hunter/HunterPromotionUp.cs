using Spine.Unity;
using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HunterPromotionUp : LobbyPopupBase
{
    public Action OnGoBackEvent;

    [FormerlySerializedAs("hunterCharactertr")] [SerializeField]
    private Transform _hunterCharactertr;

    [FormerlySerializedAs("hunter_Character")] [SerializeField]
    private HeroColor _hunterCharacter;

    [FormerlySerializedAs("hunterTiertr")] [SerializeField]
    private Transform _hunterTiertr;

    [FormerlySerializedAs("hunter_MaxLevel_Before")] [SerializeField]
    private Text _hunterMaxLevelBefore;

    [FormerlySerializedAs("hunter_MaxLevel_After")] [SerializeField]
    private Text _hunterMaxLevelAfter;

    [FormerlySerializedAs("hunter_Name")] [SerializeField]
    private Text _hunterName;

    [FormerlySerializedAs("hunter_Level")] [SerializeField]
    private Text _heroLevel;

    [FormerlySerializedAs("hunter_HP_Before")] [SerializeField]
    private Text _heroHpBefore;

    [FormerlySerializedAs("hunter_HP_After")] [SerializeField]
    private Text _heroHpAfter;

    [FormerlySerializedAs("hunter_Attack_Before")] [SerializeField]
    private Text _heroAttackBefore;

    [FormerlySerializedAs("hunter_Attack_After")] [SerializeField]
    private Text _heroAttackAfter;

    [FormerlySerializedAs("hunter_Recovery_Before")] [SerializeField]
    private Text _heroRecoveryBefore;

    [FormerlySerializedAs("hunter_Recovery_After")] [SerializeField]
    private Text _heroRecoveryAfter;
    
    private HunterInfo heroInfo;
    private PromotionEffect _promotionEffect;
    private Transform _promotionUpCharacterPos;
    private Transform _promotionUpCharacter;
    private Transform _promotionUpTier1;
    private Transform _promotionUpTier2;
    private Animator _promotionUpAnim;

    [FormerlySerializedAs("promotionUp_BG")] [SerializeField]
    private Image _promotionUpBg;

    public void ActivatPanel(HunterInfo _hunterInfo)
    {
        base.Open();
        base.gameObject.SetActive(value: true);
        Construct(_hunterInfo);
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void SetConfigure(HunterInfo _hunterInfo)
    {
        Construct(_hunterInfo);
    }

    public override void CloseProcessComplete()
    {
    }

    public void End_LevelUp_Anim()
    {
        _hunterCharacter.gameObject.SetActive(value: true);
        Transform transform = null;
        transform = MasterPoolManager.SpawnObject("Effect", "FX_Boom", base.transform, 2f);
        transform.SetAsLastSibling();
        _promotionEffect.gameObject.SetActive(value: false);
    }

    private void Construct(HunterInfo _hunterInfo)
    {
        heroInfo = _hunterInfo;
        if (_promotionEffect != null)
        {
            MasterPoolManager.ReturnToPool("Hunter", _promotionEffect.transform);
            _promotionEffect = null;
        }
        switch (heroInfo.Hunter.color)
        {
            case 0:
                _promotionEffect = MasterPoolManager.SpawnObject("Effect", "FX_PromotionUp_Blue", base.transform).GetComponent<PromotionEffect>();
                break;
            case 1:
                _promotionEffect = MasterPoolManager.SpawnObject("Effect", "FX_PromotionUp_Green", base.transform).GetComponent<PromotionEffect>();
                break;
            case 2:
                _promotionEffect = MasterPoolManager.SpawnObject("Effect", "FX_PromotionUp_Purple", base.transform).GetComponent<PromotionEffect>();
                break;
            case 3:
                _promotionEffect = MasterPoolManager.SpawnObject("Effect", "FX_PromotionUp_Red", base.transform).GetComponent<PromotionEffect>();
                break;
            case 4:
                _promotionEffect = MasterPoolManager.SpawnObject("Effect", "FX_PromotionUp_Yellow", base.transform).GetComponent<PromotionEffect>();
                break;
        }
        _promotionUpCharacterPos = _promotionEffect.PromotionUP_Character_Pos;
        _promotionUpCharacter = _promotionEffect.PromotionUP_Character;
        _promotionUpTier1 = _promotionEffect.PromotionUP_Tier1;
        _promotionUpTier2 = _promotionEffect.PromotionUP_Tier1;
        _promotionUpAnim = _promotionEffect.PromotionUP_Anim;
        _promotionEffect.SetHunterPromotionUp(this);
        switch (_hunterInfo.Stat.hunterTier)
        {
            case 2:
                _promotionUpTier1.gameObject.SetActive(value: true);
                _promotionUpTier2.gameObject.SetActive(value: false);
                break;
            case 3:
                _promotionUpTier1.gameObject.SetActive(value: false);
                _promotionUpTier2.gameObject.SetActive(value: true);
                break;
        }
        PromotionEffect();
        
        if (_hunterCharacter != null)
        {
            MasterPoolManager.ReturnToPool("Hunter", _hunterCharacter.transform);
            _hunterCharacter = null;
        }
        switch (heroInfo.Hunter.color)
        {
            case 0:
                _hunterCharacter = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg2_B2", _hunterCharactertr).GetComponent<HeroColor>();
                break;
            case 1:
                _hunterCharacter = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg2_G", _hunterCharactertr).GetComponent<HeroColor>();
                break;
            case 2:
                _hunterCharacter = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg2_P", _hunterCharactertr).GetComponent<HeroColor>();
                break;
            case 3:
                _hunterCharacter = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg2_R", _hunterCharactertr).GetComponent<HeroColor>();
                break;
            case 4:
                _hunterCharacter = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg2_Y", _hunterCharactertr).GetComponent<HeroColor>();
                break;
        }
        _hunterCharacter.transform.SetAsFirstSibling();
        _hunterCharacter.transform.localPosition = new Vector3(0f, 180f, 0f);
        _hunterCharacter.transform.localScale = new Vector3(1f, 1f, 1f);
        _hunterCharacter.Construct(_hunterInfo);
        _hunterCharacter.gameObject.SetActive(value: false);
        
        SetHeroData();
        SetStars();
    }

    private void SetStars()
    {
        for (int i = 0; i < _hunterTiertr.childCount; i++)
        {
            _hunterTiertr.GetChild(i).GetChild(0).gameObject.SetActive(value: false);
            _hunterTiertr.GetChild(i).gameObject.SetActive(value: false);
        }
        for (int j = 0; j < heroInfo.Hunter.maxTier; j++)
        {
            _hunterTiertr.GetChild(j).gameObject.SetActive(value: true);
            if (heroInfo.Stat.hunterTier >= j + 1)
            {
                _hunterTiertr.GetChild(j).GetChild(0).gameObject.SetActive(value: true);
            }
        }
    }

    private void PromotionEffect()
    {
        _promotionEffect.gameObject.SetActive(value: true);
        if (_promotionUpCharacter != null)
        {
            MasterPoolManager.ReturnToPool("Hunter", _promotionUpCharacter);
            _promotionUpCharacter = null;
        }
        _promotionUpCharacter = MasterPoolManager.SpawnObject("Hunter", heroInfo.Hunter.hunterIdx.ToString(), _promotionUpCharacterPos);
        _promotionUpCharacter.gameObject.SetActive(value: true);
        SetHunterImg();
        if (heroInfo.Hunter.hunterSize == 3)
        {
            _promotionUpCharacter.localScale = new Vector3(150f, 150f, 150f);
        }
        else if (heroInfo.Hunter.hunterSize == 2)
        {
            _promotionUpCharacter.localScale = new Vector3(200f, 200f, 200f);
        }
        else
        {
            _promotionUpCharacter.localScale = new Vector3(220f, 220f, 220f);
        }
        _promotionUpCharacter.localPosition = Vector3.zero;
        SoundController.EffectSound_Play(EffectSoundType.HunterPromotionUp);
        
        /*
        switch (hunterInfo.Hunter.color)
        {
            case 0:
                promotionUp_BG.color = new Color32(77, 122, 170, byte.MaxValue);
                break;
            case 1:
                promotionUp_BG.color = new Color32(123, 170, 77, byte.MaxValue);
                break;
            case 2:
                promotionUp_BG.color = new Color32(166, 122, 179, byte.MaxValue);
                break;
            case 3:
                promotionUp_BG.color = new Color32(190, 108, 101, byte.MaxValue);
                break;
            case 4:
                promotionUp_BG.color = new Color32(220, 179, 65, byte.MaxValue);
                break;
        }
        */
        
        switch (heroInfo.Stat.hunterTier)
        {
            case 2:
                _promotionUpAnim.ResetTrigger("Promotion1");
                _promotionUpAnim.SetTrigger("Promotion1");
                break;
            case 3:
                _promotionUpAnim.ResetTrigger("Promotion2");
                _promotionUpAnim.SetTrigger("Promotion2");
                break;
        }
    }

    private void SetHunterImg()
    {
        var anim = _promotionUpCharacter.GetChild(0).GetComponent<SkeletonAnimation>();

        if (!(anim == null))
        {
            switch (heroInfo.Stat.hunterTier)
            {
                case 1:
                    anim.initialSkinName = heroInfo.Hunter.hunterImg1;
                    break;
                case 2:
                    anim.initialSkinName = heroInfo.Hunter.hunterImg2;
                    break;
                case 3:
                    anim.initialSkinName = heroInfo.Hunter.hunterImg3;
                    break;
                case 4:
                    anim.initialSkinName = heroInfo.Hunter.hunterImg4;
                    break;
                case 5:
                    anim.initialSkinName = heroInfo.Hunter.hunterImg5;
                    break;
            }
            anim.Initialize(overwrite: true);
            _promotionUpCharacter.GetChild(0).GetComponent<MeshRenderer>().sortingLayerName = "Popup";
            _promotionUpCharacter.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 5;
        }
    }

    private void SetHeroData()
    {
        UnityEngine.Debug.Log("this.hunterInfo.Hunter.hunterIdx = " + this.heroInfo.Hunter.hunterIdx);
        UnityEngine.Debug.Log("this.hunterInfo.Stat.hunterLevel-1 = " + (this.heroInfo.Stat.hunterLevel - 1));
        HunterInfo hunterInfo = GameDataManager.GetHunterInfo(this.heroInfo.Hunter.hunterIdx, this.heroInfo.Stat.hunterLevel, this.heroInfo.Stat.hunterTier - 1);
        _hunterMaxLevelBefore.text = string.Format(MasterLocalize.GetData("common_text_max_level"), ((this.heroInfo.Stat.hunterTier - 1) * 20).ToString());
        _hunterMaxLevelAfter.text = (this.heroInfo.Stat.hunterTier * 20).ToString();
        _hunterName.text = MasterLocalize.GetData(this.heroInfo.Hunter.hunterName);
        _heroLevel.text = MasterLocalize.GetData("common_text_level") + this.heroInfo.Stat.hunterLevel.ToString();
        _heroHpBefore.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHP(hunterInfo.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx)));
        _heroHpAfter.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHP(this.heroInfo.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(this.heroInfo.Hunter.hunterIdx)));
        _heroAttackBefore.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceAttack(hunterInfo.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx)));
        _heroAttackAfter.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceAttack(this.heroInfo.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(this.heroInfo.Hunter.hunterIdx)));
        _heroRecoveryBefore.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHeal(hunterInfo.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx)));
        _heroRecoveryAfter.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHeal(this.heroInfo.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(this.heroInfo.Hunter.hunterIdx)));
        hunterInfo = null;
    }

    public void OnClickGoBack()
    {
        LobbyManager.GetExpEff(Vector3.zero);
        if (OnGoBackEvent != null)
        {
            OnGoBackEvent();
        }
        LobbyManager.ShowHunterView(heroInfo, _isSpawn: true);
        LobbyManager.OnGoBackPromotion();
    }
}