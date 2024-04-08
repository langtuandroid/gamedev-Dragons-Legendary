using System;
using UnityEngine;

namespace CompilerGenerated
{
	[Serializable]
	internal sealed class _0024adaptor_0024__GeneralSimpleUiJS_0024callable5_002429_25___0024Action_00243
	{
		protected __GeneralSimpleUiJS_0024callable5_002429_25__ _0024from;

		public _0024adaptor_0024__GeneralSimpleUiJS_0024callable5_002429_25___0024Action_00243(__GeneralSimpleUiJS_0024callable5_002429_25__ from)
		{
			_0024from = from;
		}

		public void Invoke(Vector3 force)
		{
			_0024from(force);
		}

		public static Action<Vector3> Adapt(__GeneralSimpleUiJS_0024callable5_002429_25__ from)
		{
			return new _0024adaptor_0024__GeneralSimpleUiJS_0024callable5_002429_25___0024Action_00243(from).Invoke;
		}
	}
}
