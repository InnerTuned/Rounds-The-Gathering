using System;
using UnityEngine;

// Token: 0x02000085 RID: 133
public class MoveTransform : MonoBehaviour
{
	// Token: 0x060002D6 RID: 726 RVA: 0x00012178 File Offset: 0x00010378
	private void Start()
	{
		if (this.DontRunStart)
		{
			return;
		}
		this.velocity += base.transform.TransformDirection(this.localForce) + this.worldForce;
		base.transform.rotation = Quaternion.LookRotation(this.velocity, Vector3.forward);
		if (this.spread != 0f)
		{
			this.velocity += base.transform.up * this.selectedSpread;
		}
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x0001220C File Offset: 0x0001040C
	private void Update()
	{
		float num = Mathf.Clamp(TimeHandler.deltaTime, 0f, 0.02f);
		float num2 = TimeHandler.deltaTime;
		num *= this.simulationSpeed;
		num2 *= this.simulationSpeed;
		if (this.simulateGravity == 0)
		{
			this.velocity += this.gravity * Vector3.down * num2 * this.multiplier;
		}
		if ((this.velocity.magnitude > 2f || this.allowStop) && this.velocity.magnitude > this.dragMinSpeed)
		{
			this.velocity -= this.velocity * Mathf.Clamp(this.drag * num * Mathf.Clamp(this.multiplier, 0f, 1f), 0f, 1f);
		}
		base.transform.position += this.velocity * num2 * this.multiplier;
		this.distanceTravelled += this.velocity.magnitude * num2 * this.multiplier;
		base.transform.rotation = Quaternion.LookRotation(this.velocity, Vector3.forward);
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x00012360 File Offset: 0x00010560
	public float GetUpwardsCompensation(Vector2 start, Vector2 end)
	{
		start.y = 0.5f;
		end.y = 0.5f;
		return Mathf.Pow(Vector3.Distance(start, end), 2.06f) * this.gravity / this.velocity.magnitude * 0.012f;
	}

	// Token: 0x040003F9 RID: 1017
	public float gravity = 30f;

	// Token: 0x040003FA RID: 1018
	public float drag;

	// Token: 0x040003FB RID: 1019
	public float dragMinSpeed = 1f;

	// Token: 0x040003FC RID: 1020
	public float velocitySpread;

	// Token: 0x040003FD RID: 1021
	public float spread;

	// Token: 0x040003FE RID: 1022
	public Vector3 localForce;

	// Token: 0x040003FF RID: 1023
	public Vector3 worldForce;

	// Token: 0x04000400 RID: 1024
	public float multiplier = 1f;

	// Token: 0x04000401 RID: 1025
	public Vector3 velocity;

	// Token: 0x04000402 RID: 1026
	[HideInInspector]
	public float distanceTravelled;

	// Token: 0x04000403 RID: 1027
	[HideInInspector]
	public bool DontRunStart;

	// Token: 0x04000404 RID: 1028
	public float selectedSpread;

	// Token: 0x04000405 RID: 1029
	public bool allowStop;

	// Token: 0x04000406 RID: 1030
	public int simulateGravity;

	// Token: 0x04000407 RID: 1031
	private int randomSeed;

	// Token: 0x04000408 RID: 1032
	[HideInInspector]
	internal float simulationSpeed = 1f;
}
