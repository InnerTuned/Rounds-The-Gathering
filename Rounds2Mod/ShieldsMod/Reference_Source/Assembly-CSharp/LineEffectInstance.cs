using System;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000079 RID: 121
[Serializable]
public class LineEffectInstance
{
	// Token: 0x0400038E RID: 910
	[Space(20f)]
	public bool active = true;

	// Token: 0x0400038F RID: 911
	[FoldoutGroup("Main Curve", 0, Order = 0, Expanded = true)]
	public LineEffectInstance.CurveType curveType;

	// Token: 0x04000390 RID: 912
	[FoldoutGroup("Main Curve", 0, Order = 0, Expanded = true)]
	public AnimationCurve mainCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

	// Token: 0x04000391 RID: 913
	[FoldoutGroup("Main Curve", 0, Order = 0, Expanded = true)]
	public float mainCurveMultiplier = 1f;

	// Token: 0x04000392 RID: 914
	[FoldoutGroup("Main Curve", 0, Order = 0, Expanded = true)]
	public float mainCurveTiling = 1f;

	// Token: 0x04000393 RID: 915
	[FoldoutGroup("Main Curve", 0, Order = 0, Expanded = true)]
	public bool tilingPerMeter = true;

	// Token: 0x04000394 RID: 916
	[FoldoutGroup("Modifiers", 0)]
	public AnimationCurve effectOverLineCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	// Token: 0x04000395 RID: 917
	[FoldoutGroup("Animation", 0)]
	public float mainCurveScrollSpeed;

	// Token: 0x04000396 RID: 918
	[FoldoutGroup("Animation", 0)]
	public AnimationCurve effectOverTimeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x02000359 RID: 857
	public enum CurveType
	{
		// Token: 0x04001119 RID: 4377
		Add,
		// Token: 0x0400111A RID: 4378
		Multiply
	}
}
