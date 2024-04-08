public class LoginFailPopup : GameObjectSingleton<LoginFailPopup>
{
	public void Show()
	{
		base.gameObject.SetActive(value: true);
	}

	public void OnClickSocialConnectRetry()
	{
		Protocol_Set.CallSocialLoginConnect();
		base.gameObject.SetActive(value: false);
	}

	public void OnClickGuestLogin()
	{
		Protocol_Set.CallGuestLoginConnect();
		base.gameObject.SetActive(value: false);
	}

	protected override void Awake()
	{
		base.Awake();
		UnityEngine.Debug.Log("Login FAil Awake");
	}
}
