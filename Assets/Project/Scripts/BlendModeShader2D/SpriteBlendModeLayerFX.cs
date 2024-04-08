using System;
using UnityEngine;

namespace BlendModeShader2D
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class SpriteBlendModeLayerFX : BlendModeLayerFX
	{
		private SpriteRenderer _spriteRenderer;

		public SpriteRenderer spriteRenderer
		{
			get
			{
				if (_spriteRenderer != null)
				{
					return _spriteRenderer;
				}
				SpriteRenderer component = GetComponent<SpriteRenderer>();
				if (component != null)
				{
					_spriteRenderer = component;
				}
				else
				{
					_spriteRenderer = base.gameObject.AddComponent<SpriteRenderer>();
				}
				_spriteRenderer.color = mainColor;
				return _spriteRenderer;
			}
		}

		public Color mainColor
		{
			get
			{
				return spriteRenderer.color;
			}
			set
			{
				if (spriteRenderer.color != value)
				{
					spriteRenderer.color = value;
				}
			}
		}

		protected override string sharedGrabShader => "Custom/BlendModeShader2D/SpriteBlendModeSharedGrab";

		protected override string simpleGrabShader => "Custom/BlendModeShader2D/SpriteBlendModeSimpleGrab";

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
			spriteRenderer.material = base.blendModeMaterial;
		}
	}
}
