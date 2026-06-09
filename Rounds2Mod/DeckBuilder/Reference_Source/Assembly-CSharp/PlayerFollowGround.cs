using System;
using UnityEngine;

// Token: 0x0200018A RID: 394
public class PlayerFollowGround : MonoBehaviour
{
	// Token: 0x06000800 RID: 2048 RVA: 0x0002BC99 File Offset: 0x00029E99
	private void Start()
	{
		this.data = base.GetComponent<CharacterData>();
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x0002BCA8 File Offset: 0x00029EA8
	private void FixedUpdate()
	{
		if (this.data.standOnRig == null || !this.data.isGrounded)
		{
			this.lastPos = Vector2.zero;
			return;
		}
		if (this.lastPos != Vector2.zero && this.data.standOnRig == this.lastRig)
		{
			this.data.playerVel.transform.position = this.data.playerVel.position + (this.data.standOnRig.position - this.lastPos);
		}
		this.lastPos = this.data.standOnRig.position;
		this.lastRig = this.data.standOnRig;
	}

	// Token: 0x0400095B RID: 2395
	private CharacterData data;

	// Token: 0x0400095C RID: 2396
	private Vector2 lastPos;

	// Token: 0x0400095D RID: 2397
	private Rigidbody2D lastRig;
}
