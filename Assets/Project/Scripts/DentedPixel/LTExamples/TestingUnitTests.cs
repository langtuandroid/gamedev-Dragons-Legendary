using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DentedPixel.LTExamples
{
	public class TestingUnitTests : MonoBehaviour
	{
		public GameObject cube1;

		public GameObject cube2;

		public GameObject cube3;

		public GameObject cube4;

		public GameObject cubeAlpha1;

		public GameObject cubeAlpha2;

		private bool eventGameObjectWasCalled;

		private bool eventGeneralWasCalled;

		private int lt1Id;

		private LTDescr lt2;

		private LTDescr lt3;

		private LTDescr lt4;

		private LTDescr[] groupTweens;

		private GameObject[] groupGOs;

		private int groupTweensCnt;

		private int rotateRepeat;

		private int rotateRepeatAngle;

		private GameObject boxNoCollider;

		private float timeElapsedNormalTimeScale;

		private float timeElapsedIgnoreTimeScale;

		private void Awake()
		{
			boxNoCollider = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(boxNoCollider.GetComponent(typeof(BoxCollider)));
		}

		private void Start()
		{
			LeanTest.timeout = 46f;
			LeanTest.expected = 57;
			LeanTween.init(1215);
			LeanTween.addListener(cube1, 0, eventGameObjectCalled);
			LeanTest.expect(!LeanTween.isTweening(), "NOTHING TWEEENING AT BEGINNING");
			LeanTest.expect(!LeanTween.isTweening(cube1), "OBJECT NOT TWEEENING AT BEGINNING");
			LeanTween.scaleX(cube4, 2f, 0f).setOnComplete((Action)delegate
			{
				Vector3 localScale3 = cube4.transform.localScale;
				LeanTest.expect(localScale3.x == 2f, "TWEENED WITH ZERO TIME");
			});
			LeanTween.dispatchEvent(0);
			LeanTest.expect(eventGameObjectWasCalled, "EVENT GAMEOBJECT RECEIVED");
			LeanTest.expect(!LeanTween.removeListener(cube2, 0, eventGameObjectCalled), "EVENT GAMEOBJECT NOT REMOVED");
			LeanTest.expect(LeanTween.removeListener(cube1, 0, eventGameObjectCalled), "EVENT GAMEOBJECT REMOVED");
			LeanTween.addListener(1, eventGeneralCalled);
			LeanTween.dispatchEvent(1);
			LeanTest.expect(eventGeneralWasCalled, "EVENT ALL RECEIVED");
			LeanTest.expect(LeanTween.removeListener(1, eventGeneralCalled), "EVENT ALL REMOVED");
			lt1Id = LeanTween.move(cube1, new Vector3(3f, 2f, 0.5f), 1.1f).id;
			LeanTween.move(cube2, new Vector3(-3f, -2f, -0.5f), 1.1f);
			LeanTween.reset();
			GameObject[] cubes = new GameObject[99];
			int[] tweenIds = new int[cubes.Length];
			for (int i = 0; i < cubes.Length; i++)
			{
				GameObject gameObject = cubeNamed("cancel" + i);
				tweenIds[i] = LeanTween.moveX(gameObject, 100f, 1f).id;
				cubes[i] = gameObject;
			}
			int onCompleteCount = 0;
			LeanTween.delayedCall(cubes[0], 0.2f, (Action)delegate
			{
				for (int l = 0; l < cubes.Length; l++)
				{
					if (l % 3 == 0)
					{
						LeanTween.cancel(cubes[l]);
					}
					else if (l % 3 == 1)
					{
						LeanTween.cancel(tweenIds[l]);
					}
					else if (l % 3 == 2)
					{
						LTDescr lTDescr3 = LeanTween.descr(tweenIds[l]);
						lTDescr3.setOnComplete((Action)delegate
						{
							onCompleteCount++;
							if (onCompleteCount >= 33)
							{
								LeanTest.expect(didPass: true, "CANCELS DO NOT EFFECT FINISHING");
							}
						});
					}
				}
			});
			Vector3[] pts = new Vector3[5]
			{
				new Vector3(-1f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(4f, 0f, 0f),
				new Vector3(20f, 0f, 0f),
				new Vector3(30f, 0f, 0f)
			};
			LTSpline lTSpline = new LTSpline(pts);
			lTSpline.place(cube4.transform, 0.5f);
			LeanTest.expect(Vector3.Distance(cube4.transform.position, new Vector3(10f, 0f, 0f)) <= 0.7f, "SPLINE POSITIONING AT HALFWAY", "position is:" + cube4.transform.position + " but should be:(10f,0f,0f)");
			LeanTween.color(cube4, Color.green, 0.01f);
			GameObject gameObject2 = cubeNamed("cubeDest");
			Vector3 cubeDestEnd = new Vector3(100f, 20f, 0f);
			LeanTween.move(gameObject2, cubeDestEnd, 0.7f);
			GameObject cubeToTrans = cubeNamed("cubeToTrans");
			LeanTween.move(cubeToTrans, gameObject2.transform, 1.2f).setEase(LeanTweenType.easeOutQuad).setOnComplete((Action)delegate
			{
				LeanTest.expect(cubeToTrans.transform.position == cubeDestEnd, "MOVE TO TRANSFORM WORKS");
			});
			GameObject gameObject3 = cubeNamed("cubeDestroy");
			LeanTween.moveX(gameObject3, 200f, 0.05f).setDelay(0.02f).setDestroyOnComplete(doesDestroy: true);
			LeanTween.moveX(gameObject3, 200f, 0.1f).setDestroyOnComplete(doesDestroy: true).setOnComplete((Action)delegate
			{
				LeanTest.expect(didPass: true, "TWO DESTROY ON COMPLETE'S SUCCEED");
			});
			GameObject cubeSpline = cubeNamed("cubeSpline");
			LeanTween.moveSpline(cubeSpline, new Vector3[4]
			{
				new Vector3(0.5f, 0f, 0.5f),
				new Vector3(0.75f, 0f, 0.75f),
				new Vector3(1f, 0f, 1f),
				new Vector3(1f, 0f, 1f)
			}, 0.1f).setOnComplete((Action)delegate
			{
				LeanTest.expect(Vector3.Distance(new Vector3(1f, 0f, 1f), cubeSpline.transform.position) < 0.01f, "SPLINE WITH TWO POINTS SUCCEEDS");
			});
			GameObject jumpCube = cubeNamed("jumpTime");
			jumpCube.transform.position = new Vector3(100f, 0f, 0f);
			jumpCube.transform.localScale *= 100f;
			int jumpTimeId = LeanTween.moveX(jumpCube, 200f, 1f).id;
			LeanTween.delayedCall(base.gameObject, 0.2f, (Action)delegate
			{
				LTDescr lTDescr2 = LeanTween.descr(jumpTimeId);
				Vector3 position6 = jumpCube.transform.position;
				float beforeX = position6.x;
				lTDescr2.setTime(0.5f);
				LeanTween.delayedCall(0f, (Action)delegate
				{
				}).setOnStart(delegate
				{
					float num = 1f;
					beforeX += Time.deltaTime * 100f * 2f;
					Vector3 position7 = jumpCube.transform.position;
					bool didPass7 = Mathf.Abs(position7.x - beforeX) < num;
					object[] obj6 = new object[8]
					{
						"Difference:",
						null,
						null,
						null,
						null,
						null,
						null,
						null
					};
					Vector3 position8 = jumpCube.transform.position;
					obj6[1] = Mathf.Abs(position8.x - beforeX);
					obj6[2] = " beforeX:";
					obj6[3] = beforeX;
					obj6[4] = " now:";
					Vector3 position9 = jumpCube.transform.position;
					obj6[5] = position9.x;
					obj6[6] = " dt:";
					obj6[7] = Time.deltaTime;
					LeanTest.expect(didPass7, "CHANGING TIME DOESN'T JUMP AHEAD", string.Concat(obj6));
				});
			});
			GameObject zeroCube = cubeNamed("zeroCube");
			LeanTween.moveX(zeroCube, 10f, 0f).setOnComplete((Action)delegate
			{
				Vector3 position4 = zeroCube.transform.position;
				bool didPass6 = position4.x == 10f;
				Vector3 position5 = zeroCube.transform.position;
				LeanTest.expect(didPass6, "ZERO TIME FINSHES CORRECTLY", "final x:" + position5.x);
			});
			GameObject cubeScale = cubeNamed("cubeScale");
			LeanTween.scale(cubeScale, new Vector3(5f, 5f, 5f), 0.01f).setOnStart(delegate
			{
				LeanTest.expect(didPass: true, "ON START WAS CALLED");
			}).setOnComplete((Action)delegate
			{
				Vector3 localScale = cubeScale.transform.localScale;
				bool didPass5 = localScale.z == 5f;
				object[] obj5 = new object[4]
				{
					"expected scale z:",
					5f,
					" returned:",
					null
				};
				Vector3 localScale2 = cubeScale.transform.localScale;
				obj5[3] = localScale2.z;
				LeanTest.expect(didPass5, "SCALE", string.Concat(obj5));
			});
			GameObject cubeRotate = cubeNamed("cubeRotate");
			LeanTween.rotate(cubeRotate, new Vector3(0f, 180f, 0f), 0.02f).setOnComplete((Action)delegate
			{
				Vector3 eulerAngles3 = cubeRotate.transform.eulerAngles;
				bool didPass4 = eulerAngles3.y == 180f;
				object[] obj4 = new object[4]
				{
					"expected rotate y:",
					180f,
					" returned:",
					null
				};
				Vector3 eulerAngles4 = cubeRotate.transform.eulerAngles;
				obj4[3] = eulerAngles4.y;
				LeanTest.expect(didPass4, "ROTATE", string.Concat(obj4));
			});
			GameObject cubeRotateA = cubeNamed("cubeRotateA");
			LeanTween.rotateAround(cubeRotateA, Vector3.forward, 90f, 0.3f).setOnComplete((Action)delegate
			{
				Vector3 eulerAngles = cubeRotateA.transform.eulerAngles;
				bool didPass3 = eulerAngles.z == 90f;
				object[] obj3 = new object[4]
				{
					"expected rotate z:",
					90f,
					" returned:",
					null
				};
				Vector3 eulerAngles2 = cubeRotateA.transform.eulerAngles;
				obj3[3] = eulerAngles2.z;
				LeanTest.expect(didPass3, "ROTATE AROUND", string.Concat(obj3));
			});
			GameObject cubeRotateB = cubeNamed("cubeRotateB");
			cubeRotateB.transform.position = new Vector3(200f, 10f, 8f);
			LeanTween.rotateAround(cubeRotateB, Vector3.forward, 360f, 0.3f).setPoint(new Vector3(5f, 3f, 2f)).setOnComplete((Action)delegate
			{
				LeanTest.expect(cubeRotateB.transform.position.ToString() == new Vector3(200f, 10f, 8f).ToString(), "ROTATE AROUND 360", "expected rotate pos:" + new Vector3(200f, 10f, 8f) + " returned:" + cubeRotateB.transform.position);
			});
			LeanTween.alpha(cubeAlpha1, 0.5f, 0.1f).setOnUpdate(delegate(float val)
			{
				LeanTest.expect(val != 0f, "ON UPDATE VAL");
			}).setOnCompleteParam("Hi!")
				.setOnComplete(delegate(object completeObj)
				{
					LeanTest.expect((string)completeObj == "Hi!", "ONCOMPLETE OBJECT");
					Color color = cubeAlpha1.GetComponent<Renderer>().material.color;
					LeanTest.expect(color.a == 0.5f, "ALPHA");
				});
			float onStartTime = -1f;
			LeanTween.color(cubeAlpha2, Color.cyan, 0.3f).setOnComplete((Action)delegate
			{
				LeanTest.expect(cubeAlpha2.GetComponent<Renderer>().material.color == Color.cyan, "COLOR");
				LeanTest.expect(onStartTime >= 0f && onStartTime < Time.time, "ON START", "onStartTime:" + onStartTime + " time:" + Time.time);
			}).setOnStart(delegate
			{
				onStartTime = Time.time;
			});
			Vector3 beforePos3 = cubeAlpha1.transform.position;
			LeanTween.moveY(cubeAlpha1, 3f, 0.2f).setOnComplete((Action)delegate
			{
				Vector3 position2 = cubeAlpha1.transform.position;
				int didPass2;
				if (position2.x == beforePos3.x)
				{
					Vector3 position3 = cubeAlpha1.transform.position;
					didPass2 = ((position3.z == beforePos3.z) ? 1 : 0);
				}
				else
				{
					didPass2 = 0;
				}
				LeanTest.expect((byte)didPass2 != 0, "MOVE Y");
			});
			Vector3 beforePos2 = cubeAlpha2.transform.localPosition;
			LeanTween.moveLocalZ(cubeAlpha2, 12f, 0.2f).setOnComplete((Action)delegate
			{
				Vector3 localPosition = cubeAlpha2.transform.localPosition;
				int didPass;
				if (localPosition.x == beforePos2.x)
				{
					Vector3 localPosition2 = cubeAlpha2.transform.localPosition;
					didPass = ((localPosition2.y == beforePos2.y) ? 1 : 0);
				}
				else
				{
					didPass = 0;
				}
				object[] obj2 = new object[8]
				{
					"ax:",
					null,
					null,
					null,
					null,
					null,
					null,
					null
				};
				Vector3 localPosition3 = cubeAlpha2.transform.localPosition;
				obj2[1] = localPosition3.x;
				obj2[2] = " bx:";
				obj2[3] = beforePos3.x;
				obj2[4] = " ay:";
				Vector3 localPosition4 = cubeAlpha2.transform.localPosition;
				obj2[5] = localPosition4.y;
				obj2[6] = " by:";
				obj2[7] = beforePos2.y;
				LeanTest.expect((byte)didPass != 0, "MOVE LOCAL Z", string.Concat(obj2));
			});
			AudioClip audio = LeanAudio.createAudio(new AnimationCurve(new Keyframe(0f, 1f, 0f, -1f), new Keyframe(1f, 0f, -1f, 0f)), new AnimationCurve(new Keyframe(0f, 0.001f, 0f, 0f), new Keyframe(1f, 0.001f, 0f, 0f)), LeanAudio.options());
			LeanTween.delayedSound(base.gameObject, audio, new Vector3(0f, 0f, 0f), 0.1f).setDelay(0.2f).setOnComplete((Action)delegate
			{
				LeanTest.expect(Time.time > 0f, "DELAYED SOUND");
			});
			int totalEasingCheck2 = 0;
			int totalEasingCheckSuccess2 = 0;
			int totalEasingCheck;
			int totalEasingCheckSuccess;
			for (int j = 0; j < 2; j++)
			{
				bool flag = j == 1;
				int totalTweenTypeLength = 33;
				for (int k = 0; k < totalTweenTypeLength; k++)
				{
					LeanTweenType leanTweenType = (LeanTweenType)k;
					GameObject gameObject4 = cubeNamed("cube" + leanTweenType);
					LTDescr lTDescr = LeanTween.moveLocalX(gameObject4, 5f, 0.1f).setOnComplete(delegate(object obj)
					{
						GameObject gameObject5 = obj as GameObject;
						totalEasingCheck = totalEasingCheck2 + 1;
						Vector3 position = gameObject5.transform.position;
						if (position.x == 5f)
						{
							totalEasingCheckSuccess = totalEasingCheckSuccess2 + 1;
						}
						if (totalEasingCheck2 == 2 * totalTweenTypeLength)
						{
							LeanTest.expect(totalEasingCheck2 == totalEasingCheckSuccess2, "EASING TYPES");
						}
					}).setOnCompleteParam(gameObject4);
					if (flag)
					{
						lTDescr.setFrom(-5f);
					}
				}
			}
			bool value2UpdateCalled = false;
			LeanTween.value(base.gameObject, new Vector2(0f, 0f), new Vector2(256f, 96f), 0.1f).setOnUpdate((Action<Vector2>)delegate
			{
				value2UpdateCalled = true;
			}, (object)null);
			LeanTween.delayedCall(0.2f, (Action)delegate
			{
				LeanTest.expect(value2UpdateCalled, "VALUE2 UPDATE");
			});
			StartCoroutine(timeBasedTesting());
		}

		private GameObject cubeNamed(string name)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(boxNoCollider);
			gameObject.name = name;
			return gameObject;
		}

		private IEnumerator timeBasedTesting()
		{
			yield return new WaitForEndOfFrame();
			GameObject cubeNormal = cubeNamed("normalTimeScale");
			LeanTween.moveX(cubeNormal, 12f, 1.5f).setIgnoreTimeScale(useUnScaledTime: false).setOnComplete((Action)delegate
			{
				timeElapsedNormalTimeScale = Time.time;
			});
			LTDescr[] descr = LeanTween.descriptions(cubeNormal);
			int didPass;
			if (descr.Length >= 0)
			{
				Vector3 to = descr[0].to;
				didPass = ((to.x == 12f) ? 1 : 0);
			}
			else
			{
				didPass = 0;
			}
			LeanTest.expect((byte)didPass != 0, "WE CAN RETRIEVE A DESCRIPTION");
			GameObject cubeIgnore = cubeNamed("ignoreTimeScale");
			LeanTween.moveX(cubeIgnore, 5f, 1.5f).setIgnoreTimeScale(useUnScaledTime: true).setOnComplete((Action)delegate
			{
				timeElapsedIgnoreTimeScale = Time.time;
			});
			yield return new WaitForSeconds(1.5f);
			LeanTest.expect(Mathf.Abs(timeElapsedNormalTimeScale - timeElapsedIgnoreTimeScale) < 0.7f, "START IGNORE TIMING", "timeElapsedIgnoreTimeScale:" + timeElapsedIgnoreTimeScale + " timeElapsedNormalTimeScale:" + timeElapsedNormalTimeScale);
			Time.timeScale = 4f;
			int pauseCount = 0;
			LeanTween.value(base.gameObject, 0f, 1f, 1f).setOnUpdate((Action<float>)delegate
			{
				pauseCount++;
			}).pause();
			Vector3[] roundCirc = new Vector3[16]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(-9.1f, 25.1f, 0f),
				new Vector3(-1.2f, 15.9f, 0f),
				new Vector3(-25f, 25f, 0f),
				new Vector3(-25f, 25f, 0f),
				new Vector3(-50.1f, 15.9f, 0f),
				new Vector3(-40.9f, 25.1f, 0f),
				new Vector3(-50f, 0f, 0f),
				new Vector3(-50f, 0f, 0f),
				new Vector3(-40.9f, -25.1f, 0f),
				new Vector3(-50.1f, -15.9f, 0f),
				new Vector3(-25f, -25f, 0f),
				new Vector3(-25f, -25f, 0f),
				new Vector3(0f, -15.9f, 0f),
				new Vector3(-9.1f, -25.1f, 0f),
				new Vector3(0f, 0f, 0f)
			};
			GameObject cubeRound = cubeNamed("bRound");
			Vector3 onStartPos = cubeRound.transform.position;
			LeanTween.moveLocal(cubeRound, roundCirc, 0.5f).setOnComplete((Action)delegate
			{
				LeanTest.expect(cubeRound.transform.position == onStartPos, "BEZIER CLOSED LOOP SHOULD END AT START", "onStartPos:" + onStartPos + " onEnd:" + cubeRound.transform.position);
			});
			Vector3[] roundSpline = new Vector3[6]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(2f, 0f, 0f),
				new Vector3(0.9f, 2f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			};
			GameObject cubeSpline = cubeNamed("bSpline");
			Vector3 onStartPosSpline = cubeSpline.transform.position;
			LeanTween.moveSplineLocal(cubeSpline, roundSpline, 0.5f).setOnComplete((Action)delegate
			{
				LeanTest.expect(Vector3.Distance(onStartPosSpline, cubeSpline.transform.position) <= 0.01f, "SPLINE CLOSED LOOP SHOULD END AT START", "onStartPos:" + onStartPosSpline + " onEnd:" + cubeSpline.transform.position + " dist:" + Vector3.Distance(onStartPosSpline, cubeSpline.transform.position));
			});
			groupTweens = new LTDescr[1200];
			groupGOs = new GameObject[groupTweens.Length];
			groupTweensCnt = 0;
			int descriptionMatchCount = 0;
			for (int i = 0; i < groupTweens.Length; i++)
			{
				GameObject gameObject = cubeNamed("c" + i);
				gameObject.transform.position = new Vector3(0f, 0f, i * 3);
				groupGOs[i] = gameObject;
			}
			yield return new WaitForEndOfFrame();
			bool hasGroupTweensCheckStarted = false;
			int setOnStartNum = 0;
			int setPosNum = 0;
			bool setPosOnUpdate = true;
			for (int j = 0; j < groupTweens.Length; j++)
			{
				Vector3 vector = base.transform.position + Vector3.one * 3f;
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("final", vector);
				dictionary.Add("go", groupGOs[j]);
				Dictionary<string, object> onCompleteParam = dictionary;
				groupTweens[j] = LeanTween.move(groupGOs[j], vector, 3f).setOnStart(delegate
				{
					setOnStartNum++;
				}).setOnUpdate(delegate(Vector3 newPosition)
				{
					Vector3 position6 = transform.position;
					if (position6.z > newPosition.z)
					{
						setPosOnUpdate = false;
					}
				})
					.setOnCompleteParam(onCompleteParam)
					.setOnComplete(delegate(object param)
					{
						Dictionary<string, object> dictionary2 = param as Dictionary<string, object>;
						Vector3 vector2 = (Vector3)dictionary2["final"];
						GameObject gameObject3 = dictionary2["go"] as GameObject;
						if (vector2.ToString() == gameObject3.transform.position.ToString())
						{
							setPosNum++;
						}
						if (!hasGroupTweensCheckStarted)
						{
							hasGroupTweensCheckStarted = true;
							LeanTween.delayedCall(this.gameObject, 0.1f, (Action)delegate
							{
								LeanTest.expect(setOnStartNum == groupTweens.Length, "SETONSTART CALLS", "expected:" + groupTweens.Length + " was:" + setOnStartNum);
								LeanTest.expect(groupTweensCnt == groupTweens.Length, "GROUP FINISH", "expected " + groupTweens.Length + " tweens but got " + groupTweensCnt);
								LeanTest.expect(setPosNum == groupTweens.Length, "GROUP POSITION FINISH", "expected " + groupTweens.Length + " tweens but got " + setPosNum);
								LeanTest.expect(setPosOnUpdate, "GROUP POSITION ON UPDATE");
							});
						}
						groupTweensCnt++;
					});
				if (LeanTween.description(groupTweens[j].id).trans == groupTweens[j].trans)
				{
					descriptionMatchCount++;
				}
			}
			while (LeanTween.tweensRunning < groupTweens.Length)
			{
				yield return null;
			}
			LeanTest.expect(descriptionMatchCount == groupTweens.Length, "GROUP IDS MATCH");
			int expectedSearch = groupTweens.Length + 5;
			LeanTest.expect(LeanTween.maxSearch <= expectedSearch, "MAX SEARCH OPTIMIZED", "maxSearch:" + LeanTween.maxSearch + " should be:" + expectedSearch);
			LeanTest.expect(LeanTween.isTweening(), "SOMETHING IS TWEENING");
			Vector3 position = cube4.transform.position;
			float previousXlt4 = position.x;
			lt4 = LeanTween.moveX(cube4, 5f, 1.1f).setOnComplete((Action)delegate
			{
				int didPass3;
				if (cube4 != null)
				{
					float num2 = previousXlt4;
					Vector3 position4 = cube4.transform.position;
					didPass3 = ((num2 != position4.x) ? 1 : 0);
				}
				else
				{
					didPass3 = 0;
				}
				object[] obj = new object[6]
				{
					"cube4:",
					cube4,
					" previousXlt4:",
					previousXlt4,
					" cube4.transform.position.x:",
					null
				};
				float num3;
				if (cube4 != null)
				{
					Vector3 position5 = cube4.transform.position;
					num3 = position5.x;
				}
				else
				{
					num3 = 0f;
				}
				obj[5] = num3;
				LeanTest.expect((byte)didPass3 != 0, "RESUME OUT OF ORDER", string.Concat(obj));
			}).setDestroyOnComplete(doesDestroy: true);
			lt4.resume();
			rotateRepeat = (rotateRepeatAngle = 0);
			LeanTween.rotateAround(cube3, Vector3.forward, 360f, 0.1f).setRepeat(3).setOnComplete(rotateRepeatFinished)
				.setOnCompleteOnRepeat(isOn: true)
				.setDestroyOnComplete(doesDestroy: true);
			yield return new WaitForEndOfFrame();
			LeanTween.delayedCall(1.8f, rotateRepeatAllFinished);
			int countBeforeCancel = LeanTween.tweensRunning;
			LeanTween.cancel(lt1Id);
			LeanTest.expect(countBeforeCancel == LeanTween.tweensRunning, "CANCEL AFTER RESET SHOULD FAIL", "expected " + countBeforeCancel + " but got " + LeanTween.tweensRunning);
			LeanTween.cancel(cube2);
			int tweenCount2 = 0;
			for (int k = 0; k < groupTweens.Length; k++)
			{
				if (LeanTween.isTweening(groupGOs[k]))
				{
					tweenCount2++;
				}
				if (k % 3 == 0)
				{
					LeanTween.pause(groupGOs[k]);
				}
				else if (k % 3 == 1)
				{
					groupTweens[k].pause();
				}
				else
				{
					LeanTween.pause(groupTweens[k].id);
				}
			}
			LeanTest.expect(tweenCount2 == groupTweens.Length, "GROUP ISTWEENING", "expected " + groupTweens.Length + " tweens but got " + tweenCount2);
			yield return new WaitForEndOfFrame();
			tweenCount2 = 0;
			for (int l = 0; l < groupTweens.Length; l++)
			{
				if (l % 3 == 0)
				{
					LeanTween.resume(groupGOs[l]);
				}
				else if (l % 3 == 1)
				{
					groupTweens[l].resume();
				}
				else
				{
					LeanTween.resume(groupTweens[l].id);
				}
				if ((l % 2 != 0) ? LeanTween.isTweening(groupGOs[l]) : LeanTween.isTweening(groupTweens[l].id))
				{
					tweenCount2++;
				}
			}
			LeanTest.expect(tweenCount2 == groupTweens.Length, "GROUP RESUME");
			LeanTest.expect(!LeanTween.isTweening(cube1), "CANCEL TWEEN LTDESCR");
			LeanTest.expect(!LeanTween.isTweening(cube2), "CANCEL TWEEN LEANTWEEN");
			LeanTest.expect(pauseCount == 0, "ON UPDATE NOT CALLED DURING PAUSE", "expect pause count of 0, but got " + pauseCount);
			yield return new WaitForEndOfFrame();
			Time.timeScale = 0.25f;
			float tweenTime = 0.2f;
			float expectedTime = tweenTime * (1f / Time.timeScale);
			float start = Time.realtimeSinceStartup;
			bool onUpdateWasCalled = false;
			LeanTween.moveX(cube1, -5f, tweenTime).setOnUpdate((Action<float>)delegate
			{
				onUpdateWasCalled = true;
			}).setOnComplete((Action)delegate
			{
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				float num = realtimeSinceStartup - start;
				LeanTest.expect(Mathf.Abs(expectedTime - num) < 0.05f, "SCALED TIMING DIFFERENCE", "expected to complete in roughly " + expectedTime + " but completed in " + num);
				Vector3 position2 = cube1.transform.position;
				bool didPass2 = Mathf.Approximately(position2.x, -5f);
				Vector3 position3 = cube1.transform.position;
				LeanTest.expect(didPass2, "SCALED ENDING POSITION", "expected to end at -5f, but it ended at " + position3.x);
				LeanTest.expect(onUpdateWasCalled, "ON UPDATE FIRED");
			});
			bool didGetCorrectOnUpdate = false;
			LeanTween.value(base.gameObject, new Vector3(1f, 1f, 1f), new Vector3(10f, 10f, 10f), 1f).setOnUpdate(delegate(Vector3 val)
			{
				didGetCorrectOnUpdate = (val.x >= 1f && val.y >= 1f && val.z >= 1f);
			}).setOnComplete((Action)delegate
			{
				LeanTest.expect(didGetCorrectOnUpdate, "VECTOR3 CALLBACK CALLED");
			});
			yield return new WaitForSeconds(expectedTime);
			Time.timeScale = 1f;
			int ltCount = 0;
			GameObject[] allGos = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
			GameObject[] array = allGos;
			foreach (GameObject gameObject2 in array)
			{
				if (gameObject2.name == "~LeanTween")
				{
					ltCount++;
				}
			}
			LeanTest.expect(ltCount == 1, "RESET CORRECTLY CLEANS UP");
			lotsOfCancels();
		}

		private IEnumerator lotsOfCancels()
		{
			yield return new WaitForEndOfFrame();
			Time.timeScale = 4f;
			int cubeCount = 10;
			int[] tweensA = new int[cubeCount];
			GameObject[] aGOs = new GameObject[cubeCount];
			for (int i = 0; i < aGOs.Length; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(boxNoCollider);
				gameObject.transform.position = new Vector3(0f, 0f, (float)i * 2f);
				gameObject.name = "a" + i;
				aGOs[i] = gameObject;
				tweensA[i] = LeanTween.move(gameObject, gameObject.transform.position + new Vector3(10f, 0f, 0f), 0.5f + 1f * (1f / (float)aGOs.Length)).id;
				LeanTween.color(gameObject, Color.red, 0.01f);
			}
			yield return new WaitForSeconds(1f);
			int[] tweensB = new int[cubeCount];
			GameObject[] bGOs = new GameObject[cubeCount];
			for (int j = 0; j < bGOs.Length; j++)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(boxNoCollider);
				gameObject2.transform.position = new Vector3(0f, 0f, (float)j * 2f);
				gameObject2.name = "b" + j;
				bGOs[j] = gameObject2;
				tweensB[j] = LeanTween.move(gameObject2, gameObject2.transform.position + new Vector3(10f, 0f, 0f), 2f).id;
			}
			for (int k = 0; k < aGOs.Length; k++)
			{
				LeanTween.cancel(aGOs[k]);
				GameObject gameObject3 = aGOs[k];
				tweensA[k] = LeanTween.move(gameObject3, new Vector3(0f, 0f, (float)k * 2f), 2f).id;
			}
			yield return new WaitForSeconds(0.5f);
			for (int l = 0; l < aGOs.Length; l++)
			{
				LeanTween.cancel(aGOs[l]);
				GameObject gameObject4 = aGOs[l];
				tweensA[l] = LeanTween.move(gameObject4, new Vector3(0f, 0f, (float)l * 2f) + new Vector3(10f, 0f, 0f), 2f).id;
			}
			for (int m = 0; m < bGOs.Length; m++)
			{
				LeanTween.cancel(bGOs[m]);
				GameObject gameObject5 = bGOs[m];
				tweensB[m] = LeanTween.move(gameObject5, new Vector3(0f, 0f, (float)m * 2f), 2f).id;
			}
			yield return new WaitForSeconds(2.1f);
			bool inFinalPlace = true;
			for (int n = 0; n < aGOs.Length; n++)
			{
				if (Vector3.Distance(aGOs[n].transform.position, new Vector3(0f, 0f, (float)n * 2f) + new Vector3(10f, 0f, 0f)) > 0.1f)
				{
					inFinalPlace = false;
				}
			}
			for (int num = 0; num < bGOs.Length; num++)
			{
				if (Vector3.Distance(bGOs[num].transform.position, new Vector3(0f, 0f, (float)num * 2f)) > 0.1f)
				{
					inFinalPlace = false;
				}
			}
			LeanTest.expect(inFinalPlace, "AFTER LOTS OF CANCELS");
		}

		private void rotateRepeatFinished()
		{
			Vector3 eulerAngles = cube3.transform.eulerAngles;
			if (Mathf.Abs(eulerAngles.z) < 0.0001f)
			{
				rotateRepeatAngle++;
			}
			rotateRepeat++;
		}

		private void rotateRepeatAllFinished()
		{
			LeanTest.expect(rotateRepeatAngle == 3, "ROTATE AROUND MULTIPLE", "expected 3 times received " + rotateRepeatAngle + " times");
			LeanTest.expect(rotateRepeat == 3, "ROTATE REPEAT", "expected 3 times received " + rotateRepeat + " times");
			LeanTest.expect(cube3 == null, "DESTROY ON COMPLETE", "cube3:" + cube3);
		}

		private void eventGameObjectCalled(LTEvent e)
		{
			eventGameObjectWasCalled = true;
		}

		private void eventGeneralCalled(LTEvent e)
		{
			eventGeneralWasCalled = true;
		}
	}
}
