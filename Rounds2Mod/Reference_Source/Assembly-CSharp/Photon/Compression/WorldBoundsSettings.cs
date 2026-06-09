using System;
using System.Collections.Generic;
using emotitron.Compression;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Compression
{
	// Token: 0x0200021D RID: 541
	[CreateAssetMenu]
	public class WorldBoundsSettings : SettingsScriptableObject<WorldBoundsSettings>
	{
		// Token: 0x06000C0C RID: 3084 RVA: 0x0003DAA0 File Offset: 0x0003BCA0
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Bootstrap()
		{
			WorldBoundsSettings single = SettingsScriptableObject<WorldBoundsSettings>.Single;
			List<WorldBoundsGroup> list = single.worldBoundsGroups;
			if (single != null && list.Count > 0)
			{
				WorldBoundsSettings.defaultWorldBoundsCrusher = single.worldBoundsGroups[0].crusher;
			}
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x0003DAE2 File Offset: 0x0003BCE2
		protected override void Awake()
		{
			base.Awake();
			if (this.worldBoundsGroups.Count == 0)
			{
				this.worldBoundsGroups.Add(new WorldBoundsGroup());
			}
		}

		// Token: 0x06000C0E RID: 3086 RVA: 0x0003DB08 File Offset: 0x0003BD08
		public override void Initialize()
		{
			base.Initialize();
			if (this.worldBoundsGroups.Count == 0)
			{
				this.worldBoundsGroups.Add(new WorldBoundsGroup());
			}
			WorldBoundsSettings.defaultWorldBoundsCrusher = this.worldBoundsGroups[0].crusher;
			foreach (WorldBoundsGroup worldBoundsGroup in this.worldBoundsGroups)
			{
				worldBoundsGroup.RecalculateWorldCombinedBounds();
			}
		}

		// Token: 0x06000C0F RID: 3087 RVA: 0x0003DB94 File Offset: 0x0003BD94
		public static void RemoveWorldBoundsFromAll(WorldBounds wb)
		{
			List<WorldBoundsGroup> list = SettingsScriptableObject<WorldBoundsSettings>.Single.worldBoundsGroups;
			for (int i = 0; i < list.Count; i++)
			{
				List<WorldBounds> activeWorldBounds = list[i].activeWorldBounds;
				if (activeWorldBounds.Contains(wb))
				{
					activeWorldBounds.Remove(wb);
					list[i].RecalculateWorldCombinedBounds();
				}
			}
		}

		// Token: 0x06000C10 RID: 3088 RVA: 0x0003DBE8 File Offset: 0x0003BDE8
		public static int TallyBits(ref int index, BitCullingLevel bcl = 0)
		{
			List<WorldBoundsGroup> list = SettingsScriptableObject<WorldBoundsSettings>.Single.worldBoundsGroups;
			if (index >= list.Count)
			{
				index = 0;
			}
			ElementCrusher crusher = SettingsScriptableObject<WorldBoundsSettings>.single.worldBoundsGroups[index].crusher;
			return crusher.XCrusher.GetBits(bcl) + crusher.YCrusher.GetBits(bcl) + crusher.ZCrusher.GetBits(bcl);
		}

		// Token: 0x04000C37 RID: 3127
		[HideInInspector]
		public List<WorldBoundsGroup> worldBoundsGroups = new List<WorldBoundsGroup>();

		// Token: 0x04000C38 RID: 3128
		public static ElementCrusher defaultWorldBoundsCrusher;
	}
}
