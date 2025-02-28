using CodeStage.AntiCheat.Detectors;
using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredQuaternion
	{
		[Serializable]
		public struct RawEncryptedQuaternion
		{
			public int x;

			public int y;

			public int z;

			public int w;
		}

		private static int cryptoKey = 120205;

		private static readonly Quaternion identity = Quaternion.identity;

		[SerializeField]
		private int currentCryptoKey;

		[SerializeField]
		private RawEncryptedQuaternion hiddenValue;

		[SerializeField]
		private bool inited;

		[SerializeField]
		private Quaternion fakeValue;

		[SerializeField]
		private bool fakeValueActive;

		private ObscuredQuaternion(Quaternion value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = Encrypt(value);
			bool existsAndIsRunning = ObscuredCheatingDetector.ExistsAndIsRunning;
			fakeValue = ((!existsAndIsRunning) ? identity : value);
			fakeValueActive = existsAndIsRunning;
			inited = true;
		}

		public ObscuredQuaternion(float x, float y, float z, float w)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = Encrypt(x, y, z, w, currentCryptoKey);
			if (ObscuredCheatingDetector.ExistsAndIsRunning)
			{
				fakeValue.x = x;
				fakeValue.y = y;
				fakeValue.z = z;
				fakeValue.w = w;
				fakeValueActive = true;
			}
			else
			{
				fakeValue = identity;
				fakeValueActive = false;
			}
			inited = true;
		}

		public static void SetNewCryptoKey(int newKey)
		{
			cryptoKey = newKey;
		}

		public static RawEncryptedQuaternion Encrypt(Quaternion value)
		{
			return Encrypt(value, 0);
		}

		public static RawEncryptedQuaternion Encrypt(Quaternion value, int key)
		{
			return Encrypt(value.x, value.y, value.z, value.w, key);
		}

		public static RawEncryptedQuaternion Encrypt(float x, float y, float z, float w, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}
			RawEncryptedQuaternion result = default(RawEncryptedQuaternion);
			result.x = ObscuredFloat.Encrypt(x, key);
			result.y = ObscuredFloat.Encrypt(y, key);
			result.z = ObscuredFloat.Encrypt(z, key);
			result.w = ObscuredFloat.Encrypt(w, key);
			return result;
		}

		public static Quaternion Decrypt(RawEncryptedQuaternion value)
		{
			return Decrypt(value, 0);
		}

		public static Quaternion Decrypt(RawEncryptedQuaternion value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}
			Quaternion result = default(Quaternion);
			result.x = ObscuredFloat.Decrypt(value.x, key);
			result.y = ObscuredFloat.Decrypt(value.y, key);
			result.z = ObscuredFloat.Decrypt(value.z, key);
			result.w = ObscuredFloat.Decrypt(value.w, key);
			return result;
		}

		public void ApplyNewCryptoKey()
		{
			if (currentCryptoKey != cryptoKey)
			{
				hiddenValue = Encrypt(InternalDecrypt(), cryptoKey);
				currentCryptoKey = cryptoKey;
			}
		}

		public void RandomizeCryptoKey()
		{
			Quaternion value = InternalDecrypt();
			do
			{
				currentCryptoKey = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			}
			while (currentCryptoKey == 0);
			hiddenValue = Encrypt(value, currentCryptoKey);
		}

		public RawEncryptedQuaternion GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		public void SetEncrypted(RawEncryptedQuaternion encrypted)
		{
			inited = true;
			hiddenValue = encrypted;
			if (currentCryptoKey == 0)
			{
				currentCryptoKey = cryptoKey;
			}
			if (ObscuredCheatingDetector.ExistsAndIsRunning)
			{
				fakeValue = InternalDecrypt();
				fakeValueActive = true;
			}
			else
			{
				fakeValueActive = false;
			}
		}

		public Quaternion GetDecrypted()
		{
			return InternalDecrypt();
		}

		private Quaternion InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(identity);
				fakeValue = identity;
				fakeValueActive = false;
				inited = true;
				return identity;
			}
			Quaternion quaternion = default(Quaternion);
			quaternion.x = ObscuredFloat.Decrypt(hiddenValue.x, currentCryptoKey);
			quaternion.y = ObscuredFloat.Decrypt(hiddenValue.y, currentCryptoKey);
			quaternion.z = ObscuredFloat.Decrypt(hiddenValue.z, currentCryptoKey);
			quaternion.w = ObscuredFloat.Decrypt(hiddenValue.w, currentCryptoKey);
			if (ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && !CompareQuaternionsWithTolerance(quaternion, fakeValue))
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return quaternion;
		}

		private bool CompareQuaternionsWithTolerance(Quaternion q1, Quaternion q2)
		{
			float quaternionEpsilon = ObscuredCheatingDetector.Instance.quaternionEpsilon;
			return Math.Abs(q1.x - q2.x) < quaternionEpsilon && Math.Abs(q1.y - q2.y) < quaternionEpsilon && Math.Abs(q1.z - q2.z) < quaternionEpsilon && Math.Abs(q1.w - q2.w) < quaternionEpsilon;
		}

		public static implicit operator ObscuredQuaternion(Quaternion value)
		{
			return new ObscuredQuaternion(value);
		}

		public static implicit operator Quaternion(ObscuredQuaternion value)
		{
			return value.InternalDecrypt();
		}

		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}
	}
}
