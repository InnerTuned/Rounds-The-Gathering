using System;
using System.Collections.Generic;
using emotitron.Utilities;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002AF RID: 687
	public class TeleportMarker : MonoBehaviour
	{
		// Token: 0x06000F27 RID: 3879 RVA: 0x00049944 File Offset: 0x00047B44
		private void OnEnable()
		{
			int hash = this.markerType.hash;
			if (!TeleportMarker.lookup.ContainsKey(hash))
			{
				TeleportMarker.lookup.Add(hash, new List<TeleportMarker>());
			}
			TeleportMarker.lookup[hash].Add(this);
		}

		// Token: 0x06000F28 RID: 3880 RVA: 0x0004998C File Offset: 0x00047B8C
		private void OnDisable()
		{
			int hash = this.markerType.hash;
			if (!TeleportMarker.lookup.ContainsKey(hash))
			{
				return;
			}
			if (!TeleportMarker.lookup[hash].Contains(this))
			{
				return;
			}
			TeleportMarker.lookup[hash].Remove(this);
		}

		// Token: 0x06000F29 RID: 3881 RVA: 0x000499DC File Offset: 0x00047BDC
		public static TeleportMarker GetRandomMarker(int hash, float seed)
		{
			if (!TeleportMarker.lookup.ContainsKey(hash))
			{
				return null;
			}
			List<TeleportMarker> list = TeleportMarker.lookup[hash];
			int num = Random.Range(0, list.Count);
			return list[num];
		}

		// Token: 0x06000F2A RID: 3882 RVA: 0x00049A18 File Offset: 0x00047C18
		public static TeleportMarker GetNextMarker(int hash)
		{
			if (!TeleportMarker.lookup.ContainsKey(hash))
			{
				return null;
			}
			List<TeleportMarker> list = TeleportMarker.lookup[hash];
			int num = TeleportMarker.nexts[hash];
			num++;
			if (num >= TeleportMarker.nexts.Count)
			{
				num = 0;
			}
			TeleportMarker.nexts[hash] = num;
			return list[num];
		}

		// Token: 0x04000E2C RID: 3628
		public MarkerNameType markerType;

		// Token: 0x04000E2D RID: 3629
		public static Dictionary<int, List<TeleportMarker>> lookup = new Dictionary<int, List<TeleportMarker>>();

		// Token: 0x04000E2E RID: 3630
		public static Dictionary<int, int> nexts = new Dictionary<int, int>();
	}
}
