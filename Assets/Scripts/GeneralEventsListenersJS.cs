using System;
using UnityEngine;

[Serializable]
public class GeneralEventsListenersJS : MonoBehaviour
{
	private Vector3 towardsRotation;

	private float turnForLength;

	private float turnForIter;

	private Vector3 fromColor;

	public GeneralEventsListenersJS()
	{
		turnForLength = 0.5f;
	}

	public void Awake()
	{
		LeanTween.LISTENERS_MAX = 100;
		LeanTween.EVENTS_MAX = 2;
		Color color = GetComponent<Renderer>().material.color;
		float r = color.r;
		Color color2 = GetComponent<Renderer>().material.color;
		float g = color2.g;
		Color color3 = GetComponent<Renderer>().material.color;
		fromColor = new Vector3(r, g, color3.b);
	}

	public void Start()
	{
		LeanTween.addListener(gameObject, 0, changeColor);
		LeanTween.addListener(gameObject, 1, jumpUp);
	}

	public void jumpUp(LTEvent e)
	{
		GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 300f);
	}

	public void changeColor(LTEvent e)
	{
		Transform transform = e.data as Transform;
		float num = Vector3.Distance(transform.position, this.transform.position);
		Vector3 to = new Vector3(UnityEngine.Random.Range(0f, 1f), 0f, UnityEngine.Random.Range(0f, 1f));
		LeanTween.value(gameObject, updateColor, fromColor, to, 0.8f).setLoopPingPong(1).setDelay(num * 0.05f);
	}

	public void updateColor(Vector3 v)
	{
		GetComponent<Renderer>().material.color = new Color(v.x, v.y, v.z);
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer != 2)
		{
			towardsRotation = new Vector3(0f, UnityEngine.Random.Range(-180, 180), 0f);
		}
	}

	public void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.layer != 2)
		{
			turnForIter = 0f;
			turnForLength = UnityEngine.Random.Range(0.5f, 1.5f);
		}
	}

	public void FixedUpdate()
	{
		if (!(turnForIter >= turnForLength))
		{
			GetComponent<Rigidbody>().MoveRotation(GetComponent<Rigidbody>().rotation * Quaternion.Euler(towardsRotation * Time.deltaTime));
			turnForIter += Time.deltaTime;
		}
		GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 4.5f);
	}

	public void OnMouseDown()
	{
		if (UnityEngine.Input.GetKey(KeyCode.J))
		{
			LeanTween.dispatchEvent(1);
		}
		else
		{
			LeanTween.dispatchEvent(0, transform);
		}
	}

	public void Main()
	{
	}
}
