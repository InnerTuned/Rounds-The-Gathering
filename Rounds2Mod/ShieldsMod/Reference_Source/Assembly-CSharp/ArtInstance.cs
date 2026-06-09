using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200000B RID: 11
[Serializable]
public class ArtInstance
{
	// Token: 0x06000036 RID: 54 RVA: 0x00003C8C File Offset: 0x00001E8C
	public void TogglePart(bool on)
	{
		for (int i = 0; i < this.parts.Length; i++)
		{
			this.parts[i].gameObject.SetActive(on);
			this.parts[i].Play();
		}
	}

	// Token: 0x0400002A RID: 42
	[FoldoutGroup("$profile", 0)]
	public PostProcessProfile profile;

	// Token: 0x0400002B RID: 43
	[FoldoutGroup("$profile", 0)]
	public ParticleSystem[] parts;
}
