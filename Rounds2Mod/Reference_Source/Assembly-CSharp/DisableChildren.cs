using System;
using UnityEngine;

// Token: 0x0200004A RID: 74
public class DisableChildren : MonoBehaviour
{
	// Token: 0x0600016A RID: 362 RVA: 0x000092A8 File Offset: 0x000074A8
	private void Start()
	{
		ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.SetActive(false);
		}
	}
}
