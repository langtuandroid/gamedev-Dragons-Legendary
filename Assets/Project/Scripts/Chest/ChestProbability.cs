using UnityEngine;
using UnityEngine.UI;

public class ChestProbability : MonoBehaviour
{
	[SerializeField]
	private Text freeChest_L_Explain;

	[SerializeField]
	private Text freeChest_R_Explain;

	[SerializeField]
	private Text woodenChest_L_Explain;

	[SerializeField]
	private Text woodenChest_R_Explain;

	[SerializeField]
	private Text MysteriousChest_L_Explain;

	[SerializeField]
	private Text MysteriousChest_R_Explain;

	[SerializeField]
	private Text dungeonChest_L_Explain;

	[SerializeField]
	private Text dungeonChest_R_Explain;

	private void SetLocalize()
	{
		freeChest_L_Explain.text = MasterLocalize.GetData("item_type_name_resource") + "\n" + MasterLocalize.GetData("item_type_name_token") + "\n" + MasterLocalize.GetData("item_type_name_jewel");
		woodenChest_L_Explain.text = MasterLocalize.GetData("item_type_name_resource") + "\n" + MasterLocalize.GetData("item_type_name_token") + "\n" + MasterLocalize.GetData("item_type_name_jewel");
		MysteriousChest_L_Explain.text = MasterLocalize.GetData("item_type_name_resource") + "\n" + MasterLocalize.GetData("item_type_name_token") + "\n" + MasterLocalize.GetData("item_type_name_jewel");
		dungeonChest_L_Explain.text = MasterLocalize.GetData("item_type_name_resource") + "\n" + MasterLocalize.GetData("item_type_name_token") + "\n" + MasterLocalize.GetData("item_type_name_jewel");
		freeChest_R_Explain.text = MasterLocalize.GetData("item_type_name_stamina") + "\n" + MasterLocalize.GetData("item_type_name_coin") + "\n" + MasterLocalize.GetData("item_type_name_key");
		woodenChest_R_Explain.text = MasterLocalize.GetData("item_type_name_stamina") + "\n" + MasterLocalize.GetData("item_type_name_coin") + "\n" + MasterLocalize.GetData("item_type_name_hunter");
		MysteriousChest_R_Explain.text = MasterLocalize.GetData("item_type_name_stamina") + "\n" + MasterLocalize.GetData("item_type_name_hunter");
		dungeonChest_R_Explain.text = MasterLocalize.GetData("item_type_name_stamina") + "\n" + MasterLocalize.GetData("item_type_name_coin") + "\n" + MasterLocalize.GetData("item_type_name_key") + "\n" + MasterLocalize.GetData("item_type_name_exp");
	}

	private void Start()
	{
		SetLocalize();
	}
}
