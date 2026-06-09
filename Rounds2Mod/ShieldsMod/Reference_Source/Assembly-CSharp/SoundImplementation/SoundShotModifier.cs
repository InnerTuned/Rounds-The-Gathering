using System;
using Sonigon;
using UnityEngine;

namespace SoundImplementation
{
	// Token: 0x020001DE RID: 478
	[CreateAssetMenu(fileName = "_SoundShotModifier", menuName = "Sound Implementation/Sound Shot Modifier", order = 0)]
	[Serializable]
	public class SoundShotModifier : ScriptableObject
	{
		// Token: 0x04000AC5 RID: 2757
		[Header("Sound Priority")]
		public int priority;

		// Token: 0x04000AC6 RID: 2758
		[Header("Sound Events")]
		public SoundEvent single;

		// Token: 0x04000AC7 RID: 2759
		public SoundEvent singleAutoLoop;

		// Token: 0x04000AC8 RID: 2760
		public SoundEvent singleAutoTail;

		// Token: 0x04000AC9 RID: 2761
		public SoundEvent shotgun;

		// Token: 0x04000ACA RID: 2762
		public SoundEvent shotgunAutoLoop;

		// Token: 0x04000ACB RID: 2763
		public SoundEvent shotgunAutoTail;
	}
}
