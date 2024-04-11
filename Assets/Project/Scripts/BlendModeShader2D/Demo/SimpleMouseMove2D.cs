using UnityEngine;

namespace BlendModeShader2D.Demo
{
	public class SimpleMouseMove2D : MonoBehaviour
	{
		public float moveSpeed = 0.1f;

		private Vector2 _targetPos;

		private void Start()
		{
			_targetPos = base.transform.position;
		}

		private void Update()
		{
			if (Input.GetMouseButton(1))
			{
				_targetPos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
			}
			if (Vector2.Distance(base.transform.position, _targetPos) > 0.05f)
			{
				base.transform.position = Vector2.Lerp(base.transform.position, _targetPos, moveSpeed * Time.deltaTime);
			}
		}
	}
}
