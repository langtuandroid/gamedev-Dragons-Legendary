using UnityEngine;
using UnityEngine.UI;

public class GameNameSelect : MonoBehaviour
{
	[SerializeField]
	private Sprite spriteED;

	[SerializeField]
	private Sprite spriteMH;

	private Image imageGameName;

	private void CheckGameImage()
	{
		switch (Application.systemLanguage)
		{
		case SystemLanguage.Italian:
		case SystemLanguage.Polish:
		case SystemLanguage.Spanish:
		case SystemLanguage.Thai:
		case SystemLanguage.Vietnamese:
			imageGameName.sprite = spriteMH;
			break;
		default:
			imageGameName.sprite = spriteED;
			break;
		}
	}

	private void Awake()
	{
		imageGameName = base.gameObject.GetComponent<Image>();
		CheckGameImage();
	}
}
