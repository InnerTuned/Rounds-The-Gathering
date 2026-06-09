using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020000B0 RID: 176
public class RayCastTrail : MonoBehaviour
{
	// Token: 0x060003D3 RID: 979 RVA: 0x00017804 File Offset: 0x00015A04
	private void Awake()
	{
		this.timeAtSpawn = Time.time;
	}

	// Token: 0x060003D4 RID: 980 RVA: 0x00017814 File Offset: 0x00015A14
	private void Start()
	{
		this.rayHit = base.GetComponent<RayHit>();
		ProjectileHit component = base.GetComponent<ProjectileHit>();
		if (component)
		{
			this.size = Mathf.Clamp(Mathf.Pow(component.damage, 0.85f) / 400f, 0f, 100f) + 0.3f + this.extraSize;
		}
		this.move = base.GetComponent<MoveTransform>();
		this.lastPos = base.transform.position;
	}

	// Token: 0x060003D5 RID: 981 RVA: 0x00017891 File Offset: 0x00015A91
	private void OnEnable()
	{
		this.lastPos = base.transform.position;
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x000178A4 File Offset: 0x00015AA4
	private void Update()
	{
		IEnumerable<RaycastHit2D> enumerable = Physics2D.RaycastAll(this.lastPos, base.transform.position - this.lastPos, Vector3.Distance(base.transform.position, this.lastPos), this.mask);
		RaycastHit2D[] array = Physics2D.CircleCastAll(this.lastPos, this.size, base.transform.position - this.lastPos, Vector3.Distance(base.transform.position, this.lastPos), this.playerMask);
		RaycastHit2D[] array2 = Enumerable.ToArray<RaycastHit2D>(Enumerable.Concat<RaycastHit2D>(enumerable, array));
		RaycastHit2D raycastHit2D = default(RaycastHit2D);
		raycastHit2D.distance = float.PositiveInfinity;
		for (int i = 0; i < array2.Length; i++)
		{
			if (array2[i] && !(array2[i].transform.root == base.transform.root))
			{
				Player component = array2[i].transform.root.GetComponent<Player>();
				if ((!component || component.playerID != this.teamID || this.timeAtSpawn + 0.2f < Time.time) && (!this.ignoredRig || !(this.ignoredRig == array2[i].rigidbody)))
				{
					ProjectileHitSurface component2 = array2[i].collider.GetComponent<ProjectileHitSurface>();
					if ((!component2 || component2.HitSurface(HitInfo.GetHitInfo(array2[i]), base.gameObject) != ProjectileHitSurface.HasToStop.HasToStop) && array2[i].distance < raycastHit2D.distance)
					{
						raycastHit2D = array2[i];
					}
				}
			}
		}
		if (raycastHit2D.transform)
		{
			this.rayHit.Hit(HitInfo.GetHitInfo(raycastHit2D), false);
		}
		if (GridVisualizer.instance)
		{
			GridVisualizer.instance.BulletCall(base.transform.position);
		}
		this.lastPos = base.transform.position;
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x00017ACF File Offset: 0x00015CCF
	public void WasBlocked()
	{
		this.timeAtSpawn = 0f;
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x00017891 File Offset: 0x00015A91
	public void MoveRay()
	{
		this.lastPos = base.transform.position;
	}

	// Token: 0x060003D9 RID: 985 RVA: 0x00017ADC File Offset: 0x00015CDC
	public void IgnoreRigFor(Rigidbody2D rig, float time)
	{
		base.StartCoroutine(this.DoIgnoreRigFor(rig, time));
	}

	// Token: 0x060003DA RID: 986 RVA: 0x00017AED File Offset: 0x00015CED
	private IEnumerator DoIgnoreRigFor(Rigidbody2D rig, float time)
	{
		this.ignoredRig = rig;
		yield return new WaitForSeconds(time);
		this.ignoredRig = null;
		yield break;
	}

	// Token: 0x04000531 RID: 1329
	public LayerMask mask;

	// Token: 0x04000532 RID: 1330
	public LayerMask playerMask;

	// Token: 0x04000533 RID: 1331
	public LayerMask ignoreWallsMask;

	// Token: 0x04000534 RID: 1332
	public int teamID = -1;

	// Token: 0x04000535 RID: 1333
	private float timeAtSpawn;

	// Token: 0x04000536 RID: 1334
	public float size;

	// Token: 0x04000537 RID: 1335
	public float extraSize;

	// Token: 0x04000538 RID: 1336
	private MoveTransform move;

	// Token: 0x04000539 RID: 1337
	private Vector3 lastPos;

	// Token: 0x0400053A RID: 1338
	private RayHit rayHit;

	// Token: 0x0400053B RID: 1339
	private Rigidbody2D ignoredRig;
}
