public static class SoundController
{
	public static void BGM_Play(MusicSoundType _type)
	{
		switch (_type)
		{
		case MusicSoundType.LobbyBGM:
			if (GameInfo.IS_MUSIC_SOUND && !AudioController.IsPlaying("bgm_lobby"))
			{
				AudioController.Play("bgm_lobby");
			}
			break;
		case MusicSoundType.IngameBGM:
			if (GameInfo.IS_MUSIC_SOUND && !AudioController.IsPlaying("bgm_ingame"))
			{
				AudioController.Play("bgm_ingame");
			}
			break;
		case MusicSoundType.ArenaBGM:
			if (GameInfo.IS_MUSIC_SOUND && !AudioController.IsPlaying("bgm_arena"))
			{
				AudioController.Play("bgm_arena");
			}
			break;
		case MusicSoundType.InGameDragonBgm:
			if (GameInfo.IS_MUSIC_SOUND && !AudioController.IsPlaying("bgm_dragon"))
			{
				AudioController.Play("bgm_dragon");
			}
			break;
		}
	}

	public static void BGM_Stop(MusicSoundType _type)
	{
		switch (_type)
		{
		case MusicSoundType.LobbyBGM:
			AudioController.Stop("bgm_lobby");
			break;
		case MusicSoundType.IngameBGM:
			AudioController.Stop("bgm_ingame");
			break;
		case MusicSoundType.ArenaBGM:
			AudioController.Stop("bgm_arena");
			break;
		case MusicSoundType.InGameDragonBgm:
			AudioController.Stop("bgm_dragon");
			break;
		}
	}

	public static void StopAll()
	{
		AudioController.StopAll();
		UnityEngine.Debug.Log("StopAll");
	}

	public static void EffectSound_Stop(EffectSoundType _type)
	{
		switch (_type)
		{
		case EffectSoundType.InGameWarning:
			AudioController.Stop("sfx_ingame_warning");
			break;
		case EffectSoundType.DialogueText:
			AudioController.Stop("sfx_common_text_loop_2");
			break;
		}
	}

