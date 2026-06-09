using System;
using System.Collections.Generic;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000294 RID: 660
	[AddComponentMenu("")]
	[DisallowMultipleComponent]
	public class MountsManager : MonoBehaviour, IOnAwake, IOnPreQuit
	{
		// Token: 0x06000E58 RID: 3672 RVA: 0x00044CC1 File Offset: 0x00042EC1
		public void OnAwake()
		{
			this.CollectMounts(false);
			this.bitsForMountId = ((this.indexedMounts.Count == 0) ? 0 : (this.indexedMounts.Count - 1).GetBitsForMaxValue());
		}

		// Token: 0x06000E59 RID: 3673 RVA: 0x00044CF3 File Offset: 0x00042EF3
		public void OnPreQuit()
		{
			this.UnmountAll();
		}

		// Token: 0x06000E5A RID: 3674 RVA: 0x00044CFC File Offset: 0x00042EFC
		public List<Mount> CollectMounts(bool force = false)
		{
			this.indexedMounts.Clear();
			this.mountIdLookup.Clear();
			NestedComponentUtilities.GetNestedComponentsInChildren<Mount, NetObject>(base.transform, this.indexedMounts, true);
			int i = 0;
			int count = this.indexedMounts.Count;
			while (i < count)
			{
				Mount mount = this.indexedMounts[i];
				mount.componentIndex = i;
				int id = mount.mountType.id;
				if (!this.mountIdLookup.ContainsKey(id))
				{
					this.mountIdLookup.Add(id, mount);
				}
				i++;
			}
			return this.indexedMounts;
		}

		// Token: 0x06000E5B RID: 3675 RVA: 0x00044D8C File Offset: 0x00042F8C
		public Mount GetMount(int mountId)
		{
			Mount result;
			this.mountIdLookup.TryGetValue(mountId, ref result);
			return result;
		}

		// Token: 0x06000E5C RID: 3676 RVA: 0x00044DAC File Offset: 0x00042FAC
		public void UnmountAll()
		{
			int i = 0;
			int count = this.indexedMounts.Count;
			while (i < count)
			{
				this.indexedMounts[i].DismountAll();
				i++;
			}
		}

		// Token: 0x04000D70 RID: 3440
		[NonSerialized]
		public Dictionary<int, Mount> mountIdLookup = new Dictionary<int, Mount>();

		// Token: 0x04000D71 RID: 3441
		[NonSerialized]
		public List<Mount> indexedMounts = new List<Mount>();

		// Token: 0x04000D72 RID: 3442
		[NonSerialized]
		public int bitsForMountId;
	}
}
