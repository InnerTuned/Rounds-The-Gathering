using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002F3 RID: 755
	public class GenericSpawnPoint : MonoBehaviour
	{
		// Token: 0x06001032 RID: 4146 RVA: 0x0004E8AD File Offset: 0x0004CAAD
		private void OnEnable()
		{
			GenericSpawnPoint.spawns.Add(this);
		}

		// Token: 0x06001033 RID: 4147 RVA: 0x0004E8BA File Offset: 0x0004CABA
		private void OnDisable()
		{
			GenericSpawnPoint.spawns.Remove(this);
		}

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x06001034 RID: 4148 RVA: 0x0004E8C8 File Offset: 0x0004CAC8
		public bool IsBlocked
		{
			get
			{
				return Physics.OverlapSphereNonAlloc(base.transform.position, this.blockedCheckRadius, GenericSpawnPoint.reusable, this.layerMask) != 0;
			}
		}

		// Token: 0x06001035 RID: 4149 RVA: 0x0004E8F8 File Offset: 0x0004CAF8
		public static Transform GetRandomSpawnPoint(bool avoidCollision = true)
		{
			if (GenericSpawnPoint.spawns.Count == 0)
			{
				return null;
			}
			int num = Random.Range(0, GenericSpawnPoint.spawns.Count - 1);
			if (avoidCollision)
			{
				for (int i = 0; i < GenericSpawnPoint.spawns.Count; i++)
				{
					if (!GenericSpawnPoint.spawns[(i + num) % GenericSpawnPoint.spawns.Count].IsBlocked)
					{
						return GenericSpawnPoint.spawns[(i + num) % GenericSpawnPoint.spawns.Count].transform;
					}
				}
			}
			return GenericSpawnPoint.spawns[num].transform;
		}

		// Token: 0x06001036 RID: 4150 RVA: 0x0004E98C File Offset: 0x0004CB8C
		public static Transform GetNextSpawnPoint(bool avoidCollision = true)
		{
			if (GenericSpawnPoint.spawns.Count == 0)
			{
				return null;
			}
			GenericSpawnPoint.lastPicked = (GenericSpawnPoint.lastPicked + 1) % GenericSpawnPoint.spawns.Count;
			if (avoidCollision)
			{
				for (int i = 0; i < GenericSpawnPoint.spawns.Count; i++)
				{
					int num = (i + GenericSpawnPoint.lastPicked) % GenericSpawnPoint.spawns.Count;
					if (!GenericSpawnPoint.spawns[num].IsBlocked)
					{
						GenericSpawnPoint.lastPicked = num;
						break;
					}
				}
			}
			return GenericSpawnPoint.spawns[GenericSpawnPoint.lastPicked].transform;
		}

		// Token: 0x06001037 RID: 4151 RVA: 0x0004EA18 File Offset: 0x0004CC18
		public static Transform GetSpawnPointFromValue(int value)
		{
			if (GenericSpawnPoint.spawns.Count == 0)
			{
				return null;
			}
			int num = (value + 1) % GenericSpawnPoint.spawns.Count;
			return GenericSpawnPoint.spawns[num].transform;
		}

		// Token: 0x06001038 RID: 4152 RVA: 0x0004EA52 File Offset: 0x0004CC52
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(base.transform.position, this.blockedCheckRadius);
		}

		// Token: 0x04000F49 RID: 3913
		[Header("Spawn Point Blocked Check")]
		[Tooltip("Select the physics layers for colliders to test against. If 'avoidCollision' is true and any colliders on these layers are blocking the spawn point, will attempt to find the next spawn point that isn't blocked.")]
		public LayerMask layerMask;

		// Token: 0x04000F4A RID: 3914
		public float blockedCheckRadius = 2f;

		// Token: 0x04000F4B RID: 3915
		public static readonly List<GenericSpawnPoint> spawns = new List<GenericSpawnPoint>();

		// Token: 0x04000F4C RID: 3916
		private static int lastPicked;

		// Token: 0x04000F4D RID: 3917
		private static readonly Collider[] reusable = new Collider[8];
	}
}