	public static void EffectSound_Play(EffectSoundType _type)
	{
		if (GameInfo.IS_EFFECT_SOUND)
		{
			switch (_type)
			{
			case EffectSoundType.ButtonClick:
				AudioController.Play("sfx_lobby_touch");
				break;
			case EffectSoundType.WaveStart:
				AudioController.Play("sfx_ingame_jingle_start");
				break;
			case EffectSoundType.LevelClear:
				AudioController.Play("sfx_ingame_jingle_win");
				break;
			case EffectSoundType.LevelFail:
				AudioController.Play("sfx_ingame_jingle_lose");
				break;
			case EffectSoundType.MonsterTargeting:
				AudioController.Play("sfx_ingame_targeting");
				break;
			case EffectSoundType.BlockClick:
				AudioController.Play("sfx_ingame_block_choice");
				break;
			case EffectSoundType.BlockMove:
				AudioController.Play("sfx_ingame_block_move");
				break;
			case EffectSoundType.BlockMatching:
				AudioController.Play("sfx_ingame_block_match_1");
				break;
			case EffectSoundType.BlockDestroy:
				AudioController.Play("sfx_ingame_block_match_3");
				break;
			case EffectSoundType.HealMatching:
				AudioController.Play("sfx_ingame_potion_match_1");
				break;
			case EffectSoundType.HealDestroy:
				AudioController.Play("sfx_ingame_potion_match_2");
				break;
			case EffectSoundType.Combo1:
				AudioController.Play("sfx_ingame_block_combo_1");
				break;
			case EffectSoundType.Combo2:
				AudioController.Play("sfx_ingame_block_combo_2");
				break;
			case EffectSoundType.Combo3:
				AudioController.Play("sfx_ingame_block_combo_3");
				break;
			case EffectSoundType.Combo4:
				AudioController.Play("sfx_ingame_block_combo_4");
				break;
			case EffectSoundType.Combo5:
				AudioController.Play("sfx_ingame_block_combo_5");
				break;
			case EffectSoundType.TimeOver:
				AudioController.Play("sfx_ingame_time_over");
				break;
			case EffectSoundType.ComboAdd:
				AudioController.Play("sfx_ingame_attack_motion_1");
				break;
			case EffectSoundType.HunterAttack:
				AudioController.Play("sfx_ingame_attack_motion_2");
				break;
			case EffectSoundType.MonsterHit:
				AudioController.Play("sfx_ingame_damage_monster");
				break;
			case EffectSoundType.MonsterStun:
				AudioController.Play("sfx_ingame_stun_monster");
				break;
			case EffectSoundType.MonsterDie:
				AudioController.Play("sfx_ingame_dead_monster");
				break;
			case EffectSoundType.WaveMove:
				AudioController.Play("sfx_ingame_next_wave");
				break;
			case EffectSoundType.HunterHit:
				AudioController.Play("sfx_ingame_damage_hero");
				break;
			case EffectSoundType.HunterSkillReady:
				AudioController.Play("sfx_ingame_skill_hero_ready");
				break;
			case EffectSoundType.FreeChestOpen:
				AudioController.Play("sfx_lobby_free_chest_open");
				break;
			case EffectSoundType.WornChestOpen:
				AudioController.Play("sfx_lobby_worn_chest_open");
				break;
			case EffectSoundType.MysteriousChestOpen:
				AudioController.Play("sfx_lobby_mysterious_chest_open");
				break;
			case EffectSoundType.UseCoin:
				AudioController.Play("sfx_shop_buy_item");
				break;
			case EffectSoundType.GetCoin:
				AudioController.Play("sfx_lobby_get_coin");
				break;
			case EffectSoundType.HunterLevelUp:
				AudioController.Play("sfx_lobby_hero_level_up");
				break;
			case EffectSoundType.LockBlockAppear:
				AudioController.Play("sfx_ingame_block_lock");
				break;
			case EffectSoundType.LockBlockDestroy:
				AudioController.Play("sfx_ingame_block_lock_destroy");
				break;
			case EffectSoundType.ObstacleBlockAppear:
				AudioController.Play("sfx_ingame_block_obstruction");
				break;
			case EffectSoundType.ObstacleBlockDestroy:
				AudioController.Play("sfx_ingame_block_obstruction_destroy");
				break;
			case EffectSoundType.MonsterItemDrop:
				AudioController.Play("sfx_ingame_item_drop");
				break;
			case EffectSoundType.GetStar1:
				AudioController.Play("sfx_ingame_get_star_01");
				break;
			case EffectSoundType.GetStar2:
				AudioController.Play("sfx_ingame_get_star_02");
				break;
			case EffectSoundType.GetStar3:
				AudioController.Play("sfx_ingame_get_star_03");
				break;
			case EffectSoundType.Warning:
				AudioController.Play("sfx_lobby_warning");
				break;
			case EffectSoundType.Cancel:
				AudioController.Play("sfx_lobby_cancel");
				break;
			case EffectSoundType.Scroll:
				AudioController.Play("sfx_lobby_scroll");
				break;
			case EffectSoundType.OpenPopup:
				AudioController.Play("sfx_lobby_popup");
				break;
			case EffectSoundType.LevelPlay:
				AudioController.Play("sfx_lobby_run");
				break;
			case EffectSoundType.ChestBoxIdle:
				AudioController.Play("sfx_lobby_shop_boxidle");
				break;
			case EffectSoundType.ChestHunterGet:
				AudioController.Play("sfx_lobby_shop_hunter");
				break;
			case EffectSoundType.HunterPromotionUp:
				AudioController.Play("sfx_lobby_deck_gradeup");
				break;
			case EffectSoundType.HunterSwitching:
				AudioController.Play("sfx_lobby_deck_change");
				break;
			case EffectSoundType.StoreOpen:
				AudioController.Play("sfx_lobby_shop_open");
				break;
			case EffectSoundType.FillGauge:
				AudioController.Play("sfx_lobby_fill_gauge");
				break;
			case EffectSoundType.UseJewel:
				AudioController.Play("sfx_lobby_use_jewel");
				break;
			case EffectSoundType.GetMedal:
				AudioController.Play("sfx_ingame_item_drop");
				break;
			case EffectSoundType.StoreUnlock:
				AudioController.Play("sfx_shop_new_unlock");
				break;
			case EffectSoundType.StoreUpgrade:
				AudioController.Play("sfx_shop_upgrade");
				break;
			case EffectSoundType.GetExp:
				AudioController.Play("sfx_ingame_get_exp");
				break;
			case EffectSoundType.GetJewel:
				AudioController.Play("sfx_ingame_get_jewel");
				break;
			case EffectSoundType.GetEnergy:
				AudioController.Play("sfx_ingame_get_energy");
				break;
			case EffectSoundType.GetCoinIngame:
				AudioController.Play("sfx_ingame_get_coin");
				break;
			case EffectSoundType.GetKeyIngame:
				AudioController.Play("sfx_ingame_get_key");
				break;
			case EffectSoundType.UserLevelUp:
				AudioController.Play("sfx_lobby_run");
				break;
			case EffectSoundType.OpenChapter:
				AudioController.Play("sfx_shop_get_medal");
				break;
			case EffectSoundType.GetArenaPoint:
				AudioController.Play("sfx_ingame_get_laurel");
				break;
			case EffectSoundType.MonsterSkillCut:
				AudioController.Play("sfx_monster_skill");
				break;
			case EffectSoundType.InGameWarning:
				AudioController.Play("sfx_ingame_warning");
				break;
			case EffectSoundType.TimerTick:
				AudioController.Play("sfx_timer_continue");
				break;
			case EffectSoundType.DialogueText:
				AudioController.Play("sfx_common_text_loop_2");
				break;
			}
		}
	}

