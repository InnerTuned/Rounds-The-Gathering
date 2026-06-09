using System;
using UnityEngine;

// Token: 0x020000F6 RID: 246
public abstract class Weapon : MonoBehaviour
{
	// Token: 0x060004EA RID: 1258 RVA: 0x0001C126 File Offset: 0x0001A326
	private void Start()
	{
		this.holdable = base.GetComponent<Holdable>();
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Update()
	{
	}

	// Token: 0x060004EC RID: 1260
	public abstract bool Attack(float charge, bool forceAttack = false, float damageM = 1f, float recoilMultiplier = 1f, bool useAmmo = true);

	// Token: 0x04000673 RID: 1651
	public Holdable holdable;

	// Token: 0x04000674 RID: 1652
	[HideInInspector]
	public float sinceAttack = 10f;
}
