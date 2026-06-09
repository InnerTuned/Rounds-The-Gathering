using System;

namespace Irc
{
	// Token: 0x020001CF RID: 463
	public class UserJoinedEventArgs : EventArgs
	{
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000928 RID: 2344 RVA: 0x0002F806 File Offset: 0x0002DA06
		// (set) Token: 0x06000929 RID: 2345 RVA: 0x0002F80E File Offset: 0x0002DA0E
		public string Channel { get; internal set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600092A RID: 2346 RVA: 0x0002F817 File Offset: 0x0002DA17
		// (set) Token: 0x0600092B RID: 2347 RVA: 0x0002F81F File Offset: 0x0002DA1F
		public string User { get; internal set; }

		// Token: 0x0600092C RID: 2348 RVA: 0x0002F828 File Offset: 0x0002DA28
		public UserJoinedEventArgs(string Channel, string User)
		{
			this.Channel = Channel;
			this.User = User;
		}
	}
}
