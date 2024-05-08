using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PuzzleUI : MonoBehaviour
{
	public Action<float> OnTimeFlow;

	public Action OnShowBattleReward;

	[FormerlySerializedAs("imageMatchTimerGauge")] [SerializeField]
	private Image _imageMatchTimerGauge;

	[FormerlySerializedAs("imageUserHpGauge")] [SerializeField]
	private Image _imageUserHpGauge;

	[FormerlySerializedAs("textUserHp")] [SerializeField]
	private Text _textUserHp;

	[FormerlySerializedAs("textTurnsState")] [SerializeField]
	private Text _textTurnsState;

	[FormerlySerializedAs("textWaveState")] [SerializeField]
	private Text _textWaveState;

	[FormerlySerializedAs("textIngameCoin")] [SerializeField]
	private Text _textIngameCoin;

	[FormerlySerializedAs("textUserDamage")] [SerializeField]
	private Text _textUserDamage;

	[FormerlySerializedAs("textStar2TurnClear")] [SerializeField]
	private Text _textStar2TurnClear;

	[FormerlySerializedAs("textStar3TurnClear")] [SerializeField]
	private Text _textStar3TurnClear;

	[FormerlySerializedAs("textInGameSpeed")] [SerializeField]
	private Text _textInGameSpeed;

	[FormerlySerializedAs("rtIngameCoin")] [SerializeField]
	private RectTransform _rtIngameCoin;

	[FormerlySerializedAs("btnDefaultPlayInfo")] [SerializeField]
	private Button _btnDefaultPlayInfo;

	[FormerlySerializedAs("btnDetailPlayInfo")] [SerializeField]
	private Button _btnDetailPlayInfo;

	[FormerlySerializedAs("trEffectAnchor")] [SerializeField]
	private Transform _trEffectAnchor;

	[FormerlySerializedAs("trPlayInfo")] [SerializeField]
	private Transform _trPlayInfo;

	[FormerlySerializedAs("goControlLock")] [SerializeField]
	private GameObject _goControlLock;

	[FormerlySerializedAs("goTurnInfo")] [SerializeField]
	private GameObject _goTurnInfo;

	[FormerlySerializedAs("goStar2failureLine")] [SerializeField]
	private GameObject _goStar2failureLine;

	[FormerlySerializedAs("goStar3failureLine")] [SerializeField]
	private GameObject _goStar3failureLine;

	[FormerlySerializedAs("goPauseButton")] [SerializeField]
	private GameObject _goPauseButton;

	[FormerlySerializedAs("goSpeedButton")] [SerializeField]
	private GameObject _goSpeedButton;

	[FormerlySerializedAs("goAttributeIcon")] [SerializeField]
	private GameObject _goAttributeIcon;

	[FormerlySerializedAs("goGameInfo")] [SerializeField]
	private GameObject _goGameInfo;

	[FormerlySerializedAs("goForceClear")] [SerializeField]
	private GameObject _goForceClear;

	[FormerlySerializedAs("goForceWave")] [SerializeField]
	private GameObject _goForceWave;

	[FormerlySerializedAs("goCoinIcon")] [SerializeField]
	private GameObject _goCoinIcon;

	[FormerlySerializedAs("goArenaPointIcon")] [SerializeField]
	private GameObject _goArenaPointIcon;

	[FormerlySerializedAs("goTextShuffling")] [SerializeField]
	private GameObject _goTextShuffling;

	[FormerlySerializedAs("comboPhrase")] [SerializeField]
	private ComboPhrase _comboPhrase;

	[FormerlySerializedAs("battleResult")] [SerializeField]
	private BattleResult _battleResult;

	[FormerlySerializedAs("battleDefeat")] [SerializeField]
	private BattleDefeat _battleDefeat;

	[FormerlySerializedAs("battleLose")] [SerializeField]
	private BattleLose _battleLose;

	[FormerlySerializedAs("battleReward")] [SerializeField]
	private BattleReward _battleReward;

	[FormerlySerializedAs("arenaChestOpen")] [SerializeField]
	private ArenaChestOpen _arenaChestOpen;

	[FormerlySerializedAs("jewelShop")] [SerializeField]
	private JewelShop _jewelShop;

	[FormerlySerializedAs("jewelShopBuy")] [SerializeField]
	private JewelShopBuy _jewelShopBuy;

	[FormerlySerializedAs("notEnoughJewelIngame")] [SerializeField]
	private NotEnoughJewelIngame _notEnoughJewelIngame;

	[FormerlySerializedAs("Pause")] [SerializeField]
	private Pause _Pause;

	[FormerlySerializedAs("explain_Attribute")] [SerializeField]
	private GameObject _explainAttribute;

	[FormerlySerializedAs("goEffectWarning")] [SerializeField]
	private GameObject _warningEffect;

	[FormerlySerializedAs("arrGoBoostItemIcon")] [SerializeField]
	private GameObject[] _arrGoBoostItemIcon = new GameObject[0];

	private bool _timerMatch;

	private bool _playTimer = true;

	private bool _aciveMatch;

	private float _currentHP;

	private float _maxHP;

	private float _timerStamp;

	private float _matchTimer;

	private float _maxMatchTime;

	private float _addMatchTime;

	private int _thisTurn = 1;

	private int _thisCoin;

	private int _arenaPoint;

	private Vector3 _uiShowPosition = new Vector3(-150f, -110f, 0f);

	private Vector3 _hideUIPos = new Vector3(0f, -110f, 0f);

	private Vector3 _pingPong = new Vector3(1.4f, 1.4f, 1.4f);

	private Dictionary<int, ComboMultiply> _dicComboMultiply = new Dictionary<int, ComboMultiply>();

	private Dictionary<int, HeroAttackUI> _dicHunterAttackUI = new Dictionary<int, HeroAttackUI>();

	private Coroutine _coroutineMatchTimer;

	private Coroutine _coroutineClearTurnInfo;

	private Coroutine _coroutineCoinBox;

	private Coroutine _coroutineDamage;

	private Coroutine _coroutineHp;

	public bool MatchTimerState => _timerMatch;

	public bool MatchTimeEnd => _matchTimer <= 0f;

	public bool Match => _aciveMatch;

	public BattleReward BattleReward => _battleReward;

	public Transform PlayInfo => _trPlayInfo;

	public void Construct()
	{
		_btnDefaultPlayInfo.enabled = (GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Stage);
		_btnDetailPlayInfo.enabled = (GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Stage);
		_textTurnsState.gameObject.SetActive(GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Stage);
		_goCoinIcon.SetActive(GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Stage);
		_goArenaPointIcon.SetActive(GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Arena);
	}

	public void ClearTurnInfo()
	{
		_textStar2TurnClear.text = string.Format("{0} {1}", GameInfo.inGamePlayData.star2ClearTurn, MasterLocalize.GetData("common_text_turns"));
		_textStar3TurnClear.text = string.Format("{0} {1}", GameInfo.inGamePlayData.star3ClearTurn, MasterLocalize.GetData("common_text_turns"));
		_goStar2failureLine.SetActive(value: false);
		_goStar3failureLine.SetActive(value: false);
	}

	public void ContinueTimer()
	{
		_timerStamp = Time.time;
		_playTimer = true;
	}

	public void StopTimer()
	{
		_playTimer = false;
	}

	public void IsGameTimerReady()
	{
		UnityEngine.Debug.Log("ReadyMatchTimer");
		_imageMatchTimerGauge.fillAmount = 0f;
		_matchTimer = GameInfo.inGamePlayData.matchTime;
		_maxMatchTime = GameInfo.inGamePlayData.matchTime;
		if (GameInfo.inGamePlayData.dicActiveBoostItem.ContainsKey(4))
		{
			_matchTimer += 1f;
			_maxMatchTime += 1f;
		}
		_goControlLock.SetActive(value: false);
	}

	public void StartTimer()
	{
		if (!_timerMatch)
		{
			_addMatchTime = GameInfo.inGamePlayData.matchTimeBonus;
			_dicComboMultiply.Clear();
			_dicHunterAttackUI.Clear();
			EndTimer();
			_timerMatch = true;
			_coroutineMatchTimer = StartCoroutine(MatchTimerCheck());
			_goControlLock.SetActive(value: false);
		}
	}

	public void AddMatchTimer()
	{
		UnityEngine.Debug.Log("AddMatchTime");
		if (!(_matchTimer <= 0f) && !_goControlLock.activeSelf)
		{
			_matchTimer += _addMatchTime;
			_matchTimer = Mathf.Min(_matchTimer, _maxMatchTime);
			_addMatchTime *= (float)GameInfo.inGamePlayData.matchTimeRatio * 0.01f;
		}
	}

	public void DeactivateTimer()
	{
		EndTimer();
		PuzzlePlayManager.EndMatch();
		_aciveMatch = true;
	}

	private void EndTimer()
	{
		if (_coroutineMatchTimer != null)
		{
			StopCoroutine(_coroutineMatchTimer);
			_coroutineMatchTimer = null;
		}
		_imageMatchTimerGauge.fillAmount = 1f;
		_timerMatch = false;
		_goControlLock.SetActive(value: true);
	}

	public void LockControl()
	{
		_goControlLock.SetActive(value: true);
	}

	public void ResumeControl()
	{
		_goControlLock.SetActive(value: false);
	}

	public void UpdateWave(int wave)
	{
		_textWaveState.text = $"wave {wave}/{GameInfo.inGamePlayData.dicWaveDbData.Count}";
	}

	public void UpdateTurn(int turn)
	{
		_aciveMatch = false;
		_thisTurn = turn;
		_textTurnsState.text = $"{turn}";
		if (_thisTurn > GameInfo.inGamePlayData.star2ClearTurn)
		{
			_goStar2failureLine.SetActive(value: true);
		}
		if (_thisTurn > GameInfo.inGamePlayData.star3ClearTurn)
		{
			_goStar3failureLine.SetActive(value: true);
		}
	}

	public void SetHP(int maxHp)
	{
		_currentHP = maxHp;
		_maxHP = maxHp;
		RefreshHP();
	}

	public void ComboAdd(int _combo)
	{
		_comboPhrase.Show(_combo);
	}

	public void BlockCompleted()
	{
		_comboPhrase.Complete();
	}

	public void Damage(int value)
	{
		float prevHp = _currentHP;
		_currentHP -= value;
		if (_currentHP < 0f)
		{
			_currentHP = 0f;
		}
		else
		{
			DamageTween(value);
		}
		UnityEngine.Debug.Log("************ userCurrentHp = " + _currentHP);
		if (_currentHP <= 0f && PuzzlePlayManager.LeaderSkill() != null)
		{
			PuzzlePlayManager.LeaderSkill().CheckLeaderSkillHp1(prevHp);
		}
		PuzzlePlayManager.ShakeEffect();
		RefreshHP();
	}

	public void HealHeroes(int value)
	{
		_currentHP += value;
		if (_currentHP > _maxHP)
		{
			_currentHP = _maxHP;
		}
		RefreshHP();
	}

	public void MaxHeal()
	{
		_currentHP = _maxHP;
		RefreshHP();
	}

	public void AddCoins(int addCoin)
	{
		if (_coroutineCoinBox != null)
		{
			StopCoroutine(_coroutineCoinBox);
			_coroutineCoinBox = null;
		}
		_thisCoin += addCoin;
		_textIngameCoin.text = $"{_thisCoin}";
		if (addCoin > 0)
		{
			GameInfo.userPlayData.AddCoin(addCoin);
			_coroutineCoinBox = StartCoroutine(AddCoinsProcces(addCoin));
		}
	}

	public void ArenaPoints(int _addPoint)
	{
		if (_coroutineCoinBox != null)
		{
			StopCoroutine(_coroutineCoinBox);
			_coroutineCoinBox = null;
		}
		_arenaPoint += _addPoint;
		_textIngameCoin.text = $"{_arenaPoint}";
		if (_addPoint > 0)
		{
			GameInfo.userPlayData.AddArenaPoint(_addPoint);
			_coroutineCoinBox = StartCoroutine(AddCoinsProcces(_addPoint));
		}
	}

	public void ComboAdd(float combo, int hunterColor, Vector3 position, int hunterIdx)
	{
		if (!(combo < 2f))
		{
			if (!_dicComboMultiply.ContainsKey(hunterIdx))
			{
				ComboMultiply component = MasterPoolManager.SpawnObject("Effect", "Text_HunterAttackMultiply", _trEffectAnchor).GetComponent<ComboMultiply>();
				_dicComboMultiply.Add(hunterIdx, component);
			}
			_dicComboMultiply[hunterIdx].ShowCombo(hunterColor, combo, position + new Vector3(0f, 0.8f, 0f));
		}
	}

	public void AttackAdd(int attack, int hunterColor, Vector3 position, int hunterIdx)
	{
		if (!_dicHunterAttackUI.ContainsKey(hunterIdx))
		{
			HeroAttackUI component = MasterPoolManager.SpawnObject("Effect", "Text_HunterAttackDamage", _trEffectAnchor).GetComponent<HeroAttackUI>();
			_dicHunterAttackUI.Add(hunterIdx, component);
		}
		_dicHunterAttackUI[hunterIdx].ShowAttack(attack, hunterColor, position);
	}

	public void ShowMonsterDamage(EnemyDamageUI.EnemyDamageType type, int damage, Vector3 position)
	{
		EnemyDamageUI component = MasterPoolManager.SpawnObject("Effect", "Text_MonsterDamage", _trEffectAnchor).GetComponent<EnemyDamageUI>();
		component.OpenDamageUI(type, damage, position);
	}

	public void ClearHunterUI(int hunterIdx)
	{
		if (_dicHunterAttackUI.ContainsKey(hunterIdx))
		{
			_dicHunterAttackUI[hunterIdx].Clear();
		}
	}

	public void ClearResult()
	{
		_battleResult.Show(_thisTurn);
	}

	public void ArenaClear()
	{
		_arenaChestOpen.Show();
	}

	public void DefaultResult()
	{
		_battleDefeat.Show();
	}

	public void BattleLoseShow()
	{
		_battleLose.Show();
	}

	public void ArenaLoseResult(ARENA_GAME_END_RESULT _data)
	{
		_battleLose.ShowArena(_data);
	}

	public void ClearResultShow()
	{
		if (GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Stage)
		{
			_battleReward.Init();
			if (OnShowBattleReward != null)
			{
				OnShowBattleReward();
			}
		}
	}

	public void NotEnoughJewel()
	{
		_notEnoughJewelIngame.Init();
	}

	public void ShowJewelShop()
	{
		
	}

	public void ShowJewelShopBuy(int key)
	{
		_jewelShopBuy.Init(key);
	}

	public void CloseJewelShop()
	{
		_jewelShop.ClosePopup();
	}

	public void CloseJewelShopBuy()
	{
		_jewelShopBuy.ClosePopup();
	}

	public void ShowPause()
	{
		_Pause.Init();
	}

	public void ContinueGame()
	{
		MaxHeal();
		ResumeControl();
		_timerMatch = false;
		_playTimer = true;
		PuzzlePlayManager.StartTurn();
		_battleDefeat.Hide();
	}

	public void BuffUI(Vector3 _position, BlockType _type, int _buff)
	{
		if (_buff != 1)
		{
			string text = string.Empty;
			switch (_type)
			{
			case BlockType.Blue:
				text = "PowerBonus_B";
				break;
			case BlockType.Green:
				text = "PowerBonus_G";
				break;
			case BlockType.Purple:
				text = "PowerBonus_P";
				break;
			case BlockType.Red:
				text = "PowerBonus_R";
				break;
			case BlockType.Yellow:
				text = "PowerBonus_Y";
				break;
			}
			if (!(text == string.Empty))
			{
				Transform transform = MasterPoolManager.SpawnObject("Effect", text, _trEffectAnchor);
				transform.position = new Vector3(_position.x - 0.19f, _position.y + 0.19f, _position.z);
				transform.GetComponent<Text>().text = $"{_buff}x";
			}
		}
	}

	public void OpenShuffleUI()
	{
		_goTextShuffling.SetActive(value: true);
	}

	public void CloseShuffleUI()
	{
		_goTextShuffling.SetActive(value: false);
	}

	public bool CheckUserDie()
	{
		return _currentHP <= 0f;
	}

	public void OpenWarningEffect()
	{
		_warningEffect.SetActive(value: true);
		SoundController.EffectSound_Play(EffectSoundType.InGameWarning);
	}

	public void CloseWarningEffect()
	{
		_warningEffect.SetActive(value: false);
		SoundController.EffectSound_Stop(EffectSoundType.InGameWarning);
	}

	private void OpenBoostItemIcon()
	{
		foreach (KeyValuePair<int, BoostItemDbData> item in GameInfo.inGamePlayData.dicActiveBoostItem)
		{
			_arrGoBoostItemIcon[item.Key - 1].SetActive(value: true);
		}
	}

	private void RefreshHP()
	{
		_textUserHp.text = $"{_currentHP}";
		MoveHpGauge(Mathf.Clamp(_currentHP / _maxHP, 0f, 1f));
	}

	private void MoveHpGauge(float target)
	{
		StopHpGauge();
		_coroutineHp = StartCoroutine(ProcessHpGauge(target));
	}

	private void StopHpGauge()
	{
		if (_coroutineHp != null)
		{
			StopCoroutine(_coroutineHp);
			_coroutineHp = null;
		}
	}

	private IEnumerator ProcessHpGauge(float target)
	{
		float gap = Mathf.Abs(target - _imageUserHpGauge.fillAmount) / 6f;
		if (target > _imageUserHpGauge.fillAmount)
		{
			while (target - _imageUserHpGauge.fillAmount > 0f)
			{
				_imageUserHpGauge.fillAmount += gap;
				yield return null;
			}
		}
		else
		{
			while (_imageUserHpGauge.fillAmount - target > 0f)
			{
				UnityEngine.Debug.Log("----------");
				_imageUserHpGauge.fillAmount -= gap;
				yield return null;
			}
		}
		_imageUserHpGauge.fillAmount = target;
		_coroutineHp = null;
	}

	private void DamageTween(int damage)
	{
		if (_coroutineDamage != null)
		{
			StopCoroutine(_coroutineDamage);
			_coroutineDamage = null;
		}
		_coroutineDamage = StartCoroutine(StartDamageTween(damage));
	}

	private IEnumerator StartDamageTween(int damage)
	{
		_textUserDamage.gameObject.SetActive(value: true);
		_textUserDamage.text = $"-{damage}";
		LeanTween.scale(_textUserDamage.gameObject, _pingPong, 0.2f).setLoopPingPong(1).setEase(LeanTweenType.linear);
		yield return new WaitForSeconds(0.9f);
		_textUserDamage.gameObject.SetActive(value: false);
		_coroutineDamage = null;
	}

	private IEnumerator MatchTimerCheck()
	{
		_matchTimer = _maxMatchTime;
		_timerStamp = Time.time;
		_imageMatchTimerGauge.fillAmount = 0f;
		while (_matchTimer > 0f)
		{
			yield return null;
			if (_playTimer)
			{
				_matchTimer -= Time.time - _timerStamp;
				_timerStamp = Time.time;
				if (OnTimeFlow != null)
				{
					OnTimeFlow(Mathf.Ceil(_matchTimer));
				}
				_imageMatchTimerGauge.fillAmount = (_maxMatchTime - _matchTimer) / _maxMatchTime;
			}
		}
		_imageMatchTimerGauge.fillAmount = 1f;
		_goControlLock.SetActive(value: true);
		PuzzlePlayManager.EndMatch();
		_timerMatch = false;
		_aciveMatch = true;
		SoundController.EffectSound_Play(EffectSoundType.TimeOver);
	}

	private IEnumerator AddCoinsProcces(int addCoin)
	{
		_rtIngameCoin.anchoredPosition = _hideUIPos;
		LeanTween.move(_rtIngameCoin, _uiShowPosition, 0.3f);
		yield return new WaitForSeconds(1.3f);
		LeanTween.move(_rtIngameCoin, _hideUIPos, 0.3f);
		_coroutineCoinBox = null;
	}

	private IEnumerator TurnShowInfo()
	{
		yield return new WaitForSeconds(3f);
		_goTurnInfo.SetActive(value: false);
		if (_coroutineClearTurnInfo != null)
		{
			StopCoroutine(_coroutineClearTurnInfo);
			_coroutineClearTurnInfo = null;
		}
	}

	public void OnClickShowTurnInfo()
	{
		_goTurnInfo.SetActive(value: true);
		if (_coroutineClearTurnInfo != null)
		{
			StopCoroutine(_coroutineClearTurnInfo);
			_coroutineClearTurnInfo = null;
		}
		_coroutineClearTurnInfo = StartCoroutine(TurnShowInfo());
	}

	public void OnClickHideTurnInfo()
	{
		_goTurnInfo.SetActive(value: false);
		if (_coroutineClearTurnInfo != null)
		{
			StopCoroutine(_coroutineClearTurnInfo);
			_coroutineClearTurnInfo = null;
		}
	}

	public void OnClickAttributeIcon()
	{
		PuzzlePlayManager.LockTouch();
		_explainAttribute.gameObject.SetActive(value: true);
	}

	public void OnClickAttributePopup()
	{
		PuzzlePlayManager.ActivateTouch();
		_explainAttribute.gameObject.SetActive(value: false);
	}

	public void OnClickSpeedChange()
	{
		if (GameInfo.inGameBattleSpeedRate == 1f)
		{
			GameInfo.inGameBattleSpeedRate = 2f;
		}
		else
		{
			GameInfo.inGameBattleSpeedRate = 1f;
		}
		_textInGameSpeed.text = $"x{GameInfo.inGameBattleSpeedRate}";
		PuzzlePlayManager.ChangeSpeed();
	}

	private void Awake()
	{
		_goForceClear.SetActive(value: false);
		_goForceWave.SetActive(value: false);
		_rtIngameCoin.anchoredPosition = _hideUIPos;
		_goControlLock.SetActive(value: true);
		_goTurnInfo.SetActive(value: false);
		_comboPhrase.Init();
		ClearTurnInfo();
		UpdateWave(1);
		UpdateTurn(1);
		_textInGameSpeed.text = $"x{GameInfo.inGameBattleSpeedRate}";
		if (GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Stage)
		{
			if (GameInfo.inGamePlayData.levelIdx == 0 || GameInfo.inGamePlayData.levelIdx == 1 || GameInfo.inGamePlayData.levelIdx == 2)
			{
				_goPauseButton.SetActive(value: false);
			}
			if (GameInfo.inGamePlayData.levelIdx == 0)
			{
				_goSpeedButton.SetActive(value: false);
				_goAttributeIcon.SetActive(value: false);
				_goGameInfo.SetActive(value: false);
			}
		}
		OpenBoostItemIcon();
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}
}
