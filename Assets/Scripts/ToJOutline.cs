using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/ToJ Outline", 15)]
public class ToJOutline : Shadow
{
	protected ToJOutline()
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
		int num = list.Count * 5;
		if (list.Capacity < num)
		{
			list.Capacity = num;
		}
		int num2 = 0;
		int count2 = list.Count;
		List<UIVertex> verts = list;
		Color32 color = base.effectColor;
		int start = num2;
		int count3 = list.Count;
		Vector2 effectDistance = base.effectDistance;
		float x = effectDistance.x;
		Vector2 effectDistance2 = base.effectDistance;
		ApplyShadowZeroAlloc(verts, color, start, count3, x, effectDistance2.y);
		num2 = count2;
		count2 = list.Count;
		List<UIVertex> verts2 = list;
		Color32 color2 = base.effectColor;
		int start2 = num2;
		int count4 = list.Count;
		Vector2 effectDistance3 = base.effectDistance;
		float x2 = effectDistance3.x;
		Vector2 effectDistance4 = base.effectDistance;
		ApplyShadowZeroAlloc(verts2, color2, start2, count4, x2, 0f - effectDistance4.y);
		num2 = count2;
		count2 = list.Count;
		List<UIVertex> verts3 = list;
		Color32 color3 = base.effectColor;
		int start3 = num2;
		int count5 = list.Count;
		Vector2 effectDistance5 = base.effectDistance;
		float x3 = 0f - effectDistance5.x;
		Vector2 effectDistance6 = base.effectDistance;
		ApplyShadowZeroAlloc(verts3, color3, start3, count5, x3, effectDistance6.y);
		num2 = count2;
		count2 = list.Count;
		List<UIVertex> verts4 = list;
		Color32 color4 = base.effectColor;
		int start4 = num2;
		int count6 = list.Count;
		Vector2 effectDistance7 = base.effectDistance;
		float x4 = 0f - effectDistance7.x;
		Vector2 effectDistance8 = base.effectDistance;
		ApplyShadowZeroAlloc(verts4, color4, start4, count6, x4, 0f - effectDistance8.y);
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
