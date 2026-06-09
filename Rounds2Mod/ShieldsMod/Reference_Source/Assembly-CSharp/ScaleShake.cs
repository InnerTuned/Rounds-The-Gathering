using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000C3 RID: 195
public class ScaleShake : MonoBehaviour
{
	// Token: 0x06000417 RID: 1047 RVA: 0x000190B0 File Offset: 0x000172B0
	private void Update()
	{
		float num = Mathf.Clamp(this.useTimeScale ? TimeHandler.deltaTime : Time.unscaledDeltaTime, 0f, 0.02f);
		if (this.clampVelocity != 0f)
		{
			this.velocity = Mathf.Clamp(this.velocity, -this.clampVelocity, this.clampVelocity);
		}
		this.velocity += (this.targetScale - base.transform.localScale.x) * num * 50f * this.spring;
		this.velocity -= this.drag * this.velocity * 20f * num;
		base.transform.localScale += Vector3.one * (this.velocity * 10f * num);
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x0001918F File Offset: 0x0001738F
	public void AddForce(float force)
	{
		this.velocity += force * this.multiplier * 5f;
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x000191AC File Offset: 0x000173AC
	public void AddForce()
	{
		this.velocity += 1f * this.multiplier * 5f;
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x000191CD File Offset: 0x000173CD
	public void SetTarget(float target)
	{
		this.targetScale = target;
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x000191D6 File Offset: 0x000173D6
	public void SetHigh()
	{
		this.targetScale = this.high;
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x000191E4 File Offset: 0x000173E4
	public void SetLow()
	{
		this.targetScale = this.low;
	}

	// Token: 0x0600041D RID: 1053 RVA: 0x000191F2 File Offset: 0x000173F2
	public void ScaleOutRootPhoton()
	{
		base.StartCoroutine(this.GoAwayOutRootPhoton());
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x00019201 File Offset: 0x00017401
	private IEnumerator GoAwayOutRootPhoton()
	{
		while (base.transform.localScale.x > 0f)
		{
			this.targetScale = 0f;
			yield return null;
		}
		PhotonNetwork.Destroy(base.transform.root.gameObject);
		yield break;
	}

	// Token: 0x0400059E RID: 1438
	public bool useTimeScale = true;

	// Token: 0x0400059F RID: 1439
	public float targetScale = 1f;

	// Token: 0x040005A0 RID: 1440
	public float multiplier = 1f;

	// Token: 0x040005A1 RID: 1441
	private float velocity;

	// Token: 0x040005A2 RID: 1442
	public float drag = 1f;

	// Token: 0x040005A3 RID: 1443
	public float spring = 1f;

	// Token: 0x040005A4 RID: 1444
	public float clampVelocity;

	// Token: 0x040005A5 RID: 1445
	internal float high;

	// Token: 0x040005A6 RID: 1446
	internal float low;
}
