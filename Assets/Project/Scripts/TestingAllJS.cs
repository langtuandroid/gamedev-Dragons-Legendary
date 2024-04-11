using CompilerGenerated;
using System;
using UnityEngine;
//using UnityScript.Lang;

[Serializable]
public class TestingAllJS : MonoBehaviour
{
	public AnimationCurve customAnimationCurve;

	public AnimationCurve shakeCurve;

	public Transform pt1;

	public Transform pt2;

	public Transform pt3;

	public Transform pt4;

	public Transform pt5;

	private int exampleIter;

	private __TestingAllJS_0024callable0_002433_33__[] exampleFunctions;

	private bool useEstimatedTime;

	private GameObject ltLogo;

	private GameObject cube1;

	private GameObject cube2;

	private LTDescr moveId;

	private int pingPongDescrId;

	public TestingAllJS()
	{
		exampleFunctions = new __TestingAllJS_0024callable0_002433_33__[14]
		{
			updateValue3Example,
			loopTestPingPong,
			loopTestClamp,
			moveOnACurveExample,
			punchTest,
			customTweenExample,
			moveExample,
			rotateExample,
			scaleExample,
			updateValueExample,
			alphaExample,
			moveLocalExample,
			delayedCallExample,
			rotateAroundExample
		};
		useEstimatedTime = true;
	}

	public void Awake()
	{
		LeanTween.init(400);
	}

	public void Start()
	{
		ltLogo = GameObject.Find("LeanTweenLogo");
		cycleThroughExamples();
	}

	public void OnGUI()
	{
		GUI.Label(new Rect(0.03f * (float)Screen.width, 0.03f * (float)Screen.height, 0.5f * (float)Screen.width, 0.3f * (float)Screen.height), "useEstimatedTime:" + useEstimatedTime);
	}

	public void cycleThroughExamples()
	{
		if (exampleIter == 0)
		{
			useEstimatedTime = !useEstimatedTime;
			Time.timeScale = ((!useEstimatedTime) ? 1 : 0);
		}
		exampleFunctions[exampleIter]();
        //exampleIter = ((exampleIter + 1 < Extensions.get_length((System.Array)exampleFunctions)) ? (exampleIter + 1) : 0);
        exampleIter = ((exampleIter + 1 < exampleFunctions.Length) ? (exampleIter + 1) : 0);
        LeanTween.delayedCall(1.05f, cycleThroughExamples).setUseEstimatedTime(useEstimatedTime);
	}

	public void updateValue3Example()
	{
		UnityEngine.Debug.Log("updateValue3Example");
		LeanTween.value(ltLogo, updateValue3ExampleCallback, new Vector3(0f, 270f, 0f), new Vector3(30f, 270f, 180f), 0.5f).setEase(LeanTweenType.easeInBounce).setLoopPingPong()
			.setRepeat(2)
			.setOnUpdateVector3(updateValue3ExampleUpdate)
			.setUseEstimatedTime(useEstimatedTime);
	}

	public void updateValue3ExampleUpdate(Vector3 val)
	{
		UnityEngine.Debug.Log("val:" + val);
	}

	public void updateValue3ExampleCallback(Vector3 val)
	{
		ltLogo.transform.eulerAngles = val;
	}

	public void loopTestClamp()
	{
		UnityEngine.Debug.Log("loopTestClamp");
		cube1 = GameObject.Find("Cube1");
		float z = 1f;
		Vector3 localScale = cube1.transform.localScale;
		localScale.z = z;
		Vector3 vector2 = cube1.transform.localScale = localScale;
		moveId = LeanTween.scaleZ(cube1, 4f, 1f).setEase(LeanTweenType.easeOutElastic).setLoopClamp()
			.setRepeat(7)
			.setUseEstimatedTime(useEstimatedTime);
	}

	public void loopTestPingPong()
	{
		UnityEngine.Debug.Log("loopTestPingPong");
		cube2 = GameObject.Find("Cube2");
		float y = 1f;
		Vector3 localScale = cube2.transform.localScale;
		localScale.y = y;
		Vector3 vector2 = cube2.transform.localScale = localScale;
		pingPongDescrId = LeanTween.scaleY(cube2, 4f, 1f).setEase(LeanTweenType.easeOutQuad).setLoopPingPong(4)
			.setUseEstimatedTime(useEstimatedTime)
			.id;
		}

		public void moveOnACurveExample()
		{
			UnityEngine.Debug.Log("moveOnACurveExample");
			Vector3[] to = new Vector3[8]
			{
				ltLogo.transform.position,
				pt1.position,
				pt2.position,
				pt3.position,
				pt3.position,
				pt4.position,
				pt5.position,
				ltLogo.transform.position
			};
			LeanTween.move(ltLogo, to, 1f).setEase(LeanTweenType.easeInQuad).setOrientToPath(doesOrient: true)
				.setUseEstimatedTime(useEstimatedTime);
		}

