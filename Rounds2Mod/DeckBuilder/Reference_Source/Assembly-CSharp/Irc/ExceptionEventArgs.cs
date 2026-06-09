using System;

namespace Irc
{
	// Token: 0x020001D3 RID: 467
	public class ExceptionEventArgs : EventArgs
	{
		// Token: 0x1700004A RID: 74
		// (get) Token: 0x0600093D RID: 2365 RVA: 0x0002F8EE File Offset: 0x0002DAEE
		// (set) Token: 0x0600093E RID: 2366 RVA: 0x0002F8F6 File Offset: 0x0002DAF6
		public Exception Exception { get; internal set; }

		// Token: 0x0600093F RID: 2367 RVA: 0x0002F8FF File Offset: 0x0002DAFF
		public ExceptionEventArgs(Exception x)
		{
			this.Exception = x;
		}

		// Token: 0x06000940 RID: 2368 RVA: 0x0002F90E File Offset: 0x0002DB0E
		public override string ToString()
		{
			return this.Exception.ToString();
		}
	}
}
