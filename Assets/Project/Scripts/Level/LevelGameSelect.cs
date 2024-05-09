using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelGameSelect : LobbyPopupBase
{
	public Action OnGoBackEvent;

	[FormerlySerializedAs("trContent")] [SerializeField]
	private Transform _trContent;

	[FormerlySerializedAs("textStageName")] [SerializeField]
	private Text _textStageName;

	[FormerlySerializedAs("pageToggle")] [SerializeField]
	private PageToggle _pageToggle;

	[FormerlySerializedAs("scrollSnap")] [SerializeField]
	private ScrollSnap _scrollSnap;

	[FormerlySerializedAs("scrollRect")] [SerializeField]
	private ScrollRect _scrollRect;

	private int _stageId;

	public LevelGameBlock SecondLevelCell
	{
		get
		{
			LevelGameBlock[] componentsInChildren = base.gameObject.GetComponentsInChildren<LevelGameBlock>();
			foreach (LevelGameBlock levelCell in componentsInChildren)
			{
				if (levelCell.LevelIdx == 2)
				{
					return levelCell;
				}
			}
			return null;
		}
	}

	public void Open(int id)
	{
		base.Open();
		base.gameObject.SetActive(value: true);
		_stageId = id;
		Construct();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void Complete()
	{
		ChapterBox[] componentsInChildren = _trContent.GetComponentsInChildren<ChapterBox>();
		foreach (ChapterBox chapterBox in componentsInChildren)
		{
			chapterBox.Clear();
			MasterPoolManager.ReturnToPool("Lobby", chapterBox.transform);
		}
	}

	public void Reset()
	{
		ChapterBox[] componentsInChildren = _trContent.GetComponentsInChildren<ChapterBox>();
		foreach (ChapterBox chapterBox in componentsInChildren)
		{
			chapterBox.Refresh();
		}
	}

	private void Construct()
	{
		_textStageName.text = $"- {MasterLocalize.GetData(GameDataManager.GetDicStageDbData()[_stageId].stageName)} -";
		Vector2 sizeDelta = _trContent.GetComponent<RectTransform>().sizeDelta;
		sizeDelta.x = GameDataManager.GetDicChapterDbData(_stageId).Count * 720;
		_trContent.GetComponent<RectTransform>().sizeDelta = sizeDelta;
		_scrollSnap.ScrollPageSnapEvent = ChangePage;
		_pageToggle.InitPage(GameDataManager.GetDicChapterDbData(_stageId).Count);
		_pageToggle.SetPage(0);
		foreach (KeyValuePair<int, ChapterDbData> dicChapterDbDatum in GameDataManager.GetDicChapterDbData(_stageId))
		{
			ChapterBox component = MasterPoolManager.SpawnObject("Lobby", "ChapterBox", _trContent).GetComponent<ChapterBox>();
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
		if (GameInfo.userData.userStageState[_stageId - 1].chapterList.Length != 0)
		{
			newCellIndex = GameInfo.userData.userStageState[_stageId - 1].chapterList.Length - 1;
		}
		_scrollSnap.SnapToIndex(newCellIndex);
		_pageToggle.MoveChapterPage = SelectPage;
	}

	private void ChangePage(int page)
	{
		_pageToggle.SetPage(page);
		GameInfo.inGamePlayData.chapter = page + 1;
	}

	private void SelectPage(int _page)
	{
		_scrollSnap.SnapToIndex(_page);
	}

	public void OnClickGoBack()
	{
		if (OnGoBackEvent != null)
		{
			OnGoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}
}
