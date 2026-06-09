using System;

namespace Irc
{
	// Token: 0x020001D2 RID: 466
	public class StringEventArgs : EventArgs
	{
		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000939 RID: 2361 RVA: 0x0002F8C6 File Offset: 0x0002DAC6
		// (set) Token: 0x0600093A RID: 2362 RVA: 0x0002F8CE File Offset: 0x0002DACE
		public string Result { get; internal set; }

		// Token: 0x0600093B RID: 2363 RVA: 0x0002F8D7 File Offset: 0x0002DAD7
		public StringEventArgs(string s)
		{
			this.Result = s;
		}

		// Token: 0x0600093C RID: 2364 RVA: 0x0002F8E6 File Offset: 0x0002DAE6
		public override string ToString()
		{
			return this.Result;
		}
	}
}
