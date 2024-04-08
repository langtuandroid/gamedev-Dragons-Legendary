using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UTNotifications
{
	public class Popup : MonoBehaviour
	{
		public GameObject ItemPrefab;

		public void AddItem(string label, UnityAction action)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(ItemPrefab, base.transform, worldPositionStays: false);
			Text componentInChildren = gameObject.GetComponentInChildren<Text>();
			componentInChildren.text = label;
			UnityEngine.UI.Button componentInChildren2 = gameObject.GetComponentInChildren<UnityEngine.UI.Button>();
			componentInChildren2.onClick.AddListener(action);
		}
	}
}
