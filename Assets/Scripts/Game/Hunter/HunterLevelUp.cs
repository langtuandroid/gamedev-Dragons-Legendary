using Spine.Unity;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HunterLevelUp : LobbyPopupBase
{
    public Action GoBackEvent;

    [SerializeField]
    private Transform hunterCharactertr;

    [SerializeField]
    private HunterView_Color hunter_Character;

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
    private HunterInfo hunterInfo_Before;

    [SerializeField]
    private HunterInfo hunterInfo_After;

    [SerializeField]
    private Transform levelUP_Eff;

    [SerializeField]
    private Transform levelUP_Character_Pos;

    [SerializeField]
    private Transform levelUP_Character;

    [SerializeField]
    private Animator levelUP_Anim;

    [SerializeField]
    private Image levelUp_BG;

    public void Show(HunterInfo _hunterInfo_before, HunterInfo _hunterInfo_after)
    {
        base.Show();
        base.gameObject.SetActive(value: true);
        Init(_hunterInfo_before, _hunterInfo_after);
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void SetInit(HunterInfo _hunterInfo_before, HunterInfo _hunterInfo_after)
    {
        Init(_hunterInfo_before, _hunterInfo_after);
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
        if (levelUP_Character != null)
        {
            MWPoolManager.DeSpawn("Hunter", levelUP_Character);
            levelUP_Character = null;
        }
        levelUP_Eff.gameObject.SetActive(value: false);
    }

    private void Init(HunterInfo _hunterInfo_before, HunterInfo _hunterInfo_after)
    {
        hunterInfo_Before = _hunterInfo_before;
        hunterInfo_After = _hunterInfo_after;
        if (hunter_Character != null)
        {
            MWPoolManager.DeSpawn("Hunter", hunter_Character.transform);
            hunter_Character = null;
        }
        switch (_hunterInfo_after.Hunter.color)
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
        hunter_Character.Init(_hunterInfo_after);
        SetHunterData();
        hunter_Character.gameObject.SetActive(value: false);
        LevelUpEffectPlay();
    }

    private void LevelUpEffectPlay()
    {
        levelUP_Eff.gameObject.SetActive(value: true);
        if (levelUP_Character != null)
        {
            MWPoolManager.DeSpawn("Hunter", levelUP_Character);
            levelUP_Character = null;
        }
        SoundController.EffectSound_Play(EffectSoundType.HunterLevelUp);
        switch (hunterInfo_After.Hunter.color)
        {
            case 0:
                levelUp_BG.color = new Color32(77, 122, 170, byte.MaxValue);
                levelUP_Anim.ResetTrigger("Blue");
                levelUP_Anim.SetTrigger("Blue");
                break;
            case 1:
                levelUp_BG.color = new Color32(123, 170, 77, byte.MaxValue);
                levelUP_Anim.ResetTrigger("Green");
                levelUP_Anim.SetTrigger("Green");
                break;
            case 2:
                levelUp_BG.color = new Color32(166, 122, 179, byte.MaxValue);
                levelUP_Anim.ResetTrigger("Purple");
                levelUP_Anim.SetTrigger("Purple");
                break;
            case 3:
                levelUp_BG.color = new Color32(190, 108, 101, byte.MaxValue);
                levelUP_Anim.ResetTrigger("Red");
                levelUP_Anim.SetTrigger("Red");
                break;
            case 4:
                levelUp_BG.color = new Color32(220, 179, 65, byte.MaxValue);
                levelUP_Anim.ResetTrigger("Yellow");
                levelUP_Anim.SetTrigger("Yellow");
                break;
        }
        levelUP_Character = MWPoolManager.Spawn("Hunter", hunterInfo_After.Hunter.hunterIdx.ToString(), levelUP_Character_Pos);
        UnityEngine.Debug.Log("LevelUp Hunter Parent 11 = " + levelUP_Character.parent.name);
        SetHunterImg();
        levelUP_Character.gameObject.SetActive(value: true);
        if (hunterInfo_After.Hunter.hunterSize == 3)
        {
            levelUP_Character.localScale = new Vector3(150f, 150f, 150f);
        }
        else if (hunterInfo_After.Hunter.hunterSize == 2)
        {
            levelUP_Character.localScale = new Vector3(200f, 200f, 200f);
        }
        else
        {
            levelUP_Character.localScale = new Vector3(220f, 220f, 220f);
        }
        levelUP_Character.localPosition = Vector3.zero;
        UnityEngine.Debug.Log("LevelUp Hunter Parent 22 = " + levelUP_Character.parent.name);
    }

    private void SetHunterImg()
    {
        var anim = levelUP_Character.GetChild(0).GetComponent<SkeletonAnimation>();

        if (!(anim == null))
        {
            switch (hunterInfo_After.Stat.hunterTier)
            {
                case 1:
                    anim.initialSkinName = hunterInfo_After.Hunter.hunterImg1;
                    break;
                case 2:
                    anim.initialSkinName = hunterInfo_After.Hunter.hunterImg2;
                    break;
                case 3:
                    anim.initialSkinName = hunterInfo_After.Hunter.hunterImg3;
                    break;
                case 4:
                    anim.initialSkinName = hunterInfo_After.Hunter.hunterImg4;
                    break;
                case 5:
                    anim.initialSkinName = hunterInfo_After.Hunter.hunterImg5;
                    break;
            }
            anim.Initialize(overwrite: true);
            levelUP_Character.GetChild(0).GetComponent<MeshRenderer>().sortingLayerName = "Popup";
            levelUP_Character.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 5;
        }
    }

    private void SetHunterData()
    {
        hunter_Name.text = MWLocalize.GetData(hunterInfo_After.Hunter.hunterName);
        hunter_Level.text = string.Format("{0} {1}", MWLocalize.GetData("common_text_level"), hunterInfo_After.Stat.hunterLevel);
        hunter_HP_Before.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHP(hunterInfo_Before.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(hunterInfo_Before.Hunter.hunterIdx)));
        hunter_HP_After.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHP(hunterInfo_After.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(hunterInfo_After.Hunter.hunterIdx)));
        hunter_Attack_Before.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceAttack(hunterInfo_Before.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(hunterInfo_Before.Hunter.hunterIdx)));
        hunter_Attack_After.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceAttack(hunterInfo_After.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(hunterInfo_After.Hunter.hunterIdx)));
        hunter_Recovery_Before.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHeal(hunterInfo_Before.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(hunterInfo_Before.Hunter.hunterIdx)));
        hunter_Recovery_After.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHeal(hunterInfo_After.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(hunterInfo_After.Hunter.hunterIdx)));
    }

    public void OnClickGoBack()
    {
        LobbyManager.GetExpEff(Vector3.zero);
        if (GoBackEvent != null)
        {
            GoBackEvent();
        }
        LobbyManager.ShowHunterView(hunterInfo_After, _isSpawn: false);
        LobbyManager.OnGoBackLevel();
    }
}