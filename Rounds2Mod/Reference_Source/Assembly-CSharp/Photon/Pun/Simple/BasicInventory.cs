using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000266 RID: 614
	public class BasicInventory : Inventory<Vector3Int>
	{
		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000D69 RID: 3433 RVA: 0x0004216C File Offset: 0x0004036C
		public int Volume
		{
			get
			{
				return this.capacity.x * this.capacity.y * this.capacity.z;
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000D6A RID: 3434 RVA: 0x00042194 File Offset: 0x00040394
		public int Used
		{
			get
			{
				int num = 0;
				List<IMountable> mountedObjs = base.DefaultMount.mountedObjs;
				int i = 0;
				int count = mountedObjs.Count;
				while (i < count)
				{
					IInventoryable<Vector3Int> inventoryable = mountedObjs[i] as IInventoryable<Vector3Int>;
					if (inventoryable != null)
					{
						Vector3Int size = inventoryable.Size;
						int num2 = size.x * size.y * size.z;
						num += num2;
					}
					i++;
				}
				return num;
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000D6B RID: 3435 RVA: 0x000421FC File Offset: 0x000403FC
		public int Remaining
		{
			get
			{
				return this.Volume - this.Used;
			}
		}

		// Token: 0x06000D6C RID: 3436 RVA: 0x0004220C File Offset: 0x0004040C
		public override bool TestCapacity(IInventoryable<Vector3Int> inventoryable)
		{
			Vector3Int size = inventoryable.Size;
			return size.x * size.y * size.z <= this.Remaining;
		}

		// Token: 0x04000CD7 RID: 3287
		[SerializeField]
		public Vector3Int capacity = new Vector3Int(16, 1, 1);
	}
}
