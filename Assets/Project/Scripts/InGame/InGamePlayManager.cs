using System;
using System.Collections;
using UnityEngine;

public class InGamePlayManager : GameObjectSingleton<InGamePlayManager>
{
	public static Action<Vector3, RaycastHit2D> TouchBeginEvent;

	public static Action<Vector3> TouchMoveEvent;

	public static Action<Vector3> TouchEndEvent;

	public static Action<int, int> BlockSelect;

	public static Action<float> MatchTimeFlow;

	public static Action PuzzleSwitch;

	public static Action<Block, Block, bool> PuzzleTouchEnd;

	public static Action BattleRewardOpen;

	public static Action BattleRewardTutorial;

	public static Action BattleRewardComplete;

	public static Action HunterSkillEvent;

	public static Action HunterSkillEventComplete;

	public static Action ShowBattleClearResult;

	public static Action ShowBattleReward;

	public static Action GameLose;

	public static Action continueTimer;

	public static Action UseHunterSkill;

	private const float SHAKE_CAMERA_MOVE_VALUE = 0.1f;

	private const float SHAKE_CAMERA_DURATION = 0.3f;

	[SerializeField]
	private PuzzleController puzzleController;

	[SerializeField]
	private BattleController battleController;

	[SerializeField]
	private InGameUIController uiController;

	private int checkHealCount;

	private int userCombo;

	private bool isTouchState = true;

	private bool isGameOver;

	private bool isMatching = true;

	private Ray rayTouch;

	private RaycastHit2D hitTouch;

	private Coroutine coroutineShakeCamera;

	public static Vector2 DamagePosition => GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.DamagePosition;

	public static Transform BattleRewardPickItem => GameObjectSingleton<InGamePlayManager>.Inst.uiController.BattleReward.PickItem;

	public static Transform PlayInfoUI => GameObjectSingleton<InGamePlayManager>.Inst.uiController.PlayInfo;

	public static bool MatchTimerState => GameObjectSingleton<InGamePlayManager>.Inst.uiController.MatchTimerState;

	public static bool MatchTimeEndState => GameObjectSingleton<InGamePlayManager>.Inst.uiController.MatchTimeEnd;

	public static bool MatchActive => GameObjectSingleton<InGamePlayManager>.Inst.uiController.MatchActive;

