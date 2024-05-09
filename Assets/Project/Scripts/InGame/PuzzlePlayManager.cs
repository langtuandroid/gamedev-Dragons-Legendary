using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PuzzlePlayManager : GameObjectSingleton<PuzzlePlayManager>
{
	public static Action<Vector3, RaycastHit2D> OnTouchBegin;

	public static Action<Vector3> OnTouchMove;

	public static Action<Vector3> OnTouchEnd;

	public static Action<int, int> OnClock;

	public static Action<float> OnMatchTimeFlow;

	public static Action OnPuzzleSwitch;

	public static Action<Block, Block, bool> PuzzleTouchEnd;

	public static Action OnBattleRewardOpen;

	public static Action OnBattleRewardTutorial;

	public static Action OnBattleRewardComplete;

	public static Action OnHunterSkill;

	public static Action OnHunterSkillEventComplete;

	public static Action OnShowBattleClearResult;

	public static Action ShowBattleReward;

	public static Action OnGameLose;

	public static Action OnContinueTimer;

	public static Action OnUseHunterSkill;

	[FormerlySerializedAs("puzzleController")] [SerializeField]
	private PuzzleController _puzzleController;

	[FormerlySerializedAs("battleController")] [SerializeField]
	private BattleController _battleController;

	[FormerlySerializedAs("uiController")] [SerializeField]
	private PuzzleUI _uiController;

	private int _checkHealCount;

	private int _userCombo;

	private bool _isTouchState = true;

	private bool _isGameOver;

	private bool _isMatching = true;

	private Ray _rayTouch;

	private RaycastHit2D _hitTouch;

	private Coroutine _coroutineShakeCamera;

	public static Vector2 DamagePosition => Inst._puzzleController.DamagePosition;

	public static Transform BattleRewardPickItem => Inst._uiController.BattleReward.PickItem;

	public static Transform PlayInfoUI => Inst._uiController.PlayInfo;

	public static bool MatchTimerState => Inst._uiController.MatchTimerState;

	public static bool MatchTimeEndState => Inst._uiController.MatchTimeEnd;

	public static bool MatchActive => Inst._uiController.Match;

	public static void ActivateTouch()
	{
		Inst._isTouchState = true;
	}

	public static void LockTouch()
	{
		Inst._isTouchState = false;
	}

	public static void ClaimReward()
	{
		Inst._uiController.BattleReward.Click_Claim();
	}

	public static void ContinueTimer()
	{
		Inst._uiController.ContinueTimer();
	}

	public static void PauseTimer()
	{
		Inst._uiController.StopTimer();
	}

	public static void ThisWave(int wave)
	{
		GameInfo.userPlayData.wave = wave;
		Inst._uiController.UpdateWave(wave);
		if (GameInfo.inGamePlayData.dicActiveBoostItem.ContainsKey(2))
		{
			ChangeSpecial();
		}
	}

	public static void ThisTurn(int turn)
	{
		Debug.Log("CurrentTurn");
		InfoManager.IsGamerTurn();
		GameInfo.userPlayData.turn = turn;
		Inst._uiController.UpdateTurn(turn);
	}

	public static Transform CheckIsUseHunterSkill()
	{
		return Inst._battleController.IsHunterSkillFull();
	}

	public static void BlockAttacl()
	{
		Inst._checkHealCount = 0;
		Inst._battleController.Battle_AddAttack_Start();
	}

	public static void BlockAttackEffect()
	{
		Inst._userCombo++;
		Inst._uiController.ComboAdd(Inst._userCombo);
	}

	public static void AddBlock(BlockType type, int count)
	{
		if (type == BlockType.White)
		{
			Inst._checkHealCount += count;
		}
		Inst._battleController.Battle_AddAttack_Block(type, count);
	}

	public static Vector3[] HunterPos(BlockType _type)
	{
		return Inst._battleController.Hunter_Position(_type);
	}

	public static int GetTotalHp()
	{
		return Inst._battleController.HunterHP();
	}

	public static HeroLeaderSkill LeaderSkill()
	{
		if (Inst._battleController.HunterLeaderSkillNullCheck())
		{
			return Inst._battleController.GetHunterLeaderSkill;
		}
		return null;
	}

	public static void CompleteBlock()
	{
		if (Inst._checkHealCount > 0)
		{
			float num = 1f + (Inst._userCombo - 1f) * 0.01f;
			Inst.HealUser(Inst._battleController.HunterRecovery() * Inst._checkHealCount * num);
		}
		Inst._userCombo = 0;
		Inst._battleController.Battle_AddAttack_Complete();
		Inst._uiController.BlockCompleted();
	}

	public static void StartTurn()
	{
		Debug.Log("JY -------------- StartUserTurn()");
		if (Inst._uiController.CheckUserDie())
		{
			GameOver();
			return;
		}
		if (Inst.CheckUserTurn())
		{
			ScenarioManager.EndScenarioEvent = Inst.OnCompleteScenario;
			ScenarioManager.ShowInGame(GameInfo.inGamePlayData.levelIdx);
			return;
		}
		Inst._uiController.IsGameTimerReady();
		Inst._puzzleController.ControlStart();
		Inst._battleController.User_Turn_Check(Inst._isMatching);
		Inst._isMatching = false;
		GameDataManager.ShowInGameDescription();
	}

	public static void StartEnemyTurn()
	{
		Inst._battleController.Check_Monster_Attack();
	}

	public static void EndMatch()
	{
		Inst._puzzleController.ControlEndAndMatch();
	}

	public static void StartTimer()
	{
		Inst._uiController.StartTimer();
		Inst._battleController.Battle_Start_Match();
		Inst._isMatching = true;
	}

	public static void CancelTimer()
	{
		Inst._uiController.DeactivateTimer();
	}

	public static void AddTime()
	{
		Inst._uiController.AddMatchTimer();
	}

	public static void ExeptionBlock(BlockExceptionType _type)
	{
		Inst._puzzleController.SetExceptionBlock(_type);
	}

	public static void ControllerStart()
	{
		Inst._uiController.ResumeControl();
		Inst._puzzleController.ControlStart();
	}

	public static void LockController()
	{
		Inst._puzzleController.ControlLock();
		Inst._uiController.LockControl();
	}

	public static void ChangeSpeed()
	{
		Inst._battleController.SetSpeedBattle();
	}

	public static void SetMaxHP(int hp)
	{
		Inst._uiController.SetHP(hp);
	}

	public static void StartHunterAttack()
	{
		Inst._battleController.Battle_Attack_Start();
	}

	public static void AttackOver()
	{
		Inst._puzzleController.ControlLock();
		Inst._uiController.LockControl();
	}

	public static void LastCoinEffect()
	{
		Inst._battleController.LastMonsterCoinEffect();
	}
	

	public static void SetHunterFullSkillGauge(int _index)
	{
		Inst._battleController.SetFullSkillGague(_index);
	}

	public static void ForceMonsterHP()
	{
		Inst._battleController.ForceIntroMonsterHp();
	}

	public static void StartHunterAttack(int hunterIdx)
	{
		Inst._uiController.ClearHunterUI(hunterIdx);
	}

	public static void AddCombo(float combo, int hunterColor, Vector3 position, int hunterIdx = 0)
	{
		Inst._uiController.ComboAdd(combo, hunterColor, position, hunterIdx);
	}

	public static void AddAttack(int attack, int hunterColor, Vector3 position, int hunterIdx = 0)
	{
		Inst._uiController.AttackAdd(attack, hunterColor, position, hunterIdx);
	}

	public static void HunterComboComplete()
	{
	}

	public static void ShowDamageUI(EnemyDamageUI.EnemyDamageType type, int damage, Vector3 position)
	{
		Inst._uiController.ShowMonsterDamage(type, damage, position);
	}

	public static void Block(BlockType _type)
	{
		Inst._puzzleController.ActiveBlockType(_type);
	}

	public static void DeBlock(BlockType _type)
	{
		Inst._puzzleController.DeActiveBlockType(_type);
	}

	public static void MoreCoins(int addCoin)
	{
		Debug.Log("AddCoin");
		Inst._uiController.AddCoins(addCoin);
	}

	public static void ArenaPoins(int _addPoint)
	{
		Inst._uiController.ArenaPoints(_addPoint);
	}

	public static void AddItem(int itemIdx, int count)
	{
		GameInfo.userPlayData.AddItem(itemIdx, count);
	}

	public static void MoreMonster(int monsterIdx)
	{
		GameInfo.userPlayData.AddMonster(monsterIdx);
	}

	public static void WarningEffect()
	{
		Inst._uiController.OpenWarningEffect();
	}

	public static void HideEffect()
	{
		Inst._uiController.CloseWarningEffect();
	}

	public static void HealHeroes(int value)
	{
		Inst.HealUser(value);
	}

	public static void Damage(int value)
	{
		Inst._uiController.Damage(value);
	}

	public static void GameContinue()
	{
		Inst._isGameOver = false;
		Inst._battleController.Show_Heal_Eff();
		Inst._puzzleController.GameContinue();
		Inst._uiController.ContinueGame();
		ActivateTouch();
		switch (GameInfo.inGamePlayData.inGameType)
		{
		case PuzzleInGameType.Stage:
			if (GameInfo.inGamePlayData.isDragon == 0)
			{
				SoundController.BGM_Play(MusicSoundType.IngameBGM);
			}
			else
			{
				SoundController.BGM_Play(MusicSoundType.InGameDragonBgm);
			}
			break;
		case PuzzleInGameType.Arena:
			SoundController.BGM_Play(MusicSoundType.ArenaBGM);
			break;
		}
	}

	public static void GameOver()
	{
		if (!Inst._isGameOver)
		{
			Inst._isGameOver = true;
			Inst._puzzleController.Lock();
			Inst._puzzleController.GameOver();
			Inst._uiController.DefaultResult();
			LockTouch();
		}
	}

	public static void GameQuit()
	{
		Inst._isGameOver = true;
		Inst._puzzleController.Lock();
		Inst._puzzleController.GameOver();
		LockTouch();
	}

	public static void LoseResult()
	{
		Inst._uiController.BattleLoseShow();
	}

	public static void ArenaLoseResult(ARENA_GAME_END_RESULT _data)
	{
		GameInfo.isShowArenaSalePack = true;
		Inst._uiController.ArenaLoseResult(_data);
	}

	public static void ClearGame()
	{
		if (Inst._isGameOver)
		{
			return;
		}
		Inst._isGameOver = true;
		Inst._puzzleController.Lock();
		Inst._puzzleController.GameOver();
		LockTouch();
		if (Inst.CheckGameEvent())
		{
			return;
		}
		if (InfoManager.Intro)
		{
			InfoManager.OpenTutorial();
		}
		else if (GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Stage)
		{
			Inst._uiController.ClearResult();
			if (OnShowBattleClearResult != null)
			{
				OnShowBattleClearResult();
			}
		}
		else
		{
			Inst._uiController.ArenaClear();
		}
	}

	public static void LoseGameEvent()
	{
		if (OnGameLose != null)
		{
			OnGameLose();
		}
	}

	public static void WinReward()
	{
		Inst._uiController.ClearResultShow();
	}

	public static void NotEnoughJewel(Action _onCallback)
	{
		Inst._uiController.NotEnoughJewel();
		OnContinueTimer = _onCallback;
	}

	public static void ShowJewelShop()
	{
		Inst._uiController.ShowJewelShop();
	}

	public static void BuyJewelShop(int key)
	{
		Inst._uiController.ShowJewelShopBuy(key);
	}

	public static void CloseJewelShop()
	{
		Inst._uiController.CloseJewelShop();
	}

	public static void JewelShopBuyClose()
	{
		Inst._uiController.CloseJewelShopBuy();
	}

	public static void MonsterTargeting(Enemy _monster)
	{
		Inst._battleController.MonsterTargeting(_monster);
	}

	public static void MonsterUnTargeting()
	{
		Inst._battleController.MonsterUnTargeting();
	}

	public void OnClickForceClear()
	{
	}

	public void OnClickForcePassWave()
	{
		MasterPoolManager.ReturnToPoolAll("Monster");
		MasterPoolManager.ReturnToPoolAll("Effect");
		_battleController.ForceWaveClear();
	}

	public static void ShakeEffect(bool isVibration = true)
	{
		if (Inst._coroutineShakeCamera != null)
		{
			Inst.StopCoroutine(Inst._coroutineShakeCamera);
			Inst._coroutineShakeCamera = null;
		}
		Inst.StartCoroutine(Inst.CameraShake());
		if (isVibration && GamePreferenceManager.GetIsVibration())
		{
			Handheld.Vibrate();
		}
	}

	public static void ActiveOnlySelectOneBlock(int _x, int _y)
	{
		Inst._puzzleController.ActiveOnlySelectOneBlock(_x, _y);
	}

	public static void AddActiveSelectBlock(int _x, int _y)
	{
		Inst._puzzleController.AddActiveSelectBlock(_x, _y);
	}

	public static void ActiveOnlyDeselectOneBlock(int _x, int _y)
	{
		Inst._puzzleController.ActiveOnlyDeselectOneBlock(_x, _y);
	}

	public static void SelectDeBlocks()
	{
		Inst._puzzleController.StopDeSelectAllBlock();
	}

	public static void ActiveBlocks()
	{
		Inst._puzzleController.AllActiveBlock();
	}

	public static void LockedBlocks()
	{
		Inst._puzzleController.AllLockBlock();
	}

	public static void AllDeSelectBlock()
	{
		Inst._puzzleController.AllDeSelectBlock();
	}

	public static Vector3 BlockPositions(int _x, int _y)
	{
		return Inst._puzzleController.GetBlockPosition(_x, _y);
	}

	public static Transform GetBlocks(int _x, int _y)
	{
		return Inst._puzzleController.GetBlock(_x, _y);
	}

	public static Transform GetMonsterTurn()
	{
		return Inst._battleController.GetMonsterTurn();
	}

	public static void BattleRewardOpenEvent()
	{
		Debug.Log("Ingameplay - BattleRewardOpenEvent");
		if (OnBattleRewardOpen != null)
		{
			OnBattleRewardOpen();
		}
	}

	public static void BattleRewardTutorialEvent()
	{
		if (OnBattleRewardTutorial != null)
		{
			OnBattleRewardTutorial();
		}
	}

	public static void BattleRewardCompleteEvent()
	{
		if (OnBattleRewardComplete != null)
		{
			OnBattleRewardComplete();
		}
	}

	public static void TutorialHunterSkill(Hero hunter)
	{
		Inst._battleController.UseHunterSkillForTutorial(hunter);
	}

	public static void UIBuff(Vector3 _position, BlockType _type, int _buff)
	{
		Inst._uiController.BuffUI(_position, _type, _buff);
	}

	public static void ChangeBlockType(BlockType _from, BlockType _to, int _skillIdx)
	{
		Inst._puzzleController.ChangeBlockType(_from, _to, _skillIdx);
	}

	public static void ShowShuffleUI()
	{
		Inst._uiController.OpenShuffleUI();
	}

	public static void HideShuffleUI()
	{
		Inst._uiController.CloseShuffleUI();
	}

	public static void ChangeSpecial()
	{
		Inst._puzzleController.ChangeSpecialBlock();
	}

	private void Construct()
	{
		SoundController.BGM_Stop(MusicSoundType.LobbyBGM);
		switch (GameInfo.inGamePlayData.inGameType)
		{
		case PuzzleInGameType.Stage:
			if (GameInfo.inGamePlayData.isDragon == 0)
			{
				SoundController.BGM_Play(MusicSoundType.IngameBGM);
			}
			else
			{
				SoundController.BGM_Play(MusicSoundType.InGameDragonBgm);
			}
			break;
		case PuzzleInGameType.Arena:
			SoundController.BGM_Play(MusicSoundType.ArenaBGM);
			break;
		}
		if (GameInfo.isDirectBattleReward)
		{
			Inst._uiController.ClearResultShow();
			InfoManager.IsGamerTurn();
		}
		else
		{
			_isGameOver = false;
			Inst._battleController.Set_Battle_Data(GameInfo.inGamePlayData);
			Inst._puzzleController.Init();
			Inst._puzzleController.Lock();
			Inst._puzzleController.PuzzleTouchEnd = OnPuzzleTouchEnd;
			Inst._puzzleController.PuzzleSwitch = OnPuzzleSiwtchEvent;
			Inst._puzzleController.BlockSelect = OnSelectBlock;
			Inst._uiController.Construct();
			Inst._uiController.OnTimeFlow = TimeFlowEvent;
			Inst._uiController.OnShowBattleReward = OnShowBattleReward;
		}
		GamePreferenceManager.SetIsBoostRewardVideo(isBoostRewardVideo: false);
	}

	private void HealUser(float value)
	{
		_uiController.HealHeroes((int)Mathf.Floor(value));
		_battleController.Show_Heal_Eff();
	}

	private IEnumerator CameraShake()
	{
		float time;
		for (float currentSecond = 0f; currentSecond < 0.3f; currentSecond += Time.realtimeSinceStartup - time)
		{
			time = Time.realtimeSinceStartup;
			Vector3 shakePosition = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), -10f);
			Camera.main.transform.position = shakePosition;
			yield return null;
		}
		Camera.main.transform.position = new Vector3(0f, 0f, -10f);
		_coroutineShakeCamera = null;
	}

	private void OnSelectBlock(int _x, int _y)
	{
		if (OnClock != null)
		{
			OnClock(_x, _y);
		}
	}

	private void TimeFlowEvent(float time)
	{
		if (OnMatchTimeFlow != null)
		{
			OnMatchTimeFlow(time);
		}
	}

	private void OnPuzzleTouchEnd(Block first, Block second, bool isMatchBlock)
	{
		if (PuzzleTouchEnd != null)
		{
			PuzzleTouchEnd(first, second, isMatchBlock);
		}
	}

	private void OnPuzzleSiwtchEvent()
	{
		if (OnPuzzleSwitch != null)
		{
			OnPuzzleSwitch();
		}
	}

	private void OnShowBattleReward()
	{
		if (ShowBattleReward != null)
		{
			ShowBattleReward();
		}
	}

	private void RefreshInGameCamera()
	{
		float num = Screen.width / (float)Screen.height;
		Camera.main.orthographicSize = 6.4f / Screen.width * Screen.height;
	}

	private bool CheckGameEvent()
	{
		if (GameInfo.inGamePlayData.inGameType != 0)
		{
			return false;
		}
		if (GameInfo.inGamePlayData.isShowScenario && !InfoManager.Intro && GameInfo.inGamePlayData.isDragon == 1)
		{
			ScenarioManager.EndScenarioEvent = OnScenarioEnd;
			ScenarioManager.ShowInGame(GameInfo.inGamePlayData.levelIdx);
			return true;
		}
		return false;
	}

	private void OnCompleteScenario(int _scenarioIdx)
	{
		ScenarioManager.EndScenarioEvent = null;
		GameInfo.inGamePlayData.isShowScenario = false;
		StartTurn();
	}

	private void OnScenarioEnd(int _scenarioIdx)
	{
		ScenarioManager.EndScenarioEvent = null;
		GameInfo.inGamePlayData.isShowScenario = false;
		Inst._isGameOver = false;
		ClearGame();
	}

	private bool CheckUserTurn()
	{
		return GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Stage && GameInfo.inGamePlayData.isShowScenario && GameInfo.inGamePlayData.isDragon == 0;
	}

	protected override void Awake()
	{
		base.Awake();
	}

	private void Start()
	{
		Construct();
	}

	private void Update()
	{
		if (InfoManager.DialogState || !_isTouchState)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			_rayTouch = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (OnTouchBegin != null)
			{
				OnTouchBegin(Input.mousePosition, Physics2D.GetRayIntersection(_rayTouch, float.PositiveInfinity));
			}
		}
		if (Input.GetMouseButton(0) && OnTouchMove != null)
		{
			OnTouchMove(Input.mousePosition);
		}
		if (Input.GetMouseButtonUp(0) && OnTouchEnd != null)
		{
			OnTouchEnd(Input.mousePosition);
		}
	}

	protected override void OnDestroy()
	{
		GameInfo.userPlayData.Clear();
		base.OnDestroy();
	}
}
