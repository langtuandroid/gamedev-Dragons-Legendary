using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelGameBlock : MonoBehaviour
{
	public Action OnSelectLevelCell;

	[FormerlySerializedAs("textLevelIndex")] [SerializeField]
	private Text _textLevelIndex;

	[FormerlySerializedAs("textLevelIndexForLock")] [SerializeField]
	private Text _textLevelIndexForLock;

	[FormerlySerializedAs("textEnegyCost")] [SerializeField]
	private Text _textEnegyCost;

	[FormerlySerializedAs("trRewardItemAnchor")] [SerializeField]
	private Transform _trRewardItemAnchor;

	[FormerlySerializedAs("goLock")] [SerializeField]
	private GameObject _goLock;

	[FormerlySerializedAs("goBoss")] [SerializeField]
	private GameObject _goBoss;

	[FormerlySerializedAs("goBossForLock")] [SerializeField]
	private GameObject _goBossForLock;

	[FormerlySerializedAs("arrGoStar")] [SerializeField]
	private GameObject[] _arrGoStar = new GameObject[0];

	private bool _isLock = true;

	private int _userStarCount;

	private string _typeKey;

	private TimeCheckState _timeState;

	private Transform _trRewardItem;

	private LevelGameDbData _levelDbData;

	private UserLevelState _userLevelState;

	public int LevelIdx => _levelDbData.levelIdx;

	public int StarCount => _userStarCount;

	public void SetData(LevelGameDbData data)
	{
		_levelDbData = data;
		Construct();
	}

	public void SetUnLock()
	{
		_isLock = false;
	}

	public void PlaceStars(int count)
	{
		_userStarCount = count;
		_goLock.SetActive(count < 0);
		_isLock = (count < 0);
		for (int i = 0; i < _arrGoStar.Length; i++)
		{
			_arrGoStar[i].SetActive(i + 1 <= count);
		}
	}

	public void Reset()
	{
		_userLevelState = GameInfo.userData.GetUserLevelState(_levelDbData.stage - 1, _levelDbData.chapter - 1, _levelDbData.levelIdx);
		if (_userLevelState != null)
		{
			_trRewardItemAnchor.gameObject.SetActive(value: true);
			_trRewardItem = MasterPoolManager.SpawnObject("Item", $"Item_{_levelDbData.rewardFixItem}", _trRewardItemAnchor);
		}
		else
		{
			_trRewardItemAnchor.gameObject.SetActive(value: false);
		}
		PlaceStars(GameDataManager.GetLevelStarCount(_levelDbData.stage, _levelDbData.chapter, _levelDbData.level));
	}

	private void Construct()
	{
		_textLevelIndex.text = $"{_levelDbData.level}";
		_textLevelIndexForLock.text = $"{_levelDbData.level}";
		_textEnegyCost.text = $"{_levelDbData.energyCost}";
		_textLevelIndex.gameObject.SetActive(_levelDbData.specialMark != 1);
		_textLevelIndexForLock.gameObject.SetActive(_levelDbData.specialMark != 1);
		_goBoss.SetActive(_levelDbData.specialMark == 1);
		_goBossForLock.SetActive(_levelDbData.specialMark == 1);
		_userLevelState = GameInfo.userData.GetUserLevelState(_levelDbData.stage - 1, _levelDbData.chapter - 1, _levelDbData.levelIdx);
		_typeKey = $"Stage_{GameInfo.inGamePlayData.stage}_Level_{_levelDbData.level}";
		Transform[] componentsInChildren = _trRewardItemAnchor.GetComponentsInChildren<Transform>(includeInactive: true);
		foreach (Transform trObj in componentsInChildren)
		{
			MasterPoolManager.ReturnToPool("Item", trObj);
		}
		_trRewardItem = null;
		if (_userLevelState != null)
		{
			_trRewardItemAnchor.gameObject.SetActive(value: true);
			_trRewardItem = MasterPoolManager.SpawnObject("Item", $"Item_{_levelDbData.rewardFixItem}", _trRewardItemAnchor);
		}
		else
		{
			_trRewardItemAnchor.gameObject.SetActive(value: false);
		}
		LocalTimeCheckManager.SaveAndExit(_typeKey);
	}

	public void OnClickLevelSelect()
	{
		if (!_isLock)
		{
			GameInfo.inGamePlayData.level = _levelDbData.level;
			GameInfo.inGamePlayData.levelIdx = _levelDbData.levelIdx;
			if (_userStarCount == 3)
			{
				//LobbyManager.ShowQuickLoot(levelDbData.levelIdx); //TODO 
				LobbyManager.ShowLevelPlay(_levelDbData.levelIdx);
			}
			else
			{
				LobbyManager.ShowLevelPlay(_levelDbData.levelIdx);
			}
			if (OnSelectLevelCell != null)
			{
				OnSelectLevelCell();
			}
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		}
	}

	private void OnDisable()
	{
		if (_trRewardItem != null)
		{
			MasterPoolManager.ReturnToPool("Item", _trRewardItem);
			_trRewardItem = null;
		}
	}
}