	public static void TouchActive()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.isTouchState = true;
	}

	public static void TouchLock()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.isTouchState = false;
	}

	public static void BattleRewardClaim()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.BattleReward.Click_Claim();
	}

	public static void ResumeMatchTimer()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.ResumeTimer();
	}

	public static void StopMatchTimer()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.StopTimer();
	}

	public static void CurrentWave(int wave)
	{
		GameInfo.userPlayData.wave = wave;
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.RefreshWave(wave);
		if (GameInfo.inGamePlayData.dicActiveBoostItem.ContainsKey(2))
		{
			ChangeSpecialBlock();
		}
	}

	public static void CurrentTurn(int turn)
	{
		UnityEngine.Debug.Log("CurrentTurn");
		TutorialManager.CheckInGameUserTurn();
		GameInfo.userPlayData.turn = turn;
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.RefreshTurn(turn);
	}

	public static Transform CheckIsUseHunterSkill()
	{
		return GameObjectSingleton<InGamePlayManager>.Inst.battleController.IsHunterSkillFull();
	}

	public static void StartAttackBlock()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.checkHealCount = 0;
		GameObjectSingleton<InGamePlayManager>.Inst.battleController.Battle_AddAttack_Start();
	}

	public static void StartBlockAttackEffect()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.userCombo++;
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.AddUserCombo(GameObjectSingleton<InGamePlayManager>.Inst.userCombo);
	}

	public static void AddAttackBlock(BlockType type, int count)
	{
		if (type == BlockType.White)
		{
			GameObjectSingleton<InGamePlayManager>.Inst.checkHealCount += count;
		}
		GameObjectSingleton<InGamePlayManager>.Inst.battleController.Battle_AddAttack_Block(type, count);
	}

	public static Vector3[] GetHunterPosition(BlockType _type)
	{
		return GameObjectSingleton<InGamePlayManager>.Inst.battleController.Hunter_Position(_type);
	}

	public static int GetHunterTotalHP()
	{
		return GameObjectSingleton<InGamePlayManager>.Inst.battleController.HunterHP();
	}

	public static HunterLeaderSkill GetHunterLeaderSkill()
	{
		if (GameObjectSingleton<InGamePlayManager>.Inst.battleController.HunterLeaderSkillNullCheck())
		{
			return GameObjectSingleton<InGamePlayManager>.Inst.battleController.GetHunterLeaderSkill;
		}
		return null;
	}

	public static void AttackBlockComplete()
	{
		if (GameObjectSingleton<InGamePlayManager>.Inst.checkHealCount > 0)
		{
			float num = 1f + ((float)GameObjectSingleton<InGamePlayManager>.Inst.userCombo - 1f) * 0.01f;
			GameObjectSingleton<InGamePlayManager>.Inst.HealUserHp((float)(GameObjectSingleton<InGamePlayManager>.Inst.battleController.HunterRecovery() * GameObjectSingleton<InGamePlayManager>.Inst.checkHealCount) * num);
		}
		GameObjectSingleton<InGamePlayManager>.Inst.userCombo = 0;
		GameObjectSingleton<InGamePlayManager>.Inst.battleController.Battle_AddAttack_Complete();
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.BlockComboComplete();
	}

	public static void StartUserTurn()
	{
		UnityEngine.Debug.Log("JY -------------- StartUserTurn()");
		if (GameObjectSingleton<InGamePlayManager>.Inst.uiController.CheckUserDie())
		{
			GameOver();
			return;
		}
		if (GameObjectSingleton<InGamePlayManager>.Inst.CheckScenarioUserTurn())
		{
			ScenarioManager.EndScenarioEvent = GameObjectSingleton<InGamePlayManager>.Inst.OnScenarioUserTurnComplete;
			ScenarioManager.ShowInGame(GameInfo.inGamePlayData.levelIdx);
			return;
		}
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.ReadyMatchTimer();
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.ControlStart();
		GameObjectSingleton<InGamePlayManager>.Inst.battleController.User_Turn_Check(GameObjectSingleton<InGamePlayManager>.Inst.isMatching);
		GameObjectSingleton<InGamePlayManager>.Inst.isMatching = false;
		GameDataManager.ShowInGameDescription();
	}

	public static void EndUserTurn()
	{
	}

	public static void StartMonsterTurn()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.battleController.Check_Monster_Attack();
	}

	public static void EndMonsterTurn()
	{
	}

	public static void MatchTimeEnd()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.ControlEndAndMatch();
	}

	public static void StartMatchTimer()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.StartMatchTimer();
		GameObjectSingleton<InGamePlayManager>.Inst.battleController.Battle_Start_Match();
		GameObjectSingleton<InGamePlayManager>.Inst.isMatching = true;
	}

	public static void CancelMatchTimer()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.CancelMatchTimer();
	}

	public static void AddMatchTime()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.AddMatchTime();
	}

	public static void SetExceptionBlock(BlockExceptionType _type)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.SetExceptionBlock(_type);
	}

	public static void PuzzleControlStart()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.ControlResume();
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.ControlStart();
	}

	public static void PuzzleControlLock()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.ControlLock();
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.ControlLock();
	}

	public static void ChangeBattleSpeed()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.battleController.SetSpeedBattle();
	}

	public static void SetUserMaxHp(int hp)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.SetUserHp(hp);
	}

	public static void StartHunterAttack()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.battleController.Battle_Attack_Start();
	}

	public static void EndtHunterAttack()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.ControlLock();
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.ControlLock();
	}

	public static void LastMonsterCoinEffect()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.battleController.LastMonsterCoinEffect();
	}

	public static void StartMonsterAttack()
	{
	}

	public static void EndMonsterAttack()
	{
	}

	public static void SetHunterFullSkillGauge(int _index)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.battleController.SetFullSkillGague(_index);
	}

	public static void ForceIntroMonsterHp()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.battleController.ForceIntroMonsterHp();
	}

	public static void StartHunterAttack(int hunterIdx)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.ClearHunterAttackUI(hunterIdx);
	}

	public static void AddHunterCombo(float combo, int hunterColor, Vector3 position, int hunterIdx = 0)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.AddHunterCombo(combo, hunterColor, position, hunterIdx);
	}

	public static void AddHunterAttack(int attack, int hunterColor, Vector3 position, int hunterIdx = 0)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.AddHunterAttack(attack, hunterColor, position, hunterIdx);
	}

	public static void HunterComboComplete()
	{
	}

	public static void ShowMonsterDamageUI(MonsterDamageUI.MonsterDamageType type, int damage, Vector3 position)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.ShowMonsterDamageUI(type, damage, position);
	}

	public static void ActiveBlock(BlockType _type)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.ActiveBlockType(_type);
	}

	public static void DeActiveBlock(BlockType _type)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.DeActiveBlockType(_type);
	}

	public static void AddCoin(int addCoin)
	{
		UnityEngine.Debug.Log("AddCoin");
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.ShowAddCoin(addCoin);
	}

	public static void AddArenaPoint(int _addPoint)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.ShowAddArenaPoint(_addPoint);
	}

	public static void AddItem(int itemIdx, int count)
	{
		GameInfo.userPlayData.AddItem(itemIdx, count);
	}

	public static void AddMonster(int monsterIdx)
	{
		GameInfo.userPlayData.AddMonster(monsterIdx);
	}

	public static void ShowWarningEffect()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.ShowWarningEffect();
	}

	public static void HideWarningEffect()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.HideWarningEffect();
	}

	public static void Heal(int value)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.HealUserHp(value);
	}

	public static void Damage(int value)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.Damage(value);
	}

	public static void GameContinue()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.isGameOver = false;
		GameObjectSingleton<InGamePlayManager>.Inst.battleController.Show_Heal_Eff();
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.GameContinue();
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.GameContinue();
		TouchActive();
		switch (GameInfo.inGamePlayData.inGameType)
		{
		case InGameType.Stage:
			if (GameInfo.inGamePlayData.isDragon == 0)
			{
				SoundController.BGM_Play(MusicSoundType.IngameBGM);
			}
			else
			{
				SoundController.BGM_Play(MusicSoundType.InGameDragonBgm);
			}
			break;
		case InGameType.Arena:
			SoundController.BGM_Play(MusicSoundType.ArenaBGM);
			break;
		}
	}

	public static void GameOver()
	{
		if (!GameObjectSingleton<InGamePlayManager>.Inst.isGameOver)
		{
			GameObjectSingleton<InGamePlayManager>.Inst.isGameOver = true;
			GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.Lock();
			GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.GameOver();
			GameObjectSingleton<InGamePlayManager>.Inst.uiController.ShowDefeatResult();
			TouchLock();
		}
	}

	public static void GameQuit()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.isGameOver = true;
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.Lock();
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.GameOver();
		TouchLock();
	}

	public static void ShowLoseResult()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.ShowLoseResult();
	}

	public static void ShowArenaLoseResult(ARENA_GAME_END_RESULT _data)
	{
		GameInfo.isShowArenaSalePack = true;
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.ShowArenaLoseResult(_data);
	}

	public static void GameClear()
	{
		if (GameObjectSingleton<InGamePlayManager>.Inst.isGameOver)
		{
			return;
		}
		GameObjectSingleton<InGamePlayManager>.Inst.isGameOver = true;
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.Lock();
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.GameOver();
		TouchLock();
		if (GameObjectSingleton<InGamePlayManager>.Inst.CheckSnenarioGameEndEvent())
		{
			return;
		}
		if (TutorialManager.Intro)
		{
			TutorialManager.ShowTutorial();
		}
		else if (GameInfo.inGamePlayData.inGameType == InGameType.Stage)
		{
			GameObjectSingleton<InGamePlayManager>.Inst.uiController.ShowClearResult();
			if (ShowBattleClearResult != null)
			{
				ShowBattleClearResult();
			}
		}
		else
		{
			GameObjectSingleton<InGamePlayManager>.Inst.uiController.ShowArenaClearResult();
		}
	}

	public static void CallGameLoseEvent()
	{
		if (GameLose != null)
		{
			GameLose();
		}
	}

	public static void BattleReward()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.ShowClearReward();
	}

	public static void NotEnoughJewel(Action _onCallback)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.ShowNotEnoughJewel();
		continueTimer = _onCallback;
	}

	public static void JewelShop()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.ShowJewelShop();
	}

	public static void JewelShopBuy(int key)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.ShowJewelShopBuy(key);
	}

	public static void JewelShopClose()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.CloseJewelShop();
	}

	public static void JewelShopBuyClose()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.CloseJewelShopBuy();
	}

	public static void MonsterTargeting(Monster _monster)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.battleController.MonsterTargeting(_monster);
	}

	public static void MonsterUnTargeting()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.battleController.MonsterUnTargeting();
	}

	public void OnClickForceClear()
	{
	}

	public void OnClickForcePassWave()
	{
		MWPoolManager.DeSpawnPoolAll("Monster");
		MWPoolManager.DeSpawnPoolAll("Effect");
		battleController.ForceWaveClear();
	}

	public static void ShakeCamera(bool isVibration = true)
	{
		if (GameObjectSingleton<InGamePlayManager>.Inst.coroutineShakeCamera != null)
		{
			GameObjectSingleton<InGamePlayManager>.Inst.StopCoroutine(GameObjectSingleton<InGamePlayManager>.Inst.coroutineShakeCamera);
			GameObjectSingleton<InGamePlayManager>.Inst.coroutineShakeCamera = null;
		}
		GameObjectSingleton<InGamePlayManager>.Inst.StartCoroutine(GameObjectSingleton<InGamePlayManager>.Inst.ProcessShakeCamera());
		if (isVibration && GamePreferenceManager.GetIsVibration())
		{
			Handheld.Vibrate();
		}
	}

	public static void ActiveOnlySelectOneBlock(int _x, int _y)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.ActiveOnlySelectOneBlock(_x, _y);
	}

	public static void AddActiveSelectBlock(int _x, int _y)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.AddActiveSelectBlock(_x, _y);
	}

	public static void ActiveOnlyDeselectOneBlock(int _x, int _y)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.ActiveOnlyDeselectOneBlock(_x, _y);
	}

	public static void StopDeSelectAllBlock()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.StopDeSelectAllBlock();
	}

	public static void AllActiveBlock()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.AllActiveBlock();
	}

	public static void AllLockBlock()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.AllLockBlock();
	}

	public static void AllDeSelectBlock()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.AllDeSelectBlock();
	}

	public static Vector3 GetBlockPosition(int _x, int _y)
	{
		return GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.GetBlockPosition(_x, _y);
	}

	public static Transform GetBlock(int _x, int _y)
	{
		return GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.GetBlock(_x, _y);
	}

	public static Transform GetMonsterTurn()
	{
		return GameObjectSingleton<InGamePlayManager>.Inst.battleController.GetMonsterTurn();
	}

	public static void BattleRewardOpenEvent()
	{
		UnityEngine.Debug.Log("Ingameplay - BattleRewardOpenEvent");
		if (BattleRewardOpen != null)
		{
			BattleRewardOpen();
		}
	}

	public static void BattleRewardTutorialEvent()
	{
		if (BattleRewardTutorial != null)
		{
			BattleRewardTutorial();
		}
	}

	public static void BattleRewardCompleteEvent()
	{
		if (BattleRewardComplete != null)
		{
			BattleRewardComplete();
		}
	}

	public static void UseHunterSkillForTutorial(Hero hunter)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.battleController.UseHunterSkillForTutorial(hunter);
	}

	public static void ShowBuffUI(Vector3 _position, BlockType _type, int _buff)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.ShowBuffUI(_position, _type, _buff);
	}

	public static void ChangeBlockType(BlockType _from, BlockType _to, int _skillIdx)
	{
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.ChangeBlockType(_from, _to, _skillIdx);
	}

	public static void ShowShuffleUI()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.ShowShuffleUI();
	}

	public static void HideShuffleUI()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.uiController.HideShuffleUI();
	}

	public static void ChangeSpecialBlock()
	{
		GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.ChangeSpecialBlock();
	}

	private void Init()
	{
		SoundController.BGM_Stop(MusicSoundType.LobbyBGM);
		switch (GameInfo.inGamePlayData.inGameType)
		{
		case InGameType.Stage:
			if (GameInfo.inGamePlayData.isDragon == 0)
			{
				SoundController.BGM_Play(MusicSoundType.IngameBGM);
			}
			else
			{
				SoundController.BGM_Play(MusicSoundType.InGameDragonBgm);
			}
			break;
		case InGameType.Arena:
			SoundController.BGM_Play(MusicSoundType.ArenaBGM);
			break;
		}
		if (GameInfo.isDirectBattleReward)
		{
			GameObjectSingleton<InGamePlayManager>.Inst.uiController.ShowClearReward();
			TutorialManager.CheckInGameUserTurn();
		}
		else
		{
			isGameOver = false;
			GameObjectSingleton<InGamePlayManager>.Inst.battleController.Set_Battle_Data(GameInfo.inGamePlayData);
			GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.Init();
			GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.Lock();
			GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.PuzzleTouchEnd = OnPuzzleTouchEnd;
			GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.PuzzleSwitch = OnPuzzleSiwtchEvent;
			GameObjectSingleton<InGamePlayManager>.Inst.puzzleController.BlockSelect = OnBlockSelectEvent;
			GameObjectSingleton<InGamePlayManager>.Inst.uiController.Init();
			GameObjectSingleton<InGamePlayManager>.Inst.uiController.MatchTimeFlow = OnMatchTimeFlowEvent;
			GameObjectSingleton<InGamePlayManager>.Inst.uiController.ShowBattleReward = OnShowBattleReward;
		}
		GamePreferenceManager.SetIsBoostRewardVideo(isBoostRewardVideo: false);
	}

	private void HealUserHp(float value)
	{
		uiController.Heal((int)Mathf.Floor(value));
		battleController.Show_Heal_Eff();
	}

	private IEnumerator ProcessShakeCamera()
	{
		float time;
		for (float currentSecond = 0f; currentSecond < 0.3f; currentSecond += Time.realtimeSinceStartup - time)
		{
			time = Time.realtimeSinceStartup;
			Vector3 shakePosition = new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), -10f);
			Camera.main.transform.position = shakePosition;
			yield return null;
		}
		Camera.main.transform.position = new Vector3(0f, 0f, -10f);
		coroutineShakeCamera = null;
	}

	private void OnBlockSelectEvent(int _x, int _y)
	{
		if (BlockSelect != null)
		{
			BlockSelect(_x, _y);
		}
	}

	private void OnMatchTimeFlowEvent(float time)
	{
		if (MatchTimeFlow != null)
		{
			MatchTimeFlow(time);
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
		if (PuzzleSwitch != null)
		{
			PuzzleSwitch();
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
		float num = (float)Screen.width / (float)Screen.height;
		Camera.main.orthographicSize = 6.4f / (float)Screen.width * (float)Screen.height;
	}

	private bool CheckSnenarioGameEndEvent()
	{
		if (GameInfo.inGamePlayData.inGameType != 0)
		{
			return false;
		}
		if (GameInfo.inGamePlayData.isShowScenario && !TutorialManager.Intro && GameInfo.inGamePlayData.isDragon == 1)
		{
			ScenarioManager.EndScenarioEvent = OnScenarioGameEndComplete;
			ScenarioManager.ShowInGame(GameInfo.inGamePlayData.levelIdx);
			return true;
		}
		return false;
	}

	private void OnScenarioUserTurnComplete(int _scenarioIdx)
	{
		ScenarioManager.EndScenarioEvent = null;
		GameInfo.inGamePlayData.isShowScenario = false;
		StartUserTurn();
	}

	private void OnScenarioGameEndComplete(int _scenarioIdx)
	{
		ScenarioManager.EndScenarioEvent = null;
		GameInfo.inGamePlayData.isShowScenario = false;
		GameObjectSingleton<InGamePlayManager>.Inst.isGameOver = false;
		GameClear();
	}

	private bool CheckScenarioUserTurn()
	{
		return GameInfo.inGamePlayData.inGameType == InGameType.Stage && GameInfo.inGamePlayData.isShowScenario && GameInfo.inGamePlayData.isDragon == 0;
	}

	protected override void Awake()
	{
		base.Awake();
	}

	private void Start()
	{
		Init();
	}

	private void Update()
	{
		if (TutorialManager.DialogState || !isTouchState)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			rayTouch = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
			if (TouchBeginEvent != null)
			{
				TouchBeginEvent(UnityEngine.Input.mousePosition, Physics2D.GetRayIntersection(rayTouch, float.PositiveInfinity));
			}
		}
		if (Input.GetMouseButton(0) && TouchMoveEvent != null)
		{
			TouchMoveEvent(UnityEngine.Input.mousePosition);
		}
		if (Input.GetMouseButtonUp(0) && TouchEndEvent != null)
		{
			TouchEndEvent(UnityEngine.Input.mousePosition);
		}
	}

	protected override void OnDestroy()
	{
		GameInfo.userPlayData.Clear();
		base.OnDestroy();
	}
}
