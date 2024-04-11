using Spine.Unity;
using UnityEngine;

public class HunterView_Color : MonoBehaviour
{
    [SerializeField]
    private HunterInfo hunterInfo;

    [SerializeField]
    private Transform hunter_Pos;

    [SerializeField]
    private Transform hunter_Character;

    [SerializeField]
    private Transform tier_Tr;

    public void Init(HunterInfo _hunterInfo)
    {
        hunterInfo = _hunterInfo;
        SetHunterCharacter();
        SetTierStar();
    }

    private void SetHunterCharacter()
    {
        if (hunter_Character != null)
        {
            MWPoolManager.DeSpawn("Hunter", hunter_Character);
            hunter_Character = null;
        }
        hunter_Character = MWPoolManager.Spawn("Hunter", hunterInfo.Hunter.hunterIdx.ToString(), hunter_Pos);
        SetHunterImg();
        hunter_Character.gameObject.SetActive(value: true);
        if (hunterInfo.Hunter.hunterSize == 3)
        {
            hunter_Character.localScale = new Vector3(150f, 150f, 150f);
        }
        else if (hunterInfo.Hunter.hunterSize == 2)
        {
            hunter_Character.localScale = new Vector3(200f, 200f, 200f);
        }
        else
        {
            hunter_Character.localScale = new Vector3(220f, 220f, 220f);
        }
        hunter_Character.localPosition = Vector3.zero;
    }

    private void SetHunterImg()
    {
        var anim = hunter_Character.GetChild(0).GetComponent<SkeletonAnimation>();

        if (anim != null)
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
            hunter_Character.GetChild(0).GetComponent<MeshRenderer>().sortingLayerName = "Popup";
            hunter_Character.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 5;
        }
    }

    private void SetTierStar()
    {
        for (int i = 0; i < tier_Tr.childCount; i++)
        {
            tier_Tr.GetChild(i).GetChild(0).gameObject.SetActive(value: false);
            tier_Tr.GetChild(i).gameObject.SetActive(value: false);
        }
        for (int j = 0; j < hunterInfo.Hunter.maxTier; j++)
        {
            tier_Tr.GetChild(j).gameObject.SetActive(value: true);
            if (hunterInfo.Stat.hunterTier >= j + 1)
            {
                tier_Tr.GetChild(j).GetChild(0).gameObject.SetActive(value: true);
            }
        }
    }
}