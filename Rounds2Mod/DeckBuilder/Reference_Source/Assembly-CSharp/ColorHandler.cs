using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000120 RID: 288
public class ColorHandler : SerializedMonoBehaviour
{
	// Token: 0x060005AC RID: 1452 RVA: 0x000206EB File Offset: 0x0001E8EB
	private void Awake()
	{
		ColorHandler.instance = this;
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x000206F3 File Offset: 0x0001E8F3
	public Color GetColor(ColorHandler.ColorType colorType)
	{
		return this.colors[colorType];
	}

	// Token: 0x0400074C RID: 1868
	public Dictionary<ColorHandler.ColorType, Color> colors = new Dictionary<ColorHandler.ColorType, Color>();

	// Token: 0x0400074D RID: 1869
	public static ColorHandler instance;

	// Token: 0x02000389 RID: 905
	public enum ColorType
	{
		// Token: 0x040011F3 RID: 4595
		PhysicsObject
	}
}
