using System;
using UnityEngine;

// Token: 0x020000C2 RID: 194
public class Saw : MonoBehaviour
{
	// Token: 0x06000414 RID: 1044 RVA: 0x00018D39 File Offset: 0x00016F39
	private void Start()
	{
		this.owner = base.transform.root.GetComponent<SpawnedAttack>().spawner;
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x00018D58 File Offset: 0x00016F58
	private void Update()
	{
		Player player = null;
		for (int i = 0; i < PlayerManager.instance.players.Count; i++)
		{
			Player player2 = PlayerManager.instance.players[i];
			if (player2 != this.owner && Vector3.Distance(player2.transform.position, base.transform.transform.position) < this.range * base.transform.localScale.x)
			{
				player = player2;
			}
		}
		if (player && PlayerManager.instance.CanSeePlayer(base.transform.position, player).canSee)
		{
			Vector3 normalized = (player.transform.position - base.transform.position).normalized;
			if (this.damage != 0f)
			{
				player.data.healthHandler.TakeDamage(TimeHandler.deltaTime * this.damage * normalized, base.transform.position, null, this.owner, true, false);
			}
			if (this.force != 0f)
			{
				float num = Mathf.Clamp(1f - Vector2.Distance(base.transform.position, player.transform.position) / this.range, 0f, 1f);
				ForceMultiplier component = player.GetComponent<ForceMultiplier>();
				if (component)
				{
					num *= component.multiplier;
				}
				this.forceDir = normalized;
				this.forceDir.y = this.forceDir.y * 0.5f;
				player.data.playerVel.AddForce(this.forceDir * base.transform.localScale.x * num * TimeHandler.deltaTime * this.force, 0);
				player.data.healthHandler.TakeForce(this.forceDir * num * 0.0005f * TimeHandler.deltaTime * this.force, 1, false, false, 0f);
			}
			for (int j = 0; j < this.parts.Length; j++)
			{
				if (!this.parts[j].isPlaying)
				{
					this.parts[j].Play();
				}
			}
			if (this.sparkTransform)
			{
				this.sparkTransform.transform.position = player.transform.position;
				if (normalized != Vector3.zero)
				{
					this.sparkTransform.rotation = Quaternion.LookRotation(normalized);
				}
			}
			GamefeelManager.GameFeel((normalized + Random.onUnitSphere).normalized * this.shake * TimeHandler.deltaTime * 20f);
			return;
		}
		for (int k = 0; k < this.parts.Length; k++)
		{
			if (this.parts[k].isPlaying)
			{
				this.parts[k].Stop();
			}
		}
	}

	// Token: 0x04000596 RID: 1430
	public Player owner;

	// Token: 0x04000597 RID: 1431
	public float range = 3f;

	// Token: 0x04000598 RID: 1432
	public float damage = 10f;

	// Token: 0x04000599 RID: 1433
	public float force;

	// Token: 0x0400059A RID: 1434
	public float shake = 1f;

	// Token: 0x0400059B RID: 1435
	public ParticleSystem[] parts;

	// Token: 0x0400059C RID: 1436
	public Transform sparkTransform;

	// Token: 0x0400059D RID: 1437
	private Vector3 forceDir;
}
