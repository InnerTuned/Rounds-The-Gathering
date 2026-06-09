using System;
using UnityEngine;

// Token: 0x02000086 RID: 134
public class MusicVisualizerData : MonoBehaviour
{
	// Token: 0x060002DA RID: 730 RVA: 0x000123ED File Offset: 0x000105ED
	private void Awake()
	{
		this.m_audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x060002DB RID: 731 RVA: 0x000123FB File Offset: 0x000105FB
	private void Update()
	{
		this.m_audioSource.GetSpectrumData(MusicVisualizerData.Samples, 0, 4);
	}

	// Token: 0x04000409 RID: 1033
	public static float[] Samples = new float[512];

	// Token: 0x0400040A RID: 1034
	private AudioSource m_audioSource;
}
