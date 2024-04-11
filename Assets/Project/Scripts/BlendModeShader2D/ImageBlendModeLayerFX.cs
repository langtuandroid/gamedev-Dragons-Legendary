using System;
using UnityEngine;
using UnityEngine.UI;

namespace BlendModeShader2D
{
	[RequireComponent(typeof(Image))]
	public class ImageBlendModeLayerFX : BlendModeLayerFX
	{
		private Image _image;

		public Image image
		{
			get
			{
				if (_image != null)
				{
					return _image;
				}
				Image component = GetComponent<Image>();
				if (component != null)
				{
					_image = component;
				}
				else
				{
					_image = base.gameObject.AddComponent<Image>();
				}
				_image.color = mainColor;
				return _image;
			}
		}

		public Color mainColor
		{
			get
			{
				return image.color;
			}
			set
			{
				if (image.color != value)
				{
					image.color = value;
				}
			}
		}

		protected override string sharedGrabShader => "Custom/BlendModeShader2D/UIBlendModeSharedGrab";

		protected override string simpleGrabShader => "Custom/BlendModeShader2D/UIBlendModeSimpleGrab";

		private void OnEnable()
		{
			onMaterialChange = (OnMaterialChange)Delegate.Combine(onMaterialChange, new OnMaterialChange(UpdateMaterial));
		}

		private void OnDisable()
		{
			onMaterialChange = (OnMaterialChange)Delegate.Remove(onMaterialChange, new OnMaterialChange(UpdateMaterial));
		}

		protected override void Start()
		{
			base.Start();
			UpdateMaterial();
		}

		public void UpdateMaterial()
		{
			image.material = base.blendModeMaterial;
		}
	}
}
