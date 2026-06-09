using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200000E RID: 14
public class AttackTrigger : MonoBehaviour
{
	// Token: 0x06000046 RID: 70 RVA: 0x00003F08 File Offset: 0x00002108
	private void Start()
	{
		this.data = base.GetComponentInParent<CharacterData>();
		Gun gun = this.data.weaponHandler.gun;
		gun.ShootPojectileAction = (Action<GameObject>)Delegate.Combine(gun.ShootPojectileAction, new Action<GameObject>(this.Shoot));
		this.data.weaponHandler.gun.AddAttackAction(new Action(this.Attack));
	}

	// Token: 0x06000047 RID: 71 RVA: 0x00003F74 File Offset: 0x00002174
	private void OnDestroy()
	{
		Gun gun = this.data.weaponHandler.gun;
		gun.ShootPojectileAction = (Action<GameObject>)Delegate.Remove(gun.ShootPojectileAction, new Action<GameObject>(this.Shoot));
		this.data.weaponHandler.gun.RemoveAttackAction(new Action(this.Attack));
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00003FD3 File Offset: 0x000021D3
	public void Attack()
	{
		if (!this.triggerOnEveryShot)
		{
			this.triggerEvent.Invoke();
		}
	}

	// Token: 0x06000049 RID: 73 RVA: 0x00003FE8 File Offset: 0x000021E8
	public void Shoot(GameObject projectile)
	{
		if (this.triggerOnEveryShot)
		{
			this.triggerEvent.Invoke();
		}
	}

	// Token: 0x04000034 RID: 52
	public bool triggerOnEveryShot = true;

	// Token: 0x04000035 RID: 53
	public UnityEvent triggerEvent;

	// Token: 0x04000036 RID: 54
	private CharacterData data;
}
