using Spine.Unity;
using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HunterLevelUp : LobbyPopupBase
{
    public Action GoBackEvent;

    [FormerlySerializedAs("hunterCharactertr")] [SerializeField]
    private Transform _hunterCharacterTR;

    [FormerlySerializedAs("hunter_Character")] [SerializeField]
    private HeroColor _hunterCharacter;

    [FormerlySerializedAs("hunter_Name")] [SerializeField]
    private Text _hunterName;

    [FormerlySerializedAs("hunter_Level")] [SerializeField]
    private Text _hunterLevel;

    [FormerlySerializedAs("hunter_HP_Before")] [SerializeField]
    private Text _hunterHpBefore;

    [FormerlySerializedAs("hunter_HP_After")] [SerializeField]
    private Text _hunterHpAfter;

    [FormerlySerializedAs("hunter_Attack_Before")] [SerializeField]
    private Text _hunterAttackBefore;

    [FormerlySerializedAs("hunter_Attack_After")] [SerializeField]
    private Text _hunterAttackAfter;

    [FormerlySerializedAs("hunter_Recovery_Before")] [SerializeField]
    private Text _hunterRecoveryBefore;

    [FormerlySerializedAs("hunter_Recovery_After")] [SerializeField]
    private Text _hunterRecoveryAfter;
    
    private HunterInfo _hunterInfoBefore;
    private HunterInfo _hunterInfoAfter;

    [FormerlySerializedAs("levelUP_Eff")] [SerializeField]
    private Transform _levelUpEff;

    [FormerlySerializedAs("levelUP_Character_Pos")] [SerializeField]
    private Transform _levelUpCharacterPos;

    [FormerlySerializedAs("levelUP_Character")] [SerializeField]
    private Transform _levelUpCharacter;

    [FormerlySerializedAs("levelUP_Anim")] [SerializeField]
    private Animator _levelUpAnim;

    [FormerlySerializedAs("levelUp_BG")] [SerializeField]
    private Image _levelUpBg;

    public void ShowLevel(HunterInfo _hunterInfo_before, HunterInfo _hunterInfo_after)
    {
        base.Open();
        base.gameObject.SetActive(value: true);
        Construct(_hunterInfo_before, _hunterInfo_after);
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void SetConstruct(HunterInfo _hunterInfo_before, HunterInfo _hunterInfo_after)
    {
        Construct(_hunterInfo_before, _hunterInfo_after);
    }

    public override void CloseProcessComplete()
    {
    }

    public void End_LevelUp_Anim()
    {
        _hunterCharacter.gameObject.SetActive(value: true);
        Transform transform = null;
        transform = MWPoolManager.Spawn("Effect", "FX_Boom", base.transform, 2f);
        transform.SetAsLastSibling();
        if (_levelUpCharacter != null)
        {
            MWPoolManager.DeSpawn("Hunter", _levelUpCharacter);
            _levelUpCharacter = null;
        }
        _levelUpEff.gameObject.SetActive(value: false);
    }

    private void Construct(HunterInfo _hunterInfo_before, HunterInfo _hunterInfo_after)
    {
        _hunterInfoBefore = _hunterInfo_before;
        _hunterInfoAfter = _hunterInfo_after;
        
        if (_hunterCharacter != null)
        {
            MWPoolManager.DeSpawn("Hunter", _hunterCharacter.transform);
            _hunterCharacter = null;
        }
        switch (_hunterInfo_after.Hunter.color)
        {
            case 0:
                _hunterCharacter = MWPoolManager.Spawn("Hunter", "HunterPhotoBg2_B2", _hunterCharacterTR).GetComponent<HeroColor>();
                break;
            case 1:
                _hunterCharacter = MWPoolManager.Spawn("Hunter", "HunterPhotoBg2_G", _hunterCharacterTR).GetComponent<HeroColor>();
                break;
            case 2:
                _hunterCharacter = MWPoolManager.Spawn("Hunter", "HunterPhotoBg2_P", _hunterCharacterTR).GetComponent<HeroColor>();
                break;
            case 3:
                _hunterCharacter = MWPoolManager.Spawn("Hunter", "HunterPhotoBg2_R", _hunterCharacterTR).GetComponent<HeroColor>();
                break;
            case 4:
                _hunterCharacter = MWPoolManager.Spawn("Hunter", "HunterPhotoBg2_Y", _hunterCharacterTR).GetComponent<HeroColor>();
                break;
        }
        _hunterCharacter.transform.SetAsFirstSibling();
        _hunterCharacter.transform.localPosition = new Vector3(0f, 180f, 0f);
        _hunterCharacter.transform.localScale = new Vector3(1f, 1f, 1f);
        _hunterCharacter.Construct(_hunterInfo_after);
        _hunterCharacter.gameObject.SetActive(value: false);

         
        SetHunterData();
        
        EffectPlay();
    }

    private void EffectPlay()
    {
        _levelUpEff.gameObject.SetActive(value: true);
        if (_levelUpCharacter != null)
        {
            MWPoolManager.DeSpawn("Hunter", _levelUpCharacter);
            _levelUpCharacter = null;
        }
        SoundController.EffectSound_Play(EffectSoundType.HunterLevelUp);
        /*
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
        */
        
        _levelUpCharacter = MWPoolManager.Spawn("Hunter", _hunterInfoAfter.Hunter.hunterIdx.ToString(), _levelUpCharacterPos);
        UnityEngine.Debug.Log("LevelUp Hunter Parent 11 = " + _levelUpCharacter.parent.name);
        SetHunterImg();
        _levelUpCharacter.gameObject.SetActive(value: true);
        if (_hunterInfoAfter.Hunter.hunterSize == 3)
        {
            _levelUpCharacter.localScale = new Vector3(150f, 150f, 150f);
        }
        else if (_hunterInfoAfter.Hunter.hunterSize == 2)
        {
            _levelUpCharacter.localScale = new Vector3(200f, 200f, 200f);
        }
        else
        {
            _levelUpCharacter.localScale = new Vector3(220f, 220f, 220f);
        }
        _levelUpCharacter.localPosition = Vector3.zero;
        UnityEngine.Debug.Log("LevelUp Hunter Parent 22 = " + _levelUpCharacter.parent.name);
    }

    private void SetHunterImg()
    {
        var anim = _levelUpCharacter.GetChild(0).GetComponent<SkeletonAnimation>();

        if (!(anim == null))
        {
            switch (_hunterInfoAfter.Stat.hunterTier)
            {
                case 1:
                    anim.initialSkinName = _hunterInfoAfter.Hunter.hunterImg1;
                    break;
                case 2:
                    anim.initialSkinName = _hunterInfoAfter.Hunter.hunterImg2;
                    break;
                case 3:
                    anim.initialSkinName = _hunterInfoAfter.Hunter.hunterImg3;
                    break;
                case 4:
                    anim.initialSkinName = _hunterInfoAfter.Hunter.hunterImg4;
                    break;
                case 5:
                    anim.initialSkinName = _hunterInfoAfter.Hunter.hunterImg5;
                    break;
            }
            anim.Initialize(overwrite: true);
            _levelUpCharacter.GetChild(0).GetComponent<MeshRenderer>().sortingLayerName = "Popup";
            _levelUpCharacter.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 5;
        }
    }

    private void SetHunterData()
    {
        _hunterName.text = MWLocalize.GetData(_hunterInfoAfter.Hunter.hunterName);
        _hunterLevel.text = string.Format("{0} {1}", MWLocalize.GetData("common_text_level"), _hunterInfoAfter.Stat.hunterLevel);
        _hunterHpBefore.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHP(_hunterInfoBefore.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(_hunterInfoBefore.Hunter.hunterIdx)));
        _hunterHpAfter.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHP(_hunterInfoAfter.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(_hunterInfoAfter.Hunter.hunterIdx)));
        _hunterAttackBefore.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceAttack(_hunterInfoBefore.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(_hunterInfoBefore.Hunter.hunterIdx)));
        _hunterAttackAfter.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceAttack(_hunterInfoAfter.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(_hunterInfoAfter.Hunter.hunterIdx)));
        _hunterRecoveryBefore.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHeal(_hunterInfoBefore.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(_hunterInfoBefore.Hunter.hunterIdx)));
        _hunterRecoveryAfter.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHeal(_hunterInfoAfter.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(_hunterInfoAfter.Hunter.hunterIdx)));
    }

    public void OnClickGoBack()
    {
        LobbyManager.GetExpEff(Vector3.zero);
        if (GoBackEvent != null)
        {
            GoBackEvent();
        }
        LobbyManager.ShowHunterView(_hunterInfoAfter, _isSpawn: false);
        LobbyManager.OnGoBackLevel();
    }
}