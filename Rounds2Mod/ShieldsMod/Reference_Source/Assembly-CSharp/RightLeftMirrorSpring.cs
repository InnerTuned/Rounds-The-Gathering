using System;
using UnityEngine;

// Token: 0x020001A6 RID: 422
public class RightLeftMirrorSpring : MonoBehaviour
{
	// Token: 0x06000877 RID: 2167 RVA: 0x0002D282 File Offset: 0x0002B482
	private void Start()
	{
		this.currentRot = base.transform.localEulerAngles.z;
		this.holdable = base.transform.root.GetComponent<Holdable>();
		this.rightPos = base.transform.localPosition;
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x0002D2C4 File Offset: 0x0002B4C4
	private void Update()
	{
		if (!this.holdable || !this.holdable.holder)
		{
			return;
		}
		bool flag = base.transform.root.position.x - 0.1f < this.holdable.holder.transform.position.x;
		Vector3 a = flag ? this.leftPos : this.rightPos;
		float num = flag ? this.leftRot : this.rightRot;
		this.posVel = FRILerp.Lerp(this.posVel, (a - base.transform.localPosition) * this.spring, this.drag);
		this.rotVel = FRILerp.Lerp(this.rotVel, (num - this.currentRot) * this.spring, this.drag);
		this.currentRot += this.rotVel * TimeHandler.deltaTime;
		base.transform.localPosition += this.posVel * TimeHandler.deltaTime;
		base.transform.localEulerAngles = new Vector3(0f, 0f, this.currentRot);
	}

	// Token: 0x040009B3 RID: 2483
	public Vector3 leftPos;

	// Token: 0x040009B4 RID: 2484
	private Vector3 rightPos;

	// Token: 0x040009B5 RID: 2485
	public float leftRot;

	// Token: 0x040009B6 RID: 2486
	public float rightRot;

	// Token: 0x040009B7 RID: 2487
	private Vector3 posVel;

	// Token: 0x040009B8 RID: 2488
	private float rotVel;

	// Token: 0x040009B9 RID: 2489
	public float drag = 25f;

	// Token: 0x040009BA RID: 2490
	public float spring = 25f;

	// Token: 0x040009BB RID: 2491
	private Holdable holdable;

	// Token: 0x040009BC RID: 2492
	private float currentRot;
}
