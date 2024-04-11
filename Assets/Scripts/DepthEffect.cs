using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Depth Effect", 2)]
[RequireComponent(typeof(Text))]
public class DepthEffect : BaseMeshEffect
{
	[SerializeField]
	private Color m_EffectColor = new Color(0f, 0f, 0f, 1f);

	[SerializeField]
	private Vector2 m_EffectDirectionAndDepth = new Vector2(-1f, 1f);

	[SerializeField]
	private Vector2 m_DepthPerspectiveStrength = new Vector2(0f, 0f);

	[SerializeField]
	private bool m_OnlyInitialCharactersGenerateDepth = true;

	[SerializeField]
	private bool m_UseGraphicAlpha = true;

	private Vector2 m_OverallTextSize = Vector2.zero;

	private Vector2 m_TopLeftPos = Vector2.zero;

	private Vector2 m_BottomRightPos = Vector2.zero;

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

	public Vector2 effectDirectionAndDepth
	{
		get
		{
			return m_EffectDirectionAndDepth;
		}
		set
		{
			if (!(m_EffectDirectionAndDepth == value))
			{
				m_EffectDirectionAndDepth = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}
	}

	public Vector2 depthPerspectiveStrength
	{
		get
		{
			return m_DepthPerspectiveStrength;
		}
		set
		{
			if (!(m_DepthPerspectiveStrength == value))
			{
				m_DepthPerspectiveStrength = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}
	}

	public bool onlyInitialCharactersGenerateDepth
	{
		get
		{
			return m_OnlyInitialCharactersGenerateDepth;
		}
		set
		{
			m_OnlyInitialCharactersGenerateDepth = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
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

	protected DepthEffect()
	{
	}

	protected void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y, float factor)
	{
		for (int i = start; i < end; i++)
		{
			UIVertex uIVertex = verts[i];
			verts.Add(uIVertex);
			Vector3 position = uIVertex.position;
			position.x += x * factor;
			Vector2 depthPerspectiveStrength = this.depthPerspectiveStrength;
			if (depthPerspectiveStrength.x != 0f)
			{
				float x2 = position.x;
				Vector2 depthPerspectiveStrength2 = this.depthPerspectiveStrength;
				position.x = x2 - depthPerspectiveStrength2.x * ((position.x - m_TopLeftPos.x) / m_OverallTextSize.x - 0.5f) * factor;
			}
			position.y += y * factor;
			Vector2 depthPerspectiveStrength3 = this.depthPerspectiveStrength;
			if (depthPerspectiveStrength3.y != 0f)
			{
				float y2 = position.y;
				Vector2 depthPerspectiveStrength4 = this.depthPerspectiveStrength;
				position.y = y2 + depthPerspectiveStrength4.y * ((m_TopLeftPos.y - position.y) / m_OverallTextSize.y - 0.5f) * factor;
			}
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
		Text component = GetComponent<Text>();
		List<UIVertex> list2 = new List<UIVertex>();
		list2 = ((!m_OnlyInitialCharactersGenerateDepth) ? list : list.GetRange(list.Count - component.cachedTextGenerator.characterCountVisible * 6, component.cachedTextGenerator.characterCountVisible * 6));
		if (list2.Count == 0)
		{
			return;
		}
		Vector2 depthPerspectiveStrength = this.depthPerspectiveStrength;
		if (depthPerspectiveStrength.x == 0f)
		{
			Vector2 depthPerspectiveStrength2 = this.depthPerspectiveStrength;
			if (depthPerspectiveStrength2.y == 0f)
			{
				goto IL_0252;
			}
		}
		UIVertex uIVertex = list2[0];
		m_TopLeftPos = uIVertex.position;
		UIVertex uIVertex2 = list2[list2.Count - 1];
		m_BottomRightPos = uIVertex2.position;
		for (int i = 0; i < list2.Count; i++)
		{
			UIVertex uIVertex3 = list2[i];
			if (uIVertex3.position.x < m_TopLeftPos.x)
			{
				ref Vector2 topLeftPos = ref m_TopLeftPos;
				UIVertex uIVertex4 = list2[i];
				topLeftPos.x = uIVertex4.position.x;
			}
			UIVertex uIVertex5 = list2[i];
			if (uIVertex5.position.y > m_TopLeftPos.y)
			{
				ref Vector2 topLeftPos2 = ref m_TopLeftPos;
				UIVertex uIVertex6 = list2[i];
				topLeftPos2.y = uIVertex6.position.y;
			}
			UIVertex uIVertex7 = list2[i];
			if (uIVertex7.position.x > m_BottomRightPos.x)
			{
				ref Vector2 bottomRightPos = ref m_BottomRightPos;
				UIVertex uIVertex8 = list2[i];
				bottomRightPos.x = uIVertex8.position.x;
			}
			UIVertex uIVertex9 = list2[i];
			if (uIVertex9.position.y < m_BottomRightPos.y)
			{
				ref Vector2 bottomRightPos2 = ref m_BottomRightPos;
				UIVertex uIVertex10 = list2[i];
				bottomRightPos2.y = uIVertex10.position.y;
			}
		}
		m_OverallTextSize = new Vector2(m_BottomRightPos.x - m_TopLeftPos.x, m_TopLeftPos.y - m_BottomRightPos.y);
		goto IL_0252;
		IL_0252:
		int num = 0;
		int num2 = 0;
		num = num2;
		num2 = list2.Count;
		List<UIVertex> verts = list2;
		Color32 color = effectColor;
		int start = num;
		int count2 = list2.Count;
		Vector2 effectDirectionAndDepth = this.effectDirectionAndDepth;
		float x = effectDirectionAndDepth.x;
		Vector2 effectDirectionAndDepth2 = this.effectDirectionAndDepth;
		ApplyShadowZeroAlloc(verts, color, start, count2, x, effectDirectionAndDepth2.y, 0.25f);
		num = num2;
		num2 = list2.Count;
		List<UIVertex> verts2 = list2;
		Color32 color2 = effectColor;
		int start2 = num;
		int count3 = list2.Count;
		Vector2 effectDirectionAndDepth3 = this.effectDirectionAndDepth;
		float x2 = effectDirectionAndDepth3.x;
		Vector2 effectDirectionAndDepth4 = this.effectDirectionAndDepth;
		ApplyShadowZeroAlloc(verts2, color2, start2, count3, x2, effectDirectionAndDepth4.y, 0.5f);
		num = num2;
		num2 = list2.Count;
		List<UIVertex> verts3 = list2;
		Color32 color3 = effectColor;
		int start3 = num;
		int count4 = list2.Count;
		Vector2 effectDirectionAndDepth5 = this.effectDirectionAndDepth;
		float x3 = effectDirectionAndDepth5.x;
		Vector2 effectDirectionAndDepth6 = this.effectDirectionAndDepth;
		ApplyShadowZeroAlloc(verts3, color3, start3, count4, x3, effectDirectionAndDepth6.y, 0.75f);
		num = num2;
		num2 = list2.Count;
		List<UIVertex> verts4 = list2;
		Color32 color4 = effectColor;
		int start4 = num;
		int count5 = list2.Count;
		Vector2 effectDirectionAndDepth7 = this.effectDirectionAndDepth;
		float x4 = effectDirectionAndDepth7.x;
		Vector2 effectDirectionAndDepth8 = this.effectDirectionAndDepth;
		ApplyShadowZeroAlloc(verts4, color4, start4, count5, x4, effectDirectionAndDepth8.y, 1f);
		if (onlyInitialCharactersGenerateDepth)
		{
			list2.RemoveRange(list2.Count - component.cachedTextGenerator.characterCountVisible * 6, component.cachedTextGenerator.characterCountVisible * 6);
			list2.AddRange(list);
		}
		if (component.material.shader == Shader.Find("Text Effects/Fancy Text"))
		{
			for (int j = 0; j < list2.Count - count; j++)
			{
				UIVertex value = list2[j];
				value.uv1 = new Vector2(0f, 0f);
				list2[j] = value;
			}
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(list2);
	}
}
