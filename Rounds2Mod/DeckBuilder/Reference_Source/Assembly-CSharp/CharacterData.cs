using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200002B RID: 43
public class CharacterData : MonoBehaviour
{
	// Token: 0x060000C6 RID: 198 RVA: 0x0000648C File Offset: 0x0000468C
	private void Awake()
	{
		this.crownPos = base.GetComponentInChildren<CrownPos>();
		this.view = base.GetComponent<PhotonView>();
		this.mainCol = base.GetComponent<Collider2D>();
		this.wobblePos = base.GetComponentInChildren<PlayerWobblePosition>().transform;
		this.stats = base.GetComponent<CharacterStatModifiers>();
		this.player = base.GetComponent<Player>();
		this.weaponHandler = base.GetComponent<WeaponHandler>();
		this.block = base.GetComponent<Block>();
		this.input = base.GetComponent<GeneralInput>();
		this.movement = base.GetComponent<PlayerMovement>();
		this.jump = base.GetComponent<PlayerJump>();
		this.stunHandler = base.GetComponent<StunHandler>();
		this.silenceHandler = base.GetComponent<SilenceHandler>();
		this.hand = base.GetComponentInChildren<HandPos>().transform;
		this.playerVel = base.GetComponent<PlayerVelocity>();
		this.healthHandler = base.GetComponent<HealthHandler>();
		this.playerSounds = base.GetComponent<PlayerSounds>();
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x00006570 File Offset: 0x00004770
	internal Vector3 GetCrownPos()
	{
		if (this.crownPos)
		{
			return this.crownPos.transform.position + Vector3.up * this.crownPos.GetOffset();
		}
		global::Debug.LogError("NO CROWN POS!?");
		return Vector3.up * 1000f;
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x000065CE File Offset: 0x000047CE
	private void Start()
	{
		this.groundMask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
		if (!this.view.IsMine)
		{
			PlayerManager.RegisterPlayer(this.player);
		}
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x00006606 File Offset: 0x00004806
	private void Update()
	{
		if (!this.playerVel.simulated)
		{
			this.sinceGrounded = 0f;
		}
		this.sinceJump += TimeHandler.deltaTime;
		this.Wall();
	}

	// Token: 0x060000CA RID: 202 RVA: 0x00006638 File Offset: 0x00004838
	private void FixedUpdate()
	{
		this.Ground();
	}

	// Token: 0x060000CB RID: 203 RVA: 0x00006640 File Offset: 0x00004840
	private void Ground()
	{
		if (!this.isPlaying)
		{
			return;
		}
		if (!this.isGrounded)
		{
			this.sinceGrounded += TimeHandler.fixedDeltaTime * ((this.isWallGrab && this.wallDistance < 0.7f) ? this.sinceGroundedMultiplierWhenWallGrab : 1f);
			if (this.sinceGrounded < 0f)
			{
				this.sinceGrounded = Mathf.Lerp(this.sinceGrounded, 0f, TimeHandler.fixedDeltaTime * 15f);
			}
		}
		if (!this.wasGroundedLastFrame)
		{
			this.isGrounded = false;
		}
		this.wasGroundedLastFrame = false;
	}

	// Token: 0x060000CC RID: 204 RVA: 0x000066D7 File Offset: 0x000048D7
	private void Wall()
	{
		if (!this.isWallGrab)
		{
			this.sinceWallGrab += TimeHandler.deltaTime;
		}
		if (!this.wasWallGrabLastFrame)
		{
			this.isWallGrab = false;
		}
		this.wasWallGrabLastFrame = false;
	}

	// Token: 0x060000CD RID: 205 RVA: 0x0000670C File Offset: 0x0000490C
	public void TouchGround(Vector3 pos, Vector3 groundNormal, Rigidbody2D groundRig, Transform groundTransform = null)
	{
		if (this.sinceJump > 0.2f)
		{
			this.currentJumps = this.jumps;
		}
		if (this.TouchGroundAction != null)
		{
			this.TouchGroundAction.Invoke(this.sinceGrounded, pos, groundNormal, groundTransform);
		}
		if (groundRig == null)
		{
			this.standOnRig = null;
		}
		else if (!groundRig.GetComponent<NetworkPhysicsObject>())
		{
			this.standOnRig = groundRig;
		}
		if (this.playerVel.velocity.y < -20f && !this.isGrounded)
		{
			for (int i = 0; i < this.landParts.Length; i++)
			{
				this.landParts[i].transform.localScale = Vector3.one * Mathf.Clamp(-this.playerVel.velocity.y / 40f, 0.5f, 1f) * 0.5f;
				this.landParts[i].transform.position = new Vector3(base.transform.position.x + this.playerVel.velocity.x * 0.03f, pos.y, 5f);
				this.landParts[i].transform.rotation = Quaternion.LookRotation(groundNormal);
				this.landParts[i].Play();
			}
			GamefeelManager.instance.AddGameFeel(Vector2.down * Mathf.Clamp((this.sinceGrounded - 0.5f) * 1f, 0f, 4f));
		}
		this.groundPos = pos;
		this.wasGroundedLastFrame = true;
		this.isGrounded = true;
		this.sinceGrounded = 0f;
	}

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x060000CE RID: 206 RVA: 0x000068C2 File Offset: 0x00004AC2
	// (set) Token: 0x060000CF RID: 207 RVA: 0x000027C8 File Offset: 0x000009C8
	public float HealthPercentage
	{
		get
		{
			return this.health / this.maxHealth;
		}
		internal set
		{
		}
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x000068D4 File Offset: 0x00004AD4
	public void TouchWall(Vector2 normal, Vector3 pos)
	{
		if (this.isGrounded)
		{
			return;
		}
		this.wallNormal = normal;
		this.wallPos = pos;
		this.groundPos = pos;
		this.wallDistance = Vector2.Distance(base.transform.position, pos);
		if (this.sinceJump < 0.15f)
		{
			return;
		}
		this.currentJumps = this.jumps;
		if (this.TouchWallAction != null)
		{
			this.TouchWallAction.Invoke(this.sinceWallGrab, pos, normal);
		}
		float num = this.sinceWallGrab;
		this.sinceWallGrab = 0f;
		this.wasWallGrabLastFrame = true;
		this.isWallGrab = true;
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x00006980 File Offset: 0x00004B80
	public bool ThereIsGroundBelow(Vector3 pos, float range = 5f)
	{
		RaycastHit2D raycastHit2D = Physics2D.Raycast(pos, Vector2.down, range, this.groundMask);
		return raycastHit2D.transform && raycastHit2D.distance > 0.1f;
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x000069C9 File Offset: 0x00004BC9
	public void SetAI(Player aiMaster = null)
	{
		this.master = aiMaster;
		this.input.controlledElseWhere = true;
		base.GetComponent<PlayerAPI>().enabled = true;
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x000069EA File Offset: 0x00004BEA
	public void SetWobbleObjectChild(Transform obj)
	{
		obj.transform.SetParent(this.wobblePos, true);
	}

	// Token: 0x040000CE RID: 206
	public Vector3 aimDirection;

	// Token: 0x040000CF RID: 207
	public List<CardInfo> currentCards;

	// Token: 0x040000D0 RID: 208
	public float sinceGroundedMultiplierWhenWallGrab = 0.2f;

	// Token: 0x040000D1 RID: 209
	public PlayerActions playerActions;

	// Token: 0x040000D2 RID: 210
	public ParticleSystem[] landParts;

	// Token: 0x040000D3 RID: 211
	public int jumps = 1;

	// Token: 0x040000D4 RID: 212
	public int currentJumps = 1;

	// Token: 0x040000D5 RID: 213
	public bool isPlaying;

	// Token: 0x040000D6 RID: 214
	public bool dead;

	// Token: 0x040000D7 RID: 215
	public bool isStunned;

	// Token: 0x040000D8 RID: 216
	public bool isSilenced;

	// Token: 0x040000D9 RID: 217
	public float stunTime;

	// Token: 0x040000DA RID: 218
	public float silenceTime;

	// Token: 0x040000DB RID: 219
	public float health = 100f;

	// Token: 0x040000DC RID: 220
	public float maxHealth = 100f;

	// Token: 0x040000DD RID: 221
	public AnimationCurve slamCurve;

	// Token: 0x040000DE RID: 222
	public Vector3 wallPos;

	// Token: 0x040000DF RID: 223
	public Vector2 wallNormal;

	// Token: 0x040000E0 RID: 224
	public Vector3 groundPos;

	// Token: 0x040000E1 RID: 225
	public Transform hand;

	// Token: 0x040000E2 RID: 226
	public float sinceWallGrab = float.PositiveInfinity;

	// Token: 0x040000E3 RID: 227
	public bool isWallGrab;

	// Token: 0x040000E4 RID: 228
	public float wallDistance = 1f;

	// Token: 0x040000E5 RID: 229
	private bool wasWallGrabLastFrame;

	// Token: 0x040000E6 RID: 230
	public float sinceGrounded;

	// Token: 0x040000E7 RID: 231
	public bool isGrounded = true;

	// Token: 0x040000E8 RID: 232
	private bool wasGroundedLastFrame = true;

	// Token: 0x040000E9 RID: 233
	public Player player;

	// Token: 0x040000EA RID: 234
	public float sinceJump = 1f;

	// Token: 0x040000EB RID: 235
	public PlayerVelocity playerVel;

	// Token: 0x040000EC RID: 236
	public HealthHandler healthHandler;

	// Token: 0x040000ED RID: 237
	public GeneralInput input;

	// Token: 0x040000EE RID: 238
	public PlayerMovement movement;

	// Token: 0x040000EF RID: 239
	public PlayerJump jump;

	// Token: 0x040000F0 RID: 240
	public Block block;

	// Token: 0x040000F1 RID: 241
	public CharacterStatModifiers stats;

	// Token: 0x040000F2 RID: 242
	public WeaponHandler weaponHandler;

	// Token: 0x040000F3 RID: 243
	public StunHandler stunHandler;

	// Token: 0x040000F4 RID: 244
	public SilenceHandler silenceHandler;

	// Token: 0x040000F5 RID: 245
	public Player lastSourceOfDamage;

	// Token: 0x040000F6 RID: 246
	public Player master;

	// Token: 0x040000F7 RID: 247
	public Player lastDamagedPlayer;

	// Token: 0x040000F8 RID: 248
	public Collider2D mainCol;

	// Token: 0x040000F9 RID: 249
	public PlayerSounds playerSounds;

	// Token: 0x040000FA RID: 250
	private Transform wobblePos;

	// Token: 0x040000FB RID: 251
	private LayerMask groundMask;

	// Token: 0x040000FC RID: 252
	public PhotonView view;

	// Token: 0x040000FD RID: 253
	private CrownPos crownPos;

	// Token: 0x040000FE RID: 254
	public Rigidbody2D standOnRig;

	// Token: 0x040000FF RID: 255
	public Action<float, Vector3, Vector3, Transform> TouchGroundAction;

	// Token: 0x04000100 RID: 256
	public Action<float, Vector3, Vector3> TouchWallAction;
}
