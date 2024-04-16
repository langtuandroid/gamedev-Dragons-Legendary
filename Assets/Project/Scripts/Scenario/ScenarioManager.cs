using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioManager : GameObjectSingleton<ScenarioManager>
{
	public enum ScenarioDataType
	{
		Intro,
		InGame
	}

	public static Action<int> EndScenarioEvent;

	[SerializeField]
	private GameObject goDimmed;

	[SerializeField]
	private ScenarioDialogue scenarioDialogue;

	[SerializeField]
	private ScenarioIntro scenarioIntro;

	[SerializeField]
	private GameObject goSkipButton;

	[SerializeField]
	private Image imageBg;

	[SerializeField]
	private Transform trBgAnchor;

	private int scenarioIdx;

	private int scenarioSeq;

	private bool isSound = true;

	private string activeBgName = string.Empty;

	private Transform trBg;

	private ScenarioShowType type;

	private ScenarioDataType dataType;

	private ScenarioDbData currentScenarioDbData;

	private Dictionary<int, ScenarioDbData> dicScenarioDbData = new Dictionary<int, ScenarioDbData>();

	public static void Show(int _scenarioIdx, bool isLevelData = false)
	{
		GameObjectSingleton<ScenarioManager>.Inst.scenarioIdx = _scenarioIdx;
		GameObjectSingleton<ScenarioManager>.Inst.dicScenarioDbData = GameDataManager.GetDicScenarioDbData()[_scenarioIdx];
		GameObjectSingleton<ScenarioManager>.Inst.scenarioSeq = 1;
		GameObjectSingleton<ScenarioManager>.Inst.dataType = ScenarioDataType.Intro;
		GameObjectSingleton<ScenarioManager>.Inst.ShowSeq();
		GameObjectSingleton<ScenarioManager>.Inst.PlaySound(GameObjectSingleton<ScenarioManager>.Inst.scenarioIdx);
	}

	public static void ShowInGame(int _scenarioIdx)
	{
		GameObjectSingleton<ScenarioManager>.Inst.scenarioIdx = _scenarioIdx;
		GameObjectSingleton<ScenarioManager>.Inst.dicScenarioDbData = GameDataManager.GetDicScenarioInGameDbData()[_scenarioIdx];
		GameObjectSingleton<ScenarioManager>.Inst.scenarioSeq = 1;
		GameObjectSingleton<ScenarioManager>.Inst.dataType = ScenarioDataType.InGame;
		GameObjectSingleton<ScenarioManager>.Inst.ShowSeq();
		GameObjectSingleton<ScenarioManager>.Inst.isSound = false;
	}

	private void ShowSeq()
	{
		currentScenarioDbData = dicScenarioDbData[scenarioSeq];
		type = (ScenarioShowType)currentScenarioDbData.type;
		switch (type)
		{
		case ScenarioShowType.Intro:
			scenarioDialogue.Hide();
			scenarioIntro.Show(currentScenarioDbData);
			break;
		case ScenarioShowType.Dialogue:
			scenarioIntro.Hide();
			scenarioDialogue.Show(currentScenarioDbData);
			break;
		}
		goSkipButton.SetActive(value: true);
		ShowBg(currentScenarioDbData.bg);
	}

	private void NextStep()
	{
		if (scenarioIntro.MessageFlow)
		{
			scenarioIntro.MessageComplete();
		}
		else if (scenarioDialogue.MessageFlow)
		{
			scenarioDialogue.MessageComplete();
		}
		else
		{
			NextSeq();
		}
	}

	private void NextSeq()
	{
		scenarioSeq++;
		if (dicScenarioDbData.ContainsKey(scenarioSeq))
		{
			ShowSeq();
		}
		else
		{
			EndScenario();
		}
	}

	private void ShowBg(string _bgName)
	{
		if (_bgName == "0")
		{
			goDimmed.SetActive(value: true);
			trBgAnchor.gameObject.SetActive(value: false);
			activeBgName = string.Empty;
		}
		else if (_bgName != activeBgName)
		{
			activeBgName = _bgName;
			if (activeBgName != string.Empty)
			{
				goDimmed.SetActive(value: false);
				trBgAnchor.gameObject.SetActive(value: true);
				ClearBg();
				trBg = MWPoolManager.Spawn("Scenario", GetBgName(activeBgName), null, -1f, isSpeedProcess: false, isScaleChange: false);
			}
			else
			{
				goDimmed.SetActive(value: true);
				trBgAnchor.gameObject.SetActive(value: false);
			}
		}
	}

	private void PlaySound(int _index)
	{
		GameObjectSingleton<ScenarioManager>.Inst.isSound = true;
		switch (_index)
		{
		case 1:
		case 3:
			SoundController.BGM_Play(MusicSoundType.LobbyBGM);
			break;
		case 2:
			SoundController.BGM_Play(MusicSoundType.InGameDragonBgm);
			break;
		}
	}

	private void ClearBg()
	{
		if (trBg != null)
		{
			MWPoolManager.DeSpawn("Scenario", trBg);
			trBg = null;
		}
	}

	private string GetBgName(string _name)
	{
		if (_name != null)
		{
			if (_name == "scenario_bg_001")
			{
				return "Intro_01";
			}
			if (_name == "scenario_bg_002")
			{
				return "Intro_02";
			}
			if (_name == "scenario_bg_003")
			{
				return "Intro_03";
			}
			if (_name == "scenario_bg_004")
			{
				return "Intro_04";
			}
		}
		return string.Empty;
	}

	private void HideScenario()
	{
		goDimmed.SetActive(value: false);
		goSkipButton.SetActive(value: false);
		scenarioIntro.Hide();
		scenarioDialogue.Hide();
		imageBg.gameObject.SetActive(value: false);
	}

	private void EndScenario()
	{
		UnityEngine.Debug.Log("EndScenario");
		scenarioDialogue.Clear();
		HideScenario();
		switch (dataType)
		{
		case ScenarioDataType.Intro:
			GameDataManager.SaveScenarioIntroShow(scenarioIdx);
			break;
		case ScenarioDataType.InGame:
			GameDataManager.SaveScenarioInGameShow(scenarioIdx);
			break;
		}
		if (isSound)
		{
			SoundController.StopAll();
		}
		if (EndScenarioEvent != null)
		{
			EndScenarioEvent(scenarioIdx);
		}
	}

	public void OnClickNextStep()
	{
		NextStep();
	}

	public void OnClickSkip()
	{
		EndScenario();
	}

	private void OnDisable()
	{
		ClearBg();
		activeBgName = string.Empty;
	}
}
