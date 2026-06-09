using System;
using Sonigon;
using UnityEngine;

namespace SoundImplementation
{
	// Token: 0x020001D8 RID: 472
	[Serializable]
	public class SoundHierarchySpawn
	{
		// Token: 0x0600095B RID: 2395 RVA: 0x000307DC File Offset: 0x0002E9DC
		public bool IfAnySoundIsNotNull()
		{
			return this.soundParent != null || this.soundParentStopOnDisable != null || this.soundChild != null || this.soundChildChild != null || this.soundChildChildStopOnDisable != null;
		}

		// Token: 0x04000A9A RID: 2714
		public SoundPolyGrouping soundPolyGrouping = SoundPolyGrouping.perPlayer;

		// Token: 0x04000A9B RID: 2715
		[Header("Sound Parent")]
		public SoundEvent soundParent;

		// Token: 0x04000A9C RID: 2716
		public bool soundParentVelocityToIntensity;

		// Token: 0x04000A9D RID: 2717
		public SoundEvent soundParentStopOnDisable;

		// Token: 0x04000A9E RID: 2718
		[Header("Sound Child")]
		public SoundEvent soundChild;

		// Token: 0x04000A9F RID: 2719
		public bool soundChildVelocityToIntensity;

		// Token: 0x04000AA0 RID: 2720
		[Header("Sound Child Child")]
		public SoundEvent soundChildChild;

		// Token: 0x04000AA1 RID: 2721
		public SoundEvent soundChildChildStopOnDisable;
	}
}
