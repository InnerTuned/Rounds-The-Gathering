using System;
using System.Collections;
using Photon.Pun;
using Sonigon;
using UnityEngine;

// Token: 0x02000186 RID: 390
public class PlayerCollision : MonoBehaviour
{
	// Token: 0x060007EF RID: 2031 RVA: 0x0002B585 File Offset: 0x00029785
	public void IgnoreWallForFrames(int frames)
	{
		this.ignoreWallFor = frames;
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x0002B58E File Offset: 0x0002978E
	private void Start()
	{
		this.data = base.GetComponent<CharacterData>();
		this.col = base.GetComponent<Collider2D>();
		this.cirCol = base.GetComponent<CircleCollider2D>();
		this.vel = base.GetComponent<PlayerVelocity>();
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x0002B5C0 File Offset: 0x000297C0
	private void FixedUpdate()
	{
		if (this.checkForGoThroughWall && this.ignoreWallFor <= 0)
		{
			RaycastHit2D raycastHit2D = default(RaycastHit2D);
			RaycastHit2D[] array = Physics2D.RaycastAll(this.lastPos, base.transform.position - this.lastPos, Vector2.Distance(base.transform.position, this.lastPos), this.mask);
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i].transform.root == base.transform.root))
				{
					global::Debug.DrawLine(this.lastPos, array[i].point, Color.green, 1f);
					if (Vector2.Angle(array[i].normal, base.transform.position - this.lastPos) >= 90f && (!raycastHit2D.transform || array[i].distance < raycastHit2D.distance))
					{
						raycastHit2D = array[i];
					}
				}
			}
			if (raycastHit2D)
			{
				base.transform.position = raycastHit2D.point + raycastHit2D.normal * 0.5f;
				if (this.data.healthHandler.flyingFor > 0f)
				{
					this.DoBounce(raycastHit2D);
				}
			}
		}
		this.ignoreWallFor--;
		this.lastPos = base.transform.position;
		float num = this.cirCol.radius * base.transform.localScale.x;
		float num2 = this.cirCol.radius * base.transform.localScale.x * 0.75f;
		RaycastHit2D[] array2 = Physics2D.CircleCastAll(this.lastPos, num, base.transform.position - this.lastPos, Vector2.Distance(base.transform.position, this.lastPos), this.mask);
		for (int j = 0; j < array2.Length; j++)
		{
			if (!(array2[j].transform.root == base.transform.root))
			{
				Vector2 a = base.transform.position;
				Vector2 point = array2[j].point;
				float num3 = Vector2.Distance(a, point);
				Vector2 normalized = (a - point).normalized;
				float num4 = num + -num3;
				float num5 = num2 + -num3;
				num4 = Mathf.Clamp(num4, 0f, 10f);
				num5 = Mathf.Clamp(num5, 0f, 10f);
				NetworkPhysicsObject component = array2[j].transform.GetComponent<NetworkPhysicsObject>();
				if (component)
				{
					component.Push(this.data);
				}
				if (this.vel.simulated || !this.vel.isKinematic)
				{
					this.vel.transform.position += normalized * num5;
					if (Mathf.Abs(normalized.y) < 0.45f && Mathf.Abs(this.data.input.direction.x) > 0.1f && Vector3.Angle(this.data.input.direction, normalized) > 90f)
					{
						this.data.TouchWall(normalized, point);
					}
					this.vel.velocity += normalized * num4 * 10f * TimeHandler.timeScale;
					this.vel.velocity -= this.vel.velocity * num4 * 1f * TimeHandler.timeScale;
				}
				Player componentInParent = array2[j].transform.GetComponentInParent<Player>();
				if (componentInParent != null && this.collideWithPlayerAction != null)
				{
					this.collideWithPlayerAction.Invoke(point, num4 * normalized, componentInParent);
				}
				if (this.data.healthHandler.flyingFor > 0f)
				{
					this.DoBounce(array2[j]);
				}
			}
		}
		this.lastPos = base.transform.position;
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x0002BA84 File Offset: 0x00029C84
	private void DoBounce(RaycastHit2D hit)
	{
		if (Vector2.Angle(this.data.playerVel.velocity, hit.normal) < 90f)
		{
			return;
		}
		if (this.isBounce)
		{
			return;
		}
		if (this.data.view.IsMine && this.data.playerVel.velocity.magnitude > this.bounceTreshold)
		{
			this.data.view.RPC("RPCADoBounce", 0, new object[]
			{
				hit.normal,
				base.transform.position
			});
		}
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x0002BB2C File Offset: 0x00029D2C
	[PunRPC]
	private void RPCADoBounce(Vector2 hit, Vector3 playerPos)
	{
		base.transform.position = playerPos;
		base.StartCoroutine(this.IDoBounce(Vector2.Reflect(this.data.playerVel.velocity, hit)));
		SoundManager.Instance.Play(this.soundBounce, base.transform);
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x0002BB7E File Offset: 0x00029D7E
	public IEnumerator IDoBounce(Vector2 targetVel)
	{
		this.isBounce = true;
		this.data.stunHandler.AddStun(0.2f);
		this.data.healthHandler.CallTakeDamage(targetVel.normalized * 5f, base.transform.position, null, null, true);
		GamefeelManager.instance.AddGameFeel(targetVel.normalized * 4f);
		yield return new WaitForSeconds(0.25f);
		this.data.playerVel.velocity = targetVel;
		this.isBounce = false;
		yield break;
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x0002BB94 File Offset: 0x00029D94
	private void OnDisable()
	{
		this.isBounce = false;
	}

	// Token: 0x04000949 RID: 2377
	[Header("Sounds")]
	public SoundEvent soundBounce;

	// Token: 0x0400094A RID: 2378
	[Header("Settings")]
	public LayerMask mask;

	// Token: 0x0400094B RID: 2379
	private Vector2 lastPos;

	// Token: 0x0400094C RID: 2380
	public bool checkForGoThroughWall = true;

	// Token: 0x0400094D RID: 2381
	private int ignoreWallFor;

	// Token: 0x0400094E RID: 2382
	private Collider2D col;

	// Token: 0x0400094F RID: 2383
	private CircleCollider2D cirCol;

	// Token: 0x04000950 RID: 2384
	private PlayerVelocity vel;

	// Token: 0x04000951 RID: 2385
	private CharacterData data;

	// Token: 0x04000952 RID: 2386
	public Action<Vector2, Vector2, Player> collideWithPlayerAction;

	// Token: 0x04000953 RID: 2387
	public float bounceTreshold = 1f;

	// Token: 0x04000954 RID: 2388
	private bool isBounce;
}
