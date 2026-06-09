using System;
using UnityEngine;

// Token: 0x0200014B RID: 331
public class FollowInactiveHand : MonoBehaviour
{
	// Token: 0x060006B4 RID: 1716 RVA: 0x000255EB File Offset: 0x000237EB
	private void Start()
	{
		this.data = base.transform.root.GetComponent<CharacterData>();
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x00025604 File Offset: 0x00023804
	private void Update()
	{
		if (this.data.aimDirection.x < 0f)
		{
			base.transform.position = this.rightHand.transform.TransformPoint(this.offSet);
			return;
		}
		base.transform.position = this.leftHand.transform.TransformPoint(this.offSet);
	}

	// Token: 0x04000819 RID: 2073
	private CharacterData data;

	// Token: 0x0400081A RID: 2074
	public Vector3 offSet;

	// Token: 0x0400081B RID: 2075
	public GameObject leftHand;

	// Token: 0x0400081C RID: 2076
	public GameObject rightHand;
}
