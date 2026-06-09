using System;
using UnityEngine;

// Token: 0x0200014F RID: 335
public class GetColor : MonoBehaviour
{
	// Token: 0x060006CA RID: 1738 RVA: 0x0002598C File Offset: 0x00023B8C
	public void Start()
	{
		if (this.inited)
		{
			return;
		}
		this.inited = true;
		SpriteRenderer component = base.GetComponent<SpriteRenderer>();
		if (component)
		{
			component.color = ColorHandler.instance.GetColor(this.colorType);
		}
	}

	// Token: 0x04000821 RID: 2081
	public ColorHandler.ColorType colorType;

	// Token: 0x04000822 RID: 2082
	private bool inited;
}
