using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200002E RID: 46
public class ChomaticAberrationFeeler : GameFeeler
{
	// Token: 0x060000E8 RID: 232 RVA: 0x00007234 File Offset: 0x00005434
	public override void OnAwake()
	{
		this.postProcessVolume = base.GetComponent<PostProcessVolume>();
		if (!this.postProcessVolume.profile.TryGetSettings<ChromaticAberration>(ref this.chromaticAberration))
		{
			global::Debug.LogError("No ChromaticAberration in post!");
			return;
		}
		this.targetIntensity = this.chromaticAberration.intensity.value;
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x00007288 File Offset: 0x00005488
	public override void OnGameFeel(Vector2 feelDirection)
	{
		feelDirection = Vector2.ClampMagnitude(feelDirection, 50f);
		if (feelDirection.magnitude < this.threshold)
		{
			feelDirection = feelDirection.normalized * this.threshold * 0.3f;
		}
		this.velocity += feelDirection.sqrMagnitude * 0.2f * this.force;
	}

	// Token: 0x060000EA RID: 234 RVA: 0x000072F0 File Offset: 0x000054F0
	private void Update()
	{
		this.velocity *= this.damper;
		this.intensity *= this.damperSpring;
		this.intensity += this.velocity * Mathf.Clamp(TimeHandler.deltaTime, 0f, 0.02f);
		this.chromaticAberration.intensity.value = this.intensity + this.targetIntensity;
	}

	// Token: 0x060000EB RID: 235 RVA: 0x00007368 File Offset: 0x00005568
	private void FixedUpdate()
	{
		this.intensity *= 0.9f;
	}

	// Token: 0x060000EC RID: 236 RVA: 0x000027C8 File Offset: 0x000009C8
	public override void OnUIGameFeel(Vector2 feelDirection)
	{
	}

	// Token: 0x0400012B RID: 299
	public float force = 10f;

	// Token: 0x0400012C RID: 300
	public float damper = 0.95f;

	// Token: 0x0400012D RID: 301
	public float damperSpring = 0.95f;

	// Token: 0x0400012E RID: 302
	public float threshold;

	// Token: 0x0400012F RID: 303
	private PostProcessVolume postProcessVolume;

	// Token: 0x04000130 RID: 304
	private ChromaticAberration chromaticAberration;

	// Token: 0x04000131 RID: 305
	private float intensity;

	// Token: 0x04000132 RID: 306
	private float velocity;

	// Token: 0x04000133 RID: 307
	private float targetIntensity;
}
