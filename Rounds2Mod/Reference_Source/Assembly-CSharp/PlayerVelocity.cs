using System;
using UnityEngine;

// Token: 0x0200018F RID: 399
public class PlayerVelocity : MonoBehaviour
{
	// Token: 0x17000039 RID: 57
	// (get) Token: 0x0600080C RID: 2060 RVA: 0x0002BEFF File Offset: 0x0002A0FF
	// (set) Token: 0x0600080D RID: 2061 RVA: 0x0002BF11 File Offset: 0x0002A111
	public Vector2 position
	{
		get
		{
			return base.transform.position;
		}
		set
		{
			base.transform.position = value;
		}
	}

	// Token: 0x0600080E RID: 2062 RVA: 0x000027C8 File Offset: 0x000009C8
	internal void AddTorque(float v)
	{
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x0002BF24 File Offset: 0x0002A124
	private void Start()
	{
		this.data = base.GetComponent<CharacterData>();
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x0002BF34 File Offset: 0x0002A134
	private void FixedUpdate()
	{
		if (!this.data.isPlaying)
		{
			return;
		}
		if (this.isKinematic)
		{
			this.velocity *= 0f;
		}
		if (this.simulated && !this.isKinematic)
		{
			this.velocity += Vector2.down * Time.fixedDeltaTime * TimeHandler.timeScale * 20f;
			base.transform.position += Time.fixedDeltaTime * TimeHandler.timeScale * this.velocity;
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, 0f);
		}
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x0002C01D File Offset: 0x0002A21D
	internal void AddForce(Vector2 force, ForceMode2D forceMode)
	{
		if (forceMode == null)
		{
			force *= 0.02f;
		}
		else
		{
			force *= 1f;
		}
		this.velocity += force / this.mass;
	}

	// Token: 0x06000812 RID: 2066 RVA: 0x0002C05B File Offset: 0x0002A25B
	internal void AddForce(Vector3 force, ForceMode2D forceMode)
	{
		this.AddForce(force, forceMode);
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x0002C06A File Offset: 0x0002A26A
	internal void AddForce(Vector2 force)
	{
		this.AddForce(force, 0);
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x0002C074 File Offset: 0x0002A274
	internal void AddForce(Vector3 force)
	{
		this.AddForce(force, 0);
	}

	// Token: 0x04000963 RID: 2403
	internal bool simulated = true;

	// Token: 0x04000964 RID: 2404
	internal bool isKinematic;

	// Token: 0x04000965 RID: 2405
	internal Vector2 velocity;

	// Token: 0x04000966 RID: 2406
	internal float mass = 100f;

	// Token: 0x04000967 RID: 2407
	internal float angularVelocity;

	// Token: 0x04000968 RID: 2408
	private CharacterData data;
}
