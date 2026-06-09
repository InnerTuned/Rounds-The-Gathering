using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x0200026A RID: 618
	public class InventoryContactReactors : InventoryContactReactors<Vector3Int>
	{
		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x06000D6F RID: 3439 RVA: 0x00042259 File Offset: 0x00040459
		public override Vector3Int Size
		{
			get
			{
				return this.size;
			}
		}

		// Token: 0x06000D70 RID: 3440 RVA: 0x00042261 File Offset: 0x00040461
		public override void OnAwakeInitialize(bool isNetObject)
		{
			base.OnAwakeInitialize(isNetObject);
			this.volume = this.size.x * this.size.y * this.size.z;
		}

		// Token: 0x04000CD8 RID: 3288
		[SerializeField]
		protected Vector3Int size = new Vector3Int(1, 1, 1);
	}
}
