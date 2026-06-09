using System;
using Sonigon;
using UnityEngine;

namespace SoundImplementation
{
	// Token: 0x020001DB RID: 475
	[CreateAssetMenu(fileName = "_SoundImpactModifier", menuName = "Sound Implementation/Sound Impact Modifier", order = 1)]
	[Serializable]
	public class SoundImpactModifier : ScriptableObject
	{
		// Token: 0x04000AAA RID: 2730
		[Header("Sound Priority")]
		public int priority;

		// Token: 0x04000AAB RID: 2731
		[Header("Sound Events")]
		public SoundEvent impactCharacter;

		// Token: 0x04000AAC RID: 2732
		public SoundEvent impactEnvironment;
	}
}
