using System;
using UnityEngine;

namespace CompilerGenerated
{
	[Serializable]
	internal sealed class _0024adaptor_0024__GeneralSimpleUiJS_0024callable3_002417_25___0024Action_00242
	{
		protected __GeneralSimpleUiJS_0024callable3_002417_25__ _0024from;

		public _0024adaptor_0024__GeneralSimpleUiJS_0024callable3_002417_25___0024Action_00242(__GeneralSimpleUiJS_0024callable3_002417_25__ from)
		{
			_0024from = from;
		}

		public void Invoke(Vector3 force)
		{
			_0024from(force);
		}

		public static Action<Vector3> Adapt(__GeneralSimpleUiJS_0024callable3_002417_25__ from)
		{
			return new _0024adaptor_0024__GeneralSimpleUiJS_0024callable3_002417_25___0024Action_00242(from).Invoke;
		}
	}
}
