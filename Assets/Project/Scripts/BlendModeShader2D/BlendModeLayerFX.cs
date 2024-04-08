using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlendModeShader2D
{
	[ExecuteInEditMode]
	public abstract class BlendModeLayerFX : MonoBehaviour
	{
		public enum GrabMode
		{
			Shared,
			Simple
		}

		protected delegate void OnMaterialChange();

		protected OnMaterialChange onMaterialChange;

		[SerializeField]
		private Shader _blendModeShader;

		private Material _blendModeMaterial;

		[SerializeField]
		private BlendMode _blendMode;

		[SerializeField]
		private GrabMode _grabMode;

		[SerializeField]
		private Color _baseLayerColor = Color.white;

		[SerializeField]
		private bool _enableGrayBase;

		[SerializeField]
		private Color _extraColor = Color.clear;

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

		public GrabMode grabMode
		{
			get
			{
				return _grabMode;
			}
			set
			{
				if (_grabMode != value)
				{
					_grabMode = value;
					switch (value)
					{
					case GrabMode.Shared:
						blendModeShader = Shader.Find(sharedGrabShader);
						break;
					case GrabMode.Simple:
						blendModeShader = Shader.Find(simpleGrabShader);
						break;
					}
				}
			}
		}

		public Color baseLayerColor
		{
			get
			{
				return _baseLayerColor;
			}
			set
			{
				if (_baseLayerColor != value)
				{
					_baseLayerColor = value;
					if (_blendModeMaterial != null)
					{
						_blendModeMaterial.SetColor("_BaseLayerColor", _baseLayerColor);
					}
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

		public Color extraColor
		{
			get
			{
				return _extraColor;
			}
			set
			{
				if (_extraColor != value)
				{
					_extraColor = value;
					if (_blendModeMaterial != null)
					{
						_blendModeMaterial.SetColor("_ExtraColor", _extraColor);
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

		protected abstract string sharedGrabShader
		{
			get;
		}

		protected abstract string simpleGrabShader
		{
			get;
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
				bmMaterial.SetColor("_BaseLayerColor", _baseLayerColor);
				bmMaterial.SetColor("_ExtraColor", _extraColor);
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
				string[] names2 = Enum.GetNames(typeof(BlendMode));
				foreach (string text2 in names2)
				{
					bmMaterial.DisableKeyword("_BM_" + text2.ToUpper());
				}
				bmMaterial.EnableKeyword("_BM_" + _blendMode.ToString().ToUpper());
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
