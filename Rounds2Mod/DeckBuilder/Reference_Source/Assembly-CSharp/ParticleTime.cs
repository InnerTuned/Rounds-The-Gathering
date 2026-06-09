using System;
using UnityEngine;

// Token: 0x0200017D RID: 381
public class ParticleTime : MonoBehaviour
{
	// Token: 0x060007C1 RID: 1985 RVA: 0x00029BB8 File Offset: 0x00027DB8
	private void Start()
	{
		this.main = base.GetComponentInParent<ParticleSystem>().main;
		this.startTime = this.main.simulationSpeed;
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x00029BDC File Offset: 0x00027DDC
	private void Update()
	{
		this.main.simulationSpeed = this.startTime * TimeHandler.timeScale;
	}

	// Token: 0x04000911 RID: 2321
	private float startTime = 1f;

	// Token: 0x04000912 RID: 2322
	private ParticleSystem.MainModule main;
}
