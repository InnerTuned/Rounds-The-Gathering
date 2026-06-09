using System;
using Irc;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C5 RID: 453
public class TwitchIrcExample : MonoBehaviour
{
	// Token: 0x060008F1 RID: 2289 RVA: 0x0002EFF8 File Offset: 0x0002D1F8
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
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x0002F0C4 File Offset: 0x0002D2C4
	public void Connect()
	{
		TwitchIrc.Instance.Username = this.UsernameText.text;
		TwitchIrc.Instance.OauthToken = this.TokenText.text;
		TwitchIrc.Instance.Channel = this.ChannelText.text;
		TwitchIrc.Instance.Connect();
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x0002F11C File Offset: 0x0002D31C
	public void MessageSend()
	{
		if (string.IsNullOrEmpty(this.MessageText.text))
		{
			return;
		}
		TwitchIrc.Instance.Message(this.MessageText.text);
		Text chatText = this.ChatText;
		chatText.text = string.Concat(new string[]
		{
			chatText.text,
			"<b>",
			TwitchIrc.Instance.Username,
			"</b>: ",
			this.MessageText.text,
			"\n"
		});
		this.MessageText.text = "";
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x0001B875 File Offset: 0x00019A75
	public void GoUrl(string url)
	{
		Application.OpenURL(url);
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x0002F1B5 File Offset: 0x0002D3B5
	private void OnServerMessage(string message)
	{
		Text chatText = this.ChatText;
		chatText.text = chatText.text + "<b>SERVER:</b> " + message + "\n";
		global::Debug.Log(message);
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x0002F1E0 File Offset: 0x0002D3E0
	private void OnChannelMessage(ChannelMessageEventArgs channelMessageArgs)
	{
		Text chatText = this.ChatText;
		chatText.text = string.Concat(new string[]
		{
			chatText.text,
			"<b>",
			channelMessageArgs.From,
			":</b> ",
			channelMessageArgs.Message,
			"\n"
		});
		global::Debug.Log("MESSAGE: " + channelMessageArgs.From + ": " + channelMessageArgs.Message);
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x0002F258 File Offset: 0x0002D458
	private void OnUserJoined(UserJoinedEventArgs userJoinedArgs)
	{
		Text chatText = this.ChatText;
		chatText.text = chatText.text + "<b>USER JOINED:</b> " + userJoinedArgs.User + "\n";
		global::Debug.Log("USER JOINED: " + userJoinedArgs.User);
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x0002F295 File Offset: 0x0002D495
	private void OnUserLeft(UserLeftEventArgs userLeftArgs)
	{
		Text chatText = this.ChatText;
		chatText.text = chatText.text + "<b>USER JOINED:</b> " + userLeftArgs.User + "\n";
		global::Debug.Log("USER JOINED: " + userLeftArgs.User);
	}

	// Token: 0x060008F9 RID: 2297 RVA: 0x0001B8F3 File Offset: 0x00019AF3
	private void OnExceptionThrown(Exception exeption)
	{
		global::Debug.Log(exeption);
	}

	// Token: 0x04000A47 RID: 2631
	public InputField UsernameText;

	// Token: 0x04000A48 RID: 2632
	public InputField TokenText;

	// Token: 0x04000A49 RID: 2633
	public InputField ChannelText;

	// Token: 0x04000A4A RID: 2634
	public Text ChatText;

	// Token: 0x04000A4B RID: 2635
	public InputField MessageText;
}
