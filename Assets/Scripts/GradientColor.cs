using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Gradient Color", 1)]
[RequireComponent(typeof(Text))]
public class GradientColor : BaseMeshEffect
{
	public enum GradientMode
	{
		Local,
		GlobalTextArea,
		GlobalFullRect
	}

	public enum GradientDirection
	{
		Vertical,
		Horizontal
	}

	public enum ColorMode
	{
		Override,
		Additive,
		Multiply
	}

	[SerializeField]
	private GradientMode m_GradientMode;

	[SerializeField]
	private GradientDirection m_GradientDirection;

	[SerializeField]
	private ColorMode m_ColorMode;

	[SerializeField]
	public Color m_FirstColor = Color.white;

	[SerializeField]
	public Color m_SecondColor = Color.black;

	[SerializeField]
	private bool m_UseGraphicAlpha = true;

	public GradientMode gradientMode
	{
		get
		{
			return m_GradientMode;
		}
		set
		{
			m_GradientMode = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public GradientDirection gradientDirection
	{
		get
		{
			return m_GradientDirection;
		}
		set
		{
			m_GradientDirection = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public ColorMode colorMode
	{
		get
		{
			return m_ColorMode;
		}
		set
		{
			m_ColorMode = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public Color firstColor
	{
		get
		{
			return m_FirstColor;
		}
		set
		{
			m_FirstColor = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public Color secondColor
	{
		get
		{
			return m_SecondColor;
		}
		set
		{
			m_SecondColor = value;
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

	protected GradientColor()
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
		if (gradientMode == GradientMode.GlobalTextArea || gradientMode == GradientMode.GlobalFullRect)
		{
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			if (gradientMode == GradientMode.GlobalFullRect)
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
				if (gradientDirection == GradientDirection.Vertical)
				{
					Color newColor = Color.Lerp(this.firstColor, this.secondColor, (vector.y - value.position.y) / num);
					value.color = CalculateColor(value.color, newColor, colorMode);
				}
				else
				{
					Color newColor2 = Color.Lerp(this.firstColor, this.secondColor, (value.position.x - vector.x) / num2);
					value.color = CalculateColor(value.color, newColor2, colorMode);
				}
				if (useGraphicAlpha)
				{
					ref Color32 color = ref value.color;
					byte a = value.color.a;
					UIVertex uIVertex11 = list[j];
					color.a = (byte)(a * uIVertex11.color.a / 255);
				}
				list[j] = value;
			}
		}
		else
		{
			for (int k = 0; k < list.Count; k++)
			{
				UIVertex value2 = list[k];
				if (gradientDirection == GradientDirection.Vertical)
				{
					if (k % 6 == 0 || k % 6 == 1 || k % 6 == 5)
					{
						Color firstColor = this.firstColor;
						value2.color = CalculateColor(value2.color, firstColor, colorMode);
					}
					else
					{
						Color secondColor = this.secondColor;
						value2.color = CalculateColor(value2.color, secondColor, colorMode);
					}
				}
				else if (k % 6 == 0 || k % 6 == 4 || k % 6 == 5)
				{
					Color firstColor2 = this.firstColor;
					value2.color = CalculateColor(value2.color, firstColor2, colorMode);
				}
				else
				{
					Color secondColor2 = this.secondColor;
					value2.color = CalculateColor(value2.color, secondColor2, colorMode);
				}
				if (useGraphicAlpha)
				{
					ref Color32 color2 = ref value2.color;
					byte a2 = value2.color.a;
					UIVertex uIVertex12 = list[k];
					color2.a = (byte)(a2 * uIVertex12.color.a / 255);
				}
				list[k] = value2;
			}
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(list);
	}

	private Color CalculateColor(Color initialColor, Color newColor, ColorMode colorMode)
	{
		switch (colorMode)
		{
		case ColorMode.Override:
			return newColor;
		case ColorMode.Additive:
			return initialColor + newColor;
		case ColorMode.Multiply:
			return initialColor * newColor;
		default:
			return newColor;
		}
	}
}
