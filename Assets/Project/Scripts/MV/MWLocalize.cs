using System;
using System.Collections.Generic;
using UnityEngine;

public class MWLocalize : GameObjectSingleton<MWLocalize>
{
	public static Action RefreshFont;

	private const string LOCALIZE_LANGUAGE_KEY = "LocalizeLangugeKey";

	[SerializeField]
	private Font enFont;

	[SerializeField]
	private Font koFont;

	[SerializeField]
	private Font ruFont;

	private int localizeIndex;

	private Dictionary<string, List<string>> dicLocalizeData = new Dictionary<string, List<string>>();

	public static int GetLanguage => GameObjectSingleton<MWLocalize>.Inst.localizeIndex;

	public static Font GameFont
	{
		get
		{
			if (GameObjectSingleton<MWLocalize>.Inst == null)
			{
				return null;
			}
			switch (GameObjectSingleton<MWLocalize>.Inst.localizeIndex)
			{
			case 0:
				return GameObjectSingleton<MWLocalize>.Inst.koFont;
			case 2:
				return GameObjectSingleton<MWLocalize>.Inst.ruFont;
			default:
				return GameObjectSingleton<MWLocalize>.Inst.enFont;
			}
		}
	}

	public static void SetDicData(Dictionary<string, List<string>> _dicData)
	{
		GameObjectSingleton<MWLocalize>.Inst.dicLocalizeData = _dicData;
	}

	public static string GetData(string key)
	{
		if (!GameObjectSingleton<MWLocalize>.Inst.dicLocalizeData.ContainsKey(key))
		{
			return null;
		}
		return GameObjectSingleton<MWLocalize>.Inst.dicLocalizeData[key][GameObjectSingleton<MWLocalize>.Inst.localizeIndex];
	}

	public static void SetLanguege(int _lan)
	{
		if (GameObjectSingleton<MWLocalize>.Inst.localizeIndex != _lan)
		{
			GameObjectSingleton<MWLocalize>.Inst.localizeIndex = _lan;
			GameObjectSingleton<MWLocalize>.Inst.SaveLanguage();
			MWPoolManager.DeSpawnPoolAll("Effect");
			MWPoolManager.DeSpawnPoolAll("Info");
			MWPoolManager.DeSpawnPoolAll("Item");
			MWPoolManager.DeSpawnPoolAll("Lobby");
			GameDataManager.MoveScene(SceneType.Lobby);
		}
	}

	private void Init()
	{
		localizeIndex = LoadLanguage();
		if (localizeIndex < 0)
		{
			localizeIndex = GetSystemLocalizeIndex();
		}
	}

	private void SaveLanguage()
	{
		PlayerPrefs.SetInt("LocalizeLangugeKey", localizeIndex);
		if (RefreshFont != null)
		{
			RefreshFont();
		}
	}

	private int LoadLanguage()
	{
		return PlayerPrefs.GetInt("LocalizeLangugeKey", -1);
	}

	private int GetSystemLocalizeIndex()
	{
		return 1; //TODO
		switch (Application.systemLanguage)
		{
		case SystemLanguage.Korean:
			return 0;
		case SystemLanguage.Russian:
			return 2;
		default:
			return 1;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		Init();
	}
}
