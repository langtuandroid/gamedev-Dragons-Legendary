using System;

[Serializable]
public class UserFloorStage
{
	public int stage;

	public bool isOpen;

	public UserFloorData[] floorList = new UserFloorData[0];
}
