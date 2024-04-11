public class MatchData
{
	public enum Direction
	{
		LEFT,
		RIGHT,
		TOP,
		BOTTOM
	}

	public Direction direction;

	public int length;

	public int x;

	public int y;

	public MatchData(Direction dir, int length, int x, int y)
	{
		direction = dir;
		this.length = length;
		this.x = x;
		this.y = y;
	}
}
