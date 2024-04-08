using System;

[Serializable]
public class BlockData
{
	public int x;

	public int y;

	public float xPos;

	public float yPos;

	public BlockData(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public BlockData(int x, int y, float xPos, float yPos)
	{
		this.x = x;
		this.y = y;
		this.xPos = xPos;
		this.yPos = yPos;
	}
}
