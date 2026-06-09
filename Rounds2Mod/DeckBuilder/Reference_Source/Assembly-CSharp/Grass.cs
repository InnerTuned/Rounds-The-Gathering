using System;
using UnityEngine;

// Token: 0x02000151 RID: 337
public class Grass : MonoBehaviour
{
	// Token: 0x060006CF RID: 1743 RVA: 0x00025A40 File Offset: 0x00023C40
	private void Start()
	{
		if (Random.value > 0.5f)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		base.GetComponentInChildren<SpriteRenderer>().sprite = this.sprites[Random.Range(0, this.sprites.Length)];
		this.spring = Random.Range(this.minSpring, this.maxSpring);
		this.drag = Random.Range(this.minDrag, this.maxDrag);
		this.playerEffect = Random.Range(this.minPlayerEffect, this.maxPlayerEffect);
		this.currentRot = base.transform.localEulerAngles.z;
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x00025AE0 File Offset: 0x00023CE0
	private void Update()
	{
		this.velocity = Mathf.Lerp(this.velocity, (this.target - this.currentRot) * this.spring, CappedDeltaTime.time * this.drag);
		this.currentRot += this.velocity * CappedDeltaTime.time;
		base.transform.localEulerAngles = new Vector3(0f, 0f, this.currentRot);
		if (PlayerManager.instance.players != null && PlayerManager.instance.players.Count > 0 && Vector2.Distance(PlayerManager.instance.players[0].data.playerVel.position, base.transform.position) < 1.5f)
		{
			this.AddForce(PlayerManager.instance.players[0].data.playerVel.velocity.x * this.playerEffect * CappedDeltaTime.time * 60f);
		}
		float num = 0.5f;
		float num2 = 0.05f;
		this.AddForce((Mathf.PerlinNoise(base.transform.position.x * num2 + Time.time * num, base.transform.position.y * num2 + Time.time * num) - 0.5f) * 30f * this.playerEffect * CappedDeltaTime.time * 60f);
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x00025C5A File Offset: 0x00023E5A
	public void AddForce(float f)
	{
		this.velocity += f;
	}

	// Token: 0x04000825 RID: 2085
	public float target;

	// Token: 0x04000826 RID: 2086
	private float velocity;

	// Token: 0x04000827 RID: 2087
	private float currentRot;

	// Token: 0x04000828 RID: 2088
	public float minSpring;

	// Token: 0x04000829 RID: 2089
	public float maxSpring;

	// Token: 0x0400082A RID: 2090
	public float minDrag;

	// Token: 0x0400082B RID: 2091
	public float maxDrag;

	// Token: 0x0400082C RID: 2092
	private float spring;

	// Token: 0x0400082D RID: 2093
	private float drag;

	// Token: 0x0400082E RID: 2094
	public float minPlayerEffect;

	// Token: 0x0400082F RID: 2095
	public float maxPlayerEffect;

	// Token: 0x04000830 RID: 2096
	private float playerEffect;

	// Token: 0x04000831 RID: 2097
	public Sprite[] sprites;
}
