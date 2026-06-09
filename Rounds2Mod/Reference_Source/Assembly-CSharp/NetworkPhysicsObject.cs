using System;
using System.Collections.Generic;
using Photon.Pun;
using Sonigon;
using Sonigon.Internal;
using UnityEngine;

// Token: 0x02000173 RID: 371
public class NetworkPhysicsObject : MonoBehaviour, IPunObservable
{
	// Token: 0x06000789 RID: 1929 RVA: 0x000288B4 File Offset: 0x00026AB4
	public void Awake()
	{
		this.soundParameterPitchSemitone.pitchSemitone = this.soundPitchSemitone;
		this.currentFrame = Random.Range(0, this.sendFreq);
		this.photonView = base.GetComponent<PhotonView>();
		this.col = base.GetComponent<Collider2D>();
		this.rig2D = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x0600078A RID: 1930 RVA: 0x00028908 File Offset: 0x00026B08
	private void Update()
	{
		if (!this.photonView)
		{
			return;
		}
		this.sinceRequest += Time.deltaTime;
		this.sinceDealDMG += Time.deltaTime;
		this.sendForceCounter += Time.deltaTime;
		if (this.sendForceCounter > this.sendForceRate)
		{
			bool isMine = this.photonView.IsMine;
			if (this.currentForceToSend != Vector2.zero)
			{
				this.photonView.RPC("RPCA_SendForce", this.photonView.Owner, new object[]
				{
					this.currentForceToSend,
					this.currentForcePos
				});
				this.sendForceCounter = 0f;
				this.currentForceToSend = Vector2.zero;
			}
		}
		if (this.syncPackages.Count > 0)
		{
			if (this.syncPackages[0].timeDelta > 0f)
			{
				this.syncPackages[0].timeDelta -= Time.deltaTime * 1.5f * (1f + (float)this.syncPackages.Count * 0.5f);
			}
			else
			{
				if (this.syncPackages.Count > 2)
				{
					this.syncPackages.RemoveAt(0);
				}
				this.rig2D.isKinematic = false;
				base.transform.position = this.syncPackages[0].pos;
				base.transform.rotation = Quaternion.LookRotation(Vector3.forward, this.syncPackages[0].rot);
				this.rig2D.velocity = this.syncPackages[0].vel;
				this.rig2D.angularVelocity = this.syncPackages[0].angularVel;
				this.syncPackages.RemoveAt(0);
			}
		}
		this.sinceCol += Time.deltaTime;
	}

	// Token: 0x0600078B RID: 1931 RVA: 0x00028B0C File Offset: 0x00026D0C
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		this.currentFrame++;
		if (stream.IsWriting)
		{
			if (this.currentFrame >= this.sendFreq)
			{
				this.currentFrame = 0;
				stream.SendNext(base.transform.position);
				stream.SendNext(base.transform.up);
				stream.SendNext(this.rig2D.velocity);
				stream.SendNext(this.rig2D.angularVelocity);
				if (this.lastTime == 0f)
				{
					this.lastTime = Time.time;
				}
				stream.SendNext(Time.time - this.lastTime);
				this.lastTime = Time.time;
				return;
			}
		}
		else
		{
			ObjectSyncPackage objectSyncPackage = new ObjectSyncPackage();
			objectSyncPackage.pos = (Vector2)stream.ReceiveNext();
			objectSyncPackage.rot = (Vector2)stream.ReceiveNext();
			objectSyncPackage.vel = (Vector2)stream.ReceiveNext();
			objectSyncPackage.angularVel = (float)stream.ReceiveNext();
			objectSyncPackage.timeDelta = (float)stream.ReceiveNext();
			this.syncPackages.Add(objectSyncPackage);
		}
	}

