using UnityEngine;

public class GeneralSequencer : MonoBehaviour
{
	public GameObject avatar1;

	public GameObject star;

	public GameObject dustCloudPrefab;

	public float speedScale = 1f;

	public void Start()
	{
		LTSeq lTSeq = LeanTween.sequence();
		LTSeq lTSeq2 = lTSeq;
		GameObject gameObject = avatar1;
		Vector3 localPosition = avatar1.transform.localPosition;
		lTSeq2.append(LeanTween.moveY(gameObject, localPosition.y + 6f, 1f).setEaseOutQuad());
		lTSeq.insert(LeanTween.alpha(star, 0f, 1f));
		lTSeq.insert(LeanTween.scale(star, Vector3.one * 3f, 1f));
		lTSeq.append(LeanTween.rotateAround(avatar1, Vector3.forward, 360f, 0.6f).setEaseInBack());
		LTSeq lTSeq3 = lTSeq;
		GameObject gameObject2 = avatar1;
		Vector3 localPosition2 = avatar1.transform.localPosition;
		lTSeq3.append(LeanTween.moveY(gameObject2, localPosition2.y, 1f).setEaseInQuad());
		lTSeq.append(delegate
		{
			Vector3 to = default(Vector3);
			for (int i = 0; (float)i < 50f; i++)
			{
				GameObject gameObject3 = UnityEngine.Object.Instantiate(dustCloudPrefab);
				gameObject3.transform.parent = avatar1.transform;
				gameObject3.transform.localPosition = new Vector3(UnityEngine.Random.Range(-2f, 2f), 0f, 0f);
				gameObject3.transform.eulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 360f));
				Vector3 localPosition3 = gameObject3.transform.localPosition;
				to = new Vector3(localPosition3.x, UnityEngine.Random.Range(2f, 4f), UnityEngine.Random.Range(-10f, 10f));
				LeanTween.moveLocal(gameObject3, to, 3f * speedScale).setEaseOutCirc();
				LeanTween.rotateAround(gameObject3, Vector3.forward, 720f, 3f * speedScale).setEaseOutCirc();
				LeanTween.alpha(gameObject3, 0f, 3f * speedScale).setEaseOutCirc().setDestroyOnComplete(doesDestroy: true);
			}
		});
		lTSeq.setScale(speedScale);
	}
}
