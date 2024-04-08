using UnityEngine;
using UnityEngine.UI;

namespace UTNotifications
{
	[RequireComponent(typeof(UnityEngine.UI.Button))]
	public class ValidatedInputDependent : MonoBehaviour
	{
		public bool AllowWhenPushDisabled;

		public ValidatedInputField[] ValidatedInputFields;

		private UnityEngine.UI.Button button;

		private void Start()
		{
			if (AllowWhenPushDisabled && !PushNotificationsEnabled())
			{
				base.enabled = false;
			}
			else
			{
				button = GetComponent<UnityEngine.UI.Button>();
			}
		}

		private void Update()
		{
			bool interactable = true;
			ValidatedInputField[] validatedInputFields = ValidatedInputFields;
			foreach (ValidatedInputField validatedInputField in validatedInputFields)
			{
				if (!validatedInputField.IsValid())
				{
					interactable = false;
					break;
				}
			}
			button.interactable = interactable;
		}

		private bool PushNotificationsEnabled()
		{
			return Settings.Instance.PushNotificationsEnabledFirebase || Settings.Instance.PushNotificationsEnabledAmazon;
		}
	}
}
