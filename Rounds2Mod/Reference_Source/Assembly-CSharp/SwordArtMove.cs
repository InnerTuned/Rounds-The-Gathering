using System;
using UnityEngine;

// Token: 0x020000E4 RID: 228
public class SwordArtMove : MonoBehaviour
{
	// Token: 0x06000482 RID: 1154 RVA: 0x0001AC78 File Offset: 0x00018E78
	private void Start()
	{
		this.parts = base.GetComponentsInChildren<ParticleSystem>();
		this.move = base.GetComponentInParent<MoveTransform>();
		this.move.enabled = false;
		this.startForward = base.transform.forward;
		this.startUp = Vector3.Cross(base.transform.forward, Vector3.forward);
		this.startPos = base.transform.position;
		this.awayMultiplier *= Mathf.Pow(this.move.localForce.magnitude / 40f * 2f, 0.45f);
		this.awayMultiplier /= 1f + this.move.drag * 0.05f;
		this.speed *= Mathf.Pow(this.awayMultiplier, 0.3f);
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x0001AD5C File Offset: 0x00018F5C
	private void Update()
	{
		this.counter += TimeHandler.deltaTime * this.speed;
		Vector3 vector = this.startPos + (this.heightCurve.Evaluate(this.counter) * this.heightMultiplier * this.startUp + this.AwayCurve.Evaluate(this.counter) * this.awayMultiplier * this.startForward) * this.multiplier;
		RaycastHit2D raycastHit2D = Physics2D.Raycast(this.startPos, (vector - this.startPos).normalized, Vector3.Distance(vector, this.startPos), this.mask);
		if (raycastHit2D.transform && !raycastHit2D.collider.GetComponent<Damagable>())
		{
			vector = raycastHit2D.point + raycastHit2D.normal * 0.1f;
			for (int i = 0; i < this.parts.Length; i++)
			{
				this.parts[i].transform.position = raycastHit2D.point + Vector3.forward * 8f;
				this.parts[i].transform.rotation = Quaternion.LookRotation(raycastHit2D.normal);
				this.parts[i].Emit(1);
			}
		}
		base.transform.root.position = vector;
	}

	// Token: 0x0400060C RID: 1548
	private MoveTransform move;

	// Token: 0x0400060D RID: 1549
	public LayerMask mask;

	// Token: 0x0400060E RID: 1550
	public AnimationCurve heightCurve;

	// Token: 0x0400060F RID: 1551
	public AnimationCurve AwayCurve;

	// Token: 0x04000610 RID: 1552
	private Vector3 startUp;

	// Token: 0x04000611 RID: 1553
	private Vector3 startForward;

	// Token: 0x04000612 RID: 1554
	private Vector3 startPos;

	// Token: 0x04000613 RID: 1555
	public float multiplier = 1f;

	// Token: 0x04000614 RID: 1556
	public float heightMultiplier = 1f;

	// Token: 0x04000615 RID: 1557
	public float awayMultiplier = 1f;

	// Token: 0x04000616 RID: 1558
	public float speed = 1f;

	// Token: 0x04000617 RID: 1559
	private float counter;

	// Token: 0x04000618 RID: 1560
	private ParticleSystem[] parts;
}
