using System;
using Irc;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020000EA RID: 234
public class TwitchUIHandler : MonoBehaviour
{
	// Token: 0x1700000C RID: 12
	// (get) Token: 0x0600049E RID: 1182 RVA: 0x0001B5DC File Offset: 0x000197DC
	// (set) Token: 0x0600049F RID: 1183 RVA: 0x0001B610 File Offset: 0x00019810
	public static string OAUTH_KEY
	{
		get
		{
			return PlayerPrefs.GetString("TwitchOauth" + SteamUser.GetSteamID().ToString(), string.Empty);
		}
		private set
		{
			PlayerPrefs.SetString("TwitchOauth" + SteamUser.GetSteamID().ToString(), value);
		}
	}

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x060004A0 RID: 1184 RVA: 0x0001B640 File Offset: 0x00019840
	// (set) Token: 0x060004A1 RID: 1185 RVA: 0x0001B674 File Offset: 0x00019874
	public static string TWITCH_NAME_KEY
	{
		get
		{
			return PlayerPrefs.GetString("TwitchName" + SteamUser.GetSteamID().ToString(), string.Empty);
		}
		private set
		{
			PlayerPrefs.SetString("TwitchName" + SteamUser.GetSteamID().ToString(), value);
		}
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x060004A2 RID: 1186 RVA: 0x0001B6A4 File Offset: 0x000198A4
	// (set) Token: 0x060004A3 RID: 1187 RVA: 0x0001B6AB File Offset: 0x000198AB
	public static TwitchUIHandler Instance { get; private set; }

	// Token: 0x060004A4 RID: 1188 RVA: 0x0001B6B3 File Offset: 0x000198B3
	private void Awake()
	{
		TwitchUIHandler.Instance = this;
		this.InitListeners();
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x0001B6C4 File Offset: 0x000198C4
	private void Start()
	{
		TwitchIrc instance = TwitchIrc.Instance;
		instance.OnChannelMessage = (ChannelMessage)Delegate.Combine(instance.OnChannelMessage, new ChannelMessage(this.OnChannelMessage));
		TwitchIrc instance2 = TwitchIrc.Instance;
		instance2.OnUserLeft = (UserLeft)Delegate.Combine(instance2.OnUserLeft, new UserLeft(this.OnUserLeft));
		TwitchIrc instance3 = TwitchIrc.Instance;
		instance3.OnUserJoined = (UserJoined)Delegate.Combine(instance3.OnUserJoined, new UserJoined(this.OnUserJoined));
		TwitchIrc instance4 = TwitchIrc.Instance;
		instance4.OnServerMessage = (ServerMessage)Delegate.Combine(instance4.OnServerMessage, new ServerMessage(this.OnServerMessage));
		TwitchIrc instance5 = TwitchIrc.Instance;
		instance5.OnExceptionThrown = (ExceptionThrown)Delegate.Combine(instance5.OnExceptionThrown, new ExceptionThrown(this.OnExceptionThrown));
		if (!string.IsNullOrEmpty(TwitchUIHandler.OAUTH_KEY))
		{
			this.m_OauthText.text = TwitchUIHandler.OAUTH_KEY;
		}
		if (!string.IsNullOrEmpty(TwitchUIHandler.TWITCH_NAME_KEY))
		{
			this.m_UserNameText.text = TwitchUIHandler.TWITCH_NAME_KEY;
		}
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x0001B7C7 File Offset: 0x000199C7
	private void InitListeners()
	{
		this.m_GetOAuthButton.onClick.AddListener(new UnityAction(this.GetOauth));
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x0001B7E5 File Offset: 0x000199E5
	public void AddMsgAction(Action<string, string> a)
	{
		this.m_OnMsgAction = (Action<string, string>)Delegate.Combine(this.m_OnMsgAction, a);
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x0001B800 File Offset: 0x00019A00
	public void OnContinueClick()
	{
		TwitchUIHandler.OAUTH_KEY = this.m_OauthText.text;
		TwitchUIHandler.TWITCH_NAME_KEY = this.m_UserNameText.text.ToLower();
		TwitchIrc.Instance.Username = TwitchUIHandler.TWITCH_NAME_KEY;
		TwitchIrc.Instance.OauthToken = TwitchUIHandler.OAUTH_KEY;
		TwitchIrc.Instance.Channel = TwitchUIHandler.TWITCH_NAME_KEY;
		TwitchIrc.Instance.Connect();
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x0001B869 File Offset: 0x00019A69
	private void GetOauth()
	{
		Application.OpenURL("http://twitchapps.com/tmi/");
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x000027C8 File Offset: 0x000009C8
	public void MessageSend()
	{
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x0001B875 File Offset: 0x00019A75
	public void GoUrl(string url)
	{
		Application.OpenURL(url);
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x0001B87D File Offset: 0x00019A7D
	private void OnServerMessage(string message)
	{
		global::Debug.Log(message);
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x0001B885 File Offset: 0x00019A85
	private void OnChannelMessage(ChannelMessageEventArgs channelMessageArgs)
	{
		Action<string, string> onMsgAction = this.m_OnMsgAction;
		if (onMsgAction == null)
		{
			return;
		}
		onMsgAction.Invoke(channelMessageArgs.Message, channelMessageArgs.From);
	}

	// Token: 0x060004AE RID: 1198 RVA: 0x0001B8A3 File Offset: 0x00019AA3
	private void OnUserJoined(UserJoinedEventArgs userJoinedArgs)
	{
		if (userJoinedArgs.User.ToUpper() == TwitchIrc.Instance.Username.ToUpper())
		{
			global::Debug.Log("LOCAL USER JOINED!");
			this.ConnectedToTwitch();
		}
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x0001B8D6 File Offset: 0x00019AD6
	private void ConnectedToTwitch()
	{
		if (this.m_Connected)
		{
			return;
		}
		this.m_Connected = true;
		this.m_TwitchBar.Open();
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x000027C8 File Offset: 0x000009C8
	private void OnUserLeft(UserLeftEventArgs userLeftArgs)
	{
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x0001B8F3 File Offset: 0x00019AF3
	private void OnExceptionThrown(Exception exeption)
	{
		global::Debug.Log(exeption);
	}

	// Token: 0x0400063E RID: 1598
	private const string TWITCH_OAUTH_PLAYERPREF_KEY = "TwitchOauth";

	// Token: 0x0400063F RID: 1599
	private const string TWITCH_NAME_PLAYERPREF_KEY = "TwitchName";

	// Token: 0x04000640 RID: 1600
	[SerializeField]
	private TMP_InputField m_UserNameText;

	// Token: 0x04000641 RID: 1601
	[SerializeField]
	private TMP_InputField m_OauthText;

	// Token: 0x04000642 RID: 1602
	[SerializeField]
	private Button m_GetOAuthButton;

	// Token: 0x04000643 RID: 1603
	[SerializeField]
	private ListMenuPage m_TwitchBar;

	// Token: 0x04000644 RID: 1604
	private Action<string, string> m_OnMsgAction;

	// Token: 0x04000645 RID: 1605
	private bool m_Connected;
}
