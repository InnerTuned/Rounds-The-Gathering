using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x02000197 RID: 407
[ModifierID("Round")]
public class RoundModifier : ProceduralImageModifier
{
	// Token: 0x06000841 RID: 2113 RVA: 0x0002C845 File Offset: 0x0002AA45
	public override Vector4 CalculateRadius(Rect imageRect)
	{
		float num = Mathf.Min(imageRect.width, imageRect.height) * 0.5f;
		return new Vector4(num, num, num, num);
	}
}
