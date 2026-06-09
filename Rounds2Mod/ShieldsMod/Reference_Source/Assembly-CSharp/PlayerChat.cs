using System;
using System.Collections;
using TMPro;
using UnityEngine;

// Token: 0x02000097 RID: 151
public class PlayerChat : MonoBehaviour
{
	// Token: 0x06000351 RID: 849 RVA: 0x0001499B File Offset: 0x00012B9B
	private void Start()
	{
		this.messageText = this.target.GetComponentInChildren<TextMeshProUGUI>();
		this.shaker = this.target.GetComponentInChildren<Screenshaker>();
		this.target.transform.localScale = Vector3.zero;
	}

	// Token: 0x06000352 RID: 850 RVA: 0x000149D4 File Offset: 0x00012BD4
	private void Update()
	{
		this.sinceType -= Time.unscaledDeltaTime;
		if (this.sinceType < 0f)
		{
			this.targetScale = 0f;
		}
		else
		{
			this.targetScale = 1f;
		}
		this.vel = FRILerp.Lerp(this.vel, (this.targetScale - this.currentScale) * this.spring, this.damper);
		this.currentScale += Time.unscaledDeltaTime * this.vel;
		if (this.currentScale < 0f)
		{
			this.vel = 0f;
			this.currentScale = 0f;
		}
		this.target.transform.localScale = Vector3.one * this.currentScale;
	}

	// Token: 0x06000353 RID: 851 RVA: 0x00014AA0 File Offset: 0x00012CA0
	public void Send(string message)
	{
		message = ChatFilter.instance.FilterMessage(message);
		this.messageText.text = message;
		if (this.sinceType > 0f)
		{
			this.vel += this.impulse;
		}
		this.sinceType = 2f + (float)message.Length * 0.05f;
		this.targetScale = 1f;
		if (message.ToUpper() == message)
		{
			base.StartCoroutine(this.ShakeOverTime(this.sinceType));
		}
	}

	// Token: 0x06000354 RID: 852 RVA: 0x00014B2B File Offset: 0x00012D2B
	private IEnumerator ShakeOverTime(float t)
	{
		float a = t;
		while (a > 0f)
		{
			this.shaker.OnUIGameFeel(this.shakeAmount * Random.insideUnitCircle);
			a -= Time.unscaledDeltaTime * this.shakeSpeed;
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000471 RID: 1137
	public float spring = 15f;

	// Token: 0x04000472 RID: 1138
	public float damper = 15f;

	// Token: 0x04000473 RID: 1139
	public float impulse;

	// Token: 0x04000474 RID: 1140
	public float targetScale;

	// Token: 0x04000475 RID: 1141
	private TextMeshProUGUI messageText;

	// Token: 0x04000476 RID: 1142
	private ScaleShake scaleShake;

	// Token: 0x04000477 RID: 1143
	private float currentScale;

	// Token: 0x04000478 RID: 1144
	private float vel;

	// Token: 0x04000479 RID: 1145
	public Transform target;

	// Token: 0x0400047A RID: 1146
	private Screenshaker shaker;

	// Token: 0x0400047B RID: 1147
	private float sinceType;

	// Token: 0x0400047C RID: 1148
	public float shakeAmount;

	// Token: 0x0400047D RID: 1149
	public float shakeSpeed = 1f;
}
