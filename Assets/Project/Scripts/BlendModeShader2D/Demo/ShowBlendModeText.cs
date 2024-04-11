using UnityEngine;

namespace BlendModeShader2D.Demo
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(TextMesh))]
	public class ShowBlendModeText : MonoBehaviour
	{
		private TextMesh _textMesh;

		private SpriteBlendModeFX _spriteBlendModeFX;

		private void Start()
		{
			_textMesh = GetComponent<TextMesh>();
			_spriteBlendModeFX = GetComponentInParent<SpriteBlendModeFX>();
		}

		private void Update()
		{
			_textMesh.text = _spriteBlendModeFX.blendMode.ToString();
		}
	}
}
