using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : LobbyPopupBase
{
	public Action GoBackEvent;

	[SerializeField]
	private Transform trContent;

	[SerializeField]
	private Text textStageName;

	[SerializeField]
	private PageToggle pageToggle;

	[SerializeField]
	private ScrollSnap scrollSnap;

	[SerializeField]
	private ScrollRect scrollRect;

	private int stageId;

	public LevelCell SecondLevelCell
	{
		get
		{
			LevelCell[] componentsInChildren = base.gameObject.GetComponentsInChildren<LevelCell>();
			foreach (LevelCell levelCell in componentsInChildren)
			{
				if (levelCell.LevelIdx == 2)
				{
					return levelCell;
				}
			}
			return null;
		}
	}

	public void Show(int id)
	{
		base.Show();
		base.gameObject.SetActive(value: true);
		stageId = id;
		Init();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void HideProcessComplete()
	{
		ChapterBox[] componentsInChildren = trContent.GetComponentsInChildren<ChapterBox>();
		foreach (ChapterBox chapterBox in componentsInChildren)
		{
			chapterBox.Clear();
			MWPoolManager.DeSpawn("Lobby", chapterBox.transform);
		}
	}

	public void Refresh()
	{
		ChapterBox[] componentsInChildren = trContent.GetComponentsInChildren<ChapterBox>();
		foreach (ChapterBox chapterBox in componentsInChildren)
		{
			chapterBox.Refresh();
		}
	}

	private void Init()
	{
		textStageName.text = $"- {MWLocalize.GetData(GameDataManager.GetDicStageDbData()[stageId].stageName)} -";
		Vector2 sizeDelta = trContent.GetComponent<RectTransform>().sizeDelta;
		sizeDelta.x = GameDataManager.GetDicChapterDbData(stageId).Count * 720;
		trContent.GetComponent<RectTransform>().sizeDelta = sizeDelta;
		scrollSnap.ScrollPageSnapEvent = OnChangeScrollPage;
		pageToggle.InitPage(GameDataManager.GetDicChapterDbData(stageId).Count);
		pageToggle.SetPage(0);
		foreach (KeyValuePair<int, ChapterDbData> dicChapterDbDatum in GameDataManager.GetDicChapterDbData(stageId))
		{
			ChapterBox component = MWPoolManager.Spawn("Lobby", "ChapterBox", trContent).GetComponent<ChapterBox>();
			component.transform.localScale = Vector3.one;
			component.SetData(dicChapterDbDatum.Value);
			if (dicChapterDbDatum.Value.stage > GameInfo.userData.userStageState.Length || dicChapterDbDatum.Value.chapter > GameInfo.userData.userStageState[dicChapterDbDatum.Value.stage - 1].chapterList.Length)
			{
				component.Lock();
			}
			else
			{
				component.SetOpen(GameInfo.userData.userStageState[dicChapterDbDatum.Value.stage - 1].chapterList[dicChapterDbDatum.Value.chapter - 1].isOpen);
			}
		}
		int newCellIndex = 0;
		if (GameInfo.userData.userStageState[stageId - 1].chapterList.Length != 0)
		{
			newCellIndex = GameInfo.userData.userStageState[stageId - 1].chapterList.Length - 1;
		}
		scrollSnap.SnapToIndex(newCellIndex);
		pageToggle.MoveChapterPage = OnSelectChapterPage;
	}

	private void OnChangeScrollPage(int page)
	{
		pageToggle.SetPage(page);
		GameInfo.inGamePlayData.chapter = page + 1;
	}

	private void OnSelectChapterPage(int _page)
	{
		scrollSnap.SnapToIndex(_page);
	}

	public void OnClickGoBack()
	{
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}

	private void Start()
	{
	}
}
