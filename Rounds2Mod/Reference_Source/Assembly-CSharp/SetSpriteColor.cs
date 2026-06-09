using System;
using UnityEngine;

// Token: 0x020001B0 RID: 432
public class SetSpriteColor : MonoBehaviour
{
	// Token: 0x0600089B RID: 2203 RVA: 0x0002D97C File Offset: 0x0002BB7C
	private void Start()
	{
		this.sprite = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x0002D98A File Offset: 0x0002BB8A
	public void SetColor(int id)
	{
		this.sprite.color = this.cols[id];
	}

	// Token: 0x040009D7 RID: 2519
	private SpriteRenderer sprite;

	// Token: 0x040009D8 RID: 2520
	public Color[] cols;
}
