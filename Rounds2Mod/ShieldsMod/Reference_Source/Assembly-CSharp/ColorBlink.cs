using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200011F RID: 287
public class ColorBlink : MonoBehaviour
{
	// Token: 0x060005A8 RID: 1448 RVA: 0x00020698 File Offset: 0x0001E898
	private void Start()
	{
		if (this.inited)
		{
			return;
		}
		this.inited = true;
		this.sprite = base.GetComponent<SpriteRenderer>();
		this.defaultColor = this.sprite.color;
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x000206C7 File Offset: 0x0001E8C7
	public void DoBlink()
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.IDoBlink());
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x000206DC File Offset: 0x0001E8DC
	private IEnumerator IDoBlink()
	{
		if (!this.sprite)
		{
			this.Start();
		}
		this.sprite.color = this.blinkColor;
		yield return new WaitForSeconds(this.timeAmount);
		this.sprite.color = this.defaultColor;
		yield break;
	}

	// Token: 0x04000747 RID: 1863
	public Color blinkColor;

	// Token: 0x04000748 RID: 1864
	public float timeAmount;

	// Token: 0x04000749 RID: 1865
	private Color defaultColor;

	// Token: 0x0400074A RID: 1866
	private SpriteRenderer sprite;

	// Token: 0x0400074B RID: 1867
	private bool inited;
}
