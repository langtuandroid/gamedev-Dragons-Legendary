using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArenaChestOpen : MonoBehaviour
{
	public enum ChestOpenState
	{
		None = 1,
		Open
	}

	public enum ArenaChestType
	{
		Normal = 1,
		Mysterious
	}

	private ChestListDbData[] chestListDbData;

	private ChestOpenState chestOpenState = ChestOpenState.None;

	private int currentIdx;

	private int remainOpenCount;

	[SerializeField]
	private int arenaExp;

	[SerializeField]
	private int arenaPoint;

	[SerializeField]
	private Animator chestAnim;

	[SerializeField]
	private ArenaChestType chestType;

	[SerializeField]
	private Transform hunterCard_tr;

	private HeroCard hunterCard;

	[SerializeField]
	private Text hunterName_Text;

	[SerializeField]
	private Transform itemImg_tr;

	[SerializeField]
	private Transform itemImg;

	[SerializeField]
	private Text itemCount_Text;

	[SerializeField]
	private Text itemName_Text;

	[SerializeField]
	private Text itemType_Text;

	[SerializeField]
	private Text itemProperty_Text;

	[SerializeField]
	private Text itemYouHave_Text;

	[SerializeField]
	private Text arenaExp_Text;

	[SerializeField]
	private Text arenaPoint_Text;

	[SerializeField]
	private Transform chestResult;

	[SerializeField]
	private Transform chestResult_tr;

	[SerializeField]
	private Transform newRibbon;

	public void Show()
	{
		Protocol_Set.Protocol_arena_game_end_Req(GameInfo.inGamePlayData.arenaLevelData.levelIdx, GameInfo.userPlayData.gameKey, 1, 0, GameInfo.userPlayData.wave, OnAnenaGameEndConnectComplete);
	}

	public void Init(ARENA_GAME_END_RESULT _chestList)
	{
		base.gameObject.SetActive(value: true);
		if (GameInfo.inGamePlayData.arenaLevelData.chestType == 1)
		{
			UnityEngine.Debug.Log("GameInfo.inGamePlayData.arenaLevelData.chestType 11 = " + GameInfo.inGamePlayData.arenaLevelData.chestType);
			chestType = ArenaChestType.Normal;
			PlayAnim("Arena_Normal_Idle");
		}
		else
		{
			UnityEngine.Debug.Log("GameInfo.inGamePlayData.arenaLevelData.chestType 22 = " + GameInfo.inGamePlayData.arenaLevelData.chestType);
			chestType = ArenaChestType.Mysterious;
			PlayAnim("Arena_Mysterious_Idle");
		}
		chestListDbData = _chestList.rewardList;
		arenaExp = _chestList.rewardExp;
		arenaPoint = _chestList.rewardArenaPoint;
		arenaExp_Text.text = "x" + arenaExp.ToString();
		arenaPoint_Text.text = "x" + arenaPoint.ToString();
		currentIdx = 0;
		SetRemainCount();
		chestOpenState = ChestOpenState.None;
	}

	private void OnAnenaGameEndConnectComplete(ARENA_GAME_END_RESULT _data)
	{
		Init(_data);
	}

	public void CardOpen_Anim_End()
	{
		currentIdx++;
		chestOpenState = ChestOpenState.None;
		if (remainOpenCount <= 0)
		{
			int childCount = chestResult_tr.childCount;
			for (int i = 0; i < childCount; i++)
			{
				MWPoolManager.DeSpawn("Item", chestResult_tr.GetChild(0));
			}
			for (int j = 0; j < chestListDbData.Length; j++)
			{
				CardResult_Setting(chestListDbData[j]);
			}
			StartCoroutine(ShowChestResult());
		}
	}

	public IEnumerator ShowChestResult()
	{
		chestOpenState = ChestOpenState.Open;
		yield return new WaitForSeconds(0.5f);
		chestResult.gameObject.SetActive(value: true);
	}

	private void SetRemainCount()
	{
		remainOpenCount = chestListDbData.Length;
	}

	private void SetAnim(int _idx)
	{
		SoundController.EffectSound_Play(EffectSoundType.FreeChestOpen);
		if (chestType == ArenaChestType.Normal)
		{
			PlayAnim("Arena_Normal_Open");
		}
		else if (chestListDbData[_idx].chestHunter > 0)
		{
			UnityEngine.Debug.Log("Hunter Open !!");
			PlayAnim("Arena_Mysterious_Hunter_Open");
		}
		else
		{
			UnityEngine.Debug.Log("Itme Open !!");
			PlayAnim("Arena_Mysterious_Item_Open");
		}
	}

	private void CardOpen_Setting(int _idx)
	{
		if (chestListDbData[_idx].chestHunter > 0)
		{
			if (GameUtil.GetHunterExist(chestListDbData[_idx].chestHunter))
			{
				newRibbon.gameObject.SetActive(value: false);
			}
			CardOpen_Setting_Hunter(_idx);
		}
		else
		{
			CardOpen_Setting_Item(_idx);
		}
	}

	private void CardOpen_Setting_Hunter(int _idx)
	{
		if (hunterCard != null)
		{
			MWPoolManager.DeSpawn("Hunter", hunterCard.transform);
			hunterCard = null;
		}
		hunterCard = MWPoolManager.Spawn("Hunter", "HunterCard_" + chestListDbData[_idx].chestHunter, hunterCard_tr).GetComponent<HeroCard>();
		hunterCard.Construct(HerocardType.Chestopen, GameDataManager.GetHunterInfo(chestListDbData[_idx].chestHunter, chestListDbData[_idx].hunterLevel, chestListDbData[_idx].hunterTier), _isOwn: true, _isArena: false);
		hunterCard.HeroIdx = 0;
		hunterCard.transform.localPosition = Vector3.zero;
		hunterCard.transform.localScale = Vector3.one;
		UnityEngine.Debug.Log("Set HunterCard AnchoredPosition !!");
		hunterCard.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(175f, -145f);
		hunterCard.transform.SetSiblingIndex(1);
		hunterName_Text.text = MWLocalize.GetData(GameDataManager.GetHunterInfo(chestListDbData[_idx].chestHunter, 1, 1).Hunter.hunterName);
	}

	private void CardOpen_Setting_Item(int _idx)
	{
		if (itemImg != null)
		{
			MWPoolManager.DeSpawn("Item", itemImg);
			itemImg = null;
		}
		itemImg = MWPoolManager.Spawn("Item", "Item_" + chestListDbData[_idx].chestItem, itemImg_tr);
		itemImg.transform.localPosition = Vector3.zero;
		itemImg.transform.localScale = Vector3.one;
		itemImg.transform.SetAsFirstSibling();
		itemCount_Text.text = "x" + chestListDbData[_idx].chestItemN.ToString();
		itemName_Text.text = MWLocalize.GetData(GameDataManager.GetItemListData(chestListDbData[_idx].chestItem).itemName);
		itemType_Text.text = MWLocalize.GetData(GameDataManager.GetItemListData(chestListDbData[_idx].chestItem).itemType);
		itemYouHave_Text.text = MWLocalize.GetData("common_text_you_have") + GameInfo.userData.GetItemCount(chestListDbData[_idx].chestItem).ToString();
	}

	private void CardResult_Setting(ChestListDbData _data)
	{
		bool flag = false;
		for (int i = 0; i < chestResult_tr.childCount; i++)
		{
			if (chestResult_tr.GetChild(i).GetComponent<ArenaChestResultItem>().ChestListDbData.chestItem == _data.chestItem && _data.chestItem != 0)
			{
				chestResult_tr.GetChild(i).GetComponent<ArenaChestResultItem>().AddAmount(_data.chestItemN);
				flag = true;
			}
		}
		if (!flag)
		{
			ArenaChestResultItem component = MWPoolManager.Spawn("Item", "Item03", chestResult_tr).GetComponent<ArenaChestResultItem>();
			component.Init(_data, new Vector3(1.2f, 1.2f, 1.2f));
			component.transform.localPosition = Vector3.zero;
			component.transform.localScale = Vector3.one;
		}
	}

	private void PlayAnim(string _anim)
	{
		UnityEngine.Debug.Log("_animType = " + _anim);
		chestAnim.ResetTrigger(_anim);
		chestAnim.SetTrigger(_anim);
	}

	public void Click_CardOpen()
	{
		UnityEngine.Debug.Log("CardOpen 11 !!");
		if (chestOpenState != ChestOpenState.Open && currentIdx < chestListDbData.Length)
		{
			UnityEngine.Debug.Log("CardOpen 22 !!" + currentIdx);
			UnityEngine.Debug.Log("CardOpen 22 !!" + remainOpenCount);
			chestOpenState = ChestOpenState.Open;
			CardOpen_Setting(currentIdx);
			SetAnim(currentIdx);
			remainOpenCount--;
		}
	}

	public void Click_CardResult_OK_BT()
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		MWPoolManager.DeSpawnPoolAll("Puzzle");
		MWPoolManager.DeSpawnPoolAll("Hunter");
		MWPoolManager.DeSpawnPoolAll("Monster");
		MWPoolManager.DeSpawnPoolAll("Effect");
		MWPoolManager.DeSpawnPoolAll("Stage");
		MWPoolManager.DeSpawnPoolAll("Item");
		GameDataManager.MoveScene(SceneType.Lobby);
	}

	public void Click_Skip_BT()
	{
		chestOpenState = ChestOpenState.None;
		remainOpenCount = 0;
		if (remainOpenCount <= 0)
		{
			int childCount = chestResult_tr.childCount;
			for (int i = 0; i < childCount; i++)
			{
				MWPoolManager.DeSpawn("Item", chestResult_tr.GetChild(0));
			}
			for (int j = 0; j < chestListDbData.Length; j++)
			{
				CardResult_Setting(chestListDbData[j]);
			}
			StartCoroutine(ShowChestResult());
		}
	}
}
