using System;
using UnityEngine;

// Token: 0x020000F3 RID: 243
public class WallRayCaster : MonoBehaviour
{
	// Token: 0x060004E1 RID: 1249 RVA: 0x0001BF82 File Offset: 0x0001A182
	private void Start()
	{
		this.input = base.GetComponent<GeneralInput>();
		this.data = base.GetComponent<CharacterData>();
		this.rig = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Update()
	{
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x0001BFA8 File Offset: 0x0001A1A8
	public void RayCast(Vector3 dir, float offset = 0f)
	{
		RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position + base.transform.up * offset, dir, this.rayLength * base.transform.localScale.x, this.mask);
		if (raycastHit2D.transform)
		{
			if (raycastHit2D.collider.GetComponent<DamageBox>())
			{
				return;
			}
			if (Vector3.Angle(raycastHit2D.normal, Vector3.up) > 70f && Vector3.Angle(raycastHit2D.normal, Vector3.up) < 110f)
			{
				this.data.TouchWall(raycastHit2D.normal, raycastHit2D.point);
			}
		}
	}

	// Token: 0x04000668 RID: 1640
	public float rayLength = 0.7f;

	// Token: 0x04000669 RID: 1641
	public LayerMask mask;

	// Token: 0x0400066A RID: 1642
	private GeneralInput input;

	// Token: 0x0400066B RID: 1643
	private CharacterData data;

	// Token: 0x0400066C RID: 1644
	private Rigidbody2D rig;
}
