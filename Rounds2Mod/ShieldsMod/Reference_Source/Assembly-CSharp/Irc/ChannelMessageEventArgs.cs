using System;

namespace Irc
{
	// Token: 0x020001D1 RID: 465
	public class ChannelMessageEventArgs : EventArgs
	{
		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000932 RID: 2354 RVA: 0x0002F876 File Offset: 0x0002DA76
		// (set) Token: 0x06000933 RID: 2355 RVA: 0x0002F87E File Offset: 0x0002DA7E
		public string Channel { get; internal set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000934 RID: 2356 RVA: 0x0002F887 File Offset: 0x0002DA87
		// (set) Token: 0x06000935 RID: 2357 RVA: 0x0002F88F File Offset: 0x0002DA8F
		public string From { get; internal set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000936 RID: 2358 RVA: 0x0002F898 File Offset: 0x0002DA98
		// (set) Token: 0x06000937 RID: 2359 RVA: 0x0002F8A0 File Offset: 0x0002DAA0
		public string Message { get; internal set; }

		// Token: 0x06000938 RID: 2360 RVA: 0x0002F8A9 File Offset: 0x0002DAA9
		public ChannelMessageEventArgs(string Channel, string From, string Message)
		{
			this.Channel = Channel;
			this.From = From;
			this.Message = Message;
		}
	}
}
