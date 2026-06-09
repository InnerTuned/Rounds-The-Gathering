using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Irc;
using UnityEngine;

// Token: 0x020001CC RID: 460
public class TwitchIrc : MonoBehaviour
{
	// Token: 0x06000913 RID: 2323 RVA: 0x0002F2D4 File Offset: 0x0002D4D4
	public void Connect()
	{
		if (string.IsNullOrEmpty(this.Username) || string.IsNullOrEmpty(this.OauthToken))
		{
			return;
		}
		try
		{
			this.irc = new TcpClient("irc.twitch.tv", 6667);
			this.stream = this.irc.GetStream();
			this.reader = new StreamReader(this.stream);
			this.writer = new StreamWriter(this.stream);
			this.Send("USER " + this.Username + "tmi twitch :" + this.Username);
			this.Send("PASS " + this.OauthToken);
			this.Send("NICK " + this.Username);
			base.StartCoroutine("Listen");
		}
		catch (Exception exeption)
		{
			if (this.OnExceptionThrown != null)
			{
				this.OnExceptionThrown(exeption);
			}
		}
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x0002F3C8 File Offset: 0x0002D5C8
	public void Disconnect()
	{
		this.irc = null;
		base.StopCoroutine("Listen");
		if (this.stream != null)
		{
			this.stream.Dispose();
		}
		if (this.writer != null)
		{
			this.writer.Dispose();
		}
		if (this.reader != null)
		{
			this.reader.Dispose();
		}
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x0002F420 File Offset: 0x0002D620
	public void JoinChannel()
	{
		if (string.IsNullOrEmpty(this.Channel))
		{
			return;
		}
		if (this.Channel.get_Chars(0) != '#')
		{
			this.Channel = "#" + this.Channel;
		}
		if (this.irc != null && this.irc.Connected)
		{
			this.Send("JOIN " + this.Channel);
		}
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x0002F48C File Offset: 0x0002D68C
	public void LeaveChannel()
	{
		this.Send("PART " + this.Channel);
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x0002F4A4 File Offset: 0x0002D6A4
	public void Message(string message)
	{
		this.Send("PRIVMSG " + this.Channel + " :" + message);
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x0002F4C2 File Offset: 0x0002D6C2
	private IEnumerator Listen()
	{
		for (;;)
		{
			if (this.stream.DataAvailable && (this.inputLine = this.reader.ReadLine()) != null)
			{
				this.ParseData(this.inputLine);
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x0002F4D4 File Offset: 0x0002D6D4
	private void ParseData(string data)
	{
		string[] array = data.Split(new char[]
		{
			' '
		});
		if (data.Length > 4 && data.Substring(0, 4) == "PING")
		{
			this.Send("PONG " + array[1]);
			return;
		}
		string text = array[1];
		if (!(text == "001"))
		{
			if (!(text == "JOIN"))
			{
				if (!(text == "PRIVMSG"))
				{
					if (!(text == "PART") && !(text == "QUIT"))
					{
						if (array.Length > 3 && this.OnServerMessage != null)
						{
							this.OnServerMessage(this.JoinArray(array, 3));
						}
					}
					else if (this.OnUserLeft != null)
					{
						this.OnUserLeft(new UserLeftEventArgs(array[2], array[0].Substring(1, data.IndexOf("!") - 1)));
						return;
					}
				}
				else if (array[2].ToLower() != this.Username.ToLower() && this.OnChannelMessage != null)
				{
					this.OnChannelMessage(new ChannelMessageEventArgs(array[2], array[0].Substring(1, array[0].IndexOf('!') - 1), this.JoinArray(array, 3)));
					return;
				}
			}
			else if (TwitchIrc.Instance.OnUserJoined != null)
			{
				TwitchIrc.Instance.OnUserJoined(new UserJoinedEventArgs(array[2], array[0].Substring(1, array[0].IndexOf("!") - 1)));
				return;
			}
			return;
		}
		this.Send("MODE " + this.Username + " +B");
		this.OnConnected();
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x0002F68C File Offset: 0x0002D88C
	private string StripMessage(string message)
	{
		foreach (object obj in new Regex("\u0003(?:\\d{1,2}(?:,\\d{1,2})?)?").Matches(message))
		{
			Match match = (Match)obj;
			message = message.Replace(match.Value, "");
		}
		if (message == "")
		{
			return "";
		}
		if (message.Substring(0, 1) == ":" && message.Length > 2)
		{
			return message.Substring(1, message.Length - 1);
		}
		return message;
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x0002F73C File Offset: 0x0002D93C
	private string JoinArray(string[] strArray, int startIndex)
	{
		return this.StripMessage(string.Join(" ", strArray, startIndex, strArray.Length - startIndex));
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x0002F755 File Offset: 0x0002D955
	private void Send(string message)
	{
		this.writer.WriteLine(message);
		this.writer.Flush();
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x0002F76E File Offset: 0x0002D96E
	private void OnConnectedToServer()
	{
		this.JoinChannel();
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x0002F776 File Offset: 0x0002D976
	private void Awake()
	{
		TwitchIrc.Instance = this;
		this.OnConnected = (Connected)Delegate.Combine(this.OnConnected, new Connected(this.OnConnectedToServer));
		if (this.ConnectOnAwake)
		{
			this.Connect();
		}
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x0002F7AE File Offset: 0x0002D9AE
	private void OnDisable()
	{
		this.Disconnect();
	}

	// Token: 0x04000A4C RID: 2636
	public UserJoined OnUserJoined;

	// Token: 0x04000A4D RID: 2637
	public UserLeft OnUserLeft;

	// Token: 0x04000A4E RID: 2638
	public ChannelMessage OnChannelMessage;

	// Token: 0x04000A4F RID: 2639
	public ServerMessage OnServerMessage;

	// Token: 0x04000A50 RID: 2640
	public Connected OnConnected;

	// Token: 0x04000A51 RID: 2641
	public ExceptionThrown OnExceptionThrown;

	// Token: 0x04000A52 RID: 2642
	private const string ServerName = "irc.twitch.tv";

	// Token: 0x04000A53 RID: 2643
	private const int ServerPort = 6667;

	// Token: 0x04000A54 RID: 2644
	public static TwitchIrc Instance;

	// Token: 0x04000A55 RID: 2645
	public bool ConnectOnAwake;

	// Token: 0x04000A56 RID: 2646
	public string Username;

	// Token: 0x04000A57 RID: 2647
	public string OauthToken;

	// Token: 0x04000A58 RID: 2648
	public string Channel;

	// Token: 0x04000A59 RID: 2649
	private TcpClient irc;

	// Token: 0x04000A5A RID: 2650
	private NetworkStream stream;

	// Token: 0x04000A5B RID: 2651
	private string inputLine;

	// Token: 0x04000A5C RID: 2652
	private StreamReader reader;

	// Token: 0x04000A5D RID: 2653
	private StreamWriter writer;
}
