using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002F2 RID: 754
	public class AutoLayerByAuthority : MonoBehaviour
	{
		// Token: 0x0600102E RID: 4142 RVA: 0x0004E7A4 File Offset: 0x0004C9A4
		private void Awake()
		{
			Physics.IgnoreLayerCollision(this.authorityLayer, this.projectileLayer);
			if (this.applyToChildren)
			{
				this.SetChildrenLayer(base.transform, this.projectileLayer);
				return;
			}
			base.gameObject.layer = this.projectileLayer;
		}

		// Token: 0x0600102F RID: 4143 RVA: 0x0004E7E4 File Offset: 0x0004C9E4
		public void OnChangeAuthority(bool IsMine, bool serverIsActive)
		{
			global::Debug.Log("Auth change " + base.name);
			int layer = IsMine ? this.authorityLayer : this.nonAuthorityLayer;
			if (this.applyToChildren)
			{
				this.SetChildrenLayer(base.transform, layer);
				return;
			}
			base.gameObject.layer = layer;
		}

		// Token: 0x06001030 RID: 4144 RVA: 0x0004E83C File Offset: 0x0004CA3C
		public void SetChildrenLayer(Transform t, int layer)
		{
			t.gameObject.layer = layer;
			for (int i = 0; i < t.childCount; i++)
			{
				if (!t.GetChild(i).GetComponent<AutoLayerByAuthority>())
				{
					this.SetChildrenLayer(t.GetChild(i), layer);
				}
			}
		}

		// Token: 0x04000F45 RID: 3909
		public int authorityLayer = 8;

		// Token: 0x04000F46 RID: 3910
		public int nonAuthorityLayer = 9;

		// Token: 0x04000F47 RID: 3911
		public int projectileLayer = 10;

		// Token: 0x04000F48 RID: 3912
		public bool applyToChildren = true;
	}
}
