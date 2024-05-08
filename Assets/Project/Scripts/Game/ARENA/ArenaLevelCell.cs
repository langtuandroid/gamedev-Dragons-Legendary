using UnityEngine;
using UnityEngine.UI;

public class ArenaLevelCell : MonoBehaviour
{
	[SerializeField]
	private GameObject goChestSmall;

	[SerializeField]
	private GameObject goChestBig;

	[SerializeField]
	private Text textStage;

	[SerializeField]
	private Text textTicketCost;

	[SerializeField]
	private GameObject goIconTicket;

	[SerializeField]
	private GameObject goClearDimmed;

	[SerializeField]
	private GameObject goIconClear;

	[SerializeField]
	private GameObject goClearText;

	[SerializeField]
	private GameObject goGetHunter;

	private ARENA_LEVEL_DATA arenaLevelData;

	public void Init(ARENA_LEVEL_DATA _data)
	{
		arenaLevelData = _data;
		goChestSmall.SetActive(_data.chestType == 1);
		goChestBig.SetActive(_data.chestType == 2);
		textStage.text = string.Format(MasterLocalize.GetData("arena_lobby_text_03"), _data.levelIdx);
		textTicketCost.text = $"{_data.costTicket}";
		switch (_data.passYn)
		{
		case "today":
			goClearDimmed.SetActive(value: false);
			goIconTicket.SetActive(value: true);
			textTicketCost.gameObject.SetActive(value: true);
			break;
		case "y":
			goClearDimmed.SetActive(value: true);
			goIconClear.SetActive(value: true);
			goClearText.SetActive(value: true);
			goIconTicket.SetActive(value: false);
			textTicketCost.gameObject.SetActive(value: false);
			break;
		case "n":
			goClearDimmed.SetActive(value: true);
			goIconClear.SetActive(value: false);
			goClearText.SetActive(value: false);
			goIconTicket.SetActive(value: false);
			textTicketCost.gameObject.SetActive(value: false);
			break;
		}
		goGetHunter.SetActive(arenaLevelData.levelIdx == 10);
	}
}
