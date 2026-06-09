using System;
using Photon.Utilities;

namespace Photon.Pun.Simple.Internal
{
	// Token: 0x02000302 RID: 770
	[Serializable]
	public class ParameterDefaults
	{
		// Token: 0x04000F6A RID: 3946
		public ParameterInterpolation interpolateFloats = ParameterInterpolation.Hold;

		// Token: 0x04000F6B RID: 3947
		public ParameterInterpolation interpolateInts = ParameterInterpolation.Hold;

		// Token: 0x04000F6C RID: 3948
		public ParameterExtrapolation extrapolateFloats = ParameterExtrapolation.Hold;

		// Token: 0x04000F6D RID: 3949
		public ParameterExtrapolation extrapolateInts = ParameterExtrapolation.Hold;

		// Token: 0x04000F6E RID: 3950
		public ParameterExtrapolation extrapolateBools = ParameterExtrapolation.Hold;

		// Token: 0x04000F6F RID: 3951
		public ParameterExtrapolation extrapolateTriggers;

		// Token: 0x04000F70 RID: 3952
		public bool includeFloats = true;

		// Token: 0x04000F71 RID: 3953
		public bool includeInts = true;

		// Token: 0x04000F72 RID: 3954
		public bool includeBools = true;

		// Token: 0x04000F73 RID: 3955
		public bool includeTriggers;

		// Token: 0x04000F74 RID: 3956
		public SmartVar defaultFloat = 0f;

		// Token: 0x04000F75 RID: 3957
		public SmartVar defaultInt = 0;

		// Token: 0x04000F76 RID: 3958
		public SmartVar defaultBool = false;

		// Token: 0x04000F77 RID: 3959
		public SmartVar defaultTrigger = false;
	}
}
