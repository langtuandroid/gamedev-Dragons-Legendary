using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Outer Bevel", 4)]
[RequireComponent(typeof(Text))]
public class Bevel : BaseMeshEffect
{
	[SerializeField]
	private Color m_HighlightColor = new Color(1f, 1f, 1f, 1f);

	[SerializeField]
	private Color m_ShadowColor = new Color(0f, 0f, 0f, 1f);

	[SerializeField]
	private Vector2 m_BevelDirectionAndDepth = new Vector2(1f, 1f);

	[SerializeField]
	private bool m_UseGraphicAlpha = true;

	public Color highlightColor
	{
		get
		{
			return m_HighlightColor;
		}
		set
		{
			m_HighlightColor = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public Color shadowColor
	{
		get
		{
			return m_ShadowColor;
		}
		set
		{
			m_ShadowColor = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public Vector2 bevelDirectionAndDepth
	{
		get
		{
			return m_BevelDirectionAndDepth;
		}
		set
		{
			if (!(m_BevelDirectionAndDepth == value))
			{
				m_BevelDirectionAndDepth = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}
	}

	public bool useGraphicAlpha
	{
		get
		{
			return m_UseGraphicAlpha;
		}
		set
		{
			m_UseGraphicAlpha = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	protected Bevel()
	{
	}

	protected void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
	{
		for (int i = start; i < end; i++)
		{
			UIVertex uIVertex = verts[i];
			verts.Add(uIVertex);
			Vector3 position = uIVertex.position;
			position.x += x;
			position.y += y;
			uIVertex.position = position;
			Color32 color2 = color;
			if (useGraphicAlpha)
			{
				byte a = color2.a;
				UIVertex uIVertex2 = verts[i];
				color2.a = (byte)(a * uIVertex2.color.a / 255);
			}
			uIVertex.color = color2;
			verts[i] = uIVertex;
		}
	}

	public override void ModifyMesh(VertexHelper vh)
	{
		if (!IsActive())
		{
			return;
		}
		List<UIVertex> list = new List<UIVertex>();
		vh.GetUIVertexStream(list);
		int count = list.Count;
		int num = 0;
		int num2 = 0;
		num = num2;
		num2 = list.Count;
		List<UIVertex> verts = list;
		Color32 color = shadowColor;
		int start = num;
		int count2 = list.Count;
		Vector2 bevelDirectionAndDepth = this.bevelDirectionAndDepth;
		float x = bevelDirectionAndDepth.x * 0.75f;
		Vector2 bevelDirectionAndDepth2 = this.bevelDirectionAndDepth;
		ApplyShadowZeroAlloc(verts, color, start, count2, x, (0f - bevelDirectionAndDepth2.y) * 0.75f);
		num = num2;
		num2 = list.Count;
		List<UIVertex> verts2 = list;
		Color32 color2 = shadowColor;
		int start2 = num;
		int count3 = list.Count;
		Vector2 bevelDirectionAndDepth3 = this.bevelDirectionAndDepth;
		float x2 = bevelDirectionAndDepth3.x;
		Vector2 bevelDirectionAndDepth4 = this.bevelDirectionAndDepth;
		ApplyShadowZeroAlloc(verts2, color2, start2, count3, x2, bevelDirectionAndDepth4.y * 0.5f);
		num = num2;
		num2 = list.Count;
		List<UIVertex> verts3 = list;
		Color32 color3 = shadowColor;
		int start3 = num;
		int count4 = list.Count;
		Vector2 bevelDirectionAndDepth5 = this.bevelDirectionAndDepth;
		float x3 = (0f - bevelDirectionAndDepth5.x) * 0.5f;
		Vector2 bevelDirectionAndDepth6 = this.bevelDirectionAndDepth;
		ApplyShadowZeroAlloc(verts3, color3, start3, count4, x3, 0f - bevelDirectionAndDepth6.y);
		num = num2;
		num2 = list.Count;
		List<UIVertex> verts4 = list;
		Color32 color4 = highlightColor;
		int start4 = num;
		int count5 = list.Count;
		Vector2 bevelDirectionAndDepth7 = this.bevelDirectionAndDepth;
		float x4 = 0f - bevelDirectionAndDepth7.x;
		Vector2 bevelDirectionAndDepth8 = this.bevelDirectionAndDepth;
		ApplyShadowZeroAlloc(verts4, color4, start4, count5, x4, bevelDirectionAndDepth8.y * 0.5f);
		num = num2;
		num2 = list.Count;
		List<UIVertex> verts5 = list;
		Color32 color5 = highlightColor;
		int start5 = num;
		int count6 = list.Count;
		Vector2 bevelDirectionAndDepth9 = this.bevelDirectionAndDepth;
		float x5 = (0f - bevelDirectionAndDepth9.x) * 0.5f;
		Vector2 bevelDirectionAndDepth10 = this.bevelDirectionAndDepth;
		ApplyShadowZeroAlloc(verts5, color5, start5, count6, x5, bevelDirectionAndDepth10.y);
		if (GetComponent<Text>().material.shader == Shader.Find("Text Effects/Fancy Text"))
		{
			for (int i = 0; i < list.Count - count; i++)
			{
				UIVertex value = list[i];
				value.uv1 = new Vector2(0f, 0f);
				list[i] = value;
			}
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(list);
	}
}
