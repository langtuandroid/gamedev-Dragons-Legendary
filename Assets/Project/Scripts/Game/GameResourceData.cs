using System.Collections.Generic;
using UnityEngine;

public class GameResourceData : GameObjectSingleton<GameResourceData>
{
	[SerializeField]
	private List<Sprite> listStageMenuSprite = new List<Sprite>();

	[SerializeField]
	private List<Sprite> listFloorDetailTitleSprite = new List<Sprite>();

	[SerializeField]
	private List<Sprite> listStageCellSprite = new List<Sprite>();

	[SerializeField]
	private List<Sprite> listStagePreviewSprite = new List<Sprite>();

	[SerializeField]
	private List<Sprite> listStageStoreBleand = new List<Sprite>();

	public Sprite GetStageMenuImage(int index)
	{
		if (index < listStageMenuSprite.Count)
		{
			return listStageMenuSprite[index];
		}
		return null;
	}

	public Sprite GetFloorTitleSprite(int index)
	{
		if (index < listFloorDetailTitleSprite.Count)
		{
			return listFloorDetailTitleSprite[index];
		}
		return null;
	}

	public Sprite GetStageCellSprite(int index)
	{
		if (index < listStageCellSprite.Count)
		{
			return listStageCellSprite[index];
		}
		return null;
	}

	public Sprite GetStagePreviewSprite(int index)
	{
		if (index < listStagePreviewSprite.Count)
		{
			return listStagePreviewSprite[index];
		}
		return null;
	}

	public Sprite GetStageStoreBlendSprite(int index)
	{
		if (index < listStageStoreBleand.Count)
		{
			return listStageStoreBleand[index];
		}
		return null;
	}
}
