using System;
using System.Collections;
using Photon.Pun;
using Sonigon;
using UnityEngine;

// Token: 0x02000019 RID: 25
public class BounceEffectRetarget : BounceEffect
{
	// Token: 0x06000080 RID: 128 RVA: 0x00004F88 File Offset: 0x00003188
	private void Start()
	{
		this.view = base.GetComponentInParent<PhotonView>();
		this.move = base.GetComponentInParent<MoveTransform>();
		base.GetComponentInParent<ChildRPC>().childRPCsVector2.Add("TargetBounce", new Action<Vector2>(this.SetNewVel));
		base.GetComponentInParent<ChildRPC>().childRPCsInt.Add("TargetBounceLine", new Action<int>(this.DrawLineTo));
	}

	// Token: 0x06000081 RID: 129 RVA: 0x00004FEF File Offset: 0x000031EF
	public override void DoBounce(HitInfo hit)
	{
		base.StartCoroutine(this.DelayMove(hit));
	}

	// Token: 0x06000082 RID: 130 RVA: 0x00005000 File Offset: 0x00003200
	private void ActuallyDoBounce(int playerId)
	{
		Player playerWithID = PlayerManager.instance.GetPlayerWithID(playerId);
		if (playerWithID)
		{
			base.GetComponentInParent<ChildRPC>().CallFunction("TargetBounce", (playerWithID.data.playerVel.position + Vector2.up * this.move.GetUpwardsCompensation(base.transform.position, playerWithID.data.playerVel.position) - base.transform.position).normalized * this.move.velocity.magnitude);
			SoundManager.Instance.PlayAtPosition(this.soundTargetBounceTargetPlayer, SoundManager.Instance.GetTransform(), base.transform);
			return;
		}
		base.GetComponentInParent<ChildRPC>().CallFunction("TargetBounce", this.move.velocity);
	}

	// Token: 0x06000083 RID: 131 RVA: 0x000050F1 File Offset: 0x000032F1
	private void SetNewVel(Vector2 newVel)
	{
		this.move.enabled = true;
		this.move.velocity = newVel;
	}

	// Token: 0x06000084 RID: 132 RVA: 0x00005110 File Offset: 0x00003310
	private Player FindTarget(HitInfo hit)
	{
		Player closestPlayer = PlayerManager.instance.GetClosestPlayer(base.transform.position + hit.normal * 0.1f, false);
		if (PlayerManager.instance.CanSeePlayer(base.transform.position, closestPlayer).canSee)
		{
			return closestPlayer;
		}
		return null;
	}

	// Token: 0x06000085 RID: 133 RVA: 0x00005178 File Offset: 0x00003378
	private IEnumerator DelayMove(HitInfo hit)
	{
		Player p = this.FindTarget(hit);
		if (p && this.view.IsMine)
		{
			base.GetComponentInParent<ChildRPC>().CallFunction("TargetBounceLine", p.playerID);
		}
		this.move.enabled = false;
		if (hit.rigidbody)
		{
			this.move.GetComponent<RayCastTrail>().IgnoreRigFor(hit.rigidbody, 0.5f);
		}
		yield return new WaitForSeconds(0.1f);
		if (this.view.IsMine)
		{
			if (p)
			{
				this.ActuallyDoBounce(p.playerID);
			}
			else
			{
				this.ActuallyDoBounce(-1);
			}
		}
		yield break;
	}

	// Token: 0x06000086 RID: 134 RVA: 0x00005190 File Offset: 0x00003390
	private void DrawLineTo(int playerID)
	{
		Player playerWithID = PlayerManager.instance.GetPlayerWithID(playerID);
		if (playerWithID)
		{
			base.StartCoroutine(this.DrawLine(playerWithID.transform));
		}
	}

	// Token: 0x06000087 RID: 135 RVA: 0x000051C4 File Offset: 0x000033C4
	private IEnumerator DrawLine(Transform target)
	{
		LineEffect line = base.GetComponentInChildren<LineEffect>(true);
		line.StartDraw();
		while (line)
		{
			line.DrawLine(base.transform.position, target.position);
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000082 RID: 130
	[Header("Sound")]
	public SoundEvent soundTargetBounceTargetPlayer;

	// Token: 0x04000083 RID: 131
	private MoveTransform move;

	// Token: 0x04000084 RID: 132
	private PhotonView view;
}
