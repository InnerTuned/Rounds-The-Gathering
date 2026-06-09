using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200011A RID: 282
public class Chase : MonoBehaviour
{
	// Token: 0x0600058F RID: 1423 RVA: 0x000200C5 File Offset: 0x0001E2C5
	private void Start()
	{
		this.lineEffect = base.GetComponentInChildren<LineEffect>(true);
		this.player = base.GetComponentInParent<Player>();
	}

	// Token: 0x06000590 RID: 1424 RVA: 0x000200E0 File Offset: 0x0001E2E0
	private void Update()
	{
		Player player = PlayerManager.instance.GetClosestPlayerInTeam(base.transform.position, PlayerManager.instance.GetOtherTeam(this.player.teamID), true);
		if (player && (Vector2.Angle(player.transform.position - base.transform.position, this.player.data.input.direction) > 70f || this.player.data.input.direction == Vector3.zero))
		{
			player = null;
		}
		if (player)
		{
			if (this.currentTarget != this.player)
			{
				this.currentTarget = this.player;
				this.switchTargetEvent.Invoke();
				this.lineEffect.Play(base.transform, player.transform, 0f);
			}
			if (!this.isOn)
			{
				this.isOn = true;
				this.turnOnEvent.Invoke();
				return;
			}
		}
		else
		{
			if (this.isOn)
			{
				this.isOn = false;
				this.turnOffEvent.Invoke();
			}
			if (this.lineEffect.isPlaying)
			{
				this.lineEffect.Stop();
				this.lineEffect.gameObject.SetActive(false);
			}
			this.currentTarget = null;
		}
	}

	// Token: 0x0400072F RID: 1839
	public UnityEvent turnOnEvent;

	// Token: 0x04000730 RID: 1840
	public UnityEvent turnOffEvent;

	// Token: 0x04000731 RID: 1841
	public UnityEvent switchTargetEvent;

	// Token: 0x04000732 RID: 1842
	private Player player;

	// Token: 0x04000733 RID: 1843
	private LineEffect lineEffect;

	// Token: 0x04000734 RID: 1844
	private bool isOn;

	// Token: 0x04000735 RID: 1845
	private Player currentTarget;
}
