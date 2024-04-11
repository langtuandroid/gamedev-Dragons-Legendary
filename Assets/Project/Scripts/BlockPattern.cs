using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlockPattern
{
	public BlockType type;

	public Sprite spriteActive;

	public Sprite spriteDeActive;

	public List<Sprite> listSpecialType;

	public bool isActive = true;

	public Sprite GetSprite()
	{
		if (isActive)
		{
			return spriteActive;
		}
		return spriteDeActive;
	}

	public Sprite GetSpecialSprite(int chainCount)
	{
		if (type == BlockType.White)
		{
			return GetSprite();
		}
		UnityEngine.Debug.Log("chainCount :: " + chainCount + " / " + listSpecialType.Count);
		if (chainCount >= 0)
		{
			UnityEngine.Debug.Log("GetSpecialSprite - chainCount :: " + type);
			if (chainCount < listSpecialType.Count)
			{
				return listSpecialType[chainCount];
			}
			return listSpecialType[listSpecialType.Count - 1];
		}
		UnityEngine.Debug.Log("GetSpecialSprite - GetSprite :: " + type);
		return GetSprite();
	}
}