	// Token: 0x0600078C RID: 1932 RVA: 0x00028C50 File Offset: 0x00026E50
	[PunRPC]
	public void RPCA_Collide(Vector2 colForce)
	{
		this.soundParameterIntensity.intensity = colForce.magnitude;
		SoundManager.Instance.PlayAtPosition(this.soundBoxImpact, SoundManager.Instance.GetTransform(), base.transform, new SoundParameterBase[]
		{
			this.soundParameterIntensity,
			this.soundParameterPitchSemitone
		});
		GamefeelManager.instance.AddGameFeel(colForce);
	}

	// Token: 0x0600078D RID: 1933 RVA: 0x00028CB4 File Offset: 0x00026EB4
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!this.photonView.IsMine)
		{
			return;
		}
		if (collision.contacts[0].normalImpulse < this.collisionThreshold)
		{
			return;
		}
		if (this.sinceCol < 0.1f)
		{
			return;
		}
		this.sinceCol = 0f;
		this.photonView.RPC("RPCA_Collide", 0, new object[]
		{
			Mathf.Clamp(collision.contacts[0].normalImpulse, 0f, this.maxShake) * collision.contacts[0].normal * this.shakeAmount
		});
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x00028D64 File Offset: 0x00026F64
	private void OnPlayerCollision(Vector2 collision, CharacterData player)
	{
		if (player.view.IsMine)
		{
			if (this.sinceDealDMG < 1f)
			{
				return;
			}
			Vector3 a = collision * this.dmgAmount;
			if (a.magnitude < this.playerColThreshold)
			{
				return;
			}
			float d = Mathf.Pow(this.rig2D.mass / 20000f, 2f);
			float d2 = Mathf.Pow(this.rig2D.mass / 20000f, 0.5f);
			player.healthHandler.CallTakeDamage(a * 0.3f * d, player.transform.position, null, null, true);
			player.healthHandler.CallTakeForce(collision * this.forceAmount * d2, 1, false, false, a.magnitude * 0.05f);
			if (player.block.IsBlocking())
			{
				this.rig2D.velocity *= -1.1f;
				this.rig2D.angularVelocity *= -1.1f;
			}
			else if (this.rig2D.mass < 80000f)
			{
				this.rig2D.velocity *= -0.5f * (20000f / this.rig2D.mass);
				this.rig2D.angularVelocity *= -0.5f * (20000f / this.rig2D.mass);
			}
			this.sinceDealDMG = 0f;
			this.photonView.RPC("RPCA_PlayerCollision", 5, new object[]
			{
				collision,
				this.rig2D.velocity,
				base.transform.position,
				player.view.ViewID
			});
		}
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x00028F57 File Offset: 0x00027157
	[PunRPC]
	public void RPCM_RequestCollide(Vector2 collision, Vector2 afterVel, Vector3 position, int playerId)
	{
		this.photonView.RPC("RPCA_PlayerCollision", 5, new object[]
		{
			collision,
			afterVel,
			position,
			playerId
		});
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x00028F98 File Offset: 0x00027198
	[PunRPC]
	private void RPCA_PlayerCollision(Vector2 collision, Vector2 velAfter, Vector3 position, int playerID)
	{
		CharacterData component = PhotonNetwork.GetPhotonView(playerID).GetComponent<CharacterData>();
		base.transform.position = position;
		this.rig2D.velocity = velAfter;
		this.sinceDealDMG = 0f;
		base.StartCoroutine(component.GetComponent<PlayerCollision>().IDoBounce(component.playerVel.velocity));
	}

	// Token: 0x06000791 RID: 1937 RVA: 0x00028FF2 File Offset: 0x000271F2
	public void BulletPush(Vector2 force, Vector2 localPoint, CharacterData asker)
	{
		if (this.photonView.IsMine)
		{
			this.rig2D.AddForceAtPosition(force * this.bulletPushMultiplier, base.transform.TransformPoint(localPoint), 1);
		}
	}

	// Token: 0x06000792 RID: 1938 RVA: 0x0002902F File Offset: 0x0002722F
	[PunRPC]
	public void RPCA_SendForce(Vector2 forceSent, Vector2 sentForcePos)
	{
		this.rig2D.AddForceAtPosition(forceSent, base.transform.TransformPoint(this.currentForcePos));
	}

	// Token: 0x06000793 RID: 1939 RVA: 0x00029058 File Offset: 0x00027258
	public Vector3 Push(CharacterData data)
	{
		if (!data.view.IsMine)
		{
			return Vector3.zero;
		}
		this.sincePushed = 0f;
		Vector2 vector = data.input.direction * 8f;
		Vector2 vector2 = this.col.bounds.ClosestPoint(data.transform.position);
		float num = Vector2.Angle(vector, vector2 - data.transform.position);
		float d = (90f - num) / 90f;
		Vector2 b = TimeHandler.fixedDeltaTime * vector * d * this.speed * 1000f;
		this.currentForceToSend += b;
		this.currentForcePos = base.transform.InverseTransformPoint(vector2);
		float d2 = Mathf.Clamp((Vector2.Angle(this.rig2D.velocity, base.transform.position - data.transform.position) - 90f) / 90f, 0f, 1f);
		this.OnPlayerCollision(this.rig2D.velocity * d2, data);
		return -vector;
	}

	// Token: 0x06000794 RID: 1940 RVA: 0x000291BC File Offset: 0x000273BC
	public void RequestOwnership(CharacterData player)
	{
	}

	// Token: 0x040008D5 RID: 2261
	[Header("Sounds")]
	public SoundEvent soundBoxImpact;

	// Token: 0x040008D6 RID: 2262
	public float soundPitchSemitone;

	// Token: 0x040008D7 RID: 2263
	private SoundParameterPitchSemitone soundParameterPitchSemitone = new SoundParameterPitchSemitone(0f, 1);

	// Token: 0x040008D8 RID: 2264
	private SoundParameterIntensity soundParameterIntensity = new SoundParameterIntensity(1f, 1);

	// Token: 0x040008D9 RID: 2265
	[Header("Settings")]
	public PhotonView photonView;

	// Token: 0x040008DA RID: 2266
	private Rigidbody2D rig2D;

	// Token: 0x040008DB RID: 2267
	private Collider2D col;

	// Token: 0x040008DC RID: 2268
	public int sendFreq = 5;

	// Token: 0x040008DD RID: 2269
	private int currentFrame;

	// Token: 0x040008DE RID: 2270
	private float lastTime;

	// Token: 0x040008DF RID: 2271
	private List<ObjectSyncPackage> syncPackages = new List<ObjectSyncPackage>();

	// Token: 0x040008E0 RID: 2272
	private float sinceCol;

	// Token: 0x040008E1 RID: 2273
	public float collisionThreshold;

	// Token: 0x040008E2 RID: 2274
	public float shakeAmount;

	// Token: 0x040008E3 RID: 2275
	public float maxShake;

	// Token: 0x040008E4 RID: 2276
	public float playerColThreshold = 1f;

	// Token: 0x040008E5 RID: 2277
	public float dmgAmount = 1f;

	// Token: 0x040008E6 RID: 2278
	public float forceAmount = 1f;

	// Token: 0x040008E7 RID: 2279
	private float sinceDealDMG;

	// Token: 0x040008E8 RID: 2280
	private List<Player> hitPlayers = new List<Player>();

	// Token: 0x040008E9 RID: 2281
	public float speed = 100f;

	// Token: 0x040008EA RID: 2282
	private float sinceRequest;

	// Token: 0x040008EB RID: 2283
	private float sincePushed;

	// Token: 0x040008EC RID: 2284
	public float bulletPushMultiplier = 1f;

	// Token: 0x040008ED RID: 2285
	private Vector2 currentForceToSend;

	// Token: 0x040008EE RID: 2286
	private Vector2 currentForcePos;

	// Token: 0x040008EF RID: 2287
	private float sendForceRate = 0.1f;

	// Token: 0x040008F0 RID: 2288
	private float sendForceCounter;

	// Token: 0x040008F1 RID: 2289
	public float sleepThreshold = 1f;
}
