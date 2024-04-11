using System.Reflection;
using UnityEngine;

public class GeneralEasingTypes : MonoBehaviour
{
	public float lineDrawScale = 10f;

	public AnimationCurve animationCurve;

	private string[] easeTypes = new string[35]
	{
		"EaseLinear",
		"EaseAnimationCurve",
		"EaseSpring",
		"EaseInQuad",
		"EaseOutQuad",
		"EaseInOutQuad",
		"EaseInCubic",
		"EaseOutCubic",
		"EaseInOutCubic",
		"EaseInQuart",
		"EaseOutQuart",
		"EaseInOutQuart",
		"EaseInQuint",
		"EaseOutQuint",
		"EaseInOutQuint",
		"EaseInSine",
		"EaseOutSine",
		"EaseInOutSine",
		"EaseInExpo",
		"EaseOutExpo",
		"EaseInOutExpo",
		"EaseInCirc",
		"EaseOutCirc",
		"EaseInOutCirc",
		"EaseInBounce",
		"EaseOutBounce",
		"EaseInOutBounce",
		"EaseInBack",
		"EaseOutBack",
		"EaseInOutBack",
		"EaseInElastic",
		"EaseOutElastic",
		"EaseInOutElastic",
		"EasePunch",
		"EaseShake"
	};

	private void Start()
	{
		demoEaseTypes();
	}

	private void demoEaseTypes()
	{
		for (int i = 0; i < easeTypes.Length; i++)
		{
			string text = easeTypes[i];
			Transform obj = GameObject.Find(text).transform.Find("Line");
			float obj1val = 0f;
			LTDescr lTDescr = LeanTween.value(obj.gameObject, 0f, 1f, 5f).setOnUpdate(delegate(float val)
			{
				Vector3 localPosition = obj.localPosition;
				localPosition.x = obj1val * lineDrawScale;
				localPosition.y = val * lineDrawScale;
				obj.localPosition = localPosition;
				obj1val += Time.deltaTime / 5f;
				if (obj1val > 1f)
				{
					obj1val = 0f;
				}
			});
			if (text.IndexOf("AnimationCurve") >= 0)
			{
				lTDescr.setEase(animationCurve);
			}
			else
			{
				MethodInfo method = lTDescr.GetType().GetMethod("set" + text);
				method.Invoke(lTDescr, null);
			}
			if (text.IndexOf("EasePunch") >= 0)
			{
				lTDescr.setScale(1f);
			}
			else if (text.IndexOf("EaseOutBounce") >= 0)
			{
				lTDescr.setOvershoot(2f);
			}
		}
		LeanTween.delayedCall(base.gameObject, 10f, resetLines);
		LeanTween.delayedCall(base.gameObject, 10.1f, demoEaseTypes);
	}

	private void resetLines()
	{
		for (int i = 0; i < easeTypes.Length; i++)
		{
			Transform transform = GameObject.Find(easeTypes[i]).transform.Find("Line");
			transform.localPosition = new Vector3(0f, 0f, 0f);
		}
	}
}
