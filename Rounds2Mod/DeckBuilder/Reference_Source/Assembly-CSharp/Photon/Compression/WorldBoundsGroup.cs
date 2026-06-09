using System;
using System.Collections.Generic;
using emotitron.Compression;
using UnityEngine;

namespace Photon.Compression
{
	// Token: 0x0200021B RID: 539
	[Serializable]
	public class WorldBoundsGroup
	{
		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000C06 RID: 3078 RVA: 0x0003D855 File Offset: 0x0003BA55
		public int ActiveBoundsObjCount
		{
			get
			{
				return this.activeWorldBounds.Count;
			}
		}

		// Token: 0x06000C07 RID: 3079 RVA: 0x0003D864 File Offset: 0x0003BA64
		public static ElementCrusher GetUncompressedCrusher()
		{
			return new ElementCrusher(0, false)
			{
				enableLocalSelector = false,
				hideFieldName = true,
				XCrusher = new FloatCrusher
				{
					axis = 0,
					outOfBoundsHandling = 1,
					Resolution = 100UL,
					BitsDeterminedBy = -5
				},
				YCrusher = new FloatCrusher
				{
					axis = 1,
					outOfBoundsHandling = 1,
					Resolution = 100UL,
					BitsDeterminedBy = -5
				},
				ZCrusher = new FloatCrusher
				{
					axis = 2,
					outOfBoundsHandling = 1,
					Resolution = 100UL,
					BitsDeterminedBy = -5
				}
			};
		}

		// Token: 0x06000C08 RID: 3080 RVA: 0x0003D904 File Offset: 0x0003BB04
		public void ResetActiveBounds()
		{
			this.activeWorldBounds.Clear();
		}

		// Token: 0x06000C09 RID: 3081 RVA: 0x0003D914 File Offset: 0x0003BB14
		public void RecalculateWorldCombinedBounds()
		{
			FloatCrusher xcrusher = this.crusher.XCrusher;
			FloatCrusher ycrusher = this.crusher.YCrusher;
			FloatCrusher zcrusher = this.crusher.ZCrusher;
			if (this.activeWorldBounds.Count == 0)
			{
				this._combinedWorldBounds = default(Bounds);
			}
			else
			{
				if (xcrusher.BitsDeterminedBy > 0 || xcrusher.BitsDeterminedBy == -1 || xcrusher.BitsDeterminedBy == -5 || xcrusher.BitsDeterminedBy == -4)
				{
					xcrusher.Resolution = 100UL;
					xcrusher.BitsDeterminedBy = -3;
				}
				if (ycrusher.BitsDeterminedBy > 0 || ycrusher.BitsDeterminedBy == -1 || ycrusher.BitsDeterminedBy == -5 || ycrusher.BitsDeterminedBy == -4)
				{
					ycrusher.Resolution = 100UL;
					ycrusher.BitsDeterminedBy = -3;
				}
				if (zcrusher.BitsDeterminedBy > 0 || zcrusher.BitsDeterminedBy == -1 || zcrusher.BitsDeterminedBy == -5 || zcrusher.BitsDeterminedBy == -4)
				{
					zcrusher.Resolution = 100UL;
					zcrusher.BitsDeterminedBy = -3;
				}
				this._combinedWorldBounds = this.activeWorldBounds[0].myBounds;
				for (int i = 1; i < this.activeWorldBounds.Count; i++)
				{
					this._combinedWorldBounds.Encapsulate(this.activeWorldBounds[i].myBounds);
				}
				this.crusher.Bounds = this._combinedWorldBounds;
			}
			if (this.OnWorldBoundChanged != null)
			{
				this.OnWorldBoundChanged.Invoke();
			}
		}

		// Token: 0x04000C30 RID: 3120
		public const string defaultName = "Default";

		// Token: 0x04000C31 RID: 3121
		public const string newAddName = "Unnamed";

		// Token: 0x04000C32 RID: 3122
		public string name = "Default";

		// Token: 0x04000C33 RID: 3123
		[NonSerialized]
		public Action OnWorldBoundChanged;

		// Token: 0x04000C34 RID: 3124
		public ElementCrusher crusher = WorldBoundsGroup.GetUncompressedCrusher();

		// Token: 0x04000C35 RID: 3125
		[NonSerialized]
		public readonly List<WorldBounds> activeWorldBounds = new List<WorldBounds>();

		// Token: 0x04000C36 RID: 3126
		[NonSerialized]
		public Bounds _combinedWorldBounds;
	}
}
