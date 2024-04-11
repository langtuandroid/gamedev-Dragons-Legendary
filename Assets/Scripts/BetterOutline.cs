using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Better Outline", 0)]
[RequireComponent(typeof(Text))]
public class BetterOutline : Shadow
{
	protected BetterOutline()
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
		int num = 0;
		int num2 = 0;
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (i != 0 && j != 0)
				{
					num = num2;
					num2 = list.Count;
					List<UIVertex> verts = list;
					Color32 color = base.effectColor;
					int start = num;
					int count2 = list.Count;
					float num3 = i;
					Vector2 effectDistance = base.effectDistance;
					float x = num3 * effectDistance.x * 0.707f;
					float num4 = j;
					Vector2 effectDistance2 = base.effectDistance;
					ApplyShadowZeroAlloc(verts, color, start, count2, x, num4 * effectDistance2.y * 0.707f);
				}
			}
		}
		num = num2;
		num2 = list.Count;
		List<UIVertex> verts2 = list;
		Color32 color2 = base.effectColor;
		int start2 = num;
		int count3 = list.Count;
		Vector2 effectDistance3 = base.effectDistance;
		ApplyShadowZeroAlloc(verts2, color2, start2, count3, 0f - effectDistance3.x, 0f);
		num = num2;
		num2 = list.Count;
		List<UIVertex> verts3 = list;
		Color32 color3 = base.effectColor;
		int start3 = num;
		int count4 = list.Count;
		Vector2 effectDistance4 = base.effectDistance;
		ApplyShadowZeroAlloc(verts3, color3, start3, count4, effectDistance4.x, 0f);
		num = num2;
		num2 = list.Count;
		List<UIVertex> verts4 = list;
		Color32 color4 = base.effectColor;
		int start4 = num;
		int count5 = list.Count;
		Vector2 effectDistance5 = base.effectDistance;
		ApplyShadowZeroAlloc(verts4, color4, start4, count5, 0f, 0f - effectDistance5.y);
		num = num2;
		num2 = list.Count;
		List<UIVertex> verts5 = list;
		Color32 color5 = base.effectColor;
		int start5 = num;
		int count6 = list.Count;
		Vector2 effectDistance6 = base.effectDistance;
		ApplyShadowZeroAlloc(verts5, color5, start5, count6, 0f, effectDistance6.y);
		if (GetComponent<Text>().material.shader == Shader.Find("Text Effects/Fancy Text"))
		{
			for (int k = 0; k < list.Count - count; k++)
			{
				UIVertex value = list[k];
				value.uv1 = new Vector2(0f, 0f);
				list[k] = value;
			}
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(list);
	}
}
