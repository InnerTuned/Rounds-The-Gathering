using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x02000195 RID: 405
[ModifierID("Free")]
public class FreeModifier : ProceduralImageModifier
{
	// Token: 0x1700003A RID: 58
	// (get) Token: 0x06000836 RID: 2102 RVA: 0x0002C6AD File Offset: 0x0002A8AD
	// (set) Token: 0x06000837 RID: 2103 RVA: 0x0002C6B5 File Offset: 0x0002A8B5
	public Vector4 Radius
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

	// Token: 0x06000838 RID: 2104 RVA: 0x0002C6AD File Offset: 0x0002A8AD
	public override Vector4 CalculateRadius(Rect imageRect)
	{
		return this.radius;
	}

	// Token: 0x06000839 RID: 2105 RVA: 0x0002C6CC File Offset: 0x0002A8CC
	protected void OnValidate()
	{
		this.radius.x = Mathf.Max(0f, this.radius.x);
		this.radius.y = Mathf.Max(0f, this.radius.y);
		this.radius.z = Mathf.Max(0f, this.radius.z);
		this.radius.w = Mathf.Max(0f, this.radius.w);
	}

	// Token: 0x0400098F RID: 2447
	[SerializeField]
	private Vector4 radius;
}
