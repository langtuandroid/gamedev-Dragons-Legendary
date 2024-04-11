using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Soft Shadow", 3)]
[RequireComponent(typeof(Text))]
public class SoftShadow : Shadow
{
	[SerializeField]
	private float m_BlurSpread = 1f;

	[SerializeField]
	private bool m_OnlyInitialCharactersDropShadow = true;

	public float blurSpread
	{
		get
		{
			return m_BlurSpread;
		}
		set
		{
			m_BlurSpread = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public bool onlyInitialCharactersDropShadow
	{
		get
		{
			return m_OnlyInitialCharactersDropShadow;
		}
		set
		{
			m_OnlyInitialCharactersDropShadow = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	protected SoftShadow()
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
		int count = list.Count;
		Text component = GetComponent<Text>();
		List<UIVertex> list2 = new List<UIVertex>();
		list2 = ((!onlyInitialCharactersDropShadow) ? list : list.GetRange(count - component.cachedTextGenerator.characterCountVisible * 6, component.cachedTextGenerator.characterCountVisible * 6));
		Color effectColor = base.effectColor;
		effectColor.a /= 4f;
		int num = 0;
		int count2 = list2.Count;
		List<UIVertex> verts = list2;
		Color32 color = effectColor;
		int start = num;
		int count3 = list2.Count;
		Vector2 effectDistance = base.effectDistance;
		float x = effectDistance.x;
		Vector2 effectDistance2 = base.effectDistance;
		ApplyShadowZeroAlloc(verts, color, start, count3, x, effectDistance2.y);
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (i != 0 || j != 0)
				{
					num = count2;
					count2 = list2.Count;
					List<UIVertex> verts2 = list2;
					Color32 color2 = effectColor;
					int start2 = num;
					int count4 = list2.Count;
					Vector2 effectDistance3 = base.effectDistance;
					float x2 = effectDistance3.x + (float)i * blurSpread;
					Vector2 effectDistance4 = base.effectDistance;
					ApplyShadowZeroAlloc(verts2, color2, start2, count4, x2, effectDistance4.y + (float)j * blurSpread);
				}
			}
		}
		if (onlyInitialCharactersDropShadow)
		{
			list2.RemoveRange(list2.Count - component.cachedTextGenerator.characterCountVisible * 6, component.cachedTextGenerator.characterCountVisible * 6);
			list2.AddRange(list);
		}
		if (component.material.shader == Shader.Find("Text Effects/Fancy Text"))
		{
			for (int k = 0; k < list2.Count - count; k++)
			{
				UIVertex value = list2[k];
				value.uv1 = new Vector2(0f, 0f);
				list2[k] = value;
			}
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(list2);
	}
}
