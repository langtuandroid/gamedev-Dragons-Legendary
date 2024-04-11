using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Curve Effect", 6)]
[RequireComponent(typeof(Text))]
public class CurveEffect : BaseMeshEffect
{
	public enum CurveMode
	{
		TextArea,
		FullRect
	}

	[SerializeField]
	private CurveMode m_CurveMode;

	[SerializeField]
	private AnimationCurve m_Curve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 2f), new Keyframe(1f, 0f, -2f, 0f));

	[SerializeField]
	private float m_Strength = 1f;

	public AnimationCurve curve
	{
		get
		{
			return m_Curve;
		}
		set
		{
			if (m_Curve != value)
			{
				m_Curve = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}
	}

	public float strength
	{
		get
		{
			return m_Strength;
		}
		set
		{
			if (m_Strength != value)
			{
				m_Strength = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}
	}

	protected CurveEffect()
	{
	}

	public override void ModifyMesh(VertexHelper vh)
	{
		if (!IsActive())
		{
			return;
		}
		List<UIVertex> list = new List<UIVertex>();
		vh.GetUIVertexStream(list);
		if (list.Count == 0)
		{
			return;
		}
		Vector2 vector = Vector2.zero;
		Vector2 vector2 = Vector2.zero;
		if (m_CurveMode == CurveMode.FullRect)
		{
			Rect rect = GetComponent<RectTransform>().rect;
			vector = new Vector2(rect.xMin, rect.yMax);
			vector2 = new Vector2(rect.xMax, rect.yMin);
		}
		else
		{
			UIVertex uIVertex = list[0];
			vector = uIVertex.position;
			UIVertex uIVertex2 = list[list.Count - 1];
			vector2 = uIVertex2.position;
			for (int i = 0; i < list.Count; i++)
			{
				UIVertex uIVertex3 = list[i];
				if (uIVertex3.position.x < vector.x)
				{
					UIVertex uIVertex4 = list[i];
					vector.x = uIVertex4.position.x;
				}
				UIVertex uIVertex5 = list[i];
				if (uIVertex5.position.y > vector.y)
				{
					UIVertex uIVertex6 = list[i];
					vector.y = uIVertex6.position.y;
				}
				UIVertex uIVertex7 = list[i];
				if (uIVertex7.position.x > vector2.x)
				{
					UIVertex uIVertex8 = list[i];
					vector2.x = uIVertex8.position.x;
				}
				UIVertex uIVertex9 = list[i];
				if (uIVertex9.position.y < vector2.y)
				{
					UIVertex uIVertex10 = list[i];
					vector2.y = uIVertex10.position.y;
				}
			}
		}
		float num = vector2.x - vector.x;
		for (int j = 0; j < list.Count; j++)
		{
			UIVertex value = list[j];
			value.position.y += curve.Evaluate((value.position.x - vector.x) / num) * strength;
			list[j] = value;
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(list);
	}
}
