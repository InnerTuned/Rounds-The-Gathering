using System;
using UnityEngine;

// Token: 0x02000007 RID: 7
public class Aim : MonoBehaviour
{
	// Token: 0x06000026 RID: 38 RVA: 0x00002D66 File Offset: 0x00000F66
	private void Awake()
	{
		this.input = base.GetComponent<GeneralInput>();
		this.data = base.GetComponent<CharacterData>();
		this.holdingObject = base.GetComponentInChildren<HoldingObject>();
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00002D8C File Offset: 0x00000F8C
	private void Update()
	{
		if ((double)this.input.aimDirection.magnitude > 0.2)
		{
			this.aimDirection = this.input.aimDirection;
		}
		if (this.input.direction.magnitude > 0.2f && Optionshandler.leftStickAim && this.input.aimDirection == Vector3.zero)
		{
			this.aimDirection = this.input.direction;
		}
		if (!this.holdingObject)
		{
			return;
		}
		if (this.aimDirection != Vector3.zero)
		{
			this.holdingObject.transform.rotation = Quaternion.LookRotation(this.aimDirection);
		}
		this.data.aimDirection = this.aimDirection;
	}

	// Token: 0x0400001A RID: 26
	private GeneralInput input;

	// Token: 0x0400001B RID: 27
	private HoldingObject holdingObject;

	// Token: 0x0400001C RID: 28
	private CharacterData data;

	// Token: 0x0400001D RID: 29
	private Vector3 aimDirection;
}
