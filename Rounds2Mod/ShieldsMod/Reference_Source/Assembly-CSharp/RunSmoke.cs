using System;
using UnityEngine;

// Token: 0x020000C1 RID: 193
public class RunSmoke : MonoBehaviour
{
	// Token: 0x06000410 RID: 1040 RVA: 0x00018B24 File Offset: 0x00016D24
	private void Start()
	{
		this.part = base.GetComponent<ParticleSystem>();
		this.data = base.GetComponentInParent<CharacterData>();
		this.partVel = this.part.main;
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x00018B50 File Offset: 0x00016D50
	private void Update()
	{
		this.didTryToEmit = false;
		if (this.data.isGrounded && Mathf.Abs(this.data.playerVel.velocity.x) > 5f)
		{
			base.transform.position = new Vector3(this.data.transform.position.x, this.data.groundPos.y, 5f);
			this.Go();
		}
		else if (this.data.isWallGrab && this.data.wallDistance < 0.7f && this.data.playerVel.velocity.magnitude > 5f)
		{
			base.transform.position = new Vector3(this.data.wallPos.x, this.data.transform.position.y, 5f);
			this.Go();
		}
		if (this.didTryToEmit)
		{
			this.vel = Vector3.Lerp(this.vel, this.data.playerVel.velocity, TimeHandler.deltaTime * 2f);
			return;
		}
		this.vel = Vector3.Lerp(this.vel, Vector3.zero, TimeHandler.deltaTime * 10f);
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x00018CAC File Offset: 0x00016EAC
	private void Go()
	{
		this.didTryToEmit = true;
		this.c += TimeHandler.deltaTime;
		if (this.c > 0.02f)
		{
			this.c = 0f;
			float num = 2f;
			this.part.transform.rotation = Quaternion.LookRotation(this.vel);
			this.partVel.startSpeedMultiplier = this.data.playerVel.velocity.magnitude * num;
			this.part.Emit(2);
		}
	}

	// Token: 0x0400058F RID: 1423
	private CharacterData data;

	// Token: 0x04000590 RID: 1424
	private ParticleSystem part;

	// Token: 0x04000591 RID: 1425
	private float c;

	// Token: 0x04000592 RID: 1426
	private bool groundedLastFrame;

	// Token: 0x04000593 RID: 1427
	private ParticleSystem.MainModule partVel;

	// Token: 0x04000594 RID: 1428
	private bool didTryToEmit;

	// Token: 0x04000595 RID: 1429
	private Vector3 vel;
}
