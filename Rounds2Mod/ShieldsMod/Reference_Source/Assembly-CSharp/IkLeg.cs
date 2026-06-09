using System;
using Sonigon;
using Sonigon.Internal;
using UnityEngine;

// Token: 0x02000071 RID: 113
public class IkLeg : MonoBehaviour
{
	// Token: 0x06000245 RID: 581 RVA: 0x0000E9B8 File Offset: 0x0000CBB8
	private void Start()
	{
		this.data = base.GetComponentInParent<CharacterData>();
		this.legLenth = Vector3.Distance(base.transform.position, this.footTarget.position);
		this.startOffset = this.footTarget.position - this.data.transform.position;
		this.legRootOffset = this.legRoot.position - this.data.transform.position;
	}

	// Token: 0x06000246 RID: 582 RVA: 0x0000EA48 File Offset: 0x0000CC48
	private void FixedUpdate()
	{
		if (!this.data)
		{
			return;
		}
		this.SetValuesFixed();
	}

	// Token: 0x06000247 RID: 583 RVA: 0x0000EA5E File Offset: 0x0000CC5E
	private void LateUpdate()
	{
		if (!this.data)
		{
			return;
		}
		this.DoRayCast();
		this.UpdateRayCastWorldPos();
		this.DoStep();
		this.UpdatePreviousRayCastWorldPos();
		this.UpdateMiscVariables();
		this.SetFootPos();
		this.Apply();
	}

	// Token: 0x06000248 RID: 584 RVA: 0x0000EA98 File Offset: 0x0000CC98
	private void AirFootMovement()
	{
		this.footPosition = Vector3.Lerp(this.footPosition, this.restPosition + this.data.playerVel.velocity * 0.08f, TimeHandler.deltaTime * 15f);
		this.raycastTransform = null;
	}

	// Token: 0x06000249 RID: 585 RVA: 0x0000EAFC File Offset: 0x0000CCFC
	private bool stepWasLost()
	{
		return Vector3.Distance(this.footPosition, this.legRoot.position) > this.legLenth * 0.5f;
	}

	// Token: 0x0600024A RID: 586 RVA: 0x0000EB2C File Offset: 0x0000CD2C
	private void DoStep()
	{
		if (this.footDown)
		{
			this.footDownTime += TimeHandler.deltaTime * this.stepSpeed / this.scale;
			if (this.footDownTime > 1f && this.otherLeg.footDown && (Mathf.Abs(this.data.playerVel.velocity.x) > 1f || this.stepWasLost()))
			{
				this.StartStep();
				this.footDownTime = 0f;
				this.footDown = false;
				if (!this.data.dead && this.data.isPlaying && this.data.isGrounded && !this.data.isWallGrab)
				{
					this.soundParameterIntensity.intensity = Mathf.Abs(this.data.playerVel.velocity.x);
					if (this.data.stats.SoundTransformScaleThresholdReached())
					{
						SoundManager.Instance.Play(this.soundCharacterStepBig, this.data.transform, new SoundParameterBase[]
						{
							this.soundParameterIntensity
						});
						return;
					}
					SoundManager.Instance.Play(this.soundCharacterStep, this.data.transform, new SoundParameterBase[]
					{
						this.soundParameterIntensity
					});
					return;
				}
			}
		}
		else
		{
			this.stepTime += TimeHandler.deltaTime * this.stepSpeed / this.scale;
			if (this.stepTime > 1f)
			{
				this.EndStep();
				this.stepTime = 0f;
				this.footDown = true;
			}
		}
	}

	// Token: 0x0600024B RID: 587 RVA: 0x0000ECDA File Offset: 0x0000CEDA
	private void EndStep()
	{
		if (this.raycastTransform)
		{
			this.previousRaycastTransform = this.raycastTransform;
			this.previousRaycastPosLocal = this.raycastPosLocal;
		}
	}

	// Token: 0x0600024C RID: 588 RVA: 0x000027C8 File Offset: 0x000009C8
	private void StartStep()
	{
	}

	// Token: 0x0600024D RID: 589 RVA: 0x0000ED04 File Offset: 0x0000CF04
	private void SetFootPos()
	{
		if (this.data.isGrounded)
		{
			if (this.raycastTransform)
			{
				if (this.footDown)
				{
					this.footPosition = this.previousRaycastPosWorld;
					return;
				}
				this.footPosition = Vector3.Lerp(this.previousRaycastPosWorld, this.raycastPosWorld, this.forwardCurve.Evaluate(this.stepTime)) + Vector3.up * this.upCurve.Evaluate(this.stepTime);
				return;
			}
		}
		else
		{
			this.AirFootMovement();
		}
	}

	// Token: 0x0600024E RID: 590 RVA: 0x0000EDA0 File Offset: 0x0000CFA0
	private void Apply()
	{
		this.legRoot.position = this.data.transform.position + this.legRootOffset * this.scale;
		this.footTarget.position = this.footPosition;
	}

