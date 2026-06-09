using System;
using UnityEngine;

// Token: 0x02000116 RID: 278
public class CharacterItemMirror : MonoBehaviour
{
	// Token: 0x06000581 RID: 1409 RVA: 0x0001FA9D File Offset: 0x0001DC9D
	private void Start()
	{
		this.player = base.GetComponentInParent<Player>();
		if (base.transform.localPosition.x > 0f)
		{
			this.leftRight = LeftRight.Right;
			return;
		}
		this.leftRight = LeftRight.Left;
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x0001FAD4 File Offset: 0x0001DCD4
	private void Update()
	{
		LeftRight leftRight = this.leftRight;
		if (this.player.data.playerVel.velocity.x > this.speedThreshol)
		{
			leftRight = LeftRight.Right;
		}
		if (this.player.data.playerVel.velocity.x < -this.speedThreshol)
		{
			leftRight = LeftRight.Left;
		}
		if (leftRight != this.leftRight)
		{
			base.transform.localPosition = new Vector3(base.transform.localPosition.x * -1f, base.transform.localPosition.y, base.transform.localPosition.z);
			base.transform.localScale = new Vector3(base.transform.localScale.x * -1f, base.transform.localScale.y, base.transform.localScale.z);
			this.leftRight = leftRight;
		}
	}

	// Token: 0x04000721 RID: 1825
	private float speedThreshol = 3f;

	// Token: 0x04000722 RID: 1826
	private LeftRight leftRight;

	// Token: 0x04000723 RID: 1827
	private Player player;
}
