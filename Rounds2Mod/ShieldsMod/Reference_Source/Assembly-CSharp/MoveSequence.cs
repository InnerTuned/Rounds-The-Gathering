using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000084 RID: 132
public class MoveSequence : MonoBehaviour
{
	// Token: 0x060002D0 RID: 720 RVA: 0x00011E30 File Offset: 0x00010030
	private void Start()
	{
		base.gameObject.layer = 17;
		this.startPos = base.transform.localPosition;
		this.rig = base.GetComponent<Rigidbody2D>();
		this.map = base.GetComponentInParent<Map>();
		this.myKey = string.Concat(new object[]
		{
			"MapObect ",
			base.GetComponentInParent<Map>().levelID,
			" ",
			base.transform.GetSiblingIndex()
		});
		MapManager.instance.GetComponent<ChildRPC>().childRPCsInt.Add(this.myKey, new Action<int>(this.RPCA_SetTargetID));
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x00011EE5 File Offset: 0x000100E5
	private void OnDestroy()
	{
		if (MapManager.instance)
		{
			MapManager.instance.GetComponent<ChildRPC>().childRPCsInt.Remove(this.myKey);
		}
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x00011F10 File Offset: 0x00010110
	private void OnDrawGizmos()
	{
		for (int i = 0; i < this.positions.Length; i++)
		{
			Gizmos.DrawSphere(base.transform.position + this.positions[i], 0.25f + (float)i * 0.15f);
		}
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x00011F6C File Offset: 0x0001016C
	private void Update()
	{
		if (MapTransition.isTransitioning)
		{
			return;
		}
		if (!this.map.hasEntered)
		{
			return;
		}
		Vector2 vector = this.positions[this.targetID] + this.startPos;
		Vector2 vector2 = vector - base.transform.position;
		vector2 = Vector3.ClampMagnitude(vector2, this.cap);
		if (this.rig)
		{
			this.rig.gravityScale = 0f;
			this.rig.AddForce(vector2 * this.spring * CappedDeltaTime.time * this.rig.mass);
		}
		else
		{
			this.velocity += vector2 * this.spring * CappedDeltaTime.time;
			this.velocity -= this.velocity * this.drag * CappedDeltaTime.time;
			base.transform.position += this.velocity * TimeHandler.deltaTime;
		}
		if (PhotonNetwork.IsMasterClient && Vector2.Distance(base.transform.position, vector) < this.threshold)
		{
			this.counter += TimeHandler.deltaTime;
			if (this.counter > this.timeAtPos)
			{
				this.targetID++;
				if (this.targetID >= this.positions.Length)
				{
					this.targetID = 0;
				}
				MapManager.instance.GetComponent<ChildRPC>().CallFunction(this.myKey, this.targetID);
				this.counter = 0f;
			}
		}
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x0001213B File Offset: 0x0001033B
	private void RPCA_SetTargetID(int setValue)
	{
		this.targetID = setValue;
	}

	// Token: 0x040003EC RID: 1004
	private int targetID;

	// Token: 0x040003ED RID: 1005
	public Vector2[] positions;

	// Token: 0x040003EE RID: 1006
	public float drag = 1f;

	// Token: 0x040003EF RID: 1007
	public float spring = 1f;

	// Token: 0x040003F0 RID: 1008
	public float cap = 1f;

	// Token: 0x040003F1 RID: 1009
	public float threshold = 1f;

	// Token: 0x040003F2 RID: 1010
	public float timeAtPos;

	// Token: 0x040003F3 RID: 1011
	private float counter;

	// Token: 0x040003F4 RID: 1012
	private Vector2 startPos;

	// Token: 0x040003F5 RID: 1013
	private Vector2 velocity;

	// Token: 0x040003F6 RID: 1014
	private Rigidbody2D rig;

	// Token: 0x040003F7 RID: 1015
	private Map map;

	// Token: 0x040003F8 RID: 1016
	private string myKey;
}
