using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlendModeShader2D
{
	[ExecuteInEditMode]
	public abstract class BlendModeFX : MonoBehaviour
	{
		protected delegate void OnMaterialChange();

		protected OnMaterialChange onMaterialChange;

		[SerializeField]
		private Shader _blendModeShader;

		private Material _blendModeMaterial;

		[SerializeField]
		private bool _enableGrayBase;

		[SerializeField]
		private BlendMode _blendMode;

		[SerializeField]
		private Texture2D _effectTexture;

		[SerializeField]
		private Vector2 _effectTextureTiling = Vector2.one;

		[SerializeField]
		private Vector2 _effectTextureOffset = Vector2.zero;

		[SerializeField]
		private Color _effectColor = Color.white;

		[SerializeField]
		private ColorSoloMode _colorSoloMode;

		[SerializeField]
		private bool _pixelSnap;

		private List<Material> _createdMaterials = new List<Material>();

		public Shader blendModeShader
		{
			get
			{
				return _blendModeShader;
			}
			set
			{
				if (value == null)
				{
					UnityEngine.Debug.Log("Missing shader in " + ToString());
					base.enabled = false;
				}
				else if (value.isSupported)
				{
					_blendModeShader = value;
					_blendModeMaterial = new Material(_blendModeShader);
					InitBlendModeMaterial(_blendModeMaterial);
					_blendModeMaterial.hideFlags = HideFlags.DontSave;
					_createdMaterials.Add(_blendModeMaterial);
					if (onMaterialChange != null)
					{
						onMaterialChange();
					}
				}
				else
				{
					base.enabled = false;
					UnityEngine.Debug.Log("The shader " + value.ToString() + " is not supported on this platform!");
				}
			}
		}

		public Material blendModeMaterial
		{
			get
			{
				return _blendModeMaterial;
			}
			private set
			{
				_blendModeMaterial = value;
				InitBlendModeMaterial(_blendModeMaterial);
				if (onMaterialChange != null)
				{
					onMaterialChange();
				}
			}
		}

		public bool enableGrayBase
		{
			get
			{
				return _enableGrayBase;
			}
			set
			{
				if (_enableGrayBase == value)
				{
					return;
				}
				_enableGrayBase = value;
				if (_blendModeMaterial != null)
				{
					if (_enableGrayBase)
					{
						_blendModeMaterial.EnableKeyword("GRAY_BASE_ON");
					}
					else
					{
						_blendModeMaterial.DisableKeyword("GRAY_BASE_ON");
					}
				}
			}
		}

		public BlendMode blendMode
		{
			get
			{
				return _blendMode;
			}
			set
			{
				if (_blendMode != value)
				{
					string keyword = "_BM_" + _blendMode.ToString().ToUpper();
					_blendMode = value;
					string keyword2 = "_BM_" + _blendMode.ToString().ToUpper();
					if (_blendModeMaterial != null)
					{
						_blendModeMaterial.DisableKeyword(keyword);
						_blendModeMaterial.EnableKeyword(keyword2);
					}
				}
			}
		}

		public Texture2D effectTexture
		{
			get
			{
				return _effectTexture;
			}
			set
			{
				_effectTexture = value;
				if (_blendModeMaterial != null)
				{
					_blendModeMaterial.SetTexture("_EffectTex", _effectTexture);
				}
			}
		}

		public Vector2 effectTextureTiling
		{
			get
			{
				return _effectTextureTiling;
			}
			set
			{
				if (_effectTextureTiling != value)
				{
					_effectTextureTiling = value;
					if (_blendModeMaterial != null)
					{
						_blendModeMaterial.SetTextureScale("_EffectTex", _effectTextureTiling);
					}
				}
			}
		}

		public Vector2 effectTextureOffset
		{
			get
			{
				return _effectTextureOffset;
			}
			set
			{
				if (_effectTextureOffset != value)
				{
					_effectTextureOffset = value;
					if (_blendModeMaterial != null)
					{
						_blendModeMaterial.SetTextureOffset("_EffectTex", _effectTextureOffset);
					}
				}
			}
		}

		public Color effectColor
		{
			get
			{
				return _effectColor;
			}
			set
			{
				if (_effectColor != value)
				{
					_effectColor = value;
					if (_blendModeMaterial != null)
					{
						_blendModeMaterial.SetColor("_EffectColor", _effectColor);
					}
				}
			}
		}

		public ColorSoloMode colorSoloMode
		{
			get
			{
				return _colorSoloMode;
			}
			set
			{
				if (_colorSoloMode != value)
				{
					string keyword = "_COLORSOLO_" + _colorSoloMode.ToString().ToUpper();
					_colorSoloMode = value;
					string keyword2 = "_COLORSOLO_" + _colorSoloMode.ToString().ToUpper();
					if (_blendModeMaterial != null)
					{
						_blendModeMaterial.DisableKeyword(keyword);
						_blendModeMaterial.EnableKeyword(keyword2);
					}
				}
			}
		}

		public bool pixelSnap
		{
			get
			{
				return _pixelSnap;
			}
			set
			{
				if (_pixelSnap == value)
				{
					return;
				}
				_pixelSnap = value;
				if (_blendModeMaterial != null)
				{
					if (_pixelSnap)
					{
						_blendModeMaterial.EnableKeyword("PIXELSNAP_ON");
					}
					else
					{
						_blendModeMaterial.DisableKeyword("PIXELSNAP_ON");
					}
				}
			}
		}

		protected virtual void Start()
		{
			if (_blendModeMaterial == null && _blendModeShader != null)
			{
				_blendModeMaterial = new Material(_blendModeShader);
				InitBlendModeMaterial(_blendModeMaterial);
				_blendModeMaterial.hideFlags = HideFlags.DontSave;
				_createdMaterials.Add(_blendModeMaterial);
			}
		}

		private void OnDestroy()
		{
			RemoveCreatedMaterials();
		}

		private void InitBlendModeMaterial(Material bmMaterial)
		{
			if (!(bmMaterial == null))
			{
				bmMaterial.SetTexture("_EffectTex", _effectTexture);
				bmMaterial.SetColor("_EffectColor", _effectColor);
				if (_enableGrayBase)
				{
					bmMaterial.EnableKeyword("GRAY_BASE_ON");
				}
				else
				{
					bmMaterial.DisableKeyword("GRAY_BASE_ON");
				}
				if (_pixelSnap)
				{
					bmMaterial.EnableKeyword("PIXELSNAP_ON");
				}
				else
				{
					bmMaterial.DisableKeyword("PIXELSNAP_ON");
				}
				string[] names = Enum.GetNames(typeof(ColorSoloMode));
				foreach (string text in names)
				{
					bmMaterial.DisableKeyword("_COLORSOLO_" + text.ToUpper());
				}
				bmMaterial.EnableKeyword("_COLORSOLO_" + _colorSoloMode.ToString().ToUpper());
				string[] names2 = Enum.GetNames(typeof(BlendMode));
				foreach (string text2 in names2)
				{
					bmMaterial.DisableKeyword("_BM_" + text2.ToUpper());
				}
				bmMaterial.EnableKeyword("_BM_" + _blendMode.ToString().ToUpper());
				_blendModeMaterial.SetTextureScale("_EffectTex", _effectTextureTiling);
				_blendModeMaterial.SetTextureOffset("_EffectTex", _effectTextureOffset);
			}
		}

		private void RemoveCreatedMaterials()
		{
			while (_createdMaterials.Count > 0)
			{
				Material obj = _createdMaterials[0];
				_createdMaterials.RemoveAt(0);
				UnityEngine.Object.Destroy(obj);
			}
		}
	}
}
