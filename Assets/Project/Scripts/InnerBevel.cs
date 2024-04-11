using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Inner Bevel", 19)]
[DisallowMultipleComponent]
[RequireComponent(typeof(Text))]
public class InnerBevel : BaseMeshEffect, IMaterialModifier
{
	public enum ColorMode
	{
		Override,
		Additive,
		Multiply
	}

	[SerializeField]
	private ColorMode m_HighlightColorMode;

	[SerializeField]
	public Color m_HighlightColor = Color.white;

	[SerializeField]
	private ColorMode m_ShadowColorMode;

	[SerializeField]
	public Color m_ShadowColor = Color.black;

	[SerializeField]
	private Vector2 m_BevelDirectionAndDepth = new Vector2(1f, 1f);

	private bool m_NeedsToSetMaterialDirty;

	private Material m_ModifiedMaterial;

	public ColorMode highlightColorMode
	{
		get
		{
			return m_HighlightColorMode;
		}
		set
		{
			m_HighlightColorMode = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

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

	public ColorMode shadowColorMode
	{
		get
		{
			return m_ShadowColorMode;
		}
		set
		{
			m_ShadowColorMode = value;
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

	protected InnerBevel()
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
		for (int i = 0; i < count; i += 6)
		{
			UIVertex value = list[i];
			UIVertex uIVertex = list[i + 1];
			Vector4 uv = uIVertex.uv0;
			UIVertex uIVertex2 = list[i];
			Vector4 normalized = (uv - uIVertex2.uv0).normalized;
			UIVertex uIVertex3 = list[i + 1];
			Vector4 uv2 = uIVertex3.uv0;
			UIVertex uIVertex4 = list[i + 2];
			Vector2 normalized2 = (uv2 - uIVertex4.uv0).normalized;
			Vector4 tangent = normalized;
			tangent.z = normalized2.x;
			tangent.w = normalized2.y;
			value.tangent = tangent;
			if (value.uv1 == Vector4.zero)
			{
				value.uv1 = new Vector2(1f, 1f);
			}
			list[i] = value;
			for (int j = 1; j < 6; j++)
			{
				value = list[i + j];
				UIVertex uIVertex5 = list[i];
				value.tangent = uIVertex5.tangent;
				if (value.uv1 == Vector4.zero)
				{
					value.uv1 = new Vector2(1f, 1f);
				}
				list[i + j] = value;
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
			UnityEngine.Debug.Log("\"" + base.gameObject.name + "\" doesn't have the \"Fancy Text\" shader applied. Please use it if you are using the \"Inner Bevel\" effect.");
			return baseMaterial;
		}
		if (m_ModifiedMaterial == null)
		{
			m_ModifiedMaterial = new Material(baseMaterial);
		}
		m_ModifiedMaterial.CopyPropertiesFromMaterial(baseMaterial);
		m_ModifiedMaterial.name = baseMaterial.name + " with IB";
		m_ModifiedMaterial.hideFlags = (HideFlags.HideInHierarchy | HideFlags.HideInInspector);
		m_ModifiedMaterial.shaderKeywords = baseMaterial.shaderKeywords;
		m_ModifiedMaterial.CopyPropertiesFromMaterial(baseMaterial);
		m_ModifiedMaterial.EnableKeyword("_USEBEVEL_ON");
		m_ModifiedMaterial.SetColor("_HighlightColor", highlightColor);
		m_ModifiedMaterial.SetColor("_ShadowColor", shadowColor);
		m_ModifiedMaterial.SetVector("_HighlightOffset", bevelDirectionAndDepth / 500f);
		m_ModifiedMaterial.SetInt("_HighlightColorMode", (int)highlightColorMode);
		m_ModifiedMaterial.SetInt("_ShadowColorMode", (int)shadowColorMode);
		m_NeedsToSetMaterialDirty = true;
		return m_ModifiedMaterial;
	}
}
