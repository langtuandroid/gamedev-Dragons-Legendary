using UnityEngine;

namespace BlendModeShader2D
{
	[ExecuteInEditMode]
	public class BlendModeFlow : MonoBehaviour
	{
		public Vector2 flowSpeed = Vector2.one;

		private BlendModeFX _blendModeFx;

		public BlendModeFX blendModeFX
		{
			get
			{
				if (_blendModeFx == null)
				{
					_blendModeFx = GetComponent<BlendModeFX>();
				}
				return _blendModeFx;
			}
		}

		private void Update()
		{
			blendModeFX.effectTextureOffset -= Time.deltaTime * flowSpeed;
		}
	}
}
