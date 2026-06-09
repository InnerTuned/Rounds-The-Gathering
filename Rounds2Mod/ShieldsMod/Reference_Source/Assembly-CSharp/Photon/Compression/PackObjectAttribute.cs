using System;

namespace Photon.Compression
{
	// Token: 0x020001FD RID: 509
	[AttributeUsage(12)]
	public class PackObjectAttribute : Attribute
	{
		// Token: 0x060009E7 RID: 2535 RVA: 0x00032B80 File Offset: 0x00030D80
		public PackObjectAttribute(DefaultKeyRate defaultKeyRate = DefaultKeyRate.Every)
		{
			this.defaultKeyRate = defaultKeyRate;
		}

		// Token: 0x04000B8B RID: 2955
		public DefaultKeyRate defaultKeyRate;

		// Token: 0x04000B8C RID: 2956
		public DefaultPackInclusion defaultInclusion;

		// Token: 0x04000B8D RID: 2957
		public SyncAs syncAs = SyncAs.State;

		// Token: 0x04000B8E RID: 2958
		public string postSnapCallback;

		// Token: 0x04000B8F RID: 2959
		public string postApplyCallback;
	}
}
