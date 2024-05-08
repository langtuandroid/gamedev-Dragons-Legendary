using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MasterLocalize : GameObjectSingleton<MasterLocalize>
{
	public static Action OnCangeFont;

	[FormerlySerializedAs("enFont")] [SerializeField]
	private Font _enFont;

	[FormerlySerializedAs("koFont")] [SerializeField]
	private Font _koFont;

	[FormerlySerializedAs("ruFont")] [SerializeField]
	private Font _ruFont;

	private int _localizeIndex;

	private Dictionary<string, List<string>> _dicLocalizeData = new Dictionary<string, List<string>>();

	public static int GetLanguage => Inst._localizeIndex;

	public static Font GameFont
	{
		get
		{
			if (Inst == null)
			{
				return null;
			}
			switch (Inst._localizeIndex)
			{
			case 0:
				return Inst._koFont;
			case 2:
				return Inst._ruFont;
			default:
				return Inst._enFont;
			}
		}
	}

	public static void DicData(Dictionary<string, List<string>> _dicData)
	{
		Inst._dicLocalizeData = _dicData;
	}

	public static string GetData(string key)
	{
		if (!Inst._dicLocalizeData.ContainsKey(key))
		{
			return null;
		}
		return Inst._dicLocalizeData[key][Inst._localizeIndex];
	}

	public static void ChangeLanguage(int _lan)
	{
		if (Inst._localizeIndex != _lan)
		{
			Inst._localizeIndex = _lan;
			Inst.ChangeLanguage();
			MasterPoolManager.ReturnToPoolAll("Effect");
			MasterPoolManager.ReturnToPoolAll("Info");
			MasterPoolManager.ReturnToPoolAll("Item");
			MasterPoolManager.ReturnToPoolAll("Lobby");
			GameDataManager.MoveScene(SceneType.Lobby);
		}
	}

	private void Construct()
	{
		_localizeIndex = RestoreLanguage();
		if (_localizeIndex < 0)
		{
			_localizeIndex = GetSystemLocalizeIndex();
		}
	}

	private void ChangeLanguage()
	{
		PlayerPrefs.SetInt("LocalizeLangugeKey", _localizeIndex);
		if (OnCangeFont != null)
		{
			OnCangeFont();
		}
	}

	private int RestoreLanguage()
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
		Construct();
	}
}
