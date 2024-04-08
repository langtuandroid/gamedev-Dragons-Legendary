using System;
using UnityEngine;
using UnityEngine.UI;

namespace BlendModeShader2D
{
	[RequireComponent(typeof(RawImage))]
	public class RawImageBlendModeLayerFX : BlendModeLayerFX
	{
		private RawImage _rawImage;

		public RawImage rawImage
		{
			get
			{
				if (_rawImage != null)
				{
					return _rawImage;
				}
				RawImage component = GetComponent<RawImage>();
				if (component != null)
				{
					_rawImage = component;
				}
				else
				{
					_rawImage = base.gameObject.AddComponent<RawImage>();
				}
				_rawImage.color = mainColor;
				return _rawImage;
			}
		}

		public Color mainColor
		{
			get
			{
				return rawImage.color;
			}
			set
			{
				if (rawImage.color != value)
				{
					rawImage.color = value;
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
			rawImage.material = base.blendModeMaterial;
		}
	}
}
