using System;
using UnityEngine;

// Token: 0x02000076 RID: 118
public class LegRotator : MonoBehaviour
{
	// Token: 0x0600026B RID: 619 RVA: 0x0000FDD2 File Offset: 0x0000DFD2
	private void Start()
	{
		this.rig = base.GetComponentInParent<PlayerVelocity>();
	}

	// Token: 0x0600026C RID: 620 RVA: 0x0000FDE0 File Offset: 0x0000DFE0
	private void Update()
	{
		if (!this.rig)
		{
			return;
		}
		if (this.rig.velocity.x < 0f)
		{
			base.transform.localEulerAngles = Vector3.Lerp(base.transform.localEulerAngles, new Vector3(0f, 0f, 0f), TimeHandler.deltaTime * 15f * Mathf.Clamp(Mathf.Abs(this.rig.velocity.x), 0f, 1f));
			return;
		}
		base.transform.localEulerAngles = Vector3.Lerp(base.transform.localEulerAngles, new Vector3(0f, 180f, 0f), TimeHandler.deltaTime * 15f * Mathf.Clamp(Mathf.Abs(this.rig.velocity.x), 0f, 1f));
	}

	// Token: 0x0400037F RID: 895
	private PlayerVelocity rig;
}
