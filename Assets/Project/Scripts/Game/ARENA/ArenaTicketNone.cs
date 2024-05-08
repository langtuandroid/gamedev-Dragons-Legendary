using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArenaTicketNone : LobbyPopupBase
{
	public Action GoBackEvent;

	private const float ARENA_TICKET_EFFECT_TIME = 1.2f;

	private const float ARENA_TICKET_EFFECT_DELAY_SPAWN_DURATION = 0.4f;

	[SerializeField]
	private Text textTicket;

	[SerializeField]
	private Text textNeedJewel;

	[SerializeField]
	private Transform trJewelStartPos;

	[SerializeField]
	private Button btnJewel;

	[SerializeField]
	private Button btnCancel;

	public override void Open()
	{
		base.Open();
		btnJewel.enabled = true;
		btnCancel.enabled = true;
		textNeedJewel.text = $"{GameDataManager.GetGameConfigData(ConfigDataType.Arena_ticket_price)}";
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void CloseProcessComplete()
	{
	}

	private void OnArenaTicketComplete()
	{
		StartCoroutine(ProcessShowTicketEffect());
	}

	private IEnumerator ProcessShowTicketEffect()
	{
		btnJewel.enabled = false;
		btnCancel.enabled = false;
		Transform trExpEffect = MasterPoolManager.SpawnObject("Effect", "FX_ArenaTicket", null, 1.6f);
		trExpEffect.localScale = new Vector2(0.12f, 0.12f);
		trExpEffect.position = trJewelStartPos.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
		GameObject gameObject = trExpEffect.gameObject;
		Vector3 userArenaTicketPosition = LobbyManager.UserArenaTicketPosition;
		LeanTween.moveX(gameObject, userArenaTicketPosition.x, 1.2f);
		GameObject gameObject2 = trExpEffect.gameObject;
		Vector3 userArenaTicketPosition2 = LobbyManager.UserArenaTicketPosition;
		LeanTween.moveY(gameObject2, userArenaTicketPosition2.y, 1.2f).setEaseInCubic();
		yield return new WaitForSeconds(1.2f);
		GameDataManager.UpdateUserData();
		SoundController.EffectSound_Play(EffectSoundType.GetExp);
		btnJewel.enabled = true;
		btnCancel.enabled = true;
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
	}

	public void OnClickJewel()
	{
		if (GameInfo.userData.userInfo.jewel < GameDataManager.GetGameConfigData(ConfigDataType.Arena_ticket_price))
		{
			LobbyManager.ShowNotEnoughJewel(GameDataManager.GetGameConfigData(ConfigDataType.Arena_ticket_price) - GameInfo.userData.userInfo.jewel);
			if (GoBackEvent != null)
			{
				GoBackEvent();
			}
		}
		else
		{
			Protocol_Set.Protocol_arena_buy_ticket_Req(OnArenaTicketComplete);
		}
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickCancel()
	{
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}
}
