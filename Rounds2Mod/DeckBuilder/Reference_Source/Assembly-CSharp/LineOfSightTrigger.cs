using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200015E RID: 350
public class LineOfSightTrigger : MonoBehaviour
{
	// Token: 0x06000706 RID: 1798 RVA: 0x000268BF File Offset: 0x00024ABF
	private void Start()
	{
		this.player = base.GetComponentInParent<Player>();
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x000268D0 File Offset: 0x00024AD0
	private void Update()
	{
		Player closestPlayerInTeam = PlayerManager.instance.GetClosestPlayerInTeam(base.transform.position, PlayerManager.instance.GetOtherTeam(this.player.teamID), false);
		if (closestPlayerInTeam)
		{
			if (this.currentTarget != this.player)
			{
				this.switchTargetEvent.Invoke();
				Action<Player> action = this.switchTargetAction;
				if (action != null)
				{
					action.Invoke(this.player);
				}
				this.currentTarget = closestPlayerInTeam;
			}
			if (!this.isOn)
			{
				this.isOn = true;
				Action<Player> action2 = this.turnOnAction;
				if (action2 != null)
				{
					action2.Invoke(this.player);
				}
				this.turnOnEvent.Invoke();
				return;
			}
		}
		else
		{
			if (this.isOn)
			{
				this.isOn = false;
				Action action3 = this.turnOffAction;
				if (action3 != null)
				{
					action3.Invoke();
				}
				this.turnOffEvent.Invoke();
			}
			this.currentTarget = null;
		}
	}

	// Token: 0x0400086E RID: 2158
	public UnityEvent turnOnEvent;

	// Token: 0x0400086F RID: 2159
	public UnityEvent turnOffEvent;

	// Token: 0x04000870 RID: 2160
	public UnityEvent switchTargetEvent;

	// Token: 0x04000871 RID: 2161
	public Action<Player> turnOnAction;

	// Token: 0x04000872 RID: 2162
	public Action<Player> switchTargetAction;

	// Token: 0x04000873 RID: 2163
	public Action turnOffAction;

	// Token: 0x04000874 RID: 2164
	private Player player;

	// Token: 0x04000875 RID: 2165
	private bool isOn;

	// Token: 0x04000876 RID: 2166
	private Player currentTarget;
}
