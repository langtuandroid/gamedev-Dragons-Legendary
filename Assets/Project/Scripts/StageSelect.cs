using System;
using System.Collections.Generic;
using UnityEngine;

public class StageSelect : LobbyPopupBase
{
	public Action<int> SelectStageEvent;

	public Action GoBackEvent;

	[SerializeField]
	private Transform trContent;

	[SerializeField]
	private PageToggle pageToggle;

	[SerializeField]
	private ScrollSnap scrollSnap;

	private int activePageIndex;

	private List<StageCell> listStageCell = new List<StageCell>();

	public Transform FirstStageCell
	{
		get
		{
			if (listStageCell.Count > 0)
			{
				return listStageCell[0].transform;
			}
			return null;
		}
	}

	public override void Open()
	{
		base.Open();
		base.gameObject.SetActive(value: true);
		Init();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void Complete()
	{
		scrollSnap.ScrollPageSnapEvent = null;
		StageCell[] componentsInChildren = trContent.GetComponentsInChildren<StageCell>();
		foreach (StageCell stageCell in componentsInChildren)
		{
			StageCell stageCell2 = stageCell;
			stageCell2.SelectStageEvent = (Action<int>)Delegate.Remove(stageCell2.SelectStageEvent, new Action<int>(OnSelectStage));
			MasterPoolManager.ReturnToPool("Lobby", stageCell.transform);
		}
	}

	private void Init()
	{
		Vector2 sizeDelta = trContent.GetComponent<RectTransform>().sizeDelta;
		sizeDelta.x = GameDataManager.GetDicStageDbData().Count * 720;
		trContent.GetComponent<RectTransform>().sizeDelta = sizeDelta;
		scrollSnap.ScrollPageSnapEvent = OnChangeScrollPage;
		pageToggle.InitPage(GameDataManager.GetDicStageDbData().Count);
		pageToggle.SetPage(0);
		pageToggle.MoveChapterPage = OnSelectStagePage;
		listStageCell.Clear();
		foreach (KeyValuePair<int, StageDbData> dicStageDbDatum in GameDataManager.GetDicStageDbData())
		{
			StageCell component = MasterPoolManager.SpawnObject("Lobby", "StgCell", trContent).GetComponent<StageCell>();
			component.transform.localPosition = Vector3.zero;
			component.transform.localScale = Vector3.one;
			component.SetStageId(dicStageDbDatum.Value.stageIdx);
			component.SetStageImage(GameDataManager.GetStageImage(dicStageDbDatum.Value.stageIdx - 1));
			component.SetStageName(dicStageDbDatum.Value.stageName);
			if (dicStageDbDatum.Value.stageIdx <= GameInfo.userData.userStageState.Length)
			{
				component.SetStageClearRate(Mathf.Clamp(GameInfo.userData.userStageState[dicStageDbDatum.Value.stageIdx - 1].clearRate, 0, 100));
			}
			else if (dicStageDbDatum.Value.stageLock)
			{
				component.CommingSoon();
			}
			else
			{
				component.Lock();
			}
			StageCell stageCell = component;
			stageCell.SelectStageEvent = (Action<int>)Delegate.Combine(stageCell.SelectStageEvent, new Action<int>(OnSelectStage));
			listStageCell.Add(component);
		}
		int newCellIndex = 0;
		if (GameInfo.userData.userStageState.Length != 0)
		{
			newCellIndex = GameInfo.userData.userStageState.Length - 1;
		}
		scrollSnap.SnapToIndex(newCellIndex);
	}

	private void OnSelectStage(int index)
	{
		LobbyManager.ShowChapterList(index);
		if (SelectStageEvent != null)
		{
			SelectStageEvent(index);
		}
	}

	private void OnChangeScrollPage(int page)
	{
		activePageIndex = page;
		pageToggle.SetPage(page);
	}

	private void OnSelectStagePage(int _page)
	{
		scrollSnap.SnapToIndex(_page);
	}

	public void OnClickStageSelect()
	{
		OnSelectStage(activePageIndex + 1);
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnGoBack()
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
