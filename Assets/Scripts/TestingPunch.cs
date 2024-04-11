using System;
using UnityEngine;

public class TestingPunch : MonoBehaviour
{
	public AnimationCurve exportCurve;

	public float overShootValue = 1f;

	private LTDescr descr;

	private void Start()
	{
		UnityEngine.Debug.Log("exported curve:" + curveToString(exportCurve));
	}

	private void Update()
	{
		LeanTween.dtManual = Time.deltaTime;
		if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
		{
			LeanTween.moveLocalX(base.gameObject, 5f, 1f).setOnComplete((Action)delegate
			{
				UnityEngine.Debug.Log("on complete move local X");
			}).setOnCompleteOnStart(isOn: true);
			GameObject gameObject = GameObject.Find("DirectionalLight");
			Light lt = gameObject.GetComponent<Light>();
			LeanTween.value(lt.gameObject, lt.intensity, 0f, 1.5f).setEase(LeanTweenType.linear).setLoopPingPong()
				.setRepeat(-1)
				.setOnUpdate(delegate(float val)
				{
					lt.intensity = val;
				});
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.S))
		{
			MonoBehaviour.print("scale punch!");
			tweenStatically(base.gameObject);
			LeanTween.scale(base.gameObject, new Vector3(1.15f, 1.15f, 1.15f), 0.6f);
			LeanTween.rotateAround(base.gameObject, Vector3.forward, -360f, 0.3f).setOnComplete((Action)delegate
			{
				LeanTween.rotateAround(base.gameObject, Vector3.forward, -360f, 0.4f).setOnComplete((Action)delegate
				{
					LeanTween.scale(base.gameObject, new Vector3(1f, 1f, 1f), 0.1f);
					LeanTween.value(base.gameObject, (Action<float>)delegate
					{
					}, 0f, 1f, 0.3f).setDelay(1f);
				});
			});
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.T))
		{
			Vector3[] to = new Vector3[4]
			{
				new Vector3(-1f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(4f, 0f, 0f),
				new Vector3(20f, 0f, 0f)
			};
			descr = LeanTween.move(base.gameObject, to, 15f).setOrientToPath(doesOrient: true).setDirection(1f)
				.setOnComplete((Action)delegate
				{
					UnityEngine.Debug.Log("move path finished");
				});
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Y))
		{
			descr.setDirection(0f - descr.direction);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.R))
		{
			LeanTween.rotateAroundLocal(base.gameObject, base.transform.forward, -80f, 5f).setPoint(new Vector3(1.25f, 0f, 0f));
			MonoBehaviour.print("rotate punch!");
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.M))
		{
			MonoBehaviour.print("move punch!");
			Time.timeScale = 0.25f;
			float start = Time.realtimeSinceStartup;
			LeanTween.moveX(base.gameObject, 1f, 1f).setOnComplete(destroyOnComp).setOnCompleteParam(base.gameObject)
				.setOnComplete((Action)delegate
				{
					float realtimeSinceStartup = Time.realtimeSinceStartup;
					float num = realtimeSinceStartup - start;
					object[] obj2 = new object[8]
					{
						"start:",
						start,
						" end:",
						realtimeSinceStartup,
						" diff:",
						num,
						" x:",
						null
					};
					Vector3 position10 = base.gameObject.transform.position;
					obj2[7] = position10.x;
					UnityEngine.Debug.Log(string.Concat(obj2));
				})
				.setEase(LeanTweenType.easeInBack)
				.setOvershoot(overShootValue)
				.setPeriod(0.3f);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.C))
		{
			LeanTween.color(base.gameObject, new Color(1f, 0f, 0f, 0.5f), 1f);
			Color to2 = new Color(UnityEngine.Random.Range(0f, 1f), 0f, UnityEngine.Random.Range(0f, 1f), 0f);
			GameObject gameObject2 = GameObject.Find("LCharacter");
			LeanTween.color(gameObject2, to2, 4f).setLoopPingPong(1).setEase(LeanTweenType.easeOutBounce);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.E))
		{
			LeanTween.delayedCall(base.gameObject, 0.3f, delayedMethod).setRepeat(4).setOnCompleteOnRepeat(isOn: true)
				.setOnCompleteParam("hi");
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.V))
		{
			LeanTween.value(base.gameObject, updateColor, new Color(1f, 0f, 0f, 1f), Color.blue, 4f);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.P))
		{
			LeanTween.delayedCall(0.05f, enterMiniGameStart).setOnCompleteParam(new object[1]
			{
				string.Empty + 5
			});
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.U))
		{
			LeanTween.value(base.gameObject, delegate(Vector2 val)
			{
				Transform transform2 = base.transform;
				float x4 = val.x;
				Vector3 position8 = base.transform.position;
				float y2 = position8.y;
				Vector3 position9 = base.transform.position;
				transform2.position = new Vector3(x4, y2, position9.z);
			}, new Vector2(0f, 0f), new Vector2(5f, 100f), 1f).setEase(LeanTweenType.easeOutBounce);
			GameObject i = GameObject.Find("LCharacter");
			object[] obj = new object[4]
			{
				"x:",
				null,
				null,
				null
			};
			Vector3 position = i.transform.position;
			obj[1] = position.x;
			obj[2] = " y:";
			Vector3 position2 = i.transform.position;
			obj[3] = position2.y;
			UnityEngine.Debug.Log(string.Concat(obj));
			GameObject gameObject3 = i;
			Vector3 position3 = i.transform.position;
			float x = position3.x;
			Vector3 position4 = i.transform.position;
			Vector2 from = new Vector2(x, position4.y);
			Vector3 position5 = i.transform.position;
			float x2 = position5.x;
			Vector3 position6 = i.transform.position;
			LeanTween.value(gameObject3, from, new Vector2(x2, position6.y + 5f), 1f).setOnUpdate(delegate(Vector2 val)
			{
				UnityEngine.Debug.Log("tweening vec2 val:" + val);
				Transform transform = i.transform;
				float x3 = val.x;
				float y = val.y;
				Vector3 position7 = base.transform.position;
				transform.position = new Vector3(x3, y, position7.z);
			});
		}
	}

	private static void tweenStatically(GameObject gameObject)
	{
		UnityEngine.Debug.Log("Starting to tween...");
		LeanTween.value(gameObject, delegate(float val)
		{
			UnityEngine.Debug.Log("tweening val:" + val);
		}, 0f, 1f, 1f);
	}

	private void enterMiniGameStart(object val)
	{
		object[] array = (object[])val;
		int num = int.Parse((string)array[0]);
		UnityEngine.Debug.Log("level:" + num);
	}

	private void updateColor(Color c)
	{
		GameObject gameObject = GameObject.Find("LCharacter");
		gameObject.GetComponent<Renderer>().material.color = c;
	}

	private void delayedMethod(object myVal)
	{
		string text = myVal as string;
		UnityEngine.Debug.Log("delayed call:" + Time.time + " myVal:" + text);
	}

	private void destroyOnComp(object p)
	{
		GameObject obj = (GameObject)p;
		UnityEngine.Object.Destroy(obj);
	}

	private string curveToString(AnimationCurve curve)
	{
		string text = string.Empty;
		for (int i = 0; i < curve.length; i++)
		{
			string text2 = text;
			text = text2 + "new Keyframe(" + curve[i].time + "f, " + curve[i].value + "f)";
			if (i < curve.length - 1)
			{
				text += ", ";
			}
		}
		return "new AnimationCurve( " + text + " )";
	}
}
