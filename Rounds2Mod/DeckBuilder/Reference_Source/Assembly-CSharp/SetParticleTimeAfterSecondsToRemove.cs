using System;
using UnityEngine;

// Token: 0x020000CC RID: 204
public class SetParticleTimeAfterSecondsToRemove : MonoBehaviour
{
	// Token: 0x06000439 RID: 1081 RVA: 0x00019A74 File Offset: 0x00017C74
	private void Start()
	{
		ParticleSystem component = base.GetComponent<ParticleSystem>();
		component.Stop();
		ParticleSystem.MainModule main = component.main;
		main.duration = base.GetComponentInParent<RemoveAfterSeconds>().seconds - main.startLifetime.constant;
		component.Play();
	}
}
