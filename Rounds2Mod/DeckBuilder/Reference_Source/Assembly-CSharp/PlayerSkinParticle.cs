using System;
using UnityEngine;

// Token: 0x020000A2 RID: 162
public class PlayerSkinParticle : MonoBehaviour
{
	// Token: 0x06000399 RID: 921 RVA: 0x00015F8C File Offset: 0x0001418C
	public void Init(int spriteLayerID)
	{
		this.part = base.GetComponent<ParticleSystem>();
		this.part.GetComponent<ParticleSystemRenderer>().sortingLayerID = spriteLayerID;
		this.main = this.part.main;
		this.startColor1 = this.main.startColor.colorMin;
		this.startColor2 = this.main.startColor.colorMax;
		this.part.Play();
	}

	// Token: 0x0600039A RID: 922 RVA: 0x00016004 File Offset: 0x00014204
	private void Update()
	{
		this.counter += TimeHandler.deltaTime;
	}

	// Token: 0x0600039B RID: 923 RVA: 0x00016018 File Offset: 0x00014218
	private void OnEnable()
	{
		if (this.part)
		{
			this.part.Play();
		}
	}

	// Token: 0x0600039C RID: 924 RVA: 0x00016034 File Offset: 0x00014234
	public void BlinkColor(Color blinkColor)
	{
		if (this.counter < 0.1f)
		{
			return;
		}
		this.counter = 0f;
		this.particles = new ParticleSystem.Particle[this.part.main.maxParticles];
		int num = this.part.GetParticles(this.particles);
		for (int i = 0; i < num; i++)
		{
			this.particles[i].startColor = blinkColor;
		}
		this.part.SetParticles(this.particles, num);
	}

	// Token: 0x040004B2 RID: 1202
	private Color startColor1;

	// Token: 0x040004B3 RID: 1203
	private Color startColor2;

	// Token: 0x040004B4 RID: 1204
	private ParticleSystem.MainModule main;

	// Token: 0x040004B5 RID: 1205
	private ParticleSystem part;

	// Token: 0x040004B6 RID: 1206
	private ParticleSystem.Particle[] particles;

	// Token: 0x040004B7 RID: 1207
	private float counter;
}
