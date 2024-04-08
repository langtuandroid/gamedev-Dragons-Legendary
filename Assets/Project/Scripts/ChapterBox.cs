using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterBox : MonoBehaviour
{
	private const float REWARD_BEGIN_GAUGE_VALUE = 395f;

	private const float REWARD_END_GAUGE_VALUE = -395f;

	private const float REWARD_GET_EFFECT_TIME = 1.2f;

	private const float REWARD_GET_EFFECT_DELAY_SPAWN_DURATION = 0.4f;

	[SerializeField]
	private Text textChapterName;

	[SerializeField]
	private Text textReward;

	[SerializeField]
	private Text textCollect;

	[SerializeField]
	private Text textRequireStarLock;

	[SerializeField]
	private Text textRequireStarOpen;

	[SerializeField]
	private Text textRewardCount;

	[SerializeField]
	private Button btnReward;

	[SerializeField]
	private ScrollRect scrollLevelList;

	[SerializeField]
	private Transform trContent;

	[SerializeField]
	private GameObject goLock;

	[SerializeField]
	private GameObject goOpen;

	[SerializeField]
	private GameObject goLockParent;

	[SerializeField]
	private GameObject goIconRewardProgress;

	[SerializeField]
	private GameObject goIconRewardComplete;

	[SerializeField]
	private RectTransform trRewardGauge;

	[SerializeField]
	private GameObject goCollect;

	[SerializeField]
	private GameObject goRewardObj;

	[SerializeField]
	private GameObject goRewardEffect;

	private int chapterStarCount;

	private ChapterDbData chapterDbData;

	private List<LevelDbData> listLevelDbData = new List<LevelDbData>();

	public void SetData(ChapterDbData data)
	{
		chapterDbData = data;
		Init();
	}

	public void Lock()
	{
		goLockParent.SetActive(value: true);
		goLock.SetActive(value: true);
		goOpen.SetActive(value: false);
		goRewardObj.SetActive(value: false);
	}

	public void SetOpen(bool isOpen)
	{
		goLockParent.SetActive(!isOpen);
		goLock.SetActive(value: false);
		goOpen.SetActive(!isOpen);
		goRewardObj.SetActive(isOpen);
	}

	public void Clear()
	{
		LevelCell[] componentsInChildren = trContent.GetComponentsInChildren<LevelCell>();
		foreach (LevelCell levelCell in componentsInChildren)
		{
			MWPoolManager.DeSpawn("Lobby", levelCell.transform);
		}
	}

	public void Refresh()
	{
		LevelCell[] componentsInChildren = trContent.GetComponentsInChildren<LevelCell>();
		foreach (LevelCell levelCell in componentsInChildren)
		{
			levelCell.Refresh();
		}
	}

	private void Init()
	{
		goLock.SetActive(value: false);
		goOpen.SetActive(value: false);
		goLockParent.SetActive(value: false);
		btnReward.enabled = false;
		textChapterName.text = MWLocalize.GetData(chapterDbData.chapterName);
		listLevelDbData = GameDataManager.GetLevelListDbData(chapterDbData.stage, chapterDbData.chapter);
		textRequireStarLock.text = $"{GameDataManager.GetUserClearStarCount()}/{chapterDbData.ocStar}";
		textRequireStarOpen.text = $"{GameDataManager.GetUserClearStarCount()}/{chapterDbData.ocStar}";
		for (int i = 0; i < listLevelDbData.Count; i++)
		{
			LevelDbData levelDbData = listLevelDbData[i];
			LevelCell component = MWPoolManager.Spawn("Lobby", "Cell_level", trContent).GetComponent<LevelCell>();
			component.transform.localScale = Vector3.one;
			component.SetData(levelDbData);
			component.SetStarCount(GameDataManager.GetLevelStarCount(levelDbData.stage, levelDbData.chapter, levelDbData.level));
			if (component.StarCount == 0)
			{
				SnapFianlLevel(i);
			}
		}
		ShowRewardState();
	}

	private void SnapFianlLevel(float _index)
	{
		scrollLevelList.verticalNormalizedPosition = 1f - (_index / (float)listLevelDbData.Count + 0.5f);
	}

	private void ShowRewardState()
	{
		if (chapterDbData.stage > GameInfo.userData.userStageState.Length || chapterDbData.chapter > GameInfo.userData.userStageState[chapterDbData.stage - 1].chapterList.Length)
		{
			return;
		}
		if (GameInfo.userData.userStageState[chapterDbData.stage - 1].chapterList[chapterDbData.chapter - 1].isReward)
		{
			goIconRewardComplete.SetActive(value: true);
			goIconRewardProgress.SetActive(value: false);
			textCollect.gameObject.SetActive(value: false);
			textReward.gameObject.SetActive(value: false);
			textRewardCount.gameObject.SetActive(value: false);
			goRewardEffect.SetActive(value: false);
			btnReward.enabled = false;
			Vector2 offsetMax = trRewardGauge.offsetMax;
			offsetMax.x = 395f;
			trRewardGauge.offsetMax = offsetMax;
			return;
		}
		float num = GameDataManager.GetLevelListDbData(chapterDbData.stage, chapterDbData.chapter).Count * 3;
		chapterStarCount = 0;
		if (chapterDbData.chapter <= GameInfo.userData.userStageState[chapterDbData.stage - 1].chapterList.Length)
		{
			UserLevelState[] levelList = GameInfo.userData.userStageState[chapterDbData.stage - 1].chapterList[chapterDbData.chapter - 1].levelList;
			foreach (UserLevelState userLevelState in levelList)
			{
				chapterStarCount += userLevelState.starCount;
			}
		}
		goIconRewardComplete.SetActive(value: false);
		goIconRewardProgress.SetActive(value: true);
		goRewardEffect.SetActive((float)chapterStarCount == num);
		textReward.gameObject.SetActive((float)chapterStarCount != num);
		textCollect.gameObject.SetActive((float)chapterStarCount == num);
		textRewardCount.gameObject.SetActive(value: true);
		textReward.text = $"{chapterStarCount}/{num}";
		textRewardCount.text = $"{chapterDbData.rewardCount}";
		float f = (float)chapterStarCount / num * -790f;
		Vector2 offsetMax2 = trRewardGauge.offsetMax;
		offsetMax2.x = 0f - (395f - Mathf.Abs(f));
		trRewardGauge.offsetMax = offsetMax2;
		btnReward.enabled = ((float)chapterStarCount >= num);
	}

	private void OnChapterOpenConnectComplete()
	{
		goOpen.SetActive(value: false);
		goLock.SetActive(value: false);
		goLockParent.SetActive(value: false);
		goRewardObj.SetActive(value: true);
		Transform transform = MWPoolManager.Spawn("Effect", "FX_Quickroot", null, 3f);
		transform.position = Vector3.zero;
		SoundController.EffectSound_Play(EffectSoundType.OpenChapter);
	}

	private void OnRewardCollectComplete()
	{
		StartCoroutine(ProcessGetJewelEffect());
		ShowRewardState();
	}

	private IEnumerator ProcessGetJewelEffect()
	{
		int jewelCount = UnityEngine.Random.Range(4, 8);
		for (int i = 0; i < jewelCount; i++)
		{
			float num = UnityEngine.Random.Range(-0.2f, -0.1f);
			Transform transform = MWPoolManager.Spawn("Effect", "FX_jewel_get", null, 1.2f + num + 0.4f);
			transform.position = goRewardEffect.transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
			GameObject gameObject = transform.gameObject;
			Vector3 userJewelPosition = LobbyManager.UserJewelPosition;
			LeanTween.moveX(gameObject, userJewelPosition.x, 1.2f + num).setEaseInCubic();
			GameObject gameObject2 = transform.gameObject;
			Vector3 userJewelPosition2 = LobbyManager.UserJewelPosition;
			LeanTween.moveY(gameObject2, userJewelPosition2.y, 1.2f + num);
		}
		yield return new WaitForSeconds(1.2f);
		GameDataManager.UpdateUserData();
		SoundController.EffectSound_Play(EffectSoundType.GetCoin);
	}

	public void OnClickUnLock()
	{
		Protocol_Set.Protocol_user_chapter_open_Req(chapterDbData.stage, chapterDbData.chapter, OnChapterOpenConnectComplete);
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickReward()
	{
		float num = GameDataManager.GetLevelListDbData(chapterDbData.stage, chapterDbData.chapter).Count * 3;
		if ((float)chapterStarCount >= num && !GameInfo.userData.userStageState[chapterDbData.stage - 1].chapterList[chapterDbData.chapter - 1].isReward)
		{
			Protocol_Set.Protocol_game_chapter_collect_Req(chapterDbData.stage, chapterDbData.chapter, OnRewardCollectComplete);
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		}
	}
}
