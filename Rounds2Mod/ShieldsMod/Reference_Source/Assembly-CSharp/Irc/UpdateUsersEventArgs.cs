using System;

namespace Irc
{
	// Token: 0x020001CE RID: 462
	public class UpdateUsersEventArgs : EventArgs
	{
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000923 RID: 2339 RVA: 0x0002F7CE File Offset: 0x0002D9CE
		// (set) Token: 0x06000924 RID: 2340 RVA: 0x0002F7D6 File Offset: 0x0002D9D6
		public string Channel { get; internal set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000925 RID: 2341 RVA: 0x0002F7DF File Offset: 0x0002D9DF
		// (set) Token: 0x06000926 RID: 2342 RVA: 0x0002F7E7 File Offset: 0x0002D9E7
		public string[] UserList { get; internal set; }

		// Token: 0x06000927 RID: 2343 RVA: 0x0002F7F0 File Offset: 0x0002D9F0
		public UpdateUsersEventArgs(string Channel, string[] UserList)
		{
			this.Channel = Channel;
			this.UserList = UserList;
		}
	}
}
