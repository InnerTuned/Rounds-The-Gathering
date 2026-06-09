using System;
using UnityEngine;

// Token: 0x020001C1 RID: 449
public class TracerTarget : MonoBehaviour
{
	// Token: 0x060008DE RID: 2270 RVA: 0x0002E99C File Offset: 0x0002CB9C
	private void Start()
	{
		this.move = base.GetComponent<MoveTransform>();
		this.random = Random.Range(0f, 1000f);
		this.SetPos(base.transform.forward * 100f, base.transform.up, null);
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x0002E9F4 File Offset: 0x0002CBF4
	private void Update()
	{
		float num = 1f;
		if (this.mTrans)
		{
			num += this.mTrans.velocity.magnitude * 0.1f;
		}
		if (this.mTrans && !this.done)
		{
			this.tPos += base.transform.forward + base.transform.forward * 10f;
		}
		this.c += TimeHandler.deltaTime;
		if (this.hasPos)
		{
			this.move.velocity += this.cosAmount * Mathf.Cos((Time.time + this.random) * this.cosScale) * this.upDir * CappedDeltaTime.time / num;
			this.move.velocity += (this.tPos - base.transform.position).normalized * this.spring * CappedDeltaTime.time * this.curve.Evaluate(this.c) * num;
			this.move.velocity -= this.move.velocity * CappedDeltaTime.time * this.drag * this.curve.Evaluate(this.c);
		}
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x0002EB96 File Offset: 0x0002CD96
	public void SetPos(Vector3 targetPos, Vector3 up, MoveTransform move)
	{
		this.mTrans = move;
		this.hasPos = true;
		this.tPos = targetPos;
		this.upDir = up;
	}

	// Token: 0x04000A1F RID: 2591
	private MoveTransform move;

	// Token: 0x04000A20 RID: 2592
	public bool hasPos;

	// Token: 0x04000A21 RID: 2593
	public float drag;

	// Token: 0x04000A22 RID: 2594
	public float spring;

	// Token: 0x04000A23 RID: 2595
	public AnimationCurve curve;

	// Token: 0x04000A24 RID: 2596
	public float cosScale = 1f;

	// Token: 0x04000A25 RID: 2597
	public float cosAmount;

	// Token: 0x04000A26 RID: 2598
	private float random;

	// Token: 0x04000A27 RID: 2599
	private float c;

	// Token: 0x04000A28 RID: 2600
	private bool done;

	// Token: 0x04000A29 RID: 2601
	private Vector3 tPos;

	// Token: 0x04000A2A RID: 2602
	private Vector3 upDir;

	// Token: 0x04000A2B RID: 2603
	private MoveTransform mTrans;
}
