using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000F0 RID: 240
public class GridLightBulb : GridObject
{
	// Token: 0x060004D5 RID: 1237 RVA: 0x0001BE90 File Offset: 0x0001A090
	private void Start()
	{
		this.rend = base.GetComponent<SpriteRenderer>();
		this.baseColor = this.rend.color;
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x0001BEAF File Offset: 0x0001A0AF
	public override void BopCall(float distance)
	{
		distance = 1f - distance;
		if (!this.isLitUp || (this.isLitUp && distance > this.currentDistance))
		{
			base.StopAllCoroutines();
			base.StartCoroutine(this.LightUp(distance));
		}
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x0001BEE7 File Offset: 0x0001A0E7
	private IEnumerator LightUp(float distance)
	{
		this.isLitUp = true;
		this.currentDistance = distance;
		Color lightColor = Color.Lerp(this.maxLightColor, this.baseColor, distance);
		this.rend.color = lightColor;
		yield return new WaitForSeconds(Random.Range(0.5f, 2f));
		int blinks = Random.Range(0, 8);
		bool isOn = true;
		int num;
		for (int i = 0; i < blinks; i = num + 1)
		{
			if (isOn)
			{
				this.rend.color = this.baseColor;
			}
			else
			{
				this.rend.color = lightColor;
			}
			isOn = !isOn;
			yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
			num = i;
		}
		this.rend.color = this.baseColor;
		this.isLitUp = false;
		yield break;
	}

	// Token: 0x04000663 RID: 1635
	private SpriteRenderer rend;

	// Token: 0x04000664 RID: 1636
	public Color maxLightColor;

	// Token: 0x04000665 RID: 1637
	private Color baseColor;

	// Token: 0x04000666 RID: 1638
	private bool isLitUp;

	// Token: 0x04000667 RID: 1639
	private float currentDistance;
}
