using System;
using UnityEngine;

// Token: 0x02000194 RID: 404
public static class EmptySprite
{
	// Token: 0x06000834 RID: 2100 RVA: 0x0002C678 File Offset: 0x0002A878
	public static Sprite Get()
	{
		if (EmptySprite.instance == null)
		{
			EmptySprite.instance = Resources.Load<Sprite>("procedural_ui_image_default_sprite");
		}
		return EmptySprite.instance;
	}

	// Token: 0x06000835 RID: 2101 RVA: 0x0002C69B File Offset: 0x0002A89B
	public static bool IsEmptySprite(Sprite s)
	{
		return EmptySprite.Get() == s;
	}

	// Token: 0x0400098E RID: 2446
	private static Sprite instance;
}
