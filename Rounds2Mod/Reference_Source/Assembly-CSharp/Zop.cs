using System;
using UnityEngine;

// Token: 0x020000FA RID: 250
public class Zop : MonoBehaviour
{
	// Token: 0x060004FD RID: 1277 RVA: 0x0001C73E File Offset: 0x0001A93E
	private void Start()
	{
		this.move = base.GetComponentInParent<MoveTransform>();
		if (base.transform.forward.x < 0f)
		{
			this.up = true;
		}
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x0001C76C File Offset: 0x0001A96C
	private void Update()
	{
		this.count += TimeHandler.deltaTime;
		if (this.count > this.turn)
		{
			if (this.up)
			{
				this.move.velocity = this.move.velocity.magnitude * Vector3.Cross(base.transform.forward, Vector3.forward);
			}
			else
			{
				this.move.velocity = this.move.velocity.magnitude * -Vector3.Cross(base.transform.forward, Vector3.forward);
			}
			this.count = 0f;
			if (this.randomZop && Random.value > 0.8f)
			{
				return;
			}
			this.sinceSwitch++;
			if (this.sinceSwitch == 2)
			{
				this.up = !this.up;
				this.sinceSwitch = 0;
			}
		}
	}

	// Token: 0x0400068E RID: 1678
	public float turn = 0.2f;

	// Token: 0x0400068F RID: 1679
	private float count;

	// Token: 0x04000690 RID: 1680
	private int sinceSwitch = 1;

	// Token: 0x04000691 RID: 1681
	private bool up;

	// Token: 0x04000692 RID: 1682
	private MoveTransform move;

	// Token: 0x04000693 RID: 1683
	public bool randomZop;
}
