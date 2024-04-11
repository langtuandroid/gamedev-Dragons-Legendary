using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Overlay Texture", 18)]
[DisallowMultipleComponent]
[RequireComponent(typeof(Text))]
public class OverlayTexture : BaseMeshEffect, IMaterialModifier
{
	public enum TextureMode
	{
		Local,
		GlobalTextArea,
		GlobalFullRect
	}

	public enum ColorMode
	{
		Override,
		Additive,
		Multiply
	}

	[SerializeField]
	private TextureMode m_TextureMode;

	[SerializeField]
	private ColorMode m_ColorMode;

	[SerializeField]
	public Texture2D m_OverlayTexture;

	private bool m_NeedsToSetMaterialDirty;

	private Material m_ModifiedMaterial;

	public TextureMode textureMode
	{
		get
		{
			return m_TextureMode;
		}
		set
		{
			m_TextureMode = value;
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

	public Texture2D overlayTexture
	{
		get
		{
			return m_OverlayTexture;
		}
		set
		{
			m_OverlayTexture = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	protected OverlayTexture()
	{
	}

	protected override void Start()
	{
		if (base.graphic != null)
		{
			base.graphic.SetMaterialDirty();
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
		if (list.Count == 0)
		{
			return;
		}
		UIVertex value;
		if (textureMode == TextureMode.GlobalTextArea || textureMode == TextureMode.GlobalFullRect)
		{
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			if (textureMode == TextureMode.GlobalFullRect)
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
			for (int j = 0; j < count; j++)
			{
				value = list[j];
				value.uv1 = new Vector2(1f + (value.position.x - vector.x) / num2, 2f - (vector.y - value.position.y) / num);
				list[j] = value;
			}
		}
		else
		{
			for (int k = 0; k < count; k++)
			{
				value = list[k];
				value.uv1 = new Vector2(1 + ((k % 6 != 0 && k % 6 != 5 && k % 6 != 4) ? 1 : 0), 1 + ((k % 6 != 2 && k % 6 != 3 && k % 6 != 4) ? 1 : 0));
				list[k] = value;
			}
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(list);
	}

	private void Update()
	{
		if (m_NeedsToSetMaterialDirty && base.graphic != null)
		{
			base.graphic.SetMaterialDirty();
		}
	}

	public virtual Material GetModifiedMaterial(Material baseMaterial)
	{
		if (!IsActive())
		{
			return baseMaterial;
		}
		if (baseMaterial.shader != Shader.Find("Text Effects/Fancy Text"))
		{
			UnityEngine.Debug.Log("\"" + base.gameObject.name + "\" doesn't have the \"Fancy Text\" shader applied. Please use it if you are using the \"Overlay Texture\" effect.");
			return baseMaterial;
		}
		if (m_ModifiedMaterial == null)
		{
			m_ModifiedMaterial = new Material(baseMaterial);
		}
		m_ModifiedMaterial.CopyPropertiesFromMaterial(baseMaterial);
		m_ModifiedMaterial.name = baseMaterial.name + " with OT";
		m_ModifiedMaterial.hideFlags = (HideFlags.HideInHierarchy | HideFlags.HideInInspector);
		m_ModifiedMaterial.shaderKeywords = baseMaterial.shaderKeywords;
		m_ModifiedMaterial.CopyPropertiesFromMaterial(baseMaterial);
		m_ModifiedMaterial.EnableKeyword("_USEOVERLAYTEXTURE_ON");
		m_ModifiedMaterial.SetTexture("_OverlayTex", overlayTexture);
		m_ModifiedMaterial.SetInt("_OverlayTextureColorMode", (int)colorMode);
		m_NeedsToSetMaterialDirty = true;
		return m_ModifiedMaterial;
	}
}
