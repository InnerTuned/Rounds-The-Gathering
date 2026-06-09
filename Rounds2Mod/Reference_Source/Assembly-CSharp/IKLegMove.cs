using System;
using UnityEngine;

// Token: 0x02000072 RID: 114
public class IKLegMove : MonoBehaviour
{
	// Token: 0x06000259 RID: 601 RVA: 0x0000F180 File Offset: 0x0000D380
	private void Start()
	{
		this.rig = base.GetComponentInParent<Rigidbody2D>();
		this.data = base.GetComponentInParent<CharacterData>();
		this.defaultAllowedFootDistance = Vector3.Distance(base.transform.position, this.foot.position);
	}

	// Token: 0x0600025A RID: 602 RVA: 0x0000F1BC File Offset: 0x0000D3BC
	private void LateUpdate()
	{
		this.allowedFootDistance = (this.defaultAllowedFootDistance = base.transform.root.localScale.x);
		this.animationLength = this.footCurveUp.keys[this.footCurveUp.keys.Length - 1].time;
		if (!this.data.isGrounded)
		{
			this.animationTime = 1f;
			this.hasLanded = false;
		}
		this.distanceToFoot = Vector3.Distance(base.transform.position + this.rig.velocity * 0.01f, this.target.position);
		this.velocity = this.rig.velocity;
		if (this.velocity.y > 0f)
		{
			this.velocity.y = this.velocity.y * -1f;
		}
		this.animationTime += TimeHandler.deltaTime;
		this.RayCastForFootPosition();
		this.touchesGround = (this.footCurveUp.Evaluate(this.animationTime) < 0.1f);
		this.DoSteps();
		this.MoveFoot();
	}

	// Token: 0x0600025B RID: 603 RVA: 0x0000F2F4 File Offset: 0x0000D4F4
	private void DoSteps()
	{
		float num = 0f;
		if (this.target.position.x > base.transform.position.x == this.rig.velocity.x > 0f)
		{
			num += Mathf.Abs(this.rig.velocity.x) * 0.25f;
		}
		float num2 = Mathf.Clamp(this.animationTime * 0.4f - 0.23f, 0f, 0.2f);
		if ((this.distanceToFoot > this.allowedFootDistance + num - num2 && this.data.isGrounded && (this.otherLeg.animationTime > this.animationLength * 0.5f || this.animationLength > 2f) && this.animationTime > this.animationLength) || !this.hasLanded)
		{
			this.hasLanded = true;
			this.currentHitTransform = this.hitTransform;
			this.previousTransformGroundPos = this.currentTransformGroundPos;
			this.currentTransformGroundPos = this.transformGroundPos;
			this.animationTime = 0f;
			return;
		}
		Vector3 vector = this.transformGroundPos - this.currentTransformGroundPos;
		vector *= Mathf.Clamp(this.footCurveUp.Evaluate(this.animationTime) * 1f, 0f, 1f);
		this.previousTransformGroundPos += vector;
		this.currentTransformGroundPos += vector;
	}

	// Token: 0x0600025C RID: 604 RVA: 0x0000F478 File Offset: 0x0000D678
	private void MoveFoot()
	{
		if (!this.data.isGrounded)
		{
			this.target.position = Vector3.Lerp(this.target.position, base.transform.position + Vector3.down * this.allowedFootDistance * 0.5f, TimeHandler.deltaTime * 1f);
			this.target.position += this.rig.velocity * 0.1f * TimeHandler.deltaTime;
			return;
		}
		if (this.currentHitTransform)
		{
			Vector3 vector = this.currentHitTransform.TransformPoint(this.currentTransformGroundPos);
			Vector3 a = this.currentHitTransform.TransformPoint(this.previousTransformGroundPos);
			Vector3 a2 = vector;
			float d = 0f;
			if (this.animationTime < this.animationLength)
			{
				a2 = Vector3.Lerp(a, vector, this.animationTime / this.animationLength);
				d = this.footCurveUp.Evaluate(this.animationTime);
			}
			this.target.position = a2 + Vector3.up * d;
		}
	}

	// Token: 0x0600025D RID: 605 RVA: 0x0000F5AC File Offset: 0x0000D7AC
	private void RayCastForFootPosition()
	{
		RaycastHit2D[] array = Physics2D.RaycastAll(base.transform.position, Vector3.down + this.velocity * 0.2f, 3f, this.mask);
		RaycastHit2D raycastHit2D = default(RaycastHit2D);
		float num = float.PositiveInfinity;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].transform.root != base.transform.root && array[i].distance < num)
			{
				raycastHit2D = array[i];
				num = array[i].distance;
			}
		}
		if (raycastHit2D.transform)
		{
			this.hitTransform = raycastHit2D.transform;
			this.transformGroundPos = this.hitTransform.InverseTransformPoint(raycastHit2D.point);
		}
	}

	// Token: 0x04000354 RID: 852
	private Rigidbody2D rig;

	// Token: 0x04000355 RID: 853
	public bool touchesGround;

	// Token: 0x04000356 RID: 854
	public IKLegMove otherLeg;

	// Token: 0x04000357 RID: 855
	public Transform target;

	// Token: 0x04000358 RID: 856
	public Transform foot;

	// Token: 0x04000359 RID: 857
	private Vector3 transformGroundPos;

	// Token: 0x0400035A RID: 858
	private Vector3 currentTransformGroundPos;

	// Token: 0x0400035B RID: 859
	private Vector3 previousTransformGroundPos;

	// Token: 0x0400035C RID: 860
	private Transform hitTransform;

	// Token: 0x0400035D RID: 861
	private Transform currentHitTransform;

	// Token: 0x0400035E RID: 862
	[HideInInspector]
	public float allowedFootDistance;

	// Token: 0x0400035F RID: 863
	private float defaultAllowedFootDistance;

	// Token: 0x04000360 RID: 864
	public AnimationCurve footCurveUp;

	// Token: 0x04000361 RID: 865
	private float distanceToFoot;

	// Token: 0x04000362 RID: 866
	public LayerMask mask;

	// Token: 0x04000363 RID: 867
	[HideInInspector]
	public float animationTime = 1f;

	// Token: 0x04000364 RID: 868
	private Vector3 velocity;

	// Token: 0x04000365 RID: 869
	private CharacterData data;

	// Token: 0x04000366 RID: 870
	private float animationLength;

	// Token: 0x04000367 RID: 871
	private bool hasLanded;
}
