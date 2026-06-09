using System;
using UnityEngine;

// Token: 0x0200019F RID: 415
public class RayHitDrill : RayHitEffect
{
	// Token: 0x0600085C RID: 2140 RVA: 0x0002CC24 File Offset: 0x0002AE24
	private void Start()
	{
		this.move = base.GetComponentInParent<MoveTransform>();
		this.proj = base.GetComponentInParent<ProjectileHit>();
		this.proj.canPushBox = false;
		RayHitDrill[] componentsInChildren = base.transform.root.GetComponentsInChildren<RayHitDrill>();
		if (componentsInChildren.Length == 1)
		{
			this.mainDrill = true;
		}
		bool flag = false;
		int num = -1;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].mainDrill)
			{
				flag = true;
				num = i;
			}
		}
		if (flag)
		{
			if (componentsInChildren[num] != this)
			{
				componentsInChildren[num].metersOfDrilling += this.metersOfDrilling;
			}
		}
		else
		{
			this.mainDrill = true;
		}
		if (this.mainDrill)
		{
			this.rpc = base.GetComponentInParent<ChildRPC>();
			this.rpc.childRPCsVector2.Add("DrillStop", new Action<Vector2>(this.RPCA_Deactivate));
		}
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x0002CCF4 File Offset: 0x0002AEF4
	private void Update()
	{
		if (!this.mainDrill)
		{
			return;
		}
		if (!this.proj.view.IsMine)
		{
			return;
		}
		if (this.sinceDrill > 2 && !this.proj.isAllowedToSpawnObjects)
		{
			this.rpc.CallFunction("DrillStop", base.transform.position);
		}
		this.sinceDrill++;
	}

	// Token: 0x0600085E RID: 2142 RVA: 0x0002CD64 File Offset: 0x0002AF64
	public void RPCA_Deactivate(Vector2 tpPos)
	{
		base.transform.position = tpPos;
		this.proj.isAllowedToSpawnObjects = true;
		this.move.simulationSpeed /= this.speedModFlat;
		this.move.simulationSpeed *= this.move.localForce.magnitude * this.speedMod;
		this.proj.sendCollisions = true;
		base.transform.GetChild(0).gameObject.SetActive(false);
		if (this.metersOfDrilling < 2f)
		{
			this.metersOfDrilling = -1f;
		}
	}

	// Token: 0x0600085F RID: 2143 RVA: 0x0002CE0C File Offset: 0x0002B00C
	private void OnDestroy()
	{
		if (!this.mainDrill)
		{
			return;
		}
		if (this.proj.isAllowedToSpawnObjects)
		{
			return;
		}
		Object.Instantiate<GameObject>(base.GetComponentInChildren<ParticleSystem>(true).gameObject, base.transform.position, base.transform.rotation).SetActive(true);
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x0002CE60 File Offset: 0x0002B060
	public override HasToReturn DoHitEffect(HitInfo hit)
	{
		if (!this.mainDrill)
		{
			return HasToReturn.canContinue;
		}
		if (this.proj.isAllowedToSpawnObjects)
		{
			base.transform.root.position = hit.point;
			this.move.simulationSpeed *= this.speedModFlat;
			this.move.simulationSpeed /= this.move.localForce.magnitude * this.speedMod;
			base.transform.GetChild(0).gameObject.SetActive(true);
			base.transform.GetComponentInChildren<TrailRenderer>().Clear();
		}
		this.sinceDrill = 0;
		this.proj.isAllowedToSpawnObjects = false;
		this.proj.sendCollisions = false;
		this.metersOfDrilling -= this.move.velocity.magnitude * TimeHandler.deltaTime * this.move.simulationSpeed;
		if (this.metersOfDrilling <= 0f)
		{
			this.done = true;
			return HasToReturn.canContinue;
		}
		if (!hit.transform || hit.transform.root.GetComponent<Player>())
		{
			return HasToReturn.canContinue;
		}
		return HasToReturn.hasToReturnNow;
	}

	// Token: 0x0400099F RID: 2463
	public float metersOfDrilling = 1f;

	// Token: 0x040009A0 RID: 2464
	private MoveTransform move;

	// Token: 0x040009A1 RID: 2465
	private ProjectileHit proj;

	// Token: 0x040009A2 RID: 2466
	public bool mainDrill;

	// Token: 0x040009A3 RID: 2467
	private ChildRPC rpc;

	// Token: 0x040009A4 RID: 2468
	private int sinceDrill = 10;

	// Token: 0x040009A5 RID: 2469
	public float speedModFlat = 0.5f;

	// Token: 0x040009A6 RID: 2470
	public float speedMod = 0.1f;

	// Token: 0x040009A7 RID: 2471
	private bool done;
}
