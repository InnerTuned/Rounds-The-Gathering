using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000039 RID: 57
[Serializable]
public class CurveAnimationInstance
{
	// Token: 0x0600010D RID: 269 RVA: 0x00007C31 File Offset: 0x00005E31
	public AnimationCurve Curve()
	{
		if (this.animationUse == CurveAnimationUse.Boop)
		{
			return this.boopCurve;
		}
		if (this.animationUse != CurveAnimationUse.In)
		{
			return this.outCurve;
		}
		return this.inCurve;
	}

	// Token: 0x04000167 RID: 359
	[FoldoutGroup("$animationUse", 0)]
	public CurveAnimationType animationType;

	// Token: 0x04000168 RID: 360
	[FoldoutGroup("$animationUse", 0)]
	public CurveAnimationUse animationUse;

	// Token: 0x04000169 RID: 361
	[ShowIf("animationUse", CurveAnimationUse.In, true)]
	[FoldoutGroup("$animationUse", 0)]
	public AnimationCurve inCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400016A RID: 362
	[ShowIf("animationUse", CurveAnimationUse.Out, true)]
	[FoldoutGroup("$animationUse", 0)]
	public AnimationCurve outCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x0400016B RID: 363
	[ShowIf("animationUse", CurveAnimationUse.Boop, true)]
	[FoldoutGroup("$animationUse", 0)]
	public AnimationCurve boopCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	// Token: 0x0400016C RID: 364
	[HideIf("animationType", CurveAnimationType.Scale, true)]
	[FoldoutGroup("$animationUse", 0)]
	public Vector3 animDirection;

	// Token: 0x0400016D RID: 365
	[FoldoutGroup("$animationUse/Settings", 0)]
	public bool loop;

	// Token: 0x0400016E RID: 366
	[FoldoutGroup("$animationUse/Settings", 0)]
	public bool playOnAwake;

	// Token: 0x0400016F RID: 367
	[FoldoutGroup("$animationUse/Settings", 0)]
	public float speed = 1f;

	// Token: 0x04000170 RID: 368
	[FoldoutGroup("$animationUse/Settings", 0)]
	public float multiplier = 1f;

	// Token: 0x04000171 RID: 369
	[FoldoutGroup("$animationUse/Events", 0)]
	public UnityEvent statEvent;

	// Token: 0x04000172 RID: 370
	[FoldoutGroup("$animationUse/Events", 0)]
	public UnityEvent endEvent;

	// Token: 0x04000173 RID: 371
	[FoldoutGroup("$animationUse/Events", 0)]
	public UnityEvent delayedEvent;

	// Token: 0x04000174 RID: 372
	[FoldoutGroup("$animationUse/Events", 0)]
	public float delay;

	// Token: 0x04000175 RID: 373
	[FoldoutGroup("$animationUse/Debug", 0, Order = 0, Expanded = false)]
	public bool isPlaying;

	// Token: 0x04000176 RID: 374
	[HideInInspector]
	public Coroutine animation;
}
