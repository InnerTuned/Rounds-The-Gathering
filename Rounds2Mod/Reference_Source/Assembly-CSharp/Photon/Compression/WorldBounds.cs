using System;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Compression
{
	// Token: 0x0200021A RID: 538
	[ExecuteInEditMode]
	public class WorldBounds : MonoBehaviour
	{
		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000BFA RID: 3066 RVA: 0x0003D5E4 File Offset: 0x0003B7E4
		// (set) Token: 0x06000BFB RID: 3067 RVA: 0x0003D5EC File Offset: 0x0003B7EC
		public Bounds ManualBounds
		{
			get
			{
				return this.manualBounds;
			}
			set
			{
				this.manualBounds = value;
				this.CollectMyBounds();
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000BFC RID: 3068 RVA: 0x0003D5FB File Offset: 0x0003B7FB
		// (set) Token: 0x06000BFD RID: 3069 RVA: 0x0003D603 File Offset: 0x0003B803
		public BoundsTools.BoundsType FactorIn
		{
			get
			{
				return this.factorIn;
			}
			set
			{
				this.factorIn = value;
				this.CollectMyBounds();
			}
		}

		// Token: 0x06000BFE RID: 3070 RVA: 0x0003D612 File Offset: 0x0003B812
		private void Awake()
		{
			this.CollectMyBounds();
		}

		// Token: 0x06000BFF RID: 3071 RVA: 0x0003D61C File Offset: 0x0003B81C
		public void CollectMyBounds()
		{
			WorldBoundsSettings single = SettingsScriptableObject<WorldBoundsSettings>.Single;
			if (!single)
			{
				return;
			}
			if (SettingsScriptableObject<WorldBoundsSettings>.Single.worldBoundsGroups.Count == 0)
			{
				SettingsScriptableObject<WorldBoundsSettings>.Single.worldBoundsGroups.Add(new WorldBoundsGroup());
			}
			if (this.worldBoundsGrp >= SettingsScriptableObject<WorldBoundsSettings>.Single.worldBoundsGroups.Count)
			{
				this.worldBoundsGrp = 0;
			}
			WorldBoundsGroup worldBoundsGroup = single.worldBoundsGroups[this.worldBoundsGrp];
			if (this.factorIn == BoundsTools.BoundsType.Manual)
			{
				this.myBounds = this.manualBounds;
				this.myBoundsCount = 1;
			}
			else
			{
				this.myBounds = base.gameObject.CollectMyBounds(this.factorIn, out this.myBoundsCount, this.includeChildren, false);
			}
			WorldBoundsSettings.RemoveWorldBoundsFromAll(this);
			if (this.myBoundsCount > 0 && base.enabled && !worldBoundsGroup.activeWorldBounds.Contains(this))
			{
				worldBoundsGroup.activeWorldBounds.Add(this);
				worldBoundsGroup.RecalculateWorldCombinedBounds();
			}
			if (this.OnWorldBoundsChange != null)
			{
				this.OnWorldBoundsChange.Invoke();
			}
		}

		// Token: 0x06000C00 RID: 3072 RVA: 0x000027C8 File Offset: 0x000009C8
		private void Start()
		{
		}

		// Token: 0x06000C01 RID: 3073 RVA: 0x0003D719 File Offset: 0x0003B919
		private void OnEnable()
		{
			this.FactorInBounds(true);
		}

		// Token: 0x06000C02 RID: 3074 RVA: 0x0003D722 File Offset: 0x0003B922
		private void OnDisable()
		{
			this.FactorInBounds(false);
		}

		// Token: 0x06000C03 RID: 3075 RVA: 0x0003D72C File Offset: 0x0003B92C
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireCube(SettingsScriptableObject<WorldBoundsSettings>.Single.worldBoundsGroups[this.worldBoundsGrp]._combinedWorldBounds.center, SettingsScriptableObject<WorldBoundsSettings>.Single.worldBoundsGroups[this.worldBoundsGrp]._combinedWorldBounds.size);
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x0003D788 File Offset: 0x0003B988
		public void FactorInBounds(bool b)
		{
			if (this == null)
			{
				return;
			}
			if (this.worldBoundsGrp >= SettingsScriptableObject<WorldBoundsSettings>.Single.worldBoundsGroups.Count)
			{
				this.worldBoundsGrp = 0;
			}
			WorldBoundsGroup worldBoundsGroup = SettingsScriptableObject<WorldBoundsSettings>.Single.worldBoundsGroups[this.worldBoundsGrp];
			if (b)
			{
				if (!worldBoundsGroup.activeWorldBounds.Contains(this))
				{
					worldBoundsGroup.activeWorldBounds.Add(this);
				}
			}
			else
			{
				worldBoundsGroup.activeWorldBounds.Remove(this);
			}
			worldBoundsGroup.RecalculateWorldCombinedBounds();
		}

		// Token: 0x04000C29 RID: 3113
		[SerializeField]
		[HideInInspector]
		private Bounds manualBounds = new Bounds(new Vector3(0f, 0f, 0f), new Vector3(600f, 40f, 600f));

		// Token: 0x04000C2A RID: 3114
		[Tooltip("Selects which WorldBounds group this object should be factored into.")]
		[WorldBoundsSelectAttribute]
		[SerializeField]
		[HideInInspector]
		public int worldBoundsGrp;

		// Token: 0x04000C2B RID: 3115
		[SerializeField]
		[HideInInspector]
		private bool includeChildren = true;

		// Token: 0x04000C2C RID: 3116
		[Tooltip("Awake/Destroy will consider element into the world size as long as it exists in the scene (You may need to wake it though). Enable/Disable only factors it in if it is active.")]
		[SerializeField]
		[HideInInspector]
		private BoundsTools.BoundsType factorIn;

		// Token: 0x04000C2D RID: 3117
		[HideInInspector]
		public Bounds myBounds;

		// Token: 0x04000C2E RID: 3118
		[HideInInspector]
		public int myBoundsCount;

		// Token: 0x04000C2F RID: 3119
		public Action OnWorldBoundsChange;
	}
}
