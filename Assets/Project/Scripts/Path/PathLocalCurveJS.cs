#if ENABLE_BOO
using Boo.Lang.Runtime;
#endif
using System;
using UnityEngine;

[Serializable]
public class PathLocalCurveJS : MonoBehaviour
{
	public AnimationCurve customAnimationCurve;

	public Transform pt1;

	public Transform pt2;

	public Transform pt3;

	public Transform pt4;

	private Transform containingSphere;

	private LTSpline spline;

	private GameObject ltLogo;

	public void Start()
	{
		ltLogo = GameObject.Find("LeanTweenLogo");
		containingSphere = GameObject.Find("ContaingCube").transform;
		Vector3[] array = new Vector3[6]
		{
			pt1.position,
			pt1.position,
			pt2.position,
			pt3.position,
			pt4.position,
			pt4.position
		};
		spline = new LTSpline(array);
		LeanTween.moveSplineLocal(ltLogo, array, 3f).setEase(LeanTweenType.easeInQuad).setOrientToPath(doesOrient: true)
			.setRepeat(-1);
	}

	public void Update()
	{
		Vector3 eulerAngles = containingSphere.transform.eulerAngles;
		float y = eulerAngles.y + Time.deltaTime * 3f;
		Vector3 eulerAngles2 = containingSphere.transform.eulerAngles;
		eulerAngles2.y = y;
		Vector3 vector2 = containingSphere.transform.eulerAngles = eulerAngles2;
	}

	public void OnDrawGizmos()
	{
#if ENABLE_BOO
		if (!RuntimeServices.EqualityOperator(spline, null))
		{
			spline.gizmoDraw(1f);
		}
#endif
	}

	public void Main()
	{
	}
}
