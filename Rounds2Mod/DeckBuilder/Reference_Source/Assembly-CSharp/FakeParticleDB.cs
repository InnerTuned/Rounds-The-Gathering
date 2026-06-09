using System;
using UnityEngine;

// Token: 0x02000055 RID: 85
[CreateAssetMenu(fileName = "FakeParticleDB", menuName = "FakeParticleDB", order = 999999999)]
public class FakeParticleDB : ScriptableObject
{
	// Token: 0x06000190 RID: 400 RVA: 0x0000A766 File Offset: 0x00008966
	public static Material GetParticleMaterial()
	{
		return FakeParticleDB.instance.m_ParticleMaterial;
	}

	// Token: 0x06000191 RID: 401 RVA: 0x0000A772 File Offset: 0x00008972
	public static Material GetDefaultMaterial()
	{
		return FakeParticleDB.instance.m_DefaultMaterial;
	}

	// Token: 0x17000006 RID: 6
	// (get) Token: 0x06000192 RID: 402 RVA: 0x0000A77E File Offset: 0x0000897E
	private static FakeParticleDB instance
	{
		get
		{
			if (FakeParticleDB._inst == null)
			{
				FakeParticleDB._inst = (Resources.Load("FakeParticleDB") as FakeParticleDB);
			}
			return FakeParticleDB._inst;
		}
	}

	// Token: 0x04000229 RID: 553
	public Material m_DefaultMaterial;

	// Token: 0x0400022A RID: 554
	public Material m_ParticleMaterial;

	// Token: 0x0400022B RID: 555
	private static FakeParticleDB _inst;
}
