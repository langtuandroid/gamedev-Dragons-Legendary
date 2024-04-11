using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageToggle : MonoBehaviour
{
	public Action<int> MoveChapterPage;

	[SerializeField]
	private List<Toggle> listPageToggle = new List<Toggle>();

	public void InitPage(int count)
	{
		for (int i = 0; i < listPageToggle.Count; i++)
		{
			listPageToggle[i].gameObject.SetActive(i < count);
		}
	}

	public void SetPage(int index)
	{
		for (int i = 0; i < listPageToggle.Count; i++)
		{
			listPageToggle[i].isOn = (i == index);
		}
	}

	public void OnClickPage(int _page)
	{
		if (MoveChapterPage != null)
		{
			MoveChapterPage(_page);
		}
	}
}
