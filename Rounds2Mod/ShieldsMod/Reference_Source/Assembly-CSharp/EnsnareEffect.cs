using System;
using Sirenix.OdinInspector;
using Sonigon;
using UnityEngine;

// Token: 0x02000050 RID: 80
public class EnsnareEffect : MonoBehaviour
{
	// Token: 0x06000181 RID: 385 RVA: 0x00009B64 File Offset: 0x00007D64
	private void Start()
	{
		this.scale *= base.transform.localScale.x;
		this.range *= (1f + base.transform.localScale.x) * 0.5f;
		Player closestPlayer = PlayerManager.instance.GetClosestPlayer(base.transform.position, true);
		if (closestPlayer && Vector3.Distance(base.transform.position, closestPlayer.transform.position) < this.range)
		{
			this.target = closestPlayer.data;
			this.startPos = this.target.transform.position;
			if (this.startPosIsMyPos)
			{
				this.startPos = base.transform.position;
			}
			float bezierOffset = 1f;
			if (this.startPosIsMyPos)
			{
				bezierOffset = 0f;
			}
			this.line = base.GetComponentInChildren<LineEffect>(true);
			this.line.Play(base.transform, closestPlayer.transform, bezierOffset);
			if (this.soundEnsnare != null)
			{
				SoundManager.Instance.Play(this.soundEnsnare, this.target.transform);
			}
			if (this.soundEnsnareJumpChange)
			{
				this.target.playerSounds.AddEnsnareEffect(this);
			}
		}
	}

	// Token: 0x06000182 RID: 386 RVA: 0x00009CC4 File Offset: 0x00007EC4
	private void FixedUpdate()
	{
		if (this.target && this.line.currentWidth > 0f)
		{
			this.target.sinceGrounded = 0f;
			this.target.playerVel.velocity *= Mathf.Pow(this.drag, this.scale);
			this.target.playerVel.AddForce(Vector2.ClampMagnitude(this.startPos - this.target.transform.position, 60f) * this.scale * this.force, 0);
			if (!this.done && Vector2.Distance(this.startPos, this.target.transform.position) > this.breakDistance)
			{
				this.done = true;
				this.line.counter = Mathf.Clamp(this.line.counter, this.snapCapLineTime, float.PositiveInfinity);
				if (this.soundEnsnareJumpChange)
				{
					this.target.playerSounds.RemoveEnsnareEffect(this);
				}
				if (this.soundEnsnareBreak != null)
				{
					SoundManager.Instance.Play(this.soundEnsnareBreak, this.target.transform);
				}
			}
		}
	}

	// Token: 0x040001F1 RID: 497
	[Header("Sounds")]
	public SoundEvent soundEnsnare;

	// Token: 0x040001F2 RID: 498
	public SoundEvent soundEnsnareBreak;

	// Token: 0x040001F3 RID: 499
	public bool soundEnsnareJumpChange;

	// Token: 0x040001F4 RID: 500
	public float range = 1f;

	// Token: 0x040001F5 RID: 501
	private float seconds = 1f;

	// Token: 0x040001F6 RID: 502
	public float force = 2000f;

	// Token: 0x040001F7 RID: 503
	public float breakDistance = 2f;

	// Token: 0x040001F8 RID: 504
	public float drag = 0.98f;

	// Token: 0x040001F9 RID: 505
	private CharacterData target;

	// Token: 0x040001FA RID: 506
	private Vector2 startPos;

	// Token: 0x040001FB RID: 507
	public bool startPosIsMyPos;

	// Token: 0x040001FC RID: 508
	private float scale = 1f;

	// Token: 0x040001FD RID: 509
	private LineEffect line;

	// Token: 0x040001FE RID: 510
	[FoldoutGroup("SNAP", 0)]
	public float snapCapLineTime = 0.5f;

	// Token: 0x040001FF RID: 511
	[FoldoutGroup("SNAP", 0)]
	public float snapCapTimeTime = 0.5f;

	// Token: 0x04000200 RID: 512
	private bool done;

	// Token: 0x04000201 RID: 513
	private bool doneDone;
}