	public static void HunterSkillCutPlay(int _hunterIdx)
	{
		if (GameInfo.IS_EFFECT_SOUND)
		{
			switch (_hunterIdx)
			{
			case 20001:
				AudioController.Play("tws_sfx_human_rogue_attack");
				break;
			case 20002:
				AudioController.Play("tws_sfx_orc_bomber_explosion");
				break;
			case 20003:
				AudioController.Play("tws_sfx_orc_tauren_attack_02");
				break;
			case 20004:
				AudioController.Play("tws_sfx_orc_rider_attack_01");
				break;
			case 20006:
				AudioController.Play("tws_sfx_orc_hunter_attack");
				break;
			case 20007:
				AudioController.Play("tws_sfx_human_guard_attack");
				break;
			case 20008:
				AudioController.Play("tws_sfx_human_spear_attack");
				break;
			case 20009:
				AudioController.Play("tws_sfx_magic_meteor_01");
				break;
			case 20011:
				AudioController.Play("tws_sfx_human_archer_attack");
				break;
			case 20012:
				AudioController.Play("tws_sfx_orc_champ_attack");
				break;
			case 20014:
				AudioController.Play("tws_sfx_orc_hunter_attack");
				break;
			case 20015:
				AudioController.Play("tws_sfx_magic_heal");
				break;
			case 20016:
				AudioController.Play("tws_sfx_human_griffon_attack");
				break;
			case 20017:
				AudioController.Play("tws_sfx_human_spear_attack");
				break;
			case 20019:
				AudioController.Play("tws_sfx_human_priest_heal");
				break;
			case 20020:
				AudioController.Play("tws_sfx_human_musketeer_attack");
				break;
			case 20022:
				AudioController.Play("tws_sfx_undead_ass_attack");
				break;
			case 20023:
				AudioController.Play("tws_sfx_undead_skeleton_attack");
				break;
			case 20024:
				AudioController.Play("tws_sfx_undead_necro_attack_01");
				break;
			case 20025:
				AudioController.Play("tws_sfx_undead_reaper_attack_02");
				break;
			case 20501:
				AudioController.Play("tws_sfx_human_knight_attack_01");
				break;
			case 20502:
				AudioController.Play("tws_sfx_orc_captain_skill_01");
				break;
			case 20503:
				AudioController.Play("tws_sfx_undead_hunter_attack");
				break;
			case 20504:
				AudioController.Play("tws_sfx_human_paladin_attack");
				break;
			case 20505:
				AudioController.Play("tws_sfx_magic_meteor_01");
				break;
			}
		}
	}

