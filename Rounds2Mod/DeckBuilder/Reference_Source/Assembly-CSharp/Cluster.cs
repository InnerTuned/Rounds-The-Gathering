using System;
using UnityEngine;

// Token: 0x0200002F RID: 47
public class Cluster : MonoBehaviour
{
	// Token: 0x060000EE RID: 238 RVA: 0x000073A5 File Offset: 0x000055A5
	private void Start()
	{
		this.move = base.GetComponentInParent<MoveTransform>();
	}

	// Token: 0x060000EF RID: 239 RVA: 0x000073B4 File Offset: 0x000055B4
	private void Update()
	{
		if (this.move.distanceTravelled > this.distanceToTravel)
		{
			for (int i = 0; i < this.clusters; i++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(base.transform.root.gameObject, base.transform.root.position, base.transform.root.rotation);
				Cluster componentInChildren = gameObject.GetComponentInChildren<Cluster>();
				if (componentInChildren)
				{
					Object.Destroy(componentInChildren);
				}
				MoveTransform component = gameObject.GetComponent<MoveTransform>();
				if (component)
				{
					component.DontRunStart = true;
					component.velocity = base.transform.root.GetComponent<MoveTransform>().velocity;
					component.multiplier = base.transform.root.GetComponent<MoveTransform>().multiplier;
					component.velocity += base.transform.right * Random.Range(-this.spread, this.spread);
					component.velocity *= Random.Range(1f - this.velocitySpread * 0.01f, 1f + this.velocitySpread * 0.01f);
				}
			}
			Object.Destroy(this);
		}
	}

	// Token: 0x04000134 RID: 308
	public float distanceToTravel = 8f;

	// Token: 0x04000135 RID: 309
	public float spread;

	// Token: 0x04000136 RID: 310
	public float velocitySpread;

	// Token: 0x04000137 RID: 311
	public int clusters = 3;

	// Token: 0x04000138 RID: 312
	private MoveTransform move;
}
