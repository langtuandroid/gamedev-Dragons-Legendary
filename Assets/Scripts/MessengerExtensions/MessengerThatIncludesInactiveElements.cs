using System;
using System.Reflection;
using UnityEngine;

namespace MessengerExtensions
{
	public static class MessengerThatIncludesInactiveElements
	{
		private static BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static void InvokeIfExists(this object objectToCheck, string methodName, params object[] parameters)
		{
			MethodInfo methodInfo = null;
			Type type = objectToCheck.GetType();
			while (true)
			{
				methodInfo = type.GetMethod(methodName, flags);
				type = type.BaseType;
				if (methodInfo != null)
				{
					break;
				}
				if (type == null)
				{
					return;
				}
			}
			methodInfo.Invoke(objectToCheck, parameters);
		}

		private static void InvokeIfExists(this object objectToCheck, string methodName)
		{
			MethodInfo methodInfo = null;
			Type type = objectToCheck.GetType();
			while (true)
			{
				methodInfo = type.GetMethod(methodName, flags);
				type = type.BaseType;
				if (methodInfo != null)
				{
					break;
				}
				if (type == null)
				{
					return;
				}
			}
			methodInfo.Invoke(objectToCheck, null);
		}

		public static void InvokeMethod(this GameObject gameobject, string methodName, bool includeInactive, params object[] parameters)
		{
			MonoBehaviour[] components = gameobject.GetComponents<MonoBehaviour>();
			foreach (MonoBehaviour monoBehaviour in components)
			{
				if (includeInactive || monoBehaviour.isActiveAndEnabled)
				{
					monoBehaviour.InvokeIfExists(methodName, parameters);
				}
			}
		}

		public static void InvokeMethod(this GameObject gameobject, string methodName, bool includeInactive)
		{
			MonoBehaviour[] components = gameobject.GetComponents<MonoBehaviour>();
			foreach (MonoBehaviour monoBehaviour in components)
			{
				if (includeInactive || monoBehaviour.isActiveAndEnabled)
				{
					monoBehaviour.InvokeIfExists(methodName);
				}
			}
		}

		public static void InvokeMethod(this Component component, string methodName, bool includeInactive, params object[] parameters)
		{
			component.gameObject.InvokeMethod(methodName, includeInactive, parameters);
		}

		public static void InvokeMethod(this Component component, string methodName, bool includeInactive)
		{
			component.gameObject.InvokeMethod(methodName, includeInactive);
		}

		public static void InvokeMethodInChildren(this GameObject gameobject, string methodName, bool includeInactive, params object[] parameters)
		{
			MonoBehaviour[] componentsInChildren = gameobject.GetComponentsInChildren<MonoBehaviour>(includeInactive);
			foreach (MonoBehaviour monoBehaviour in componentsInChildren)
			{
				if (includeInactive || monoBehaviour.isActiveAndEnabled)
				{
					monoBehaviour.InvokeIfExists(methodName, parameters);
				}
			}
		}

		public static void InvokeMethodInChildren(this GameObject gameobject, string methodName, bool includeInactive)
		{
			MonoBehaviour[] componentsInChildren = gameobject.GetComponentsInChildren<MonoBehaviour>(includeInactive);
			foreach (MonoBehaviour monoBehaviour in componentsInChildren)
			{
				if (includeInactive || monoBehaviour.isActiveAndEnabled)
				{
					monoBehaviour.InvokeIfExists(methodName);
				}
			}
		}

		public static void InvokeMethodInChildren(this Component component, string methodName, bool includeInactive, params object[] parameters)
		{
			component.gameObject.InvokeMethodInChildren(methodName, includeInactive, parameters);
		}

		public static void InvokeMethodInChildren(this Component component, string methodName, bool includeInactive)
		{
			component.gameObject.InvokeMethodInChildren(methodName, includeInactive);
		}

		public static void SendMessageUpwardsToAll(this GameObject gameobject, string methodName, bool includeInactive, params object[] parameters)
		{
			Transform transform = gameobject.transform;
			while (transform != null)
			{
				transform.gameObject.InvokeMethod(methodName, includeInactive, parameters);
				transform = transform.parent;
			}
		}

		public static void SendMessageUpwardsToAll(this Component component, string methodName, bool includeInactive, params object[] parameters)
		{
			component.gameObject.SendMessageUpwardsToAll(methodName, includeInactive, parameters);
		}
	}
}
