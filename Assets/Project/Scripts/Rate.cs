using UnityEngine;

public class Rate : MonoBehaviour
{
	[SerializeField]
	private Transform[] Star_Set;

	private int StarCount = 5;

	public void Init()
	{
		base.gameObject.SetActive(value: true);
		StarCount = 5;
		for (int i = 0; i < Star_Set.Length; i++)
		{
			Star_Set[i].gameObject.SetActive(value: true);
		}
	}

	public void OnClickStar(int count)
	{
		StarCount = count + 1;
		for (int i = 0; i < Star_Set.Length; i++)
		{
			if (count >= i)
			{
				Star_Set[i].gameObject.SetActive(value: true);
			}
			else
			{
				Star_Set[i].gameObject.SetActive(value: false);
			}
		}
	}

	public void OnClickOkButton()
	{
		switch (StarCount)
		{
		case 4:
		case 5:
			Application.OpenURL("https://play.google.com/store/apps/details?id=com.cookapps.matchhero");
			break;
		}
		GamePreferenceManager.SetIsRate();
		base.gameObject.SetActive(value: false);
	}

	public void OnClickLaterButton()
	{
		base.gameObject.SetActive(value: false);
	}
}