		public void punchTest()
		{
			LeanTween.moveX(ltLogo, 7f, 1f).setEase(LeanTweenType.punch).setUseEstimatedTime(useEstimatedTime);
		}

		public void customTweenExample()
		{
			UnityEngine.Debug.Log("customTweenExample");
			LeanTween.moveX(ltLogo, -10f, 0.5f).setEase(customAnimationCurve).setUseEstimatedTime(useEstimatedTime);
			LeanTween.moveX(ltLogo, 0f, 0.5f).setDelay(0.5f).setEase(customAnimationCurve)
				.setUseEstimatedTime(useEstimatedTime);
		}

		public void moveExample()
		{
			UnityEngine.Debug.Log("moveExample");
			LeanTween.move(ltLogo, new Vector3(-2f, -1f, 0f), 0.5f).setUseEstimatedTime(useEstimatedTime);
			LeanTween.move(ltLogo, ltLogo.transform.position, 0.5f).setDelay(0.5f).setUseEstimatedTime(useEstimatedTime);
		}

		public void rotateExample()
		{
			UnityEngine.Debug.Log("rotateExample");
			LeanTween.rotate(ltLogo, new Vector3(0f, 360f, 0f), 1f).setEase(LeanTweenType.easeOutQuad).setUseEstimatedTime(useEstimatedTime);
		}

		public void scaleExample()
		{
			UnityEngine.Debug.Log("scaleExample");
			Vector3 localScale = ltLogo.transform.localScale;
			LeanTween.scale(ltLogo, new Vector3(localScale.x + 0.2f, localScale.y + 0.2f, localScale.z + 0.2f), 1f).setEase(LeanTweenType.easeOutBounce).setUseEstimatedTime(useEstimatedTime);
		}

		public void updateValueExample()
		{
			UnityEngine.Debug.Log("updateValueExample");
			GameObject gameObject = ltLogo;
			Action<float> callOnUpdate = updateValueExampleCallback;
			Vector3 eulerAngles = ltLogo.transform.eulerAngles;
			LeanTween.value(gameObject, callOnUpdate, eulerAngles.y, 270f, 1f).setEase(LeanTweenType.easeOutElastic).setUseEstimatedTime(useEstimatedTime);
		}

		public void updateValueExampleCallback(float val)
		{
			Vector3 eulerAngles = ltLogo.transform.eulerAngles;
			eulerAngles.y = val;
			Vector3 vector2 = ltLogo.transform.eulerAngles = eulerAngles;
		}

		public void delayedCallExample()
		{
			UnityEngine.Debug.Log("delayedCallExample");
			LeanTween.delayedCall(0.5f, delayedCallExampleCallback).setUseEstimatedTime(useEstimatedTime);
		}

		public void delayedCallExampleCallback()
		{
			UnityEngine.Debug.Log("Delayed function was called");
			Vector3 localScale = gameObject.transform.localScale;
			LeanTween.scale(ltLogo, new Vector3(localScale.x - 0.2f, localScale.y - 0.2f, localScale.z - 0.2f), 0.5f).setEase(LeanTweenType.easeInOutCirc).setUseEstimatedTime(useEstimatedTime);
		}

		public void alphaExample()
		{
			UnityEngine.Debug.Log("alphaExample");
			GameObject gameObject = GameObject.Find("LCharacter");
			LeanTween.alpha(gameObject, 0f, 0.5f).setUseEstimatedTime(useEstimatedTime);
			LeanTween.alpha(gameObject, 1f, 0.5f).setDelay(0.5f).setUseEstimatedTime(useEstimatedTime);
		}

		public void moveLocalExample()
		{
			UnityEngine.Debug.Log("moveLocalExample");
			GameObject gameObject = GameObject.Find("LCharacter");
			Vector3 localPosition = gameObject.transform.localPosition;
			LeanTween.moveLocal(gameObject, new Vector3(0f, 2f, 0f), 0.5f).setUseEstimatedTime(useEstimatedTime);
			LeanTween.moveLocal(gameObject, localPosition, 0.5f).setDelay(0.5f).setUseEstimatedTime(useEstimatedTime);
		}

		public void rotateAroundExample()
		{
			UnityEngine.Debug.Log("rotateAroundExample");
			GameObject gameObject = GameObject.Find("LCharacter");
			LeanTween.rotateAround(gameObject, Vector3.up, 360f, 1f).setUseEstimatedTime(useEstimatedTime);
		}

		public void moveXExample()
		{
			LeanTween.moveX(ltLogo, 5f, 0.5f);
		}

		public void rotateXExample()
		{
		}

		public void scaleXExample()
		{
		}

		public void loopPause()
		{
			moveId.pause();
		}

		public void loopResume()
		{
			moveId.resume();
		}

		public void loopCancel()
		{
			LeanTween.cancel(pingPongDescrId);
		}

		public void Main()
		{
		}
	}