	public static void Monster_Play(int _monsterIdx)
	{
		if (GameInfo.IS_EFFECT_SOUND)
		{
			switch (_monsterIdx)
			{
			case 33110:
			case 33120:
			case 33130:
				AudioController.Play("tws_sfx_orc_tauren_attack_02");
				break;
			case 33410:
			case 33420:
			case 33430:
				AudioController.Play("tws_sfx_orc_hunter_attack");
				break;
			case 31410:
			case 31420:
			case 31430:
				AudioController.Play("tws_sfx_human_guard_attack");
				break;
			case 32410:
			case 32420:
			case 32430:
				AudioController.Play("tws_sfx_human_spear_attack");
				break;
			case 31310:
			case 31320:
			case 31330:
				AudioController.Play("tws_sfx_human_archer_attack");
				break;
			case 30210:
			case 30220:
			case 30230:
				AudioController.Play("tws_sfx_orc_champ_attack");
				break;
			case 32310:
			case 32320:
			case 32330:
				AudioController.Play("tws_sfx_orc_hunter_attack");
				break;
			case 32210:
			case 32220:
			case 32230:
				AudioController.Play("tws_sfx_human_spear_attack");
				break;
			case 31210:
			case 31220:
			case 31230:
				AudioController.Play("tws_sfx_human_priest_heal");
				break;
			case 33210:
			case 33220:
			case 33230:
				AudioController.Play("tws_sfx_human_musketeer_attack");
				break;
			case 33510:
			case 33520:
			case 33530:
				AudioController.Play("tws_sfx_undead_skeleton_attack");
				break;
			case 32510:
			case 32520:
			case 32530:
				AudioController.Play("tws_sfx_undead_necro_attack_01");
				break;
			case 31510:
			case 31520:
			case 31530:
				AudioController.Play("tws_sfx_undead_reaper_attack_02");
				break;
			case 30110:
			case 30120:
			case 30130:
				AudioController.Play("tws_sfx_human_knight_attack_01");
				break;
			case 30310:
			case 30320:
			case 30330:
				AudioController.Play("tws_sfx_undead_hunter_attack");
				break;
			case 30510:
			case 30520:
			case 30530:
				AudioController.Play("tws_sfx_magic_meteor_01");
				break;
			}
		}
	}

	public static void HunterSkillSound(int _skillIdx)
	{
		if (GameInfo.IS_EFFECT_SOUND)
		{
			switch (_skillIdx)
			{
			case 25001:
				AudioController.Play("sfx_ingame_hero_skill_warrior_01");
				break;
			case 25002:
				AudioController.Play("sfx_ingame_hero_skill_warrior_02");
				break;
			case 25003:
				AudioController.Play("sfx_ingame_hero_skill_warrior_03");
				break;
			case 25004:
				AudioController.Play("sfx_ingame_hero_skill_warrior_04");
				break;
			case 25006:
				AudioController.Play("sfx_ingame_hero_skill_wizard_01");
				break;
			case 25007:
				AudioController.Play("sfx_ingame_hero_skill_wizard_02");
				break;
			case 25008:
				AudioController.Play("sfx_ingame_hero_skill_wizard_03");
				break;
			case 25009:
				AudioController.Play("sfx_ingame_hero_skill_wizard_04");
				break;
			case 25011:
				AudioController.Play("sfx_ingame_hero_skill_archer_01");
				break;
			case 25012:
				AudioController.Play("sfx_ingame_hero_skill_archer_02");
				break;
			case 25014:
				AudioController.Play("sfx_ingame_hero_skill_archer_03");
				break;
			case 25015:
				AudioController.Play("sfx_ingame_hero_skill_archer_04");
				break;
			case 25016:
				AudioController.Play("sfx_ingame_hero_skill_knight_01");
				break;
			case 25017:
				AudioController.Play("sfx_ingame_hero_skill_knight_02");
				break;
			case 25019:
				AudioController.Play("sfx_ingame_hero_skill_knight_03");
				break;
			case 25020:
				AudioController.Play("sfx_ingame_hero_skill_knight_04");
				break;
			case 25022:
				AudioController.Play("sfx_ingame_hero_skill_thief_01");
				break;
			case 25023:
				AudioController.Play("sfx_ingame_hero_skill_thief_02");
				break;
			case 25024:
				AudioController.Play("sfx_ingame_hero_skill_thief_03");
				break;
			case 25025:
				AudioController.Play("sfx_ingame_hero_skill_thief_04");
				break;
			case 25501:
				AudioController.Play("sfx_arena_hero_skill_wizard_01");
				break;
			case 25502:
				AudioController.Play("sfx_arena_hero_skill_archer_01");
				break;
			case 25503:
				AudioController.Play("sfx_arena_hero_skill_thief_01");
				break;
			case 25504:
				AudioController.Play("sfx_arena_hero_skill_warrior_01");
				break;
			case 25505:
				AudioController.Play("sfx_arena_hero_skill_wizard_01");
				break;
			}
		}
	}
}
