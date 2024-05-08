using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

public class HeroColor : MonoBehaviour
{
    private HunterInfo _heroInfo;

    [FormerlySerializedAs("hunter_Pos")] [SerializeField]
    private Transform _hunterPos;

    [FormerlySerializedAs("hunter_Character")] [SerializeField]
    private Transform _hunterCharacter;

    [FormerlySerializedAs("tier_Tr")] [SerializeField]
    private Transform _tierTr;

    public void Construct(HunterInfo _hunterInfo)
    {
        _heroInfo = _hunterInfo;
        SetCharacter();
        SetStars();
    }

    private void SetCharacter()
    {
        if (_hunterCharacter != null)
        {
            MasterPoolManager.ReturnToPool("Hunter", _hunterCharacter);
            _hunterCharacter = null;
        }
        _hunterCharacter = MasterPoolManager.SpawnObject("Hunter", _heroInfo.Hunter.hunterIdx.ToString(), _hunterPos);
        SetImage();
        _hunterCharacter.gameObject.SetActive(value: true);
        if (_heroInfo.Hunter.hunterSize == 3)
        {
            _hunterCharacter.localScale = new Vector3(150f, 150f, 150f);
        }
        else if (_heroInfo.Hunter.hunterSize == 2)
        {
            _hunterCharacter.localScale = new Vector3(200f, 200f, 200f);
        }
        else
        {
            _hunterCharacter.localScale = new Vector3(220f, 220f, 220f);
        }
        _hunterCharacter.localPosition = Vector3.zero;
    }

    private void SetImage()
    {
        var anim = _hunterCharacter.GetChild(0).GetComponent<SkeletonAnimation>();

        if (anim != null)
        {
            switch (_heroInfo.Stat.hunterTier)
            {
                case 1:
                    anim.initialSkinName = _heroInfo.Hunter.hunterImg1;
                    break;
                case 2:
                    anim.initialSkinName = _heroInfo.Hunter.hunterImg2;
                    break;
                case 3:
                    anim.initialSkinName = _heroInfo.Hunter.hunterImg3;
                    break;
                case 4:
                    anim.initialSkinName = _heroInfo.Hunter.hunterImg4;
                    break;
                case 5:
                    anim.initialSkinName = _heroInfo.Hunter.hunterImg5;
                    break;
            }
            anim.Initialize(overwrite: true);
            _hunterCharacter.GetChild(0).GetComponent<MeshRenderer>().sortingLayerName = "Popup";
            _hunterCharacter.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 5;
        }
    }

    private void SetStars()
    {
        for (int i = 0; i < _tierTr.childCount; i++)
        {
            _tierTr.GetChild(i).GetChild(0).gameObject.SetActive(value: false);
            _tierTr.GetChild(i).gameObject.SetActive(value: false);
        }
        for (int j = 0; j < _heroInfo.Hunter.maxTier; j++)
        {
            _tierTr.GetChild(j).gameObject.SetActive(value: true);
            if (_heroInfo.Stat.hunterTier >= j + 1)
            {
                _tierTr.GetChild(j).GetChild(0).gameObject.SetActive(value: true);
            }
        }
    }
}