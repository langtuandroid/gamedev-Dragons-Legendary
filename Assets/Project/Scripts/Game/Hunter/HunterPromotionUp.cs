using Spine.Unity;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HunterPromotionUp : LobbyPopupBase
{
    public Action GoBackEvent;

    [SerializeField]
    private Transform hunterCharactertr;

    [SerializeField]
    private HunterView_Color hunter_Character;

    [SerializeField]
    private Transform hunterTiertr;

    [SerializeField]
    private Text hunter_MaxLevel_Before;

    [SerializeField]
    private Text hunter_MaxLevel_After;

    [SerializeField]
    private Text hunter_Name;

    [SerializeField]
    private Text hunter_Level;

    [SerializeField]
    private Text hunter_HP_Before;

    [SerializeField]
    private Text hunter_HP_After;

    [SerializeField]
    private Text hunter_Attack_Before;

    [SerializeField]
    private Text hunter_Attack_After;

    [SerializeField]
    private Text hunter_Recovery_Before;

    [SerializeField]
    private Text hunter_Recovery_After;

    [SerializeField]
    private HunterInfo hunterInfo;

    [SerializeField]
    private HunterPromotionEffect promotionUP_Eff;

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

    public void Show(HunterInfo _hunterInfo)
    {
        base.Show();
        base.gameObject.SetActive(value: true);
        Init(_hunterInfo);
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void SetInit(HunterInfo _hunterInfo)
    {
        Init(_hunterInfo);
    }

    public override void HideProcessComplete()
    {
    }

    public void End_LevelUp_Anim()
    {
        hunter_Character.gameObject.SetActive(value: true);
        Transform transform = null;
        transform = MWPoolManager.Spawn("Effect", "FX_Boom", base.transform, 2f);
        transform.SetAsLastSibling();
        promotionUP_Eff.gameObject.SetActive(value: false);
    }

    private void Init(HunterInfo _hunterInfo)
    {
        hunterInfo = _hunterInfo;
        if (promotionUP_Eff != null)
        {
            MWPoolManager.DeSpawn("Hunter", promotionUP_Eff.transform);
            promotionUP_Eff = null;
        }
        switch (hunterInfo.Hunter.color)
        {
            case 0:
                promotionUP_Eff = MWPoolManager.Spawn("Effect", "FX_PromotionUp_Blue", base.transform).GetComponent<HunterPromotionEffect>();
                break;
            case 1:
                promotionUP_Eff = MWPoolManager.Spawn("Effect", "FX_PromotionUp_Green", base.transform).GetComponent<HunterPromotionEffect>();
                break;
            case 2:
                promotionUP_Eff = MWPoolManager.Spawn("Effect", "FX_PromotionUp_Purple", base.transform).GetComponent<HunterPromotionEffect>();
                break;
            case 3:
                promotionUP_Eff = MWPoolManager.Spawn("Effect", "FX_PromotionUp_Red", base.transform).GetComponent<HunterPromotionEffect>();
                break;
            case 4:
                promotionUP_Eff = MWPoolManager.Spawn("Effect", "FX_PromotionUp_Yellow", base.transform).GetComponent<HunterPromotionEffect>();
                break;
        }
        promotionUP_Character_Pos = promotionUP_Eff.PromotionUP_Character_Pos;
        promotionUP_Character = promotionUP_Eff.PromotionUP_Character;
        promotionUP_Tier1 = promotionUP_Eff.PromotionUP_Tier1;
        promotionUP_Tier2 = promotionUP_Eff.PromotionUP_Tier1;
        promotionUP_Anim = promotionUP_Eff.PromotionUP_Anim;
        promotionUP_Eff.SetHunterPromotionUp(this);
        switch (_hunterInfo.Stat.hunterTier)
        {
            case 2:
                promotionUP_Tier1.gameObject.SetActive(value: true);
                promotionUP_Tier2.gameObject.SetActive(value: false);
                break;
            case 3:
                promotionUP_Tier1.gameObject.SetActive(value: false);
                promotionUP_Tier2.gameObject.SetActive(value: true);
                break;
        }
        PromotionUpEffectPlay();
        if (hunter_Character != null)
        {
            MWPoolManager.DeSpawn("Hunter", hunter_Character.transform);
            hunter_Character = null;
        }
        switch (hunterInfo.Hunter.color)
        {
            case 0:
                hunter_Character = MWPoolManager.Spawn("Hunter", "HunterPhotoBg2_B2", hunterCharactertr).GetComponent<HunterView_Color>();
                break;
            case 1:
                hunter_Character = MWPoolManager.Spawn("Hunter", "HunterPhotoBg2_G", hunterCharactertr).GetComponent<HunterView_Color>();
                break;
            case 2:
                hunter_Character = MWPoolManager.Spawn("Hunter", "HunterPhotoBg2_P", hunterCharactertr).GetComponent<HunterView_Color>();
                break;
            case 3:
                hunter_Character = MWPoolManager.Spawn("Hunter", "HunterPhotoBg2_R", hunterCharactertr).GetComponent<HunterView_Color>();
                break;
            case 4:
                hunter_Character = MWPoolManager.Spawn("Hunter", "HunterPhotoBg2_Y", hunterCharactertr).GetComponent<HunterView_Color>();
                break;
        }
        hunter_Character.transform.SetAsFirstSibling();
        hunter_Character.transform.localPosition = new Vector3(0f, 180f, 0f);
        hunter_Character.transform.localScale = new Vector3(1f, 1f, 1f);
        hunter_Character.Init(_hunterInfo);
        hunter_Character.gameObject.SetActive(value: false);
        SetHunterData();
        SetTierStar();
    }

    private void SetTierStar()
    {
        for (int i = 0; i < hunterTiertr.childCount; i++)
        {
            hunterTiertr.GetChild(i).GetChild(0).gameObject.SetActive(value: false);
            hunterTiertr.GetChild(i).gameObject.SetActive(value: false);
        }
        for (int j = 0; j < hunterInfo.Hunter.maxTier; j++)
        {
            hunterTiertr.GetChild(j).gameObject.SetActive(value: true);
            if (hunterInfo.Stat.hunterTier >= j + 1)
            {
                hunterTiertr.GetChild(j).GetChild(0).gameObject.SetActive(value: true);
            }
        }
    }

    private void PromotionUpEffectPlay()
    {
        promotionUP_Eff.gameObject.SetActive(value: true);
        if (promotionUP_Character != null)
        {
            MWPoolManager.DeSpawn("Hunter", promotionUP_Character);
            promotionUP_Character = null;
        }
        promotionUP_Character = MWPoolManager.Spawn("Hunter", hunterInfo.Hunter.hunterIdx.ToString(), promotionUP_Character_Pos);
        promotionUP_Character.gameObject.SetActive(value: true);
        SetHunterImg();
        if (hunterInfo.Hunter.hunterSize == 3)
        {
            promotionUP_Character.localScale = new Vector3(150f, 150f, 150f);
        }
        else if (hunterInfo.Hunter.hunterSize == 2)
        {
            promotionUP_Character.localScale = new Vector3(200f, 200f, 200f);
        }
        else
        {
            promotionUP_Character.localScale = new Vector3(220f, 220f, 220f);
        }
        promotionUP_Character.localPosition = Vector3.zero;
        SoundController.EffectSound_Play(EffectSoundType.HunterPromotionUp);
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
        switch (hunterInfo.Stat.hunterTier)
        {
            case 2:
                promotionUP_Anim.ResetTrigger("Promotion1");
                promotionUP_Anim.SetTrigger("Promotion1");
                break;
            case 3:
                promotionUP_Anim.ResetTrigger("Promotion2");
                promotionUP_Anim.SetTrigger("Promotion2");
                break;
        }
    }

    private void SetHunterImg()
    {
        var anim = promotionUP_Character.GetChild(0).GetComponent<SkeletonAnimation>();

        if (!(anim == null))
        {
            switch (hunterInfo.Stat.hunterTier)
            {
                case 1:
                    anim.initialSkinName = hunterInfo.Hunter.hunterImg1;
                    break;
                case 2:
                    anim.initialSkinName = hunterInfo.Hunter.hunterImg2;
                    break;
                case 3:
                    anim.initialSkinName = hunterInfo.Hunter.hunterImg3;
                    break;
                case 4:
                    anim.initialSkinName = hunterInfo.Hunter.hunterImg4;
                    break;
                case 5:
                    anim.initialSkinName = hunterInfo.Hunter.hunterImg5;
                    break;
            }
            anim.Initialize(overwrite: true);
            promotionUP_Character.GetChild(0).GetComponent<MeshRenderer>().sortingLayerName = "Popup";
            promotionUP_Character.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 5;
        }
    }

    private void SetHunterData()
    {
        UnityEngine.Debug.Log("this.hunterInfo.Hunter.hunterIdx = " + this.hunterInfo.Hunter.hunterIdx);
        UnityEngine.Debug.Log("this.hunterInfo.Stat.hunterLevel-1 = " + (this.hunterInfo.Stat.hunterLevel - 1));
        HunterInfo hunterInfo = GameDataManager.GetHunterInfo(this.hunterInfo.Hunter.hunterIdx, this.hunterInfo.Stat.hunterLevel, this.hunterInfo.Stat.hunterTier - 1);
        hunter_MaxLevel_Before.text = string.Format(MWLocalize.GetData("common_text_max_level"), ((this.hunterInfo.Stat.hunterTier - 1) * 20).ToString());
        hunter_MaxLevel_After.text = (this.hunterInfo.Stat.hunterTier * 20).ToString();
        hunter_Name.text = MWLocalize.GetData(this.hunterInfo.Hunter.hunterName);
        hunter_Level.text = MWLocalize.GetData("common_text_level") + this.hunterInfo.Stat.hunterLevel.ToString();
        hunter_HP_Before.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHP(hunterInfo.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx)));
        hunter_HP_After.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHP(this.hunterInfo.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(this.hunterInfo.Hunter.hunterIdx)));
        hunter_Attack_Before.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceAttack(hunterInfo.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx)));
        hunter_Attack_After.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceAttack(this.hunterInfo.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(this.hunterInfo.Hunter.hunterIdx)));
        hunter_Recovery_Before.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHeal(hunterInfo.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx)));
        hunter_Recovery_After.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHeal(this.hunterInfo.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(this.hunterInfo.Hunter.hunterIdx)));
        hunterInfo = null;
    }

    public void OnClickGoBack()
    {
        LobbyManager.GetExpEff(Vector3.zero);
        if (GoBackEvent != null)
        {
            GoBackEvent();
        }
        LobbyManager.ShowHunterView(hunterInfo, _isSpawn: true);
        LobbyManager.OnGoBackPromotion();
    }
}