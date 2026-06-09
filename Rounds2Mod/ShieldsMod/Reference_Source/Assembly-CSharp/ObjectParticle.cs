using System;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000060 RID: 96
[Serializable]
public class ObjectParticle
{
	// Token: 0x0400025F RID: 607
	public float size = 1f;

	// Token: 0x04000260 RID: 608
	public AnimationCurve sizeOverTime = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	// Token: 0x04000261 RID: 609
	public float lifetime = 1f;

	// Token: 0x04000262 RID: 610
	public float rotation;

	// Token: 0x04000263 RID: 611
	public float randomRotation;

	// Token: 0x04000264 RID: 612
	[FoldoutGroup("Color", 0)]
	public Color color = Color.magenta;

	// Token: 0x04000265 RID: 613
	[FoldoutGroup("Color", 0)]
	public Color randomColor = Color.magenta;

	// Token: 0x04000266 RID: 614
	[FoldoutGroup("Color", 0)]
	public Color randomAddedColor = Color.black;

	// Token: 0x04000267 RID: 615
	[FoldoutGroup("Color", 0)]
	public float randomAddedSaturation;

	// Token: 0x04000268 RID: 616
	[FoldoutGroup("Color", 0)]
	public bool singleRandomValueColor = true;

	// Token: 0x04000269 RID: 617
	public AnimationCurve alphaOverTime = AnimationCurve.Linear(0f, 1f, 1f, 1f);
}
