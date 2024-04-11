using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/ToJ Shadow", 14)]
public class ToJShadow : BaseMeshEffect
{
	[SerializeField]
	private Color m_EffectColor = new Color(0f, 0f, 0f, 0.5f);

	[SerializeField]
	private Vector2 m_EffectDistance = new Vector2(1f, -1f);

	[SerializeField]
	private bool m_UseGraphicAlpha = true;

	public Color effectColor
	{
		get
		{
			return m_EffectColor;
		}
		set
		{
			m_EffectColor = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public Vector2 effectDistance
	{
		get
		{
			return m_EffectDistance;
		}
		set
		{
			if (value.x > 600f)
			{
				value.x = 600f;
			}
			if (value.x < -600f)
			{
				value.x = -600f;
			}
			if (value.y > 600f)
			{
				value.y = 600f;
			}
			if (value.y < -600f)
			{
				value.y = -600f;
			}
			if (!(m_EffectDistance == value))
			{
				m_EffectDistance = value;
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

	protected ToJShadow()
	{
	}

	protected void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
	{
		int num = verts.Count * 2;
		if (verts.Capacity < num)
		{
			verts.Capacity = num;
		}
		for (int i = start; i < end; i++)
		{
			UIVertex uIVertex = verts[i];
			verts.Add(uIVertex);
			Vector3 position = uIVertex.position;
			position.x += x;
			position.y += y;
			uIVertex.position = position;
			Color32 color2 = color;
			if (m_UseGraphicAlpha)
			{
				byte a = color2.a;
				UIVertex uIVertex2 = verts[i];
				color2.a = (byte)(a * uIVertex2.color.a / 255);
			}
			uIVertex.color = color2;
			verts[i] = uIVertex;
		}
	}

	protected void ApplyShadow(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
	{
		int num = verts.Count * 2;
		if (verts.Capacity < num)
		{
			verts.Capacity = num;
		}
		ApplyShadowZeroAlloc(verts, color, start, end, x, y);
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
		List<UIVertex> verts = list;
		Color32 color = effectColor;
		int count2 = list.Count;
		Vector2 effectDistance = this.effectDistance;
		float x = effectDistance.x;
		Vector2 effectDistance2 = this.effectDistance;
		ApplyShadow(verts, color, 0, count2, x, effectDistance2.y);
		Text component = GetComponent<Text>();
		if (component != null && component.material.shader == Shader.Find("Text Effects/Fancy Text"))
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
