using Spine.Unity;
using UnityEngine;

public class ScenarioCharacter : MonoBehaviour
{
	[SerializeField]
	private SkeletonAnimation spineAnimation;

    private void Awake()
    {
        if (spineAnimation == null) spineAnimation = GetComponentInChildren<SkeletonAnimation>();
    }

    public void Talk()
	{
		spineAnimation.loop = true;
		spineAnimation.AnimationName = "Idle";
	}

	public void Dimmed()
	{
		spineAnimation.loop = false;
		spineAnimation.AnimationName = "Dimmed";
	}
}
