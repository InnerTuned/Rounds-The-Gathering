using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x0200028E RID: 654
	public class EnableOnNetObjReady : MonoBehaviour, IOnNetObjReady
	{
		// Token: 0x06000E40 RID: 3648 RVA: 0x00044A35 File Offset: 0x00042C35
		public void Reset()
		{
			this.visibilityObject = base.gameObject;
		}

		// Token: 0x06000E41 RID: 3649 RVA: 0x00044A43 File Offset: 0x00042C43
		public void Awake()
		{
			if (this.visibilityObject == null)
			{
				this.visibilityObject = base.gameObject;
			}
		}

		// Token: 0x06000E42 RID: 3650 RVA: 0x00044A60 File Offset: 0x00042C60
		private void Start()
		{
			NetObject componentInParent = base.GetComponentInParent<NetObject>();
			if (componentInParent)
			{
				this.visibilityObject.SetActive(componentInParent.AllObjsAreReady);
			}
		}

		// Token: 0x06000E43 RID: 3651 RVA: 0x00044A8D File Offset: 0x00042C8D
		public void OnNetObjReadyChange(bool ready)
		{
			if (this.visibilityObject == null)
			{
				this.visibilityObject = base.gameObject;
			}
			this.visibilityObject.SetActive(ready);
		}

		// Token: 0x04000D60 RID: 3424
		public GameObject visibilityObject;
	}
}
