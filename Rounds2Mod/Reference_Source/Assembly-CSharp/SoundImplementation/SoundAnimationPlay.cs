using System;
using Sonigon;

namespace SoundImplementation
{
	// Token: 0x020001D4 RID: 468
	[Serializable]
	public class SoundAnimationPlay
	{
		// Token: 0x04000A69 RID: 2665
		public SoundEvent soundEvent;

		// Token: 0x04000A6A RID: 2666
		public CurveAnimationUse curveAnimationUse;

		// Token: 0x04000A6B RID: 2667
		public float soundDelay;

		// Token: 0x04000A6C RID: 2668
		[NonSerialized]
		public bool soundHasPlayed;
	}
}
