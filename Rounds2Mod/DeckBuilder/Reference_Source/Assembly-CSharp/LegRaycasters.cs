using System;
using UnityEngine;

// Token: 0x02000074 RID: 116
public class LegRaycasters : MonoBehaviour
{
	// Token: 0x06000263 RID: 611 RVA: 0x0000F7A4 File Offset: 0x0000D9A4
	private void Awake()
	{
		this.legs = base.transform.root.GetComponentsInChildren<IkLeg>();
	}

	// Token: 0x06000264 RID: 612 RVA: 0x0000F7BC File Offset: 0x0000D9BC
	private void Start()
	{
		this.rig = base.GetComponentInParent<PlayerVelocity>();
		this.data = base.GetComponentInParent<CharacterData>();
	}

	// Token: 0x06000265 RID: 613 RVA: 0x0000F7D8 File Offset: 0x0000D9D8
	private void FixedUpdate()
	{
		this.totalStepTime = 0f;
		for (int i = 0; i < this.legs.Length; i++)
		{
			if (!this.legs[i].footDown)
			{
				this.totalStepTime += this.legs[i].stepTime;
			}
		}
		for (int j = 0; j < this.legCastPositions.Length; j++)
		{
			RaycastHit2D[] array = Physics2D.RaycastAll(this.legCastPositions[j].transform.position + Vector3.up * 0.5f, Vector2.down, 1f * base.transform.root.localScale.x, this.mask);
			for (int k = 0; k < array.Length; k++)
			{
				if (array[k].transform && array[k].transform.root != base.transform.root)
				{
					this.HitGround(this.legCastPositions[j], array[k]);
					break;
				}
			}
		}
	}

	// Token: 0x06000266 RID: 614 RVA: 0x0000F8FC File Offset: 0x0000DAFC
	private void HitGround(Transform leg, RaycastHit2D hit)
	{
		if (this.data.sinceJump < 0.2f)
		{
			return;
		}
		if (Vector3.Angle(Vector3.up, hit.normal) > 70f)
		{
			return;
		}
		this.data.TouchGround(hit.point, hit.normal, hit.rigidbody, null);
		Vector3 vector = (hit.point - leg.transform.position) / base.transform.root.localScale.x;
		if (this.data.input.direction.x != 0f)
		{
			vector.y += this.wobbleCurve.Evaluate(this.totalStepTime) * base.transform.root.localScale.x;
			this.rig.AddForce(Vector3.up * this.forceCurve.Evaluate(this.totalStepTime) * this.rig.mass);
		}
		this.rig.AddForce(this.animationCurve.Evaluate(Mathf.Abs(vector.y)) * Vector3.up * this.rig.mass * this.force);
		this.rig.AddForce(this.animationCurve.Evaluate(Mathf.Abs(vector.y)) * -this.rig.velocity.y * Vector2.up * this.rig.mass * this.drag);
	}

	// Token: 0x0400036D RID: 877
	public LayerMask mask;

	// Token: 0x0400036E RID: 878
	public float force;

	// Token: 0x0400036F RID: 879
	public float drag;

	// Token: 0x04000370 RID: 880
	public Transform[] legCastPositions;

	// Token: 0x04000371 RID: 881
	public AnimationCurve animationCurve;

	// Token: 0x04000372 RID: 882
	private PlayerVelocity rig;

	// Token: 0x04000373 RID: 883
	private CharacterData data;

	// Token: 0x04000374 RID: 884
	public AnimationCurve wobbleCurve;

	// Token: 0x04000375 RID: 885
	public AnimationCurve forceCurve;

	// Token: 0x04000376 RID: 886
	private IkLeg[] legs;

	// Token: 0x04000377 RID: 887
	private float totalStepTime;
}
