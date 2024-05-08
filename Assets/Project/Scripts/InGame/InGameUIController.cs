using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIController : MonoBehaviour
{
	public Action<float> MatchTimeFlow;

	public Action ShowBattleReward;

	private const float COIN_BOX_MOVE_DURATION = 0.3f;

	private const float COIN_BOX_SHOW_DURATION = 1f;

	private const float USER_DAMAGE_TWEEN_DURATION = 0.2f;

	private const float USER_DAMAGE_SHOW_DURATION = 0.5f;

	private const float CLEAR_TURN_INFO_SHOW_DURATION = 3f;

	private const float BOOST_MATCH_TIME_PLUS_TIME = 1f;

	[SerializeField]
	private Image imageMatchTimerGauge;

	[SerializeField]
	private Image imageUserHpGauge;

	[SerializeField]
	private Text textUserHp;

	[SerializeField]
	private Text textTurnsState;

	[SerializeField]
	private Text textWaveState;

	[SerializeField]
	private Text textIngameCoin;

	[SerializeField]
	private Text textUserDamage;

	[SerializeField]
	private Text textStar2TurnClear;

	[SerializeField]
	private Text textStar3TurnClear;

	[SerializeField]
	private Text textInGameSpeed;

	[SerializeField]
	private RectTransform rtIngameCoin;

	[SerializeField]
	private Button btnDefaultPlayInfo;

	[SerializeField]
	private Button btnDetailPlayInfo;

	[SerializeField]
	private Transform trEffectAnchor;

	[SerializeField]
	private Transform trPlayInfo;

	[SerializeField]
	private GameObject goControlLock;

	[SerializeField]
	private GameObject goTurnInfo;

	[SerializeField]
	private GameObject goStar2failureLine;

	[SerializeField]
	private GameObject goStar3failureLine;

	[SerializeField]
	private GameObject goPauseButton;

	[SerializeField]
	private GameObject goSpeedButton;

	[SerializeField]
	private GameObject goAttributeIcon;

	[SerializeField]
	private GameObject goGameInfo;

	[SerializeField]
	private GameObject goForceClear;

	[SerializeField]
	private GameObject goForceWave;

	[SerializeField]
	private GameObject goCoinIcon;

	[SerializeField]
	private GameObject goArenaPointIcon;

	[SerializeField]
	private GameObject goTextShuffling;

	[SerializeField]
	private ComboPhrase comboPhrase;

	[SerializeField]
	private BattleResult battleResult;

	[SerializeField]
	private BattleDefeat battleDefeat;

	[SerializeField]
	private BattleLose battleLose;

	[SerializeField]
	private BattleReward battleReward;

	[SerializeField]
	private ArenaChestOpen arenaChestOpen;

	[SerializeField]
	private JewelShop jewelShop;

	[SerializeField]
	private JewelShopBuy jewelShopBuy;

	[SerializeField]
	private NotEnoughJewelIngame notEnoughJewelIngame;

	[SerializeField]
	private Pause Pause;

	[SerializeField]
	private GameObject explain_Attribute;

	[SerializeField]
	private GameObject goEffectWarning;

	[SerializeField]
	private GameObject[] arrGoBoostItemIcon = new GameObject[0];

	private bool isMatchTimer;

	private bool isTimerPlay = true;

	private bool isMatchActive;

	private float userCurrentHp;

	private float userMaxHp;

	private float matchTimerStamp;

	private float currentMatchTime;

	private float maxMatchTime;

	private float currentAddMatchTime;

	private int currentTurn = 1;

	private int currentCoin;

	private int currentArenaPoint;

	private Vector3 coinUiShowPosition = new Vector3(-150f, -110f, 0f);

	private Vector3 coinUiHidePosition = new Vector3(0f, -110f, 0f);

	private Vector3 pingPongScale = new Vector3(1.4f, 1.4f, 1.4f);

	private Dictionary<int, ComboMultiply> dicComboMultiply = new Dictionary<int, ComboMultiply>();

	private Dictionary<int, HeroAttackUI> dicHunterAttackUI = new Dictionary<int, HeroAttackUI>();

	private Coroutine coroutineMatchTimer;

	private Coroutine coroutineClearTurnInfo;

	private Coroutine coroutineCoinBox;

	private Coroutine coroutineDamage;

	private Coroutine coroutineHp;

	public bool MatchTimerState => isMatchTimer;

	public bool MatchTimeEnd => currentMatchTime <= 0f;

	public bool MatchActive => isMatchActive;

	public BattleReward BattleReward => battleReward;

	public Transform PlayInfo => trPlayInfo;

	public void Init()
	{
		btnDefaultPlayInfo.enabled = (GameInfo.inGamePlayData.inGameType == InGameType.Stage);
		btnDetailPlayInfo.enabled = (GameInfo.inGamePlayData.inGameType == InGameType.Stage);
		textTurnsState.gameObject.SetActive(GameInfo.inGamePlayData.inGameType == InGameType.Stage);
		goCoinIcon.SetActive(GameInfo.inGamePlayData.inGameType == InGameType.Stage);
		goArenaPointIcon.SetActive(GameInfo.inGamePlayData.inGameType == InGameType.Arena);
	}

	public void SetClearTurnInfo()
	{
		textStar2TurnClear.text = string.Format("{0} {1}", GameInfo.inGamePlayData.star2ClearTurn, MWLocalize.GetData("common_text_turns"));
		textStar3TurnClear.text = string.Format("{0} {1}", GameInfo.inGamePlayData.star3ClearTurn, MWLocalize.GetData("common_text_turns"));
		goStar2failureLine.SetActive(value: false);
		goStar3failureLine.SetActive(value: false);
	}

	public void ResumeTimer()
	{
		matchTimerStamp = Time.time;
		isTimerPlay = true;
	}

	public void StopTimer()
	{
		isTimerPlay = false;
	}

	public void ReadyMatchTimer()
	{
		UnityEngine.Debug.Log("ReadyMatchTimer");
		imageMatchTimerGauge.fillAmount = 0f;
		currentMatchTime = GameInfo.inGamePlayData.matchTime;
		maxMatchTime = GameInfo.inGamePlayData.matchTime;
		if (GameInfo.inGamePlayData.dicActiveBoostItem.ContainsKey(4))
		{
			currentMatchTime += 1f;
			maxMatchTime += 1f;
		}
		goControlLock.SetActive(value: false);
	}

	public void StartMatchTimer()
	{
		if (!isMatchTimer)
		{
			currentAddMatchTime = GameInfo.inGamePlayData.matchTimeBonus;
			dicComboMultiply.Clear();
			dicHunterAttackUI.Clear();
			StopMatchTimer();
			isMatchTimer = true;
			coroutineMatchTimer = StartCoroutine(CheckMatchTimer());
			goControlLock.SetActive(value: false);
		}
	}

	public void AddMatchTime()
	{
		UnityEngine.Debug.Log("AddMatchTime");
		if (!(currentMatchTime <= 0f) && !goControlLock.activeSelf)
		{
			currentMatchTime += currentAddMatchTime;
			currentMatchTime = Mathf.Min(currentMatchTime, maxMatchTime);
			currentAddMatchTime *= (float)GameInfo.inGamePlayData.matchTimeRatio * 0.01f;
		}
	}

	public void CancelMatchTimer()
	{
		StopMatchTimer();
		InGamePlayManager.MatchTimeEnd();
		isMatchActive = true;
	}

	public void StopMatchTimer()
	{
		if (coroutineMatchTimer != null)
		{
			StopCoroutine(coroutineMatchTimer);
			coroutineMatchTimer = null;
		}
		imageMatchTimerGauge.fillAmount = 1f;
		isMatchTimer = false;
		goControlLock.SetActive(value: true);
	}

	public void ControlLock()
	{
		goControlLock.SetActive(value: true);
	}

	public void ControlResume()
	{
		goControlLock.SetActive(value: false);
	}

	public void RefreshWave(int wave)
	{
		textWaveState.text = $"wave {wave}/{GameInfo.inGamePlayData.dicWaveDbData.Count}";
	}

	public void RefreshTurn(int turn)
	{
		isMatchActive = false;
		currentTurn = turn;
		textTurnsState.text = $"{turn}";
		if (currentTurn > GameInfo.inGamePlayData.star2ClearTurn)
		{
			goStar2failureLine.SetActive(value: true);
		}
		if (currentTurn > GameInfo.inGamePlayData.star3ClearTurn)
		{
			goStar3failureLine.SetActive(value: true);
		}
	}

	public void SetUserHp(int maxHp)
	{
		userCurrentHp = maxHp;
		userMaxHp = maxHp;
		RefreshUserHp();
	}

	public void AddUserCombo(int _combo)
	{
		comboPhrase.Show(_combo);
	}

	public void BlockComboComplete()
	{
		comboPhrase.Complete();
	}

	public void Damage(int value)
	{
		float prevHp = userCurrentHp;
		userCurrentHp -= value;
		if (userCurrentHp < 0f)
		{
			userCurrentHp = 0f;
		}
		else
		{
			StartDamageTween(value);
		}
		UnityEngine.Debug.Log("************ userCurrentHp = " + userCurrentHp);
		if (userCurrentHp <= 0f && InGamePlayManager.GetHunterLeaderSkill() != null)
		{
			InGamePlayManager.GetHunterLeaderSkill().CheckLeaderSkillHp1(prevHp);
		}
		InGamePlayManager.ShakeCamera();
		RefreshUserHp();
	}

	public void Heal(int value)
	{
		userCurrentHp += value;
		if (userCurrentHp > userMaxHp)
		{
			userCurrentHp = userMaxHp;
		}
		RefreshUserHp();
	}

	public void HealMax()
	{
		userCurrentHp = userMaxHp;
		RefreshUserHp();
	}

	public void ShowAddCoin(int addCoin)
	{
		if (coroutineCoinBox != null)
		{
			StopCoroutine(coroutineCoinBox);
			coroutineCoinBox = null;
		}
		currentCoin += addCoin;
		textIngameCoin.text = $"{currentCoin}";
		if (addCoin > 0)
		{
			GameInfo.userPlayData.AddCoin(addCoin);
			coroutineCoinBox = StartCoroutine(ProcessShowAddCoin(addCoin));
		}
	}

	public void ShowAddArenaPoint(int _addPoint)
	{
		if (coroutineCoinBox != null)
		{
			StopCoroutine(coroutineCoinBox);
			coroutineCoinBox = null;
		}
		currentArenaPoint += _addPoint;
		textIngameCoin.text = $"{currentArenaPoint}";
		if (_addPoint > 0)
		{
			GameInfo.userPlayData.AddArenaPoint(_addPoint);
			coroutineCoinBox = StartCoroutine(ProcessShowAddCoin(_addPoint));
		}
	}

	public void AddHunterCombo(float combo, int hunterColor, Vector3 position, int hunterIdx)
	{
		if (!(combo < 2f))
		{
			if (!dicComboMultiply.ContainsKey(hunterIdx))
			{
				ComboMultiply component = MWPoolManager.Spawn("Effect", "Text_HunterAttackMultiply", trEffectAnchor).GetComponent<ComboMultiply>();
				dicComboMultiply.Add(hunterIdx, component);
			}
			dicComboMultiply[hunterIdx].ShowCombo(hunterColor, combo, position + new Vector3(0f, 0.8f, 0f));
		}
	}

	public void AddHunterAttack(int attack, int hunterColor, Vector3 position, int hunterIdx)
	{
		if (!dicHunterAttackUI.ContainsKey(hunterIdx))
		{
			HeroAttackUI component = MWPoolManager.Spawn("Effect", "Text_HunterAttackDamage", trEffectAnchor).GetComponent<HeroAttackUI>();
			dicHunterAttackUI.Add(hunterIdx, component);
		}
		dicHunterAttackUI[hunterIdx].ShowAttack(attack, hunterColor, position);
	}

	public void ShowMonsterDamageUI(MonsterDamageUI.MonsterDamageType type, int damage, Vector3 position)
	{
		MonsterDamageUI component = MWPoolManager.Spawn("Effect", "Text_MonsterDamage", trEffectAnchor).GetComponent<MonsterDamageUI>();
		component.ShowDamageUI(type, damage, position);
	}

	public void ClearHunterAttackUI(int hunterIdx)
	{
		if (dicHunterAttackUI.ContainsKey(hunterIdx))
		{
			dicHunterAttackUI[hunterIdx].Clear();
		}
	}

	public void ShowClearResult()
	{
		battleResult.Show(currentTurn);
	}

	public void ShowArenaClearResult()
	{
		arenaChestOpen.Show();
	}

	public void ShowDefeatResult()
	{
		battleDefeat.Show();
	}

	public void ShowLoseResult()
	{
		battleLose.Show();
	}

	public void ShowArenaLoseResult(ARENA_GAME_END_RESULT _data)
	{
		battleLose.ShowArena(_data);
	}

	public void ShowClearReward()
	{
		if (GameInfo.inGamePlayData.inGameType == InGameType.Stage)
		{
			battleReward.Init();
			if (ShowBattleReward != null)
			{
				ShowBattleReward();
			}
		}
	}

	public void ShowNotEnoughJewel()
	{
		notEnoughJewelIngame.Init();
	}

	public void ShowJewelShop()
	{
		//jewelShop.Init();
	}

	public void ShowJewelShopBuy(int key)
	{
		jewelShopBuy.Init(key);
	}

	public void CloseJewelShop()
	{
		jewelShop.ClosePopup();
	}

	public void CloseJewelShopBuy()
	{
		jewelShopBuy.ClosePopup();
	}

	public void ShowPause()
	{
		Pause.Init();
	}

	public void GameContinue()
	{
		HealMax();
		ControlResume();
		isMatchTimer = false;
		isTimerPlay = true;
		InGamePlayManager.StartUserTurn();
		battleDefeat.Hide();
	}

	public void ShowBuffUI(Vector3 _position, BlockType _type, int _buff)
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
				Transform transform = MWPoolManager.Spawn("Effect", text, trEffectAnchor);
				transform.position = new Vector3(_position.x - 0.19f, _position.y + 0.19f, _position.z);
				transform.GetComponent<Text>().text = $"{_buff}x";
			}
		}
	}

	public void ShowShuffleUI()
	{
		goTextShuffling.SetActive(value: true);
	}

	public void HideShuffleUI()
	{
		goTextShuffling.SetActive(value: false);
	}

	public bool CheckUserDie()
	{
		return userCurrentHp <= 0f;
	}

	public void ShowWarningEffect()
	{
		goEffectWarning.SetActive(value: true);
		SoundController.EffectSound_Play(EffectSoundType.InGameWarning);
	}

	public void HideWarningEffect()
	{
		goEffectWarning.SetActive(value: false);
		SoundController.EffectSound_Stop(EffectSoundType.InGameWarning);
	}

	private void ShowBoostItemIcon()
	{
		foreach (KeyValuePair<int, BoostItemDbData> item in GameInfo.inGamePlayData.dicActiveBoostItem)
		{
			arrGoBoostItemIcon[item.Key - 1].SetActive(value: true);
		}
	}

	private void RefreshUserHp()
	{
		textUserHp.text = $"{userCurrentHp}";
		MoveHpGauge(Mathf.Clamp(userCurrentHp / userMaxHp, 0f, 1f));
	}

	private void MoveHpGauge(float target)
	{
		StopHpGauge();
		coroutineHp = StartCoroutine(ProcessHpGauge(target));
	}

	private void StopHpGauge()
	{
		if (coroutineHp != null)
		{
			StopCoroutine(coroutineHp);
			coroutineHp = null;
		}
	}

	private IEnumerator ProcessHpGauge(float target)
	{
		float gap = Mathf.Abs(target - imageUserHpGauge.fillAmount) / 6f;
		if (target > imageUserHpGauge.fillAmount)
		{
			while (target - imageUserHpGauge.fillAmount > 0f)
			{
				imageUserHpGauge.fillAmount += gap;
				yield return null;
			}
		}
		else
		{
			while (imageUserHpGauge.fillAmount - target > 0f)
			{
				UnityEngine.Debug.Log("----------");
				imageUserHpGauge.fillAmount -= gap;
				yield return null;
			}
		}
		imageUserHpGauge.fillAmount = target;
		coroutineHp = null;
	}

	private void StartDamageTween(int damage)
	{
		if (coroutineDamage != null)
		{
			StopCoroutine(coroutineDamage);
			coroutineDamage = null;
		}
		coroutineDamage = StartCoroutine(ProcessDamageTween(damage));
	}

	private IEnumerator ProcessDamageTween(int damage)
	{
		textUserDamage.gameObject.SetActive(value: true);
		textUserDamage.text = $"-{damage}";
		LeanTween.scale(textUserDamage.gameObject, pingPongScale, 0.2f).setLoopPingPong(1).setEase(LeanTweenType.linear);
		yield return new WaitForSeconds(0.9f);
		textUserDamage.gameObject.SetActive(value: false);
		coroutineDamage = null;
	}

	private IEnumerator CheckMatchTimer()
	{
		currentMatchTime = maxMatchTime;
		matchTimerStamp = Time.time;
		imageMatchTimerGauge.fillAmount = 0f;
		while (currentMatchTime > 0f)
		{
			yield return null;
			if (isTimerPlay)
			{
				currentMatchTime -= Time.time - matchTimerStamp;
				matchTimerStamp = Time.time;
				if (MatchTimeFlow != null)
				{
					MatchTimeFlow(Mathf.Ceil(currentMatchTime));
				}
				imageMatchTimerGauge.fillAmount = (maxMatchTime - currentMatchTime) / maxMatchTime;
			}
		}
		imageMatchTimerGauge.fillAmount = 1f;
		goControlLock.SetActive(value: true);
		InGamePlayManager.MatchTimeEnd();
		isMatchTimer = false;
		isMatchActive = true;
		SoundController.EffectSound_Play(EffectSoundType.TimeOver);
	}

	private IEnumerator ProcessShowAddCoin(int addCoin)
	{
		rtIngameCoin.anchoredPosition = coinUiHidePosition;
		LeanTween.move(rtIngameCoin, coinUiShowPosition, 0.3f);
		yield return new WaitForSeconds(1.3f);
		LeanTween.move(rtIngameCoin, coinUiHidePosition, 0.3f);
		coroutineCoinBox = null;
	}

	private IEnumerator CheckShowTurnInfo()
	{
		yield return new WaitForSeconds(3f);
		goTurnInfo.SetActive(value: false);
		if (coroutineClearTurnInfo != null)
		{
			StopCoroutine(coroutineClearTurnInfo);
			coroutineClearTurnInfo = null;
		}
	}

	public void OnClickShowTurnInfo()
	{
		goTurnInfo.SetActive(value: true);
		if (coroutineClearTurnInfo != null)
		{
			StopCoroutine(coroutineClearTurnInfo);
			coroutineClearTurnInfo = null;
		}
		coroutineClearTurnInfo = StartCoroutine(CheckShowTurnInfo());
	}

	public void OnClickHideTurnInfo()
	{
		goTurnInfo.SetActive(value: false);
		if (coroutineClearTurnInfo != null)
		{
			StopCoroutine(coroutineClearTurnInfo);
			coroutineClearTurnInfo = null;
		}
	}

	public void OnClickAttributeIcon()
	{
		InGamePlayManager.TouchLock();
		explain_Attribute.gameObject.SetActive(value: true);
	}

	public void OnClickAttributePopup()
	{
		InGamePlayManager.TouchActive();
		explain_Attribute.gameObject.SetActive(value: false);
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
		textInGameSpeed.text = $"x{GameInfo.inGameBattleSpeedRate}";
		InGamePlayManager.ChangeBattleSpeed();
	}

	private void Awake()
	{
		goForceClear.SetActive(value: false);
		goForceWave.SetActive(value: false);
		rtIngameCoin.anchoredPosition = coinUiHidePosition;
		goControlLock.SetActive(value: true);
		goTurnInfo.SetActive(value: false);
		comboPhrase.Init();
		SetClearTurnInfo();
		RefreshWave(1);
		RefreshTurn(1);
		textInGameSpeed.text = $"x{GameInfo.inGameBattleSpeedRate}";
		if (GameInfo.inGamePlayData.inGameType == InGameType.Stage)
		{
			if (GameInfo.inGamePlayData.levelIdx == 0 || GameInfo.inGamePlayData.levelIdx == 1 || GameInfo.inGamePlayData.levelIdx == 2)
			{
				goPauseButton.SetActive(value: false);
			}
			if (GameInfo.inGamePlayData.levelIdx == 0)
			{
				goSpeedButton.SetActive(value: false);
				goAttributeIcon.SetActive(value: false);
				goGameInfo.SetActive(value: false);
			}
		}
		ShowBoostItemIcon();
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}
}
