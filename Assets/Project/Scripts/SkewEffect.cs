using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Skew Effect", 5)]
[RequireComponent(typeof(Text))]
public class SkewEffect : BaseMeshEffect
{
	public enum SkewMode
	{
		TextArea,
		FullRect
	}

	[SerializeField]
	private SkewMode m_SkewMode;

	[SerializeField]
	private Vector2 m_UpperLeftOffset = Vector2.zero;

	[SerializeField]
	private Vector2 m_UpperRightOffset = Vector2.zero;

	[SerializeField]
	private Vector2 m_LowerLeftOffset = Vector2.zero;

	[SerializeField]
	private Vector2 m_LowerRightOffset = Vector2.zero;

	public Vector2 upperLeftOffset
	{
		get
		{
			return m_UpperLeftOffset;
		}
		set
		{
			m_UpperLeftOffset = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public Vector2 upperRightOffset
	{
		get
		{
			return m_UpperRightOffset;
		}
		set
		{
			m_UpperRightOffset = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public Vector2 lowerLeftOffset
	{
		get
		{
			return m_LowerLeftOffset;
		}
		set
		{
			m_LowerLeftOffset = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public Vector2 lowerRightOffset
	{
		get
		{
			return m_LowerRightOffset;
		}
		set
		{
			m_LowerRightOffset = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	protected SkewEffect()
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
		if (m_SkewMode == SkewMode.FullRect)
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
		float num = vector.y - vector2.y;
		float num2 = vector2.x - vector.x;
		for (int j = 0; j < list.Count; j++)
		{
			UIVertex value = list[j];
			float num3 = (value.position.y - vector2.y) / num;
			float num4 = 1f - num3;
			float num5 = (vector2.x - value.position.x) / num2;
			float num6 = 1f - num5;
			Vector3 zero = Vector3.zero;
			Vector2 upperLeftOffset = this.upperLeftOffset;
			float num7 = upperLeftOffset.y * num3;
			Vector2 lowerLeftOffset = this.lowerLeftOffset;
			float num8 = (num7 + lowerLeftOffset.y * num4) * num5;
			Vector2 upperRightOffset = this.upperRightOffset;
			float num9 = upperRightOffset.y * num3;
			Vector2 lowerRightOffset = this.lowerRightOffset;
			zero.y = num8 + (num9 + lowerRightOffset.y * num4) * num6;
			Vector2 upperLeftOffset2 = this.upperLeftOffset;
			float num10 = upperLeftOffset2.x * num5;
			Vector2 upperRightOffset2 = this.upperRightOffset;
			float num11 = (num10 + upperRightOffset2.x * num6) * num3;
			Vector2 lowerLeftOffset2 = this.lowerLeftOffset;
			float num12 = lowerLeftOffset2.x * num5;
			Vector2 lowerRightOffset2 = this.lowerRightOffset;
			zero.x = num11 + (num12 + lowerRightOffset2.x * num6) * num4;
			value.position += zero;
			list[j] = value;
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(list);
	}
}
