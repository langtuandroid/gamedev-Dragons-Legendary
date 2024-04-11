using System;
using System.Collections.Generic;
using UnityEngine;

public class PathSplineEndlessCS : MonoBehaviour
{
	public GameObject trackTrailRenderers;

	public GameObject car;

	public GameObject carInternal;

	public GameObject[] cubes;

	private int cubesIter;

	public GameObject[] trees;

	private int treesIter;

	public float randomIterWidth = 0.1f;

	private LTSpline track;

	private List<Vector3> trackPts = new List<Vector3>();

	private int zIter;

	private float carIter;

	private float carAdd;

	private int trackMaxItems = 15;

	private int trackIter = 1;

	private float pushTrackAhead;

	private float randomIter;

	private void Start()
	{
		for (int i = 0; i < 4; i++)
		{
			addRandomTrackPoint();
		}
		refreshSpline();
		LeanTween.value(base.gameObject, 0f, 0.3f, 2f).setOnUpdate(delegate(float val)
		{
			pushTrackAhead = val;
		});
	}

	private void Update()
	{
		Vector3 vector = trackPts[trackPts.Count - 1];
		float z = vector.z;
		Vector3 position = base.transform.position;
		float num = z - position.z;
		if (num < 200f)
		{
			addRandomTrackPoint();
			refreshSpline();
		}
		track.place(car.transform, carIter);
		carIter += carAdd * Time.deltaTime;
		track.place(trackTrailRenderers.transform, carIter + pushTrackAhead);
		float axis = UnityEngine.Input.GetAxis("Horizontal");
		if (Input.anyKeyDown)
		{
			if (axis < 0f && trackIter > 0)
			{
				trackIter--;
				playSwish();
			}
			else if (axis > 0f && trackIter < 2)
			{
				trackIter++;
				playSwish();
			}
			LeanTween.moveLocalX(carInternal, (float)(trackIter - 1) * 6f, 0.3f).setEase(LeanTweenType.easeOutBack);
		}
	}

	private GameObject objectQueue(GameObject[] arr, ref int lastIter)
	{
		lastIter = ((lastIter < arr.Length - 1) ? (lastIter + 1) : 0);
		arr[lastIter].transform.localScale = Vector3.one;
		arr[lastIter].transform.rotation = Quaternion.identity;
		return arr[lastIter];
	}

	private void addRandomTrackPoint()
	{
		float num = Mathf.PerlinNoise(0f, randomIter);
		randomIter += randomIterWidth;
		Vector3 vector = new Vector3((num - 0.5f) * 20f, 0f, (float)zIter * 40f);
		GameObject gameObject = objectQueue(cubes, ref cubesIter);
		gameObject.transform.position = vector;
		GameObject gameObject2 = objectQueue(trees, ref treesIter);
		float num2 = (zIter % 2 != 0) ? 15f : (-15f);
		gameObject2.transform.position = new Vector3(vector.x + num2, 0f, (float)zIter * 40f);
		LeanTween.rotateAround(gameObject2, Vector3.forward, 0f, 1f).setFrom((zIter % 2 != 0) ? (-180f) : 180f).setEase(LeanTweenType.easeOutBack);
		trackPts.Add(vector);
		if (trackPts.Count > trackMaxItems)
		{
			trackPts.RemoveAt(0);
		}
		zIter++;
	}

	private void refreshSpline()
	{
		track = new LTSpline(trackPts.ToArray());
		carIter = track.ratioAtPoint(car.transform.position);
		carAdd = 40f / track.distance;
	}

	private void playSwish()
	{
		AnimationCurve volume = new AnimationCurve(new Keyframe(0f, 0.005464481f, 1.83897f, 0f), new Keyframe(0.1114856f, 2.281785f, 0f, 0f), new Keyframe(578f / (741f * (float)Math.PI), 2.271654f, 0f, 0f), new Keyframe(0.3f, 0.01670286f, 0f, 0f));
		AnimationCurve frequency = new AnimationCurve(new Keyframe(0f, 0.00136725f, 0f, 0f), new Keyframe(0.1482391f, 0.005405405f, 0f, 0f), new Keyframe(0.2650336f, 0.002480127f, 0f, 0f));
		AudioClip audio = LeanAudio.createAudio(volume, frequency, LeanAudio.options().setVibrato(new Vector3[1]
		{
			new Vector3(0.2f, 0.5f, 0f)
		}).setWaveNoise()
			.setWaveNoiseScale(1000f));
		LeanAudio.play(audio);
	}
}
