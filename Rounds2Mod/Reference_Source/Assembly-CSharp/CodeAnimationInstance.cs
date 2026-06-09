using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000030 RID: 48
[Serializable]
public class CodeAnimationInstance
{
	// Token: 0x04000139 RID: 313
	public float animationSpeed = 1f;

	// Token: 0x0400013A RID: 314
	[FoldoutGroup("Random", 0)]
	public float randomSpeedAmount;

	// Token: 0x0400013B RID: 315
	[FoldoutGroup("USE", 0)]
	public bool X = true;

	// Token: 0x0400013C RID: 316
	[FoldoutGroup("USE", 0)]
	public bool Y = true;

	// Token: 0x0400013D RID: 317
	[FoldoutGroup("USE", 0)]
	public bool Z = true;

	// Token: 0x0400013E RID: 318
	[Space(15f)]
	public CodeAnimationInstance.AnimationType animationType;

	// Token: 0x0400013F RID: 319
	public CodeAnimationInstance.AnimationUse animationUse;

	// Token: 0x04000140 RID: 320
	public AnimationCurve curve;

	// Token: 0x04000141 RID: 321
	public float multiplier = 1f;

	// Token: 0x04000142 RID: 322
	public Vector3 direction;

	// Token: 0x04000143 RID: 323
	[FoldoutGroup("Events", 0)]
	public UnityEvent startEvent;

	// Token: 0x04000144 RID: 324
	[FoldoutGroup("Events", 0)]
	public UnityEvent timedEvent;

	// Token: 0x04000145 RID: 325
	[FoldoutGroup("Events", 0)]
	public float eventTiming;

	// Token: 0x04000146 RID: 326
	[FoldoutGroup("Events", 0)]
	public UnityEvent endEvent;

	// Token: 0x02000337 RID: 823
	public enum AnimationType
	{
		// Token: 0x04001061 RID: 4193
		position,
		// Token: 0x04001062 RID: 4194
		scale,
		// Token: 0x04001063 RID: 4195
		rectPosition,
		// Token: 0x04001064 RID: 4196
		floatNumber,
		// Token: 0x04001065 RID: 4197
		rotation
	}

	// Token: 0x02000338 RID: 824
	public enum AnimationUse
	{
		// Token: 0x04001067 RID: 4199
		In,
		// Token: 0x04001068 RID: 4200
		Out,
		// Token: 0x04001069 RID: 4201
		None,
		// Token: 0x0400106A RID: 4202
		Boop
	}
}
