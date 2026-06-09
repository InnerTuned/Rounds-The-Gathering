using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x02000198 RID: 408
[ModifierID("Uniform")]
public class UniformModifier : ProceduralImageModifier
{
	// Token: 0x1700003D RID: 61
	// (get) Token: 0x06000843 RID: 2115 RVA: 0x0002C868 File Offset: 0x0002AA68
	// (set) Token: 0x06000844 RID: 2116 RVA: 0x0002C870 File Offset: 0x0002AA70
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

	// Token: 0x06000845 RID: 2117 RVA: 0x0002C884 File Offset: 0x0002AA84
	public override Vector4 CalculateRadius(Rect imageRect)
	{
		float num = this.radius;
		return new Vector4(num, num, num, num);
	}

	// Token: 0x04000992 RID: 2450
	[SerializeField]
	private float radius;
}
