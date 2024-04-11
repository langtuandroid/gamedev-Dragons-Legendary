using System;

[Serializable]
public class UserStageState
{
	public int stage;

	public int clearRate;

	public UserChapterState[] chapterList = new UserChapterState[0];
}
