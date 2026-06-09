using System;
using UnityEngine;

// Token: 0x02000070 RID: 112
public class IKArmMove : MonoBehaviour
{
	// Token: 0x06000241 RID: 577 RVA: 0x0000E773 File Offset: 0x0000C973
	private void Start()
	{
		this.data = base.GetComponentInParent<CharacterData>();
		this.rig = base.GetComponentInParent<PlayerVelocity>();
		this.startPos = this.target.localPosition;
		this.holding = base.GetComponentInParent<Holding>();
	}

	// Token: 0x06000242 RID: 578 RVA: 0x0000E7AC File Offset: 0x0000C9AC
	private void Update()
	{
		this.isActive = false;
		this.sinceRaise += TimeHandler.deltaTime;
		if (this.holding.holdable && this.holding.holdable.rig && this.holding.holdable.rig.transform.position.x > this.rig.transform.position.x == base.transform.position.x > this.rig.transform.position.x)
		{
			this.target.position = this.holding.holdable.rig.transform.position;
			this.velolcity = Vector3.zero;
			this.isActive = true;
			return;
		}
		Vector3 a = base.transform.parent.TransformPoint(this.startPos) + ((this.sinceRaise < 0.3f) ? Vector3.up : Vector3.zero);
		Vector3 a2 = this.rig.velocity;
		a2.x *= 0.3f;
		this.velolcity = FRILerp.Lerp(this.velolcity, (a - this.target.position) * 15f, 15f);
		this.target.position += this.velolcity * TimeHandler.deltaTime;
		this.target.position += a2 * -0.3f * TimeHandler.deltaTime;
	}

	// Token: 0x06000243 RID: 579 RVA: 0x0000E970 File Offset: 0x0000CB70
	public void RaiseHands()
	{
		if (!this.isActive)
		{
			this.sinceRaise = 0f;
			this.velolcity += Vector3.up * 20f;
		}
	}

	// Token: 0x0400032E RID: 814
	private CharacterData data;

	// Token: 0x0400032F RID: 815
	private PlayerVelocity rig;

	// Token: 0x04000330 RID: 816
	public Transform target;

	// Token: 0x04000331 RID: 817
	private Vector3 startPos;

	// Token: 0x04000332 RID: 818
	private Holding holding;

	// Token: 0x04000333 RID: 819
	private bool isActive;

	// Token: 0x04000334 RID: 820
	private Vector3 velolcity;

	// Token: 0x04000335 RID: 821
	private float sinceRaise = 10f;
}
