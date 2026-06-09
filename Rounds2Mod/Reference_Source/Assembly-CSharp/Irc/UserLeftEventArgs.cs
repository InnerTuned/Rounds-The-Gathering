using System;

namespace Irc
{
	// Token: 0x020001D0 RID: 464
	public class UserLeftEventArgs : EventArgs
	{
		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600092D RID: 2349 RVA: 0x0002F83E File Offset: 0x0002DA3E
		// (set) Token: 0x0600092E RID: 2350 RVA: 0x0002F846 File Offset: 0x0002DA46
		public string Channel { get; internal set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600092F RID: 2351 RVA: 0x0002F84F File Offset: 0x0002DA4F
		// (set) Token: 0x06000930 RID: 2352 RVA: 0x0002F857 File Offset: 0x0002DA57
		public string User { get; internal set; }

		// Token: 0x06000931 RID: 2353 RVA: 0x0002F860 File Offset: 0x0002DA60
		public UserLeftEventArgs(string Channel, string User)
		{
			this.Channel = Channel;
			this.User = User;
		}
	}
}
