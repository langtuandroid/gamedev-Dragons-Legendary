using System;
using UnityEngine;
using UnityEngine.UI;

namespace BlendModeShader2D
{
	[RequireComponent(typeof(RawImage))]
	public class RawImageBlendModeFX : BlendModeFX
	{
		private RawImage _rawImage;

		public RawImage image
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
