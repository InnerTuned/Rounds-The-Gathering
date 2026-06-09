using System;
using UnityEngine;

// Token: 0x020000E0 RID: 224
public class StandStillTrigger : MonoBehaviour
{
	// Token: 0x06000474 RID: 1140 RVA: 0x0001A90D File Offset: 0x00018B0D
	private void Start()
	{
		this.data = base.transform.GetComponentInParent<CharacterData>();
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x0001A920 File Offset: 0x00018B20
	private void Update()
	{
		if (this.data.isGrounded && this.data.input.direction.magnitude < 0.1f)
		{
			this.sinceStandStill += TimeHandler.deltaTime;
			return;
		}
		this.sinceStandStill = 0f;
	}

	// Token: 0x04000602 RID: 1538
	public float sinceStandStill;

	// Token: 0x04000603 RID: 1539
	private CharacterData data;
}
