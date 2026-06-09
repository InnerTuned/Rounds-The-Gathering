using System;

namespace Photon.Pun.Simple
{
	// Token: 0x02000297 RID: 663
	public struct AnimPassThru
	{
		// Token: 0x06000E62 RID: 3682 RVA: 0x00044F95 File Offset: 0x00043195
		public AnimPassThru(PassThruType triggerType, int hash, int layer, float normTime, float otherTime, float duration, LocalApplyTiming localApplyTiming = LocalApplyTiming.OnSend)
		{
			this.passThruType = triggerType;
			this.hash = hash;
			this.normlTime = normTime;
			this.fixedTime = otherTime;
			this.duration = duration;
			this.layer = layer;
			this.localApplyTiming = localApplyTiming;
		}

		// Token: 0x04000D81 RID: 3457
		public PassThruType passThruType;

		// Token: 0x04000D82 RID: 3458
		public int hash;

		// Token: 0x04000D83 RID: 3459
		public float normlTime;

		// Token: 0x04000D84 RID: 3460
		public float fixedTime;

		// Token: 0x04000D85 RID: 3461
		public float duration;

		// Token: 0x04000D86 RID: 3462
		public int layer;

		// Token: 0x04000D87 RID: 3463
		public LocalApplyTiming localApplyTiming;
	}
}