	// Token: 0x0600024F RID: 591 RVA: 0x0000EE00 File Offset: 0x0000D000
	private void DoRayCast()
	{
		Vector2 b = this.deltaPos * this.prediction;
		Vector2 vector = this.data.transform.position + base.transform.right * base.transform.localPosition.x;
		Vector2 vector2 = Vector2.down + b;
		float num = this.legLenth * 1.5f * this.scale + b.magnitude;
		RaycastHit2D[] array = Physics2D.RaycastAll(vector, vector2, num, this.mask);
		RaycastHit2D hit = default(RaycastHit2D);
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i].transform.root == this.data.transform) && array[i].transform)
			{
				if (!hit.transform)
				{
					hit = array[i];
				}
				else if (array[i].distance < hit.distance)
				{
					hit = array[i];
				}
			}
		}
		if (hit.transform)
		{
			this.HitGround(hit);
			return;
		}
		this.HitNothing();
	}

	// Token: 0x06000250 RID: 592 RVA: 0x000027C8 File Offset: 0x000009C8
	private void HitNothing()
	{
	}

	// Token: 0x06000251 RID: 593 RVA: 0x0000EF40 File Offset: 0x0000D140
	private void HitGround(RaycastHit2D hit)
	{
		bool flag = false;
		if (!this.raycastTransform && hit.transform)
		{
			flag = true;
		}
		if (this.raycastTransform && this.raycastTransform != hit.transform)
		{
			this.MigrateGroundHit(this.raycastTransform, hit.transform, hit);
		}
		this.raycastTransform = hit.transform;
		this.raycastPosLocal = this.raycastTransform.InverseTransformPoint(hit.point);
		if (flag)
		{
			this.Land(hit);
		}
	}

	// Token: 0x06000252 RID: 594 RVA: 0x000027C8 File Offset: 0x000009C8
	private void MigrateGroundHit(Transform from, Transform to, RaycastHit2D hit)
	{
	}

	// Token: 0x06000253 RID: 595 RVA: 0x0000EFDA File Offset: 0x0000D1DA
	private void Land(RaycastHit2D hit)
	{
		this.EndStep();
		this.UpdatePreviousRayCastWorldPos();
	}

	// Token: 0x06000254 RID: 596 RVA: 0x0000EFE8 File Offset: 0x0000D1E8
	private void UpdateRayCastWorldPos()
	{
		if (this.raycastTransform)
		{
			this.raycastPosWorld = this.raycastTransform.TransformPoint(this.raycastPosLocal);
		}
	}

	// Token: 0x06000255 RID: 597 RVA: 0x0000F018 File Offset: 0x0000D218
	private void UpdatePreviousRayCastWorldPos()
	{
		if (this.previousRaycastTransform)
		{
			this.previousRaycastPosWorld = this.previousRaycastTransform.TransformPoint(this.previousRaycastPosLocal);
		}
	}

	// Token: 0x06000256 RID: 598 RVA: 0x0000F048 File Offset: 0x0000D248
	private void UpdateMiscVariables()
	{
		this.scale = this.data.transform.localScale.x;
		this.restPosition = this.data.transform.position + this.startOffset * 0.7f * this.scale;
	}

	// Token: 0x06000257 RID: 599 RVA: 0x0000F0AC File Offset: 0x0000D2AC
	private void SetValuesFixed()
	{
		if (!this.moveDeltaTransform)
		{
			return;
		}
		this.deltaPos = Vector3.Lerp(this.deltaPos, this.moveDeltaTransform.position - this.lastPos, TimeHandler.deltaTime * 15f);
		this.lastPos = this.moveDeltaTransform.position;
		this.deltaPos.y = 0f;
	}

	// Token: 0x04000336 RID: 822
	[Header("Sounds")]
	[SerializeField]
	private SoundEvent soundCharacterStep;

	// Token: 0x04000337 RID: 823
	[SerializeField]
	private SoundEvent soundCharacterStepBig;

	// Token: 0x04000338 RID: 824
	private SoundParameterIntensity soundParameterIntensity = new SoundParameterIntensity(1f, 1);

	// Token: 0x04000339 RID: 825
	private CharacterData data;

	// Token: 0x0400033A RID: 826
	private float legLenth;

	// Token: 0x0400033B RID: 827
	public Transform legRoot;

	// Token: 0x0400033C RID: 828
	public Transform footTarget;

	// Token: 0x0400033D RID: 829
	public LayerMask mask;

	// Token: 0x0400033E RID: 830
	public AnimationCurve upCurve;

	// Token: 0x0400033F RID: 831
	public AnimationCurve forwardCurve;

	// Token: 0x04000340 RID: 832
	public Transform moveDeltaTransform;

	// Token: 0x04000341 RID: 833
	[HideInInspector]
	public float stepTime;

	// Token: 0x04000342 RID: 834
	public float stepSpeed = 1f;

	// Token: 0x04000343 RID: 835
	private float footDownTime;

	// Token: 0x04000344 RID: 836
	[HideInInspector]
	public bool footDown = true;

	// Token: 0x04000345 RID: 837
	public IkLeg otherLeg;

	// Token: 0x04000346 RID: 838
	private Vector2 restPosition;

	// Token: 0x04000347 RID: 839
	private Vector2 startOffset;

	// Token: 0x04000348 RID: 840
	private Vector2 legRootOffset;

	// Token: 0x04000349 RID: 841
	private float scale = 1f;

	// Token: 0x0400034A RID: 842
	public float prediction = 1f;

	// Token: 0x0400034B RID: 843
	private Vector2 raycastPosLocal;

	// Token: 0x0400034C RID: 844
	private Vector2 raycastPosWorld;

	// Token: 0x0400034D RID: 845
	private Vector2 previousRaycastPosLocal;

	// Token: 0x0400034E RID: 846
	private Vector2 previousRaycastPosWorld;

	// Token: 0x0400034F RID: 847
	private Transform raycastTransform;

	// Token: 0x04000350 RID: 848
	private Transform previousRaycastTransform;

	// Token: 0x04000351 RID: 849
	private Vector2 footPosition;

	// Token: 0x04000352 RID: 850
	private Vector2 deltaPos;

	// Token: 0x04000353 RID: 851
	private Vector2 lastPos;
}
