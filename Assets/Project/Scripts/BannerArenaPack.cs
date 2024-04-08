using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class BannerArenaPack : MonoBehaviour
{
	private const float PACKAGE_EFFECT_TIME = 1.2f;

	private const float PACKAGE_EFFECT_DELAY_SPAWN_DURATION = 0.4f;

	private const float PACKAGE_REWARD_BOX_SHOW_DELAY = 0.5f;

	[SerializeField]
	private Text textDisCount;

	[SerializeField]
	private Text textPrice;

	private int hunterCount;

	private Button btnPurchase;

	private System.Object purchase_args;

	private void Init()
	{
		base.gameObject.SetActive(value: true);
		btnPurchase = base.gameObject.GetComponent<Button>();
		btnPurchase.enabled = true;
		textDisCount.text = $"{GameInfo.arenaPackageData.discount}%";
	}

	private void OnInAppPurchaseComplete(int _purchase_id, string _signature, string _receipt, System.Object _args)
	{
		purchase_args = _args;
		Protocol_Set.Protocol_shop_buy_package_Req(_purchase_id, _signature, _receipt, OnBuyProductComplete);
	}

	private void OnBuyProductComplete(ChestListDbData[] _arrRewardList)
	{
		hunterCount = 0;
		List<ChestListDbData> list = new List<ChestListDbData>();
		foreach (ChestListDbData chestListDbData in _arrRewardList)
		{
			if (chestListDbData.chestItem != 50032 && chestListDbData.chestItem != 50031)
			{
				if (chestListDbData.chestHunter > 0)
				{
					hunterCount++;
				}
				list.Add(chestListDbData);
			}
		}
		StartCoroutine(ProcessUserDataEffect(list));
	}

	private IEnumerator ProcessUserDataEffect(List<ChestListDbData> _listReward)
	{
		btnPurchase.enabled = false;
		int jewelCount = UnityEngine.Random.Range(4, 8);
		for (int i = 0; i < jewelCount; i++)
		{
			float num = UnityEngine.Random.Range(-0.2f, -0.1f);
			Transform transform = MWPoolManager.Spawn("Effect", "FX_jewel_get", null, 1.2f + num + 0.4f);
			transform.localScale = new Vector2(0.12f, 0.12f);
			transform.position = btnPurchase.transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
			GameObject gameObject = transform.gameObject;
			Vector3 userJewelPosition = LobbyManager.UserJewelPosition;
			LeanTween.moveX(gameObject, userJewelPosition.x, 1.2f + num);
			GameObject gameObject2 = transform.gameObject;
			Vector3 userJewelPosition2 = LobbyManager.UserJewelPosition;
			LeanTween.moveY(gameObject2, userJewelPosition2.y, 1.2f + num).setEaseInCubic();
		}
		int ticketCount = UnityEngine.Random.Range(4, 8);
		for (int j = 0; j < ticketCount; j++)
		{
			float num2 = UnityEngine.Random.Range(-0.2f, -0.1f);
			Transform transform2 = MWPoolManager.Spawn("Effect", "FX_ArenaTicket", null, 1.2f + num2 + 0.4f);
			transform2.localScale = new Vector2(0.12f, 0.12f);
			transform2.position = btnPurchase.transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
			GameObject gameObject3 = transform2.gameObject;
			Vector3 userArenaTicketPosition = LobbyManager.UserArenaTicketPosition;
			LeanTween.moveX(gameObject3, userArenaTicketPosition.x, 1.2f + num2);
			GameObject gameObject4 = transform2.gameObject;
			Vector3 userArenaTicketPosition2 = LobbyManager.UserArenaTicketPosition;
			LeanTween.moveY(gameObject4, userArenaTicketPosition2.y, 1.2f + num2).setEaseInCubic();
		}
		yield return new WaitForSeconds(1.2f);
		GameDataManager.UpdateUserData();
		SoundController.EffectSound_Play(EffectSoundType.GetJewel);
		SoundController.EffectSound_Play(EffectSoundType.GetCoin);
		yield return new WaitForSeconds(0.5f);
		LobbyManager.ShowChestOpen(_listReward, ChestType.Mysterious, hunterCount);
		LobbyManager.PurchaseSpecialOfferComplete();
		btnPurchase.enabled = true;
	}

	public void OnClickPurchase()
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	private void Start()
	{
		Init();
	}
}
