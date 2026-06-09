using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000EF RID: 239
public class GridBounceCircle : GridObject
{
	// Token: 0x060004D0 RID: 1232 RVA: 0x0001BE00 File Offset: 0x0001A000
	private void Start()
	{
		this.m_startSize = base.transform.localScale.x;
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x0001BE18 File Offset: 0x0001A018
	public override void BopCall(float power)
	{
		if (!this.isBlooping || (this.isBlooping && power > this.currentDistance))
		{
			base.StopAllCoroutines();
			base.StartCoroutine(this.Blop(power));
		}
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x0001BE47 File Offset: 0x0001A047
	public override void OnSetSize(float size)
	{
		if (!this.isBlooping)
		{
			base.transform.localScale = Vector3.one * size * this.m_startSize;
		}
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x0001BE72 File Offset: 0x0001A072
	private IEnumerator Blop(float power)
	{
		this.isBlooping = true;
		this.currentDistance = power;
		float maxSize = Mathf.Lerp(1f * this.m_startSize, 3f * this.m_startSize, power);
		float timer = 0f;
		while (timer < 1f)
		{
			timer += TimeHandler.deltaTime * 10f;
			base.transform.localScale = Vector3.one * Mathf.Lerp(this.m_startSize, maxSize, timer);
			yield return null;
		}
		yield return new WaitForSeconds(0.2f);
		timer = 0f;
		while (timer < 1f)
		{
			timer += TimeHandler.deltaTime * 6f;
			base.transform.localScale = Vector3.one * Mathf.Lerp(maxSize, this.m_startSize, timer);
			yield return null;
		}
		base.transform.localScale = Vector3.one * this.m_startSize;
		this.isBlooping = false;
		yield break;
	}

	// Token: 0x04000660 RID: 1632
	private float m_startSize;

	// Token: 0x04000661 RID: 1633
	private bool isBlooping;

	// Token: 0x04000662 RID: 1634
	private float currentDistance;
}
