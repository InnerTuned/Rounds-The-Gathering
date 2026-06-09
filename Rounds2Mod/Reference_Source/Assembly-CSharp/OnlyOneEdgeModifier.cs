using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x02000196 RID: 406
[ModifierID("Only One Edge")]
public class OnlyOneEdgeModifier : ProceduralImageModifier
{
	// Token: 0x1700003B RID: 59
	// (get) Token: 0x0600083B RID: 2107 RVA: 0x0002C761 File Offset: 0x0002A961
	// (set) Token: 0x0600083C RID: 2108 RVA: 0x0002C769 File Offset: 0x0002A969
	public float Radius
	{
		get
		{
			return this.radius;
		}
		set
		{
			this.radius = value;
			base._Graphic.SetVerticesDirty();
		}
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x0600083D RID: 2109 RVA: 0x0002C77D File Offset: 0x0002A97D
	// (set) Token: 0x0600083E RID: 2110 RVA: 0x0002C785 File Offset: 0x0002A985
	public OnlyOneEdgeModifier.ProceduralImageEdge Side
	{
		get
		{
			return this.side;
		}
		set
		{
			this.side = value;
		}
	}

	// Token: 0x0600083F RID: 2111 RVA: 0x0002C790 File Offset: 0x0002A990
	public override Vector4 CalculateRadius(Rect imageRect)
	{
		switch (this.side)
		{
		case OnlyOneEdgeModifier.ProceduralImageEdge.Top:
			return new Vector4(this.radius, this.radius, 0f, 0f);
		case OnlyOneEdgeModifier.ProceduralImageEdge.Bottom:
			return new Vector4(0f, 0f, this.radius, this.radius);
		case OnlyOneEdgeModifier.ProceduralImageEdge.Left:
			return new Vector4(this.radius, 0f, 0f, this.radius);
		case OnlyOneEdgeModifier.ProceduralImageEdge.Right:
			return new Vector4(0f, this.radius, this.radius, 0f);
		default:
			return new Vector4(0f, 0f, 0f, 0f);
		}
	}

	// Token: 0x04000990 RID: 2448
	[SerializeField]
	private float radius;

	// Token: 0x04000991 RID: 2449
	[SerializeField]
	private OnlyOneEdgeModifier.ProceduralImageEdge side;

	// Token: 0x020003A9 RID: 937
	public enum ProceduralImageEdge
	{
		// Token: 0x0400127E RID: 4734
		Top,
		// Token: 0x0400127F RID: 4735
		Bottom,
		// Token: 0x04001280 RID: 4736
		Left,
		// Token: 0x04001281 RID: 4737
		Right
	}
}
