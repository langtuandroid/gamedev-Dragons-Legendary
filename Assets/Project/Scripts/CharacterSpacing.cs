using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Character Spacing", 7)]
[RequireComponent(typeof(Text))]
public class CharacterSpacing : BaseMeshEffect
{
	private const string REGEX_TAGS = "<b>|</b>|<i>|</i>|<size=.*?>|</size>|<color=.*?>|</color>|<material=.*?>|</material>";

	[SerializeField]
	private float m_Offset;

	public float offset
	{
		get
		{
			return m_Offset;
		}
		set
		{
			if (m_Offset != value)
			{
				m_Offset = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}
	}

	protected CharacterSpacing()
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
		Text component = GetComponent<Text>();
		List<string> list2 = new List<string>();
		for (int i = 0; i < component.cachedTextGenerator.lineCount; i++)
		{
			UILineInfo uILineInfo = component.cachedTextGenerator.lines[i];
			int startCharIdx = uILineInfo.startCharIdx;
			int num;
			if (i < component.cachedTextGenerator.lineCount - 1)
			{
				UILineInfo uILineInfo2 = component.cachedTextGenerator.lines[i + 1];
				num = uILineInfo2.startCharIdx;
			}
			else
			{
				num = component.text.Length;
			}
			int num2 = num;
			list2.Add(component.text.Substring(startCharIdx, num2 - startCharIdx));
		}
		float num3 = offset * (float)component.fontSize / 100f;
		float num4 = 0f;
		IEnumerator enumerator = null;
		Match match = null;
		if (component.alignment == TextAnchor.LowerLeft || component.alignment == TextAnchor.MiddleLeft || component.alignment == TextAnchor.UpperLeft)
		{
			num4 = 0f;
		}
		else if (component.alignment == TextAnchor.LowerCenter || component.alignment == TextAnchor.MiddleCenter || component.alignment == TextAnchor.UpperCenter)
		{
			num4 = 0.5f;
		}
		else if (component.alignment == TextAnchor.LowerRight || component.alignment == TextAnchor.MiddleRight || component.alignment == TextAnchor.UpperRight)
		{
			num4 = 1f;
		}
		bool flag = true;
		int num5 = 0;
		for (int j = 0; j < list2.Count; j++)
		{
			if (!flag)
			{
				break;
			}
			string text = list2[j];
			int lengthWithoutTags = text.Length;
			if (lengthWithoutTags > component.cachedTextGenerator.characterCountVisible - num5)
			{
				lengthWithoutTags = component.cachedTextGenerator.characterCountVisible - num5;
				text = text.Substring(0, lengthWithoutTags) + " ";
				lengthWithoutTags++;
			}
			if (component.supportRichText)
			{
				enumerator = GetRegexMatchedTags(text, out lengthWithoutTags).GetEnumerator();
				match = null;
				if (enumerator.MoveNext())
				{
					match = (Match)enumerator.Current;
				}
			}
			bool flag2 = list2[j].Length > 0 && (list2[j][list2[j].Length - 1] == ' ' || list2[j][list2[j].Length - 1] == '\n');
			float num6 = (float)(-(lengthWithoutTags - 1 - (flag2 ? 1 : 0))) * num3 * num4;
			float num7 = 0f;
			for (int k = 0; k < text.Length; k++)
			{
				if (!flag)
				{
					break;
				}
				if (component.supportRichText && match != null && match.Index == k)
				{
					k += match.Length - 1;
					num5 += match.Length - 1;
					num7 -= 1f;
					match = null;
					if (enumerator.MoveNext())
					{
						match = (Match)enumerator.Current;
					}
				}
				if (num5 * 6 + 5 >= list.Count)
				{
					flag = false;
					break;
				}
				for (int l = 0; l < 6; l++)
				{
					UIVertex value = list[num5 * 6 + l];
					value.position += Vector3.right * (num3 * num7 + num6);
					list[num5 * 6 + l] = value;
				}
				num5++;
				num7 += 1f;
			}
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(list);
	}

	private MatchCollection GetRegexMatchedTags(string text, out int lengthWithoutTags)
	{
		MatchCollection matchCollection = Regex.Matches(text, "<b>|</b>|<i>|</i>|<size=.*?>|</size>|<color=.*?>|</color>|<material=.*?>|</material>");
		lengthWithoutTags = 0;
		int num = 0;
		IEnumerator enumerator = matchCollection.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Match match = (Match)enumerator.Current;
				num += match.Length;
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		lengthWithoutTags = text.Length - num;
		return matchCollection;
	}
}
