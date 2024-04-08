using System;

[Serializable]
public class UserChapterState
{
	public int chapter;

	public bool isClear;

	public bool isOpen;

	public bool isReward;

	public UserLevelState[] levelList = new UserLevelState[0];
}
