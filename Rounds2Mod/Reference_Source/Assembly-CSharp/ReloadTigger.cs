using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001A2 RID: 418
public class ReloadTigger : MonoBehaviour
{
	// Token: 0x06000868 RID: 2152 RVA: 0x0002D07C File Offset: 0x0002B27C
	private void Start()
	{
		CharacterStatModifiers componentInParent = base.GetComponentInParent<CharacterStatModifiers>();
		componentInParent.OnReloadDoneAction = (Action<int>)Delegate.Combine(componentInParent.OnReloadDoneAction, new Action<int>(this.OnReloadDone));
		CharacterStatModifiers componentInParent2 = base.GetComponentInParent<CharacterStatModifiers>();
		componentInParent2.OutOfAmmpAction = (Action<int>)Delegate.Combine(componentInParent2.OutOfAmmpAction, new Action<int>(this.OnOutOfAmmo));
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x0002D0D8 File Offset: 0x0002B2D8
	private void OnDestroy()
	{
		CharacterStatModifiers componentInParent = base.GetComponentInParent<CharacterStatModifiers>();
		componentInParent.OnReloadDoneAction = (Action<int>)Delegate.Remove(componentInParent.OnReloadDoneAction, new Action<int>(this.OnReloadDone));
		CharacterStatModifiers componentInParent2 = base.GetComponentInParent<CharacterStatModifiers>();
		componentInParent2.OutOfAmmpAction = (Action<int>)Delegate.Remove(componentInParent2.OutOfAmmpAction, new Action<int>(this.OnOutOfAmmo));
	}

	// Token: 0x0600086A RID: 2154 RVA: 0x0002D133 File Offset: 0x0002B333
	private void OnReloadDone(int bulletsReloaded)
	{
		this.reloadDoneEvent.Invoke();
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x0002D140 File Offset: 0x0002B340
	private void OnOutOfAmmo(int bulletsReloaded)
	{
		this.outOfAmmoEvent.Invoke();
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Update()
	{
	}

	// Token: 0x040009AB RID: 2475
	public UnityEvent reloadDoneEvent;

	// Token: 0x040009AC RID: 2476
	public UnityEvent outOfAmmoEvent;
}
