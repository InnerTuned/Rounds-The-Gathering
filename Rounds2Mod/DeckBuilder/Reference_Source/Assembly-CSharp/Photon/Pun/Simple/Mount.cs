using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000290 RID: 656
	public class Mount : NetComponent, IOnPreQuit, IOnPreNetDestroy
	{
		// Token: 0x06000E45 RID: 3653 RVA: 0x00044AB5 File Offset: 0x00042CB5
		public override void OnAwake()
		{
			base.OnAwake();
			this.mountsLookup = this.netObj.GetComponent<MountsManager>();
		}

		// Token: 0x06000E46 RID: 3654 RVA: 0x00044ACE File Offset: 0x00042CCE
		public void OnPreQuit()
		{
			this.DismountAll();
		}

		// Token: 0x06000E47 RID: 3655 RVA: 0x00044AD6 File Offset: 0x00042CD6
		public void OnPreNetDestroy(NetObject rootNetObj)
		{
			if (rootNetObj == this.netObj)
			{
				this.DismountAll();
			}
		}

		// Token: 0x06000E48 RID: 3656 RVA: 0x00044AEC File Offset: 0x00042CEC
		public void DismountAll()
		{
			for (int i = this.mountedObjs.Count - 1; i >= 0; i--)
			{
				if (this.mountedObjs[i] as Component)
				{
					this.mountedObjs[i].ImmediateUnmount();
				}
			}
		}

		// Token: 0x06000E49 RID: 3657 RVA: 0x00044B3C File Offset: 0x00042D3C
		public static void ChangeMounting(IMountable mountable, Mount prevMount, Mount newMount)
		{
			if (prevMount != null)
			{
				prevMount.mountedObjs.Remove(mountable);
			}
			if (newMount != null)
			{
				List<IMountable> list = newMount.mountedObjs;
				if (!list.Contains(mountable))
				{
					list.Add(mountable);
				}
			}
		}

		// Token: 0x04000D66 RID: 3430
		public const string ROOT_MOUNT_NAME = "Root";

		// Token: 0x04000D67 RID: 3431
		[Tooltip("A Mount component can be associated with more than one mount name. The first root will always include 'Root'.")]
		[SerializeField]
		[HideInInspector]
		public MountSelector mountType = new MountSelector(1);

		// Token: 0x04000D68 RID: 3432
		[SerializeField]
		[HideInInspector]
		public int componentIndex;

		// Token: 0x04000D69 RID: 3433
		[NonSerialized]
		public List<IMountable> mountedObjs = new List<IMountable>();

		// Token: 0x04000D6A RID: 3434
		[NonSerialized]
		public MountsManager mountsLookup;
	}
}
