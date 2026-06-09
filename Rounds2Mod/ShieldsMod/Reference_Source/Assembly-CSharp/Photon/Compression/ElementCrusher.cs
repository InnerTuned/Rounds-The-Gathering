using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using emotitron.Compression;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Compression
{
	// Token: 0x02000209 RID: 521
	[Serializable]
	public class ElementCrusher : Crusher<ElementCrusher>, IEquatable<ElementCrusher>, ICrusherCopy<ElementCrusher>
	{
		// Token: 0x06000A38 RID: 2616 RVA: 0x00033D20 File Offset: 0x00031F20
		public static ElementCrusher GetStaticPositionCrusher(Bounds bounds, int resolution)
		{
			return ElementCrusher.CheckAgainstStatics(new ElementCrusher(ElementCrusher.StaticTRSType.Position)
			{
				XCrusher = FloatCrusher.GetStaticFloatCrusher(resolution, bounds.min.x, bounds.max.x, 4, 0),
				YCrusher = FloatCrusher.GetStaticFloatCrusher(resolution, bounds.min.y, bounds.max.y, 4, 0),
				ZCrusher = FloatCrusher.GetStaticFloatCrusher(resolution, bounds.min.z, bounds.max.z, 4, 0)
			}, true);
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x00033DAB File Offset: 0x00031FAB
		public static ElementCrusher GetStaticQuatCrusher(int minBits)
		{
			return ElementCrusher.CheckAgainstStatics(new ElementCrusher(ElementCrusher.StaticTRSType.Quaternion)
			{
				QCrusher = new QuatCrusher(false, false)
				{
					Bits = minBits
				}
			}, true);
		}

		// Token: 0x06000A3A RID: 2618 RVA: 0x00033DD0 File Offset: 0x00031FD0
		public static ElementCrusher CheckAgainstStatics(ElementCrusher ec, bool CheckAgainstFloatCrushersAsWell = true)
		{
			if (ec == null)
			{
				return null;
			}
			if (CheckAgainstFloatCrushersAsWell)
			{
				if (ec.cache_xEnabled)
				{
					ec.XCrusher = FloatCrusher.CheckAgainstStatics(ec._xcrusher);
				}
				if (ec.cache_yEnabled)
				{
					ec.YCrusher = FloatCrusher.CheckAgainstStatics(ec._ycrusher);
				}
				if (ec.cache_zEnabled)
				{
					ec.ZCrusher = FloatCrusher.CheckAgainstStatics(ec._zcrusher);
				}
				if (ec.cache_uEnabled)
				{
					ec.UCrusher = FloatCrusher.CheckAgainstStatics(ec._ucrusher);
				}
			}
			int hashCode = ec.GetHashCode();
			if (ElementCrusher.staticElementCrushers.ContainsKey(hashCode))
			{
				return ElementCrusher.staticElementCrushers[hashCode];
			}
			ElementCrusher.staticElementCrushers.Add(hashCode, ec);
			return ec;
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000A3B RID: 2619 RVA: 0x00033E76 File Offset: 0x00032076
		// (set) Token: 0x06000A3C RID: 2620 RVA: 0x00033E7E File Offset: 0x0003207E
		public TRSType TRSType
		{
			get
			{
				return this._trsType;
			}
			set
			{
				this._trsType = value;
				this._xcrusher.TRSType = value;
				this._ycrusher.TRSType = value;
				this._zcrusher.TRSType = value;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000A3D RID: 2621 RVA: 0x00033EAB File Offset: 0x000320AB
		// (set) Token: 0x06000A3E RID: 2622 RVA: 0x00033EB3 File Offset: 0x000320B3
		[Obsolete("Use the XCrusher property instead.")]
		public FloatCrusher xcrusher
		{
			get
			{
				return this.XCrusher;
			}
			set
			{
				this.XCrusher = value;
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000A3F RID: 2623 RVA: 0x00033EBC File Offset: 0x000320BC
		// (set) Token: 0x06000A40 RID: 2624 RVA: 0x00033EC4 File Offset: 0x000320C4
		[Obsolete("Use the YCrusher property instead.")]
		public FloatCrusher ycrusher
		{
			get
			{
				return this.YCrusher;
			}
			set
			{
				this.YCrusher = value;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000A41 RID: 2625 RVA: 0x00033ECD File Offset: 0x000320CD
		// (set) Token: 0x06000A42 RID: 2626 RVA: 0x00033ED5 File Offset: 0x000320D5
		[Obsolete("Use the ZCrusher property instead.")]
		public FloatCrusher zcrusher
		{
			get
			{
				return this.ZCrusher;
			}
			set
			{
				this.ZCrusher = value;
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000A43 RID: 2627 RVA: 0x00033EDE File Offset: 0x000320DE
		// (set) Token: 0x06000A44 RID: 2628 RVA: 0x00033EE6 File Offset: 0x000320E6
		[Obsolete("Use the UCrusher property instead.")]
		public FloatCrusher ucrusher
		{
			get
			{
				return this.UCrusher;
			}
			set
			{
				this.UCrusher = value;
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000A45 RID: 2629 RVA: 0x00033EEF File Offset: 0x000320EF
		// (set) Token: 0x06000A46 RID: 2630 RVA: 0x00033EF7 File Offset: 0x000320F7
		[Obsolete("Use the QCrusher property instead.")]
		public QuatCrusher qcrusher
		{
			get
			{
				return this.QCrusher;
			}
			set
			{
				this.QCrusher = value;
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000A47 RID: 2631 RVA: 0x00033F00 File Offset: 0x00032100
		// (set) Token: 0x06000A48 RID: 2632 RVA: 0x00033F08 File Offset: 0x00032108
		public FloatCrusher XCrusher
		{
			get
			{
				return this._xcrusher;
			}
			set
			{
				if (this._xcrusher == value)
				{
					return;
				}
				if (this._xcrusher != null)
				{
					FloatCrusher xcrusher = this._xcrusher;
					xcrusher.OnRecalculated = (Action<FloatCrusher>)Delegate.Remove(xcrusher.OnRecalculated, new Action<FloatCrusher>(this.OnCrusherChange));
				}
				this._xcrusher = value;
				if (this._xcrusher != null)
				{
					FloatCrusher xcrusher2 = this._xcrusher;
					xcrusher2.OnRecalculated = (Action<FloatCrusher>)Delegate.Combine(xcrusher2.OnRecalculated, new Action<FloatCrusher>(this.OnCrusherChange));
				}
				this.CacheValues();
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000A49 RID: 2633 RVA: 0x00033F8A File Offset: 0x0003218A
		// (set) Token: 0x06000A4A RID: 2634 RVA: 0x00033F94 File Offset: 0x00032194
		public FloatCrusher YCrusher
		{
			get
			{
				return this._ycrusher;
			}
			set
			{
				if (this._ycrusher == value)
				{
					return;
				}
				if (this._ycrusher != null)
				{
					FloatCrusher ycrusher = this._ycrusher;
					ycrusher.OnRecalculated = (Action<FloatCrusher>)Delegate.Remove(ycrusher.OnRecalculated, new Action<FloatCrusher>(this.OnCrusherChange));
				}
				this._ycrusher = value;
				if (this._ycrusher != null)
				{
					FloatCrusher ycrusher2 = this._ycrusher;
					ycrusher2.OnRecalculated = (Action<FloatCrusher>)Delegate.Combine(ycrusher2.OnRecalculated, new Action<FloatCrusher>(this.OnCrusherChange));
				}
				this.CacheValues();
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000A4B RID: 2635 RVA: 0x00034016 File Offset: 0x00032216
		// (set) Token: 0x06000A4C RID: 2636 RVA: 0x00034020 File Offset: 0x00032220
		public FloatCrusher ZCrusher
		{
			get
			{
				return this._zcrusher;
			}
			set
			{
				if (this._zcrusher == value)
				{
					return;
				}
				if (this._zcrusher != null)
				{
					FloatCrusher zcrusher = this._zcrusher;
					zcrusher.OnRecalculated = (Action<FloatCrusher>)Delegate.Remove(zcrusher.OnRecalculated, new Action<FloatCrusher>(this.OnCrusherChange));
				}
				this._zcrusher = value;
				if (this._zcrusher != null)
				{
					FloatCrusher zcrusher2 = this._zcrusher;
					zcrusher2.OnRecalculated = (Action<FloatCrusher>)Delegate.Combine(zcrusher2.OnRecalculated, new Action<FloatCrusher>(this.OnCrusherChange));
				}
				this.CacheValues();
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000A4D RID: 2637 RVA: 0x000340A2 File Offset: 0x000322A2
		// (set) Token: 0x06000A4E RID: 2638 RVA: 0x000340AC File Offset: 0x000322AC
		public FloatCrusher UCrusher
		{
			get
			{
				return this._ucrusher;
			}
			set
			{
				if (this._ucrusher == value)
				{
					return;
				}
				if (this._ucrusher != null)
				{
					FloatCrusher ucrusher = this._ucrusher;
					ucrusher.OnRecalculated = (Action<FloatCrusher>)Delegate.Remove(ucrusher.OnRecalculated, new Action<FloatCrusher>(this.OnCrusherChange));
				}
				this._ucrusher = value;
				if (this._ucrusher != null)
				{
					FloatCrusher ucrusher2 = this._ucrusher;
					ucrusher2.OnRecalculated = (Action<FloatCrusher>)Delegate.Combine(ucrusher2.OnRecalculated, new Action<FloatCrusher>(this.OnCrusherChange));
				}
				this.CacheValues();
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000A4F RID: 2639 RVA: 0x0003412E File Offset: 0x0003232E
		// (set) Token: 0x06000A50 RID: 2640 RVA: 0x00034138 File Offset: 0x00032338
		public QuatCrusher QCrusher
		{
			get
			{
				return this._qcrusher;
			}
			set
			{
				if (this._qcrusher == value)
				{
					return;
				}
				if (this._qcrusher != null)
				{
					QuatCrusher qcrusher = this._qcrusher;
					qcrusher.OnRecalculated = (Action<QuatCrusher>)Delegate.Remove(qcrusher.OnRecalculated, new Action<QuatCrusher>(this.OnCrusherChange));
				}
				this._qcrusher = value;
				if (this._qcrusher != null)
				{
					QuatCrusher qcrusher2 = this._qcrusher;
					qcrusher2.OnRecalculated = (Action<QuatCrusher>)Delegate.Combine(qcrusher2.OnRecalculated, new Action<QuatCrusher>(this.OnCrusherChange));
				}
				this.CacheValues();
			}
		}

		// Token: 0x06000A51 RID: 2641 RVA: 0x000341C6 File Offset: 0x000323C6
		public void OnCrusherChange(QuatCrusher crusher)
		{
			this.CacheValues();
		}

		// Token: 0x06000A52 RID: 2642 RVA: 0x000341C6 File Offset: 0x000323C6
		public void OnCrusherChange(FloatCrusher crusher)
		{
			this.CacheValues();
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000A53 RID: 2643 RVA: 0x000341CE File Offset: 0x000323CE
		// (set) Token: 0x06000A54 RID: 2644 RVA: 0x000341D6 File Offset: 0x000323D6
		[SerializeField]
		public bool UseWorldBounds
		{
			get
			{
				return this.useWorldBounds;
			}
			set
			{
				this.ApplyWorldCrusherSettings(value, this.boundsGroupId);
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000A55 RID: 2645 RVA: 0x000341E5 File Offset: 0x000323E5
		// (set) Token: 0x06000A56 RID: 2646 RVA: 0x000341ED File Offset: 0x000323ED
		public int BoundsGroupId
		{
			get
			{
				return this.boundsGroupId;
			}
			set
			{
				this.ApplyWorldCrusherSettings(this.useWorldBounds, value);
			}
		}

		// Token: 0x06000A57 RID: 2647 RVA: 0x000341FC File Offset: 0x000323FC
		private void OnWorldBoundsReady()
		{
			this.ApplyWorldCrusherSettings();
			this.CacheValues();
		}

		// Token: 0x06000A58 RID: 2648 RVA: 0x0003420C File Offset: 0x0003240C
		public void ApplyWorldCrusherSettings()
		{
			if (this.useWorldBounds)
			{
				if (SettingsScriptableObject<WorldBoundsSettings>.single == null)
				{
					SettingsScriptableObject<WorldBoundsSettings>.OnSingletonReady = (Action)Delegate.Remove(SettingsScriptableObject<WorldBoundsSettings>.OnSingletonReady, new Action(this.OnWorldBoundsReady));
					SettingsScriptableObject<WorldBoundsSettings>.OnSingletonReady = (Action)Delegate.Combine(SettingsScriptableObject<WorldBoundsSettings>.OnSingletonReady, new Action(this.OnWorldBoundsReady));
					return;
				}
				SettingsScriptableObject<WorldBoundsSettings>.OnSingletonReady = (Action)Delegate.Remove(SettingsScriptableObject<WorldBoundsSettings>.OnSingletonReady, new Action(this.OnWorldBoundsReady));
				List<WorldBoundsGroup> worldBoundsGroups = SettingsScriptableObject<WorldBoundsSettings>.single.worldBoundsGroups;
				if (this.boundsGroupId >= worldBoundsGroups.Count)
				{
					global::Debug.LogError("WorldBoundsGroup " + this.boundsGroupId + " no longer exists. Using Default(0).");
					this.boundsGroupId = 0;
				}
				WorldBoundsGroup worldBoundsGroup = worldBoundsGroups[this.boundsGroupId];
				worldBoundsGroup.OnWorldBoundChanged = (Action)Delegate.Remove(worldBoundsGroup.OnWorldBoundChanged, new Action(this.CacheValues));
				worldBoundsGroup.OnWorldBoundChanged = (Action)Delegate.Combine(worldBoundsGroup.OnWorldBoundChanged, new Action(this.CacheValues));
				ElementCrusher crusher = worldBoundsGroup.crusher;
				if (this._xcrusher != crusher._xcrusher)
				{
					this.XCrusher = crusher.XCrusher;
				}
				if (this._ycrusher != crusher._ycrusher)
				{
					this.YCrusher = crusher.YCrusher;
				}
				if (this._zcrusher != crusher._zcrusher)
				{
					this.ZCrusher = crusher.ZCrusher;
				}
				this.local = crusher.local;
			}
		}

		// Token: 0x06000A59 RID: 2649 RVA: 0x00034384 File Offset: 0x00032584
		public void ApplyWorldCrusherSettings(bool newUseBounds, int newBndsGrpId)
		{
			if (newUseBounds != this.useWorldBounds)
			{
				this.useWorldBounds = newUseBounds;
				if (!this.useWorldBounds)
				{
					this.Defaults(0);
				}
			}
			if (SettingsScriptableObject<WorldBoundsSettings>.single == null)
			{
				global::Debug.LogWarning("Not Ready to Change the World");
				return;
			}
			List<WorldBoundsGroup> worldBoundsGroups = SettingsScriptableObject<WorldBoundsSettings>.single.worldBoundsGroups;
			if (newBndsGrpId != this.boundsGroupId)
			{
				if (this.boundsGroupId < worldBoundsGroups.Count)
				{
					WorldBoundsGroup worldBoundsGroup = worldBoundsGroups[this.boundsGroupId];
					if (worldBoundsGroup != null)
					{
						WorldBoundsGroup worldBoundsGroup2 = worldBoundsGroup;
						worldBoundsGroup2.OnWorldBoundChanged = (Action)Delegate.Remove(worldBoundsGroup2.OnWorldBoundChanged, new Action(this.CacheValues));
					}
				}
				if (newBndsGrpId >= worldBoundsGroups.Count)
				{
					this.boundsGroupId = 0;
				}
				else
				{
					this.boundsGroupId = newBndsGrpId;
				}
				WorldBoundsGroup worldBoundsGroup3 = worldBoundsGroups[this.boundsGroupId];
				if (worldBoundsGroup3 != null && this.useWorldBounds)
				{
					WorldBoundsGroup worldBoundsGroup4 = worldBoundsGroup3;
					worldBoundsGroup4.OnWorldBoundChanged = (Action)Delegate.Combine(worldBoundsGroup4.OnWorldBoundChanged, new Action(this.CacheValues));
					ElementCrusher crusher = worldBoundsGroup3.crusher;
					if (this._xcrusher != crusher._xcrusher)
					{
						this.XCrusher = crusher.XCrusher;
					}
					if (this._ycrusher != crusher._ycrusher)
					{
						this.YCrusher = crusher.YCrusher;
					}
					if (this._zcrusher != crusher._zcrusher)
					{
						this.ZCrusher = crusher.ZCrusher;
					}
					this.local = crusher.local;
				}
			}
		}

		// Token: 0x06000A5A RID: 2650 RVA: 0x000344DC File Offset: 0x000326DC
		private WorldBoundsGroup GetUsedWorldBounds()
		{
			if (this._trsType == null && this.useWorldBounds)
			{
				if (this.boundsGroupId >= SettingsScriptableObject<WorldBoundsSettings>.Single.worldBoundsGroups.Count)
				{
					this.boundsGroupId = 0;
				}
				return SettingsScriptableObject<WorldBoundsSettings>.Single.worldBoundsGroups[this.boundsGroupId];
			}
			return null;
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000A5B RID: 2651 RVA: 0x00034530 File Offset: 0x00032730
		// (set) Token: 0x06000A5C RID: 2652 RVA: 0x000345F4 File Offset: 0x000327F4
		public Bounds Bounds
		{
			get
			{
				this.bounds.SetMinMax(new Vector3((this._xcrusher != null) ? this._xcrusher.Min : 0f, (this._ycrusher != null) ? this._ycrusher.Min : 0f, (this._zcrusher != null) ? this._zcrusher.Min : 0f), new Vector3((this._xcrusher != null) ? this._xcrusher.Max : 0f, (this._ycrusher != null) ? this._ycrusher.Max : 0f, (this._zcrusher != null) ? this._zcrusher.Max : 0f));
				return this.bounds;
			}
			set
			{
				if (this._xcrusher != null)
				{
					this._xcrusher.SetRange(value.min.x, value.max.x);
				}
				if (this._ycrusher != null)
				{
					this._ycrusher.SetRange(value.min.y, value.max.y);
				}
				if (this._zcrusher != null)
				{
					this._zcrusher.SetRange(value.min.z, value.max.z);
				}
				this.CacheValues();
			}
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x000027C8 File Offset: 0x000009C8
		public override void OnBeforeSerialize()
		{
		}

		// Token: 0x06000A5E RID: 2654 RVA: 0x000027C8 File Offset: 0x000009C8
		public override void OnAfterDeserialize()
		{
		}

		// Token: 0x06000A5F RID: 2655 RVA: 0x00034688 File Offset: 0x00032888
		public void CacheValues()
		{
			this.ApplyWorldCrusherSettings();
			this.NullUnusedCrushers();
			if (this._trsType == 2)
			{
				this.cache_qEnabled = (this._qcrusher != null && this._qcrusher.Enabled && this._qcrusher.Bits > 0);
				this.cache_qBits = (this.cache_qEnabled ? this._qcrusher.Bits : 0);
				this.cache_TotalBits[0] = this.cache_qBits;
				this.cache_TotalBits[1] = this.cache_qBits;
				this.cache_TotalBits[2] = this.cache_qBits;
				this.cache_TotalBits[3] = this.cache_qBits;
				this.cache_isUniformScale = false;
			}
			else if (this._trsType == 3 && this.uniformAxes != ElementCrusher.UniformAxes.NonUniform)
			{
				this.cache_uEnabled = (this._ucrusher != null && this._ucrusher.Enabled);
				for (int i = 0; i < 4; i++)
				{
					int num = this.cache_uEnabled ? this._ucrusher.GetBits(i) : 0;
					this.cache_uBits[i] = num;
					this.cache_TotalBits[i] = num;
				}
				this.cache_isUniformScale = true;
			}
			else
			{
				this.cache_mustCorrectRotationX = (this._trsType == 1 && this._xcrusher.UseHalfRangeX);
				for (int j = 0; j < 4; j++)
				{
					this.cache_xEnabled = (this._xcrusher != null && this._xcrusher.Enabled);
					this.cache_yEnabled = (this._ycrusher != null && this._ycrusher.Enabled);
					this.cache_zEnabled = (this._zcrusher != null && this._zcrusher.Enabled);
					this.cache_xBits[j] = (this.cache_xEnabled ? this._xcrusher.GetBits(j) : 0);
					this.cache_yBits[j] = (this.cache_yEnabled ? this._ycrusher.GetBits(j) : 0);
					this.cache_zBits[j] = (this.cache_zEnabled ? this._zcrusher.GetBits(j) : 0);
					this.cache_TotalBits[j] = this.cache_xBits[j] + this.cache_yBits[j] + this.cache_zBits[j];
					this.cache_isUniformScale = false;
				}
			}
			this.Cached_TotalBits = Array.AsReadOnly<int>(this.cache_TotalBits);
			this.cached = true;
			if (this.OnRecalculated != null)
			{
				this.OnRecalculated.Invoke(this);
			}
		}

		// Token: 0x06000A60 RID: 2656 RVA: 0x000348E4 File Offset: 0x00032AE4
		private void NullUnusedCrushers()
		{
			if (this._trsType == 2)
			{
				this.XCrusher = null;
				this.YCrusher = null;
				this.ZCrusher = null;
				this.UCrusher = null;
				return;
			}
			if (this._trsType == 3 && this.uniformAxes != ElementCrusher.UniformAxes.NonUniform)
			{
				this.XCrusher = null;
				this.YCrusher = null;
				this.ZCrusher = null;
				this.QCrusher = null;
				return;
			}
			this.QCrusher = null;
			this.UCrusher = null;
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000A61 RID: 2657 RVA: 0x00034954 File Offset: 0x00032B54
		// (set) Token: 0x06000A62 RID: 2658 RVA: 0x000349C8 File Offset: 0x00032BC8
		public bool Enabled
		{
			get
			{
				if (this.TRSType == 2)
				{
					return this._qcrusher.Enabled && this._qcrusher.Bits > 0;
				}
				if (this.TRSType == 3 && this.uniformAxes != ElementCrusher.UniformAxes.NonUniform)
				{
					return this._ucrusher.Enabled;
				}
				return this._xcrusher.Enabled | this._ycrusher.Enabled | this._zcrusher.Enabled;
			}
			set
			{
				if (this.TRSType == 2)
				{
					this._qcrusher.Enabled = value;
				}
				else if (this.TRSType == 3 && this.uniformAxes != ElementCrusher.UniformAxes.NonUniform)
				{
					this._ucrusher.Enabled = value;
				}
				this._xcrusher.Enabled = value;
				this._ycrusher.Enabled = value;
				this._zcrusher.Enabled = value;
			}
		}

		// Token: 0x17000066 RID: 102
		public FloatCrusher this[int axis]
		{
			get
			{
				switch (axis)
				{
				case 0:
					return this._xcrusher;
				case 1:
					return this._ycrusher;
				case 2:
					return this._zcrusher;
				default:
					global::Debug.Log("AXIS " + axis + " should not be calling happening");
					return null;
				}
			}
		}

		// Token: 0x06000A64 RID: 2660 RVA: 0x00034A84 File Offset: 0x00032C84
		public ElementCrusher()
		{
			this.Defaults(4);
		}

		// Token: 0x06000A65 RID: 2661 RVA: 0x00034AE4 File Offset: 0x00032CE4
		internal ElementCrusher(ElementCrusher.StaticTRSType staticTrsType)
		{
			this._trsType = staticTrsType;
		}

		// Token: 0x06000A66 RID: 2662 RVA: 0x00034B44 File Offset: 0x00032D44
		public ElementCrusher(bool enableTRSTypeSelector = true)
		{
			this._trsType = 4;
			this.Defaults(4);
			this.enableTRSTypeSelector = enableTRSTypeSelector;
		}

		// Token: 0x06000A67 RID: 2663 RVA: 0x00034BB0 File Offset: 0x00032DB0
		public ElementCrusher(TRSType trsType, bool enableTRSTypeSelector = true)
		{
			this._trsType = trsType;
			this.Defaults(trsType);
			this.enableTRSTypeSelector = enableTRSTypeSelector;
		}

		// Token: 0x06000A68 RID: 2664 RVA: 0x00034C1C File Offset: 0x00032E1C
		public void Defaults(TRSType trs)
		{
			if (trs == 2 || trs == 1)
			{
				this.XCrusher = new FloatCrusher(10, -90f, 90f, 0, 1, true);
				this.YCrusher = new FloatCrusher(12, -180f, 180f, 1, 1, true);
				this.ZCrusher = new FloatCrusher(10, -180f, 180f, 2, 1, true);
				this.QCrusher = new QuatCrusher(true, false);
				return;
			}
			if (trs == 3)
			{
				this.XCrusher = new FloatCrusher(12, 0f, 2f, 0, 3, true);
				this.YCrusher = new FloatCrusher(10, 0f, 2f, 1, 3, true);
				this.ZCrusher = new FloatCrusher(10, 0f, 2f, 2, 3, true);
				this.UCrusher = new FloatCrusher(10, 0f, 2f, 3, 3, true);
				return;
			}
			this.XCrusher = new FloatCrusher(12, -20f, 20f, 0, trs, true);
			this.YCrusher = new FloatCrusher(10, -5f, 5f, 1, trs, true);
			this.ZCrusher = new FloatCrusher(10, -5f, 5f, 2, trs, true);
		}

		// Token: 0x06000A69 RID: 2665 RVA: 0x00034D48 File Offset: 0x00032F48
		public void Write(CompressedElement nonalloc, Transform trans, byte[] bytes, ref int bitposition, BitCullingLevel bcl = 0)
		{
			switch (this.TRSType)
			{
			case 0:
				this.Write(nonalloc, this.local ? trans.localPosition : trans.position, bytes, ref bitposition, bcl);
				return;
			case 1:
				this.Write(nonalloc, this.local ? trans.localEulerAngles : trans.eulerAngles, bytes, ref bitposition, bcl);
				return;
			case 2:
				this.Write(nonalloc, this.local ? trans.localRotation : trans.rotation, bytes, ref bitposition, bcl);
				return;
			case 3:
				this.Write(nonalloc, trans.localScale, bytes, ref bitposition, bcl);
				return;
			default:
				this.Write(nonalloc, this.local ? trans.localPosition : trans.position, bytes, ref bitposition, bcl);
				return;
			}
		}

		// Token: 0x06000A6A RID: 2666 RVA: 0x00034E14 File Offset: 0x00033014
		[Obsolete]
		public CompressedElement Write(Transform trans, byte[] bytes, ref int bitposition, BitCullingLevel bcl = 0)
		{
			switch (this.TRSType)
			{
			case 0:
				return this.Write(this.local ? trans.localPosition : trans.position, bytes, ref bitposition, bcl);
			case 1:
				return this.Write(this.local ? trans.localEulerAngles : trans.eulerAngles, bytes, ref bitposition, bcl);
			case 2:
				return this.Write(this.local ? trans.localRotation : trans.rotation, bytes, ref bitposition, bcl);
			case 3:
				return this.Write(trans.localScale, bytes, ref bitposition, bcl);
			default:
				return this.Write(this.local ? trans.localPosition : trans.position, bytes, ref bitposition, bcl);
			}
		}

		// Token: 0x06000A6B RID: 2667 RVA: 0x00034ED4 File Offset: 0x000330D4
		public void Write(CompressedElement ce, byte[] buffer, ref int bitposition, IncludedAxes ia = IncludedAxes.XYZ, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.TRSType == 2)
			{
				this._qcrusher.Write(ce.cQuat, buffer, ref bitposition);
				return;
			}
			if (this.cache_isUniformScale)
			{
				this._ucrusher.Write(ce.cUniform, buffer, ref bitposition, bcl);
				return;
			}
			if (this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None)
			{
				this._xcrusher.Write(ce.cx, buffer, ref bitposition, bcl);
			}
			if (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None)
			{
				this._ycrusher.Write(ce.cy, buffer, ref bitposition, bcl);
			}
			if (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None)
			{
				this._zcrusher.Write(ce.cz, buffer, ref bitposition, bcl);
			}
		}

		// Token: 0x06000A6C RID: 2668 RVA: 0x00034FA0 File Offset: 0x000331A0
		public void Write(CompressedElement ce, uint[] buffer, ref int bitposition, IncludedAxes ia = IncludedAxes.XYZ, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.TRSType == 2)
			{
				this._qcrusher.Write(ce.cQuat, buffer, ref bitposition);
				return;
			}
			if (this.cache_isUniformScale)
			{
				this._ucrusher.Write(ce.cUniform, buffer, ref bitposition, bcl);
				return;
			}
			if (this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None)
			{
				this._xcrusher.Write(ce.cx, buffer, ref bitposition, bcl);
			}
			if (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None)
			{
				this._ycrusher.Write(ce.cy, buffer, ref bitposition, bcl);
			}
			if (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None)
			{
				this._zcrusher.Write(ce.cz, buffer, ref bitposition, bcl);
			}
		}

		// Token: 0x06000A6D RID: 2669 RVA: 0x0003506C File Offset: 0x0003326C
		public void Write(CompressedElement ce, ulong[] buffer, ref int bitposition, IncludedAxes ia = IncludedAxes.XYZ, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.TRSType == 2)
			{
				this._qcrusher.Write(ce.cQuat, buffer, ref bitposition);
				return;
			}
			if (this.cache_isUniformScale)
			{
				this._ucrusher.Write(ce.cUniform, buffer, ref bitposition, bcl);
				return;
			}
			if (this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None)
			{
				this._xcrusher.Write(ce.cx, buffer, ref bitposition, bcl);
			}
			if (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None)
			{
				this._ycrusher.Write(ce.cy, buffer, ref bitposition, bcl);
			}
			if (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None)
			{
				this._zcrusher.Write(ce.cz, buffer, ref bitposition, bcl);
			}
		}

		// Token: 0x06000A6E RID: 2670 RVA: 0x00035138 File Offset: 0x00033338
		public void Write(CompressedElement nonalloc, Vector3 v3, byte[] bytes, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.Write(nonalloc, v3, bytes, ref num, 0);
		}

		// Token: 0x06000A6F RID: 2671 RVA: 0x00035154 File Offset: 0x00033354
		[Obsolete]
		public CompressedElement Write(Vector3 v3, byte[] bytes, BitCullingLevel bcl = 0)
		{
			int num = 0;
			return this.Write(v3, bytes, ref num, 0);
		}

		// Token: 0x06000A70 RID: 2672 RVA: 0x00035170 File Offset: 0x00033370
		public void Write(CompressedElement nonalloc, Vector3 v3, byte[] bytes, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.cache_isUniformScale)
			{
				CompressedFloat compressedFloat = this._ucrusher.Write((this.uniformAxes == ElementCrusher.UniformAxes.YZ) ? v3.y : v3.x, bytes, ref bitposition, bcl);
				nonalloc.Set(this, compressedFloat.cvalue);
				return;
			}
			if (this.TRSType == 2)
			{
				ulong cQuat = this._qcrusher.Write(Quaternion.Euler(v3), bytes, ref bitposition);
				nonalloc.Set(this, cQuat);
				return;
			}
			if (this.cache_mustCorrectRotationX)
			{
				v3 = FloatCrusherUtilities.GetXCorrectedEuler(v3);
			}
			nonalloc.Set(this, this.cache_xEnabled ? this._xcrusher.Write(v3.x, bytes, ref bitposition, bcl) : default(CompressedFloat), this.cache_yEnabled ? this._ycrusher.Write(v3.y, bytes, ref bitposition, bcl) : default(CompressedFloat), this.cache_zEnabled ? this._zcrusher.Write(v3.z, bytes, ref bitposition, bcl) : default(CompressedFloat));
		}

		// Token: 0x06000A71 RID: 2673 RVA: 0x00035284 File Offset: 0x00033484
		[Obsolete]
		public CompressedElement Write(Vector3 v3, byte[] bytes, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.cache_isUniformScale)
			{
				CompressedValue compressedValue = this._ucrusher.Write((this.uniformAxes == ElementCrusher.UniformAxes.YZ) ? v3.y : v3.x, bytes, ref bitposition, bcl);
				return new CompressedElement(this, (uint)compressedValue.cvalue);
			}
			if (this.TRSType == 2)
			{
				ulong cQuat = this._qcrusher.Write(Quaternion.Euler(v3), bytes, ref bitposition);
				return new CompressedElement(this, cQuat);
			}
			if (this.cache_mustCorrectRotationX)
			{
				v3 = FloatCrusherUtilities.GetXCorrectedEuler(v3);
			}
			return new CompressedElement(this, this.cache_xEnabled ? this._xcrusher.Write(v3.x, bytes, ref bitposition, bcl) : 0U, this.cache_yEnabled ? this._ycrusher.Write(v3.y, bytes, ref bitposition, bcl) : 0U, this.cache_zEnabled ? this._zcrusher.Write(v3.z, bytes, ref bitposition, bcl) : 0U);
		}

		// Token: 0x06000A72 RID: 2674 RVA: 0x0003538B File Offset: 0x0003358B
		public void Write(CompressedElement nonalloc, Quaternion quat, byte[] bytes, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			nonalloc.Set(this, this._qcrusher.Write(quat, bytes, ref bitposition));
		}

		// Token: 0x06000A73 RID: 2675 RVA: 0x000353B1 File Offset: 0x000335B1
		[Obsolete]
		public CompressedElement Write(Quaternion quat, byte[] bytes, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			return new CompressedElement(this, this._qcrusher.Write(quat, bytes, ref bitposition));
		}

		// Token: 0x06000A74 RID: 2676 RVA: 0x000353D8 File Offset: 0x000335D8
		public void Write(CompressedElement nonalloc, Transform trans, ulong[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			switch (this.TRSType)
			{
			case 0:
				this.Write(nonalloc, this.local ? trans.localPosition : trans.position, buffer, ref bitposition, bcl);
				return;
			case 1:
				this.Write(nonalloc, this.local ? trans.localEulerAngles : trans.eulerAngles, buffer, ref bitposition, bcl);
				return;
			case 2:
				this.Write(nonalloc, this.local ? trans.localRotation : trans.rotation, buffer, ref bitposition, bcl);
				return;
			case 3:
				this.Write(nonalloc, trans.localScale, buffer, ref bitposition, bcl);
				return;
			default:
				this.Write(nonalloc, this.local ? trans.localPosition : trans.position, buffer, ref bitposition, bcl);
				return;
			}
		}

		// Token: 0x06000A75 RID: 2677 RVA: 0x000354A4 File Offset: 0x000336A4
		public void Write(CompressedElement nonalloc, Vector3 v3, ulong[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.cache_isUniformScale)
			{
				CompressedFloat compressedFloat = this._ucrusher.Write((this.uniformAxes == ElementCrusher.UniformAxes.YZ) ? v3.y : v3.x, buffer, ref bitposition, bcl);
				nonalloc.Set(this, compressedFloat.cvalue);
				return;
			}
			if (this.TRSType == 2)
			{
				ulong cQuat = this._qcrusher.Write(Quaternion.Euler(v3), buffer, ref bitposition);
				nonalloc.Set(this, cQuat);
				return;
			}
			if (this.cache_mustCorrectRotationX)
			{
				v3 = FloatCrusherUtilities.GetXCorrectedEuler(v3);
			}
			nonalloc.Set(this, this.cache_xEnabled ? this._xcrusher.Write(v3.x, buffer, ref bitposition, bcl) : default(CompressedFloat), this.cache_yEnabled ? this._ycrusher.Write(v3.y, buffer, ref bitposition, bcl) : default(CompressedFloat), this.cache_zEnabled ? this._zcrusher.Write(v3.z, buffer, ref bitposition, bcl) : default(CompressedFloat));
		}

		// Token: 0x06000A76 RID: 2678 RVA: 0x000355B6 File Offset: 0x000337B6
		public void Write(CompressedElement nonalloc, Quaternion quat, ulong[] bytes, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			nonalloc.Set(this, this._qcrusher.Write(quat, bytes, ref bitposition));
		}

		// Token: 0x06000A77 RID: 2679 RVA: 0x000355DC File Offset: 0x000337DC
		public void Read(CompressedElement nonalloc, byte[] bytes, IncludedAxes ia = IncludedAxes.XYZ, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.Read(nonalloc, bytes, ref num, ia, bcl);
		}

		// Token: 0x06000A78 RID: 2680 RVA: 0x000355F8 File Offset: 0x000337F8
		public CompressedElement Read(byte[] buffer, IncludedAxes ia = IncludedAxes.XYZ, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.Read(CompressedElement.reusable, buffer, ref num, ia, bcl);
			return CompressedElement.reusable;
		}

		// Token: 0x06000A79 RID: 2681 RVA: 0x0003561C File Offset: 0x0003381C
		public void Read(CompressedElement nonalloc, byte[] bytes, ref int bitposition, IncludedAxes ia = IncludedAxes.XYZ, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.TRSType == 2)
			{
				nonalloc.Set(this, ArraySerializeExt.Read(bytes, ref bitposition, this.cache_qBits));
				return;
			}
			if (this.cache_isUniformScale)
			{
				nonalloc.Set(this, (uint)ArraySerializeExt.Read(bytes, ref bitposition, this.cache_uBits[bcl]));
				return;
			}
			CompressedFloat cx;
			if ((ia & IncludedAxes.X) != IncludedAxes.None)
			{
				int num = this.cache_xBits[bcl];
				cx = (this.cache_xEnabled ? new CompressedFloat(this._xcrusher, (uint)ArraySerializeExt.Read(bytes, ref bitposition, num)) : default(CompressedFloat));
			}
			else
			{
				cx = default(CompressedFloat);
			}
			CompressedFloat cy;
			if ((ia & IncludedAxes.Y) != IncludedAxes.None)
			{
				int num2 = this.cache_yBits[bcl];
				cy = (this.cache_yEnabled ? new CompressedFloat(this._ycrusher, (uint)ArraySerializeExt.Read(bytes, ref bitposition, num2)) : default(CompressedFloat));
			}
			else
			{
				cy = default(CompressedFloat);
			}
			CompressedFloat cz;
			if ((ia & IncludedAxes.Z) != IncludedAxes.None)
			{
				int num3 = this.cache_zBits[bcl];
				cz = (this.cache_zEnabled ? new CompressedFloat(this._zcrusher, (uint)ArraySerializeExt.Read(bytes, ref bitposition, num3)) : default(CompressedFloat));
			}
			else
			{
				cz = default(CompressedFloat);
			}
			nonalloc.Set(this, cx, cy, cz);
		}

		// Token: 0x06000A7A RID: 2682 RVA: 0x0003574D File Offset: 0x0003394D
		public CompressedElement Read(byte[] bytes, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedElement.reusable, bytes, ref bitposition, IncludedAxes.XYZ, 0);
			return CompressedElement.reusable;
		}

		// Token: 0x06000A7B RID: 2683 RVA: 0x00035764 File Offset: 0x00033964
		public void Read(CompressedElement nonalloc, ulong[] buffer, ref int bitposition, IncludedAxes ia = IncludedAxes.XYZ, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.TRSType == 2)
			{
				nonalloc.Set(this, ArraySerializeExt.Read(buffer, ref bitposition, this.cache_qBits));
				return;
			}
			if (this.cache_isUniformScale)
			{
				nonalloc.Set(this, (uint)ArraySerializeExt.Read(buffer, ref bitposition, this.cache_uBits[bcl]));
				return;
			}
			CompressedFloat cx;
			if ((ia & IncludedAxes.X) != IncludedAxes.None)
			{
				int num = this.cache_xBits[bcl];
				cx = (this.cache_xEnabled ? new CompressedFloat(this._xcrusher, (uint)ArraySerializeExt.Read(buffer, ref bitposition, num)) : default(CompressedFloat));
			}
			else
			{
				cx = default(CompressedFloat);
			}
			CompressedFloat cy;
			if ((ia & IncludedAxes.Y) != IncludedAxes.None)
			{
				int num2 = this.cache_yBits[bcl];
				cy = (this.cache_yEnabled ? new CompressedFloat(this._ycrusher, (uint)ArraySerializeExt.Read(buffer, ref bitposition, num2)) : default(CompressedFloat));
			}
			else
			{
				cy = default(CompressedFloat);
			}
			CompressedFloat cz;
			if ((ia & IncludedAxes.Z) != IncludedAxes.None)
			{
				int num3 = this.cache_zBits[bcl];
				cz = (this.cache_zEnabled ? new CompressedFloat(this._zcrusher, (uint)ArraySerializeExt.Read(buffer, ref bitposition, num3)) : default(CompressedFloat));
			}
			else
			{
				cz = default(CompressedFloat);
			}
			nonalloc.Set(this, cx, cy, cz);
		}

		// Token: 0x06000A7C RID: 2684 RVA: 0x00035898 File Offset: 0x00033A98
		public void Read(CompressedElement nonalloc, uint[] buffer, ref int bitposition, IncludedAxes ia = IncludedAxes.XYZ, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.TRSType == 2)
			{
				nonalloc.Set(this, ArraySerializeExt.Read(buffer, ref bitposition, this.cache_qBits));
				return;
			}
			if (this.cache_isUniformScale)
			{
				nonalloc.Set(this, (uint)ArraySerializeExt.Read(buffer, ref bitposition, this.cache_uBits[bcl]));
				return;
			}
			CompressedFloat cx;
			if ((ia & IncludedAxes.X) != IncludedAxes.None)
			{
				int num = this.cache_xBits[bcl];
				cx = (this.cache_xEnabled ? new CompressedFloat(this._xcrusher, (uint)ArraySerializeExt.Read(buffer, ref bitposition, num)) : default(CompressedFloat));
			}
			else
			{
				cx = default(CompressedFloat);
			}
			CompressedFloat cy;
			if ((ia & IncludedAxes.Y) != IncludedAxes.None)
			{
				int num2 = this.cache_yBits[bcl];
				cy = (this.cache_yEnabled ? new CompressedFloat(this._ycrusher, (uint)ArraySerializeExt.Read(buffer, ref bitposition, num2)) : default(CompressedFloat));
			}
			else
			{
				cy = default(CompressedFloat);
			}
			CompressedFloat cz;
			if ((ia & IncludedAxes.Z) != IncludedAxes.None)
			{
				int num3 = this.cache_zBits[bcl];
				cz = (this.cache_zEnabled ? new CompressedFloat(this._zcrusher, (uint)ArraySerializeExt.Read(buffer, ref bitposition, num3)) : default(CompressedFloat));
			}
			else
			{
				cz = default(CompressedFloat);
			}
			nonalloc.Set(this, cx, cy, cz);
		}

		// Token: 0x06000A7D RID: 2685 RVA: 0x000359C9 File Offset: 0x00033BC9
		public Element ReadAndDecompress(byte[] bytes, ref int bitposition, IncludedAxes ia = IncludedAxes.XYZ, BitCullingLevel bcl = 0)
		{
			this.Read(ElementCrusher.reusableCE, bytes, ref bitposition, ia, bcl);
			return this.Decompress(ElementCrusher.reusableCE);
		}

		// Token: 0x06000A7E RID: 2686 RVA: 0x000359E8 File Offset: 0x00033BE8
		public void Write(CompressedElement nonalloc, Transform trans, ref ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			switch (this.TRSType)
			{
			case 0:
				this.Write(nonalloc, this.local ? trans.localPosition : trans.position, ref buffer, ref bitposition, bcl);
				return;
			case 1:
				this.Write(nonalloc, this.local ? trans.localEulerAngles : trans.eulerAngles, ref buffer, ref bitposition, bcl);
				return;
			case 2:
				this.Write(nonalloc, this.local ? trans.localRotation : trans.rotation, ref buffer, ref bitposition);
				return;
			case 3:
				this.Write(nonalloc, trans.localScale, ref buffer, ref bitposition, bcl);
				return;
			default:
				this.Write(nonalloc, this.local ? trans.localPosition : trans.position, ref buffer, ref bitposition, bcl);
				return;
			}
		}

		// Token: 0x06000A7F RID: 2687 RVA: 0x00035AB0 File Offset: 0x00033CB0
		public void Write(Transform trans, ref ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			switch (this.TRSType)
			{
			case 0:
				this.Write(this.local ? trans.localPosition : trans.position, ref buffer, ref bitposition, bcl);
				return;
			case 1:
				this.Write(this.local ? trans.localEulerAngles : trans.eulerAngles, ref buffer, ref bitposition, bcl);
				return;
			case 2:
				this.Write(this.local ? trans.localRotation : trans.rotation, ref buffer, ref bitposition);
				return;
			case 3:
				this.Write(trans.localScale, ref buffer, ref bitposition, bcl);
				return;
			default:
				this.Write(this.local ? trans.localPosition : trans.position, ref buffer, ref bitposition, bcl);
				return;
			}
		}

		// Token: 0x06000A80 RID: 2688 RVA: 0x00035B70 File Offset: 0x00033D70
		public void Write(CompressedElement nonalloc, Vector3 v3, ref ulong buffer, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.Write(nonalloc, v3, ref buffer, ref num, bcl);
		}

		// Token: 0x06000A81 RID: 2689 RVA: 0x00035B8C File Offset: 0x00033D8C
		public void Write(Vector3 v3, ref ulong buffer, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.Write(v3, ref buffer, ref num, bcl);
		}

		// Token: 0x06000A82 RID: 2690 RVA: 0x00035BA8 File Offset: 0x00033DA8
		public void Write(CompressedElement nonalloc, Vector3 v3, ref ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			CompressedFloat cx = this.cache_xEnabled ? this._xcrusher.Write(v3.x, ref buffer, ref bitposition, bcl) : default(CompressedFloat);
			CompressedFloat cy = this.cache_yEnabled ? this._ycrusher.Write(v3.y, ref buffer, ref bitposition, bcl) : default(CompressedFloat);
			CompressedFloat cz = this.cache_zEnabled ? this._zcrusher.Write(v3.z, ref buffer, ref bitposition, bcl) : default(CompressedFloat);
			nonalloc.Set(this, cx, cy, cz);
		}

		// Token: 0x06000A83 RID: 2691 RVA: 0x00035C4C File Offset: 0x00033E4C
		public void Write(Vector3 v3, ref ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.cache_xEnabled)
			{
				this._xcrusher.Write(v3.x, ref buffer, ref bitposition, bcl);
			}
			if (this.cache_yEnabled)
			{
				this._ycrusher.Write(v3.y, ref buffer, ref bitposition, bcl);
			}
			if (this.cache_zEnabled)
			{
				this._zcrusher.Write(v3.z, ref buffer, ref bitposition, bcl);
			}
		}

		// Token: 0x06000A84 RID: 2692 RVA: 0x00035CC4 File Offset: 0x00033EC4
		public void Write(CompressedElement nonalloc, Quaternion quat, ref ulong buffer)
		{
			int num = 0;
			this.Write(nonalloc, quat, ref buffer, ref num);
		}

		// Token: 0x06000A85 RID: 2693 RVA: 0x00035CE0 File Offset: 0x00033EE0
		public void Write(Quaternion quat, ref ulong buffer)
		{
			int num = 0;
			this.Write(quat, ref buffer, ref num);
		}

		// Token: 0x06000A86 RID: 2694 RVA: 0x00035CFC File Offset: 0x00033EFC
		public void Write(CompressedElement nonalloc, Quaternion quat, ref ulong buffer, ref int bitposition)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			ulong cQuat = this.cache_qEnabled ? this._qcrusher.Write(quat, ref buffer, ref bitposition) : 0UL;
			nonalloc.Set(this, cQuat);
		}

		// Token: 0x06000A87 RID: 2695 RVA: 0x00035D3B File Offset: 0x00033F3B
		public void Write(Quaternion quat, ref ulong buffer, ref int bitposition)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.cache_qEnabled)
			{
				this._qcrusher.Write(quat, ref buffer, ref bitposition);
			}
		}

		// Token: 0x06000A88 RID: 2696 RVA: 0x00035D64 File Offset: 0x00033F64
		public CompressedElement Write(CompressedElement ce, ref ulong buffer, BitCullingLevel bcl = 0)
		{
			int num = 0;
			return this.Write(ce, ref buffer, ref num, bcl);
		}

		// Token: 0x06000A89 RID: 2697 RVA: 0x00035D80 File Offset: 0x00033F80
		public CompressedElement Write(CompressedElement ce, ref ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.cache_qEnabled)
			{
				PrimitiveSerializeExt.Inject(ce.cQuat.cvalue, ref buffer, ref bitposition, this.cache_qBits);
			}
			else if (this.cache_uEnabled)
			{
				PrimitiveSerializeExt.Inject(ce.cUniform.cvalue, ref buffer, ref bitposition, this.cache_uBits[bcl]);
			}
			else
			{
				if (this.cache_xEnabled)
				{
					PrimitiveSerializeExt.Inject(ce.cx.cvalue, ref buffer, ref bitposition, this.cache_xBits[bcl]);
				}
				if (this.cache_yEnabled)
				{
					PrimitiveSerializeExt.Inject(ce.cy.cvalue, ref buffer, ref bitposition, this.cache_yBits[bcl]);
				}
				if (this.cache_zEnabled)
				{
					PrimitiveSerializeExt.Inject(ce.cz.cvalue, ref buffer, ref bitposition, this.cache_zBits[bcl]);
				}
			}
			return ce;
		}

		// Token: 0x06000A8A RID: 2698 RVA: 0x00035E50 File Offset: 0x00034050
		public Element Read(ulong buffer, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			int num = 0;
			if (this.TRSType == 2)
			{
				ulong compressed = PrimitiveSerializeExt.Read(buffer, ref num, this.cache_qBits);
				return this._qcrusher.Decompress(compressed);
			}
			if (this.cache_isUniformScale)
			{
				float num2 = this._ucrusher.ReadAndDecompress(buffer, ref num, bcl);
				return new Vector3(num2, num2, num2);
			}
			return new Vector3(this.cache_xEnabled ? this._xcrusher.ReadAndDecompress(buffer, ref num, bcl) : 0f, this.cache_yEnabled ? this._ycrusher.ReadAndDecompress(buffer, ref num, bcl) : 0f, this.cache_zEnabled ? this._zcrusher.ReadAndDecompress(buffer, ref num, bcl) : 0f);
		}

		// Token: 0x06000A8B RID: 2699 RVA: 0x00035F20 File Offset: 0x00034120
		public Element Read(ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.TRSType == 2)
			{
				ulong compressed = PrimitiveSerializeExt.Read(buffer, ref bitposition, this.cache_qBits);
				return this._qcrusher.Decompress(compressed);
			}
			if (this.cache_isUniformScale)
			{
				float num = this._ucrusher.ReadAndDecompress(buffer, ref bitposition, bcl);
				return new Vector3(num, num, num);
			}
			return new Vector3(this.cache_xEnabled ? this._xcrusher.ReadAndDecompress(buffer, ref bitposition, bcl) : 0f, this.cache_yEnabled ? this._ycrusher.ReadAndDecompress(buffer, ref bitposition, bcl) : 0f, this.cache_zEnabled ? this._zcrusher.ReadAndDecompress(buffer, ref bitposition, bcl) : 0f);
		}

		// Token: 0x06000A8C RID: 2700 RVA: 0x00035FE8 File Offset: 0x000341E8
		public void Read(CompressedElement nonalloc, ulong buffer, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.Read(nonalloc, buffer, ref num, bcl);
		}

		// Token: 0x06000A8D RID: 2701 RVA: 0x00036004 File Offset: 0x00034204
		public void Read(CompressedElement nonalloc, ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.TRSType == 2)
			{
				nonalloc.Set(this, PrimitiveSerializeExt.Read(buffer, ref bitposition, this.cache_qBits));
				return;
			}
			if (this.cache_isUniformScale)
			{
				nonalloc.Set(this, (uint)PrimitiveSerializeExt.Read(buffer, ref bitposition, this.cache_uBits[bcl]));
				return;
			}
			CompressedFloat cx = this.cache_xEnabled ? new CompressedFloat(this._xcrusher, PrimitiveSerializeExt.Read(buffer, ref bitposition, this.cache_xBits[bcl])) : default(CompressedFloat);
			CompressedFloat cy = this.cache_yEnabled ? new CompressedFloat(this._ycrusher, PrimitiveSerializeExt.Read(buffer, ref bitposition, this.cache_yBits[bcl])) : default(CompressedFloat);
			CompressedFloat cz = this.cache_zEnabled ? new CompressedFloat(this._zcrusher, PrimitiveSerializeExt.Read(buffer, ref bitposition, this.cache_zBits[bcl])) : default(CompressedFloat);
			nonalloc.Set(this, cx, cy, cz);
		}

		// Token: 0x06000A8E RID: 2702 RVA: 0x000360F8 File Offset: 0x000342F8
		public void Read(CompressedElement nonalloc, ulong frag0, ulong frag1 = 0UL, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			int num = 0;
			ArraySerializeExt.Write(ElementCrusher.reusableArray64, frag0, ref num, 64);
			ArraySerializeExt.Write(ElementCrusher.reusableArray64, frag1, ref num, 64);
			num = 0;
			this.Read(nonalloc, ElementCrusher.reusableArray64, ref num, IncludedAxes.XYZ, bcl);
		}

		// Token: 0x06000A8F RID: 2703 RVA: 0x00036146 File Offset: 0x00034346
		public CompressedElement Read(ulong frag0, ulong frag1 = 0UL, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedElement.reusable, frag0, frag1, bcl);
			return CompressedElement.reusable;
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x0003615C File Offset: 0x0003435C
		public void Compress(CompressedElement nonalloc, Transform trans)
		{
			switch (this.TRSType)
			{
			case 0:
				this.Compress(nonalloc, this.local ? trans.localPosition : trans.position, IncludedAxes.XYZ);
				return;
			case 1:
				this.Compress(nonalloc, this.local ? trans.localEulerAngles : trans.eulerAngles, IncludedAxes.XYZ);
				return;
			case 2:
				this.Compress(nonalloc, this.local ? trans.localRotation : trans.rotation);
				return;
			case 3:
				this.Compress(nonalloc, this.local ? trans.localScale : trans.lossyScale, IncludedAxes.XYZ);
				return;
			default:
				this.Compress(nonalloc, this.local ? trans.localPosition : trans.position, IncludedAxes.XYZ);
				return;
			}
		}

		// Token: 0x06000A91 RID: 2705 RVA: 0x00036221 File Offset: 0x00034421
		public CompressedElement Compress(Transform trans)
		{
			this.Compress(CompressedElement.reusable, trans);
			return CompressedElement.reusable;
		}

		// Token: 0x06000A92 RID: 2706 RVA: 0x00036234 File Offset: 0x00034434
		public void CompressAndWrite(Transform trans, byte[] buffer, ref int bitposition)
		{
			switch (this.TRSType)
			{
			case 0:
				this.CompressAndWrite(this.local ? trans.localPosition : trans.position, buffer, ref bitposition, IncludedAxes.XYZ);
				return;
			case 1:
				this.CompressAndWrite(this.local ? trans.localEulerAngles : trans.eulerAngles, buffer, ref bitposition, IncludedAxes.XYZ);
				return;
			case 2:
				this.CompressAndWrite(this.local ? trans.localRotation : trans.rotation, buffer, ref bitposition);
				return;
			case 3:
				this.CompressAndWrite(this.local ? trans.localScale : trans.lossyScale, buffer, ref bitposition, IncludedAxes.XYZ);
				return;
			default:
				this.CompressAndWrite(this.local ? trans.localPosition : trans.position, buffer, ref bitposition, IncludedAxes.XYZ);
				return;
			}
		}

		// Token: 0x06000A93 RID: 2707 RVA: 0x000362FE File Offset: 0x000344FE
		public void Compress(CompressedElement nonalloc, Element e)
		{
			if (this.TRSType == 2)
			{
				this.Compress(nonalloc, e.quat);
				return;
			}
			this.Compress(nonalloc, e.v, IncludedAxes.XYZ);
		}

		// Token: 0x06000A94 RID: 2708 RVA: 0x00036325 File Offset: 0x00034525
		public CompressedElement Compress(Element e)
		{
			if (this.TRSType == 2)
			{
				this.Compress(CompressedElement.reusable, e.quat);
			}
			else
			{
				this.Compress(CompressedElement.reusable, e.v, IncludedAxes.XYZ);
			}
			return CompressedElement.reusable;
		}

		// Token: 0x06000A95 RID: 2709 RVA: 0x0003635A File Offset: 0x0003455A
		public void CompressAndWrite(Element e, byte[] buffer, ref int bitposition)
		{
			if (this.TRSType == 2)
			{
				this.CompressAndWrite(e.quat, buffer, ref bitposition);
				return;
			}
			this.CompressAndWrite(e.v, buffer, ref bitposition, IncludedAxes.XYZ);
		}

		// Token: 0x06000A96 RID: 2710 RVA: 0x00036384 File Offset: 0x00034584
		public void Compress(CompressedElement nonalloc, Rigidbody rb, IncludedAxes ia = IncludedAxes.XYZ)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			switch (this._trsType)
			{
			case 0:
			{
				Vector3 vector;
				if (this.local)
				{
					Transform parent = rb.transform.parent;
					vector = (parent ? parent.InverseTransformPoint(rb.position) : rb.position);
				}
				else
				{
					vector = rb.position;
				}
				CompressedFloat cx = (this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? this._xcrusher.Compress(vector.x, 0) : default(CompressedFloat);
				CompressedFloat cy = (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? this._ycrusher.Compress(vector.y, 0) : default(CompressedFloat);
				CompressedFloat cz = (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? this._zcrusher.Compress(vector.z, 0) : default(CompressedFloat);
				nonalloc.Set(this, cx, cy, cz);
				return;
			}
			case 1:
			{
				Vector3 vector2;
				if (this.local)
				{
					Transform parent2 = rb.transform.parent;
					vector2 = (parent2 ? (Quaternion.Inverse(parent2.rotation) * rb.rotation).eulerAngles : rb.rotation.eulerAngles);
				}
				else
				{
					vector2 = rb.rotation.eulerAngles;
				}
				CompressedFloat cx2 = (this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? this._xcrusher.Compress(vector2.x, 0) : default(CompressedFloat);
				CompressedFloat cy2 = (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? this._ycrusher.Compress(vector2.y, 0) : default(CompressedFloat);
				CompressedFloat cz2 = (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? this._zcrusher.Compress(vector2.z, 0) : default(CompressedFloat);
				nonalloc.Set(this, cx2, cy2, cz2);
				return;
			}
			case 2:
				if (this.cache_qEnabled)
				{
					Quaternion quat;
					if (this.local)
					{
						Transform parent3 = rb.transform.parent;
						quat = (parent3 ? (Quaternion.Inverse(parent3.rotation) * rb.rotation) : rb.rotation);
					}
					else
					{
						quat = rb.rotation;
					}
					nonalloc.Set(this, this._qcrusher.Compress(quat));
					return;
				}
				break;
			case 3:
				this.Compress(nonalloc, this.local ? rb.transform.localScale : rb.transform.lossyScale, ia);
				return;
			default:
				nonalloc.Clear();
				break;
			}
		}

		// Token: 0x06000A97 RID: 2711 RVA: 0x00036624 File Offset: 0x00034824
		public void Compress(CompressedElement nonalloc, Vector3 v, IncludedAxes ia = IncludedAxes.XYZ)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this._trsType == 2)
			{
				global::Debug.LogError("We shouldn't be seeing this. Quats should not be getting compressed from Eulers!");
				if (this.cache_qEnabled)
				{
					nonalloc.Set(this, this._qcrusher.Compress(Quaternion.Euler(v)));
					return;
				}
			}
			else if (this._trsType == 3 && this.uniformAxes != ElementCrusher.UniformAxes.NonUniform)
			{
				if (this.cache_uEnabled)
				{
					nonalloc.Set(this, this._ucrusher.Compress((this.uniformAxes == ElementCrusher.UniformAxes.YZ) ? v.y : v.x, 0));
					return;
				}
			}
			else
			{
				CompressedFloat cx = (this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? this._xcrusher.Compress(v.x, 0) : default(CompressedFloat);
				CompressedFloat cy = (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? this._ycrusher.Compress(v.y, 0) : default(CompressedFloat);
				CompressedFloat cz = (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? this._zcrusher.Compress(v.z, 0) : default(CompressedFloat);
				nonalloc.Set(this, cx, cy, cz);
			}
		}

		// Token: 0x06000A98 RID: 2712 RVA: 0x0003674D File Offset: 0x0003494D
		public CompressedElement Compress(Vector3 v)
		{
			this.Compress(CompressedElement.reusable, v, IncludedAxes.XYZ);
			return CompressedElement.reusable;
		}

		// Token: 0x06000A99 RID: 2713 RVA: 0x00036764 File Offset: 0x00034964
		public void CompressAndWrite(Vector3 v, byte[] buffer, ref int bitposition, IncludedAxes ia = IncludedAxes.XYZ)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this._trsType == 3 && this.uniformAxes != ElementCrusher.UniformAxes.NonUniform)
			{
				ulong num = this.cache_uEnabled ? this._ucrusher.Compress((this.uniformAxes == ElementCrusher.UniformAxes.YZ) ? v.y : v.x, 0) : 0UL;
				ArraySerializeExt.Write(buffer, num, ref bitposition, this.cache_uBits[0]);
				return;
			}
			if (this._trsType == 2)
			{
				global::Debug.Log("We shouldn't be seeing this. Quats should not be getting compressed from Eulers!");
				if (this.cache_qEnabled)
				{
					ArraySerializeExt.Write(buffer, this._qcrusher.Compress(Quaternion.Euler(v)), ref bitposition, this.cache_qBits);
					return;
				}
			}
			else
			{
				if (this.cache_xEnabled)
				{
					ArraySerializeExt.Write(buffer, (ulong)this._xcrusher.Compress(v.x, 0).cvalue, ref bitposition, this.cache_xBits[0]);
				}
				if (this.cache_yEnabled)
				{
					ArraySerializeExt.Write(buffer, (ulong)this._ycrusher.Compress(v.y, 0).cvalue, ref bitposition, this.cache_yBits[0]);
				}
				if (this.cache_zEnabled)
				{
					ArraySerializeExt.Write(buffer, (ulong)this._zcrusher.Compress(v.z, 0).cvalue, ref bitposition, this.cache_zBits[0]);
				}
			}
		}

		// Token: 0x06000A9A RID: 2714 RVA: 0x000368A0 File Offset: 0x00034AA0
		public void Compress(CompressedElement nonalloc, Quaternion quat)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.cache_qEnabled)
			{
				nonalloc.Set(this, this._qcrusher.Compress(quat));
			}
		}

		// Token: 0x06000A9B RID: 2715 RVA: 0x000368CB File Offset: 0x00034ACB
		public CompressedElement Compress(Quaternion quat)
		{
			this.Compress(CompressedElement.reusable, quat);
			return CompressedElement.reusable;
		}

		// Token: 0x06000A9C RID: 2716 RVA: 0x000368DE File Offset: 0x00034ADE
		public void CompressAndWrite(Quaternion quat, byte[] buffer, ref int bitposition)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.cache_qEnabled)
			{
				ArraySerializeExt.Write(buffer, this._qcrusher.Compress(quat), ref bitposition, this.cache_qBits);
			}
		}

		// Token: 0x06000A9D RID: 2717 RVA: 0x00036910 File Offset: 0x00034B10
		public Element Decompress(CompressedElement compressed)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this._trsType == 3 && this.uniformAxes != ElementCrusher.UniformAxes.NonUniform)
			{
				float num = this._ucrusher.Decompress(compressed.cUniform);
				return new Vector3(num, num, num);
			}
			if (this._trsType == 2)
			{
				return this._qcrusher.Decompress(compressed.cQuat);
			}
			return new Vector3(this.cache_xEnabled ? this._xcrusher.Decompress(compressed.cx) : 0f, this.cache_yEnabled ? this._ycrusher.Decompress(compressed.cy) : 0f, this.cache_zEnabled ? this._zcrusher.Decompress(compressed.cz) : 0f);
		}

		// Token: 0x06000A9E RID: 2718 RVA: 0x00036A00 File Offset: 0x00034C00
		public Element Decompress(ulong cval, IncludedAxes ia = IncludedAxes.XYZ)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this._trsType == 3 && this.uniformAxes != ElementCrusher.UniformAxes.NonUniform)
			{
				float num = this._ucrusher.Decompress((uint)cval);
				return new Vector3(num, num, num);
			}
			if (this._trsType == 2)
			{
				return this._qcrusher.Decompress(cval);
			}
			int num2 = 0;
			return new Vector3((this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? this._xcrusher.ReadAndDecompress(cval, ref num2, 0) : 0f, (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? this._ycrusher.ReadAndDecompress(cval, ref num2, 0) : 0f, (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? this._zcrusher.ReadAndDecompress(cval, ref num2, 0) : 0f);
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x00036AD6 File Offset: 0x00034CD6
		[Obsolete("Apply for Rigidbody has been replaced with Move and Set, to indicate usage of MovePosition/Rotation vs rb.position/rotation.")]
		public void Apply(Rigidbody rb, CompressedElement ce, IncludedAxes ia = IncludedAxes.XYZ)
		{
			this.Apply(rb, this.Decompress(ce), ia);
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x00036AE8 File Offset: 0x00034CE8
		[Obsolete("Apply for Rigidbody has been replaced with Move and Set, to indicate usage of MovePosition/Rotation vs rb.position/rotation.")]
		public void Apply(Rigidbody rb, Element e, IncludedAxes ia = IncludedAxes.XYZ)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			switch (this._trsType)
			{
			case 0:
			{
				Vector3 vector = (this.local & rb.transform.parent) ? rb.transform.TransformPoint(e.v) : e.v;
				Vector3 position = rb.position;
				rb.MovePosition(new Vector3((this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? vector.x : position.x, (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? vector.y : position.y, (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? vector.z : position.z));
				return;
			}
			case 1:
			{
				Vector3 eulerAngles = rb.rotation.eulerAngles;
				if (this.local && rb.transform.parent)
				{
					rb.transform.eulerAngles = new Vector3((this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? e.v.x : eulerAngles.x, (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? e.v.y : eulerAngles.y, (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? e.v.z : eulerAngles.z);
					return;
				}
				rb.MoveRotation(Quaternion.Euler((this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? e.v.x : eulerAngles.x, (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? e.v.y : eulerAngles.y, (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? e.v.z : eulerAngles.z));
				return;
			}
			case 2:
				if (this.cache_qEnabled)
				{
					if (this.local && rb.transform.parent)
					{
						rb.transform.localRotation = e.quat;
						return;
					}
					rb.MoveRotation(e.quat);
				}
				return;
			default:
				global::Debug.LogError("Are you trying to Apply scale to a Rigidbody?");
				return;
			}
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x00036D0D File Offset: 0x00034F0D
		public void Move(Rigidbody rb, CompressedElement ce, IncludedAxes ia = IncludedAxes.XYZ)
		{
			this.Move(rb, this.Decompress(ce), ia);
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x00036D20 File Offset: 0x00034F20
		public void Move(Rigidbody rb, Element e, IncludedAxes ia = IncludedAxes.XYZ)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			switch (this._trsType)
			{
			case 0:
			{
				Vector3 vector = (this.local & rb.transform.parent) ? rb.transform.TransformPoint(e.v) : e.v;
				rb.MovePosition(new Vector3((this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? vector.x : rb.position.x, (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? vector.y : rb.position.y, (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? vector.z : rb.position.z));
				return;
			}
			case 1:
			{
				Vector3 eulerAngles = rb.rotation.eulerAngles;
				if (this.local && rb.transform.parent)
				{
					rb.transform.eulerAngles = new Vector3((this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? e.v.x : eulerAngles.x, (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? e.v.y : eulerAngles.y, (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? e.v.z : eulerAngles.z);
					return;
				}
				rb.MoveRotation(Quaternion.Euler((this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? e.v.x : eulerAngles.x, (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? e.v.y : eulerAngles.y, (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? e.v.z : eulerAngles.z));
				return;
			}
			case 2:
				if (this.cache_qEnabled)
				{
					if (this.local && rb.transform.parent)
					{
						rb.MoveRotation(rb.transform.parent.rotation * e.quat);
						return;
					}
					rb.MoveRotation(e.quat);
				}
				return;
			default:
				global::Debug.LogError("Are you trying to Apply scale to a Rigidbody?");
				return;
			}
		}

		// Token: 0x06000AA3 RID: 2723 RVA: 0x00036F5C File Offset: 0x0003515C
		public void Set(Rigidbody rb, CompressedElement ce, IncludedAxes ia = IncludedAxes.XYZ)
		{
			this.Set(rb, this.Decompress(ce), ia);
		}

		// Token: 0x06000AA4 RID: 2724 RVA: 0x00036F6D File Offset: 0x0003516D
		public void Set(Rigidbody2D rb, CompressedElement ce, IncludedAxes ia = IncludedAxes.XYZ)
		{
			this.Set(rb, this.Decompress(ce), ia);
		}

		// Token: 0x06000AA5 RID: 2725 RVA: 0x00036F80 File Offset: 0x00035180
		public void Set(Rigidbody rb, Element e, IncludedAxes ia = IncludedAxes.XYZ)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			switch (this._trsType)
			{
			case 0:
			{
				Vector3 vector = (this.local & rb.transform.parent) ? rb.transform.TransformPoint(e.v) : e.v;
				Vector3 position = rb.position;
				rb.position = new Vector3((this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? vector.x : position.x, (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? vector.y : position.y, (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? vector.z : position.z);
				return;
			}
			case 1:
			{
				Vector3 eulerAngles = rb.rotation.eulerAngles;
				if (this.local && rb.transform.parent)
				{
					rb.transform.eulerAngles = new Vector3((this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? e.v.x : eulerAngles.x, (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? e.v.y : eulerAngles.y, (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? e.v.z : eulerAngles.z);
					return;
				}
				rb.rotation = Quaternion.Euler((this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? e.v.x : eulerAngles.x, (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? e.v.y : eulerAngles.y, (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? e.v.z : eulerAngles.z);
				return;
			}
			case 2:
				if (this.cache_qEnabled)
				{
					if (this.local && rb.transform.parent)
					{
						rb.rotation = rb.transform.parent.rotation * e.quat;
						return;
					}
					rb.rotation = e.quat;
				}
				return;
			default:
				global::Debug.LogError("Are you trying to Apply scale to a Rigidbody?");
				return;
			}
		}

		// Token: 0x06000AA6 RID: 2726 RVA: 0x000371B8 File Offset: 0x000353B8
		public void Set(Rigidbody2D rb2d, Element e, IncludedAxes ia = IncludedAxes.XYZ)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			switch (this._trsType)
			{
			case 0:
			{
				Vector3 vector = (this.local & rb2d.transform.parent) ? rb2d.transform.TransformPoint(e.v) : e.v;
				rb2d.position = new Vector2((this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? vector.x : rb2d.position.x, (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? vector.y : rb2d.position.y);
				return;
			}
			case 1:
				if (this.local && rb2d.transform.parent)
				{
					rb2d.transform.localEulerAngles = new Vector3(0f, 0f, (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? e.v.z : rb2d.rotation);
					return;
				}
				rb2d.rotation = ((this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? e.v.z : rb2d.rotation);
				return;
			case 2:
				if (this.cache_qEnabled)
				{
					if (this.local && rb2d.transform.parent)
					{
						rb2d.transform.localRotation = e.quat;
						return;
					}
					rb2d.rotation = e.quat.z;
				}
				return;
			default:
				global::Debug.LogError("Are you trying to Apply scale to a Rigidbody?");
				return;
			}
		}

		// Token: 0x06000AA7 RID: 2727 RVA: 0x0003733C File Offset: 0x0003553C
		public void Apply(Transform trans, CompressedElement ce, IncludedAxes ia = IncludedAxes.XYZ)
		{
			this.Apply(trans, this.Decompress(ce), ia);
		}

		// Token: 0x06000AA8 RID: 2728 RVA: 0x00037350 File Offset: 0x00035550
		public void Apply(Transform trans, Element e, IncludedAxes ia = IncludedAxes.XYZ)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			switch (this._trsType)
			{
			case 0:
				if (this.local)
				{
					trans.localPosition = new Vector3((this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? e.v.x : trans.localPosition.x, (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? e.v.y : trans.localPosition.y, (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? e.v.z : trans.localPosition.z);
					return;
				}
				trans.position = new Vector3((this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? e.v.x : trans.position.x, (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? e.v.y : trans.position.y, (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? e.v.z : trans.position.z);
				return;
			case 1:
				if (this.local)
				{
					trans.localEulerAngles = new Vector3((this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? e.v.x : trans.localEulerAngles.x, (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? e.v.y : trans.localEulerAngles.y, (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? e.v.z : trans.localEulerAngles.z);
					return;
				}
				trans.eulerAngles = new Vector3((this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? e.v.x : trans.eulerAngles.x, (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? e.v.y : trans.eulerAngles.y, (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? e.v.z : trans.eulerAngles.z);
				return;
			case 2:
				if (this.cache_qEnabled)
				{
					if (this.local)
					{
						trans.localRotation = e.quat;
						return;
					}
					trans.rotation = e.quat;
				}
				return;
			default:
				if (this.local)
				{
					if (this.uniformAxes == ElementCrusher.UniformAxes.NonUniform)
					{
						trans.localScale = new Vector3((this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? e.v.x : trans.localScale.x, (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? e.v.y : trans.localScale.y, (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? e.v.z : trans.localScale.z);
						return;
					}
					float num = ((this.uniformAxes & (ElementCrusher.UniformAxes)1) != ElementCrusher.UniformAxes.NonUniform) ? e.v.x : e.v.y;
					trans.localScale = new Vector3(((this.uniformAxes & (ElementCrusher.UniformAxes)1) != ElementCrusher.UniformAxes.NonUniform) ? num : trans.localScale.x, ((this.uniformAxes & (ElementCrusher.UniformAxes)2) != ElementCrusher.UniformAxes.NonUniform) ? num : trans.localScale.y, ((this.uniformAxes & (ElementCrusher.UniformAxes)4) != ElementCrusher.UniformAxes.NonUniform) ? num : trans.localScale.z);
					return;
				}
				else
				{
					if (this.uniformAxes == ElementCrusher.UniformAxes.NonUniform)
					{
						trans.localScale = new Vector3((this.cache_xEnabled && (ia & IncludedAxes.X) != IncludedAxes.None) ? e.v.x : trans.lossyScale.x, (this.cache_yEnabled && (ia & IncludedAxes.Y) != IncludedAxes.None) ? e.v.y : trans.lossyScale.y, (this.cache_zEnabled && (ia & IncludedAxes.Z) != IncludedAxes.None) ? e.v.z : trans.lossyScale.z);
						return;
					}
					float num2 = ((this.uniformAxes & (ElementCrusher.UniformAxes)1) != ElementCrusher.UniformAxes.NonUniform) ? e.v.x : e.v.y;
					trans.localScale = new Vector3(((this.uniformAxes & (ElementCrusher.UniformAxes)1) != ElementCrusher.UniformAxes.NonUniform) ? num2 : trans.lossyScale.x, ((this.uniformAxes & (ElementCrusher.UniformAxes)2) != ElementCrusher.UniformAxes.NonUniform) ? num2 : trans.lossyScale.y, ((this.uniformAxes & (ElementCrusher.UniformAxes)4) != ElementCrusher.UniformAxes.NonUniform) ? num2 : trans.lossyScale.z);
					return;
				}
				break;
			}
		}

		// Token: 0x06000AA9 RID: 2729 RVA: 0x000377B0 File Offset: 0x000359B0
		public Vector3 Clamp(Vector3 v3)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.TRSType == 2)
			{
				return v3;
			}
			if (this.TRSType == 3)
			{
				if (this.uniformAxes == ElementCrusher.UniformAxes.NonUniform)
				{
					return new Vector3(this.cache_xEnabled ? this._xcrusher.Clamp(v3.x) : 0f, this.cache_yEnabled ? this._ycrusher.Clamp(v3.y) : 0f, this.cache_zEnabled ? this._zcrusher.Clamp(v3.z) : 0f);
				}
				return new Vector3(((this.uniformAxes & (ElementCrusher.UniformAxes)1) != ElementCrusher.UniformAxes.NonUniform) ? this._ucrusher.Clamp(v3.x) : 0f, ((this.uniformAxes & (ElementCrusher.UniformAxes)2) != ElementCrusher.UniformAxes.NonUniform) ? this._ucrusher.Clamp(v3.x) : 0f, ((this.uniformAxes & (ElementCrusher.UniformAxes)4) != ElementCrusher.UniformAxes.NonUniform) ? this._ucrusher.Clamp(v3.x) : 0f);
			}
			else
			{
				if (this.TRSType == 1)
				{
					return new Vector3(this.cache_xEnabled ? this._xcrusher.ClampRotation(v3.x) : 0f, this.cache_yEnabled ? this._ycrusher.ClampRotation(v3.y) : 0f, this.cache_zEnabled ? this._zcrusher.ClampRotation(v3.z) : 0f);
				}
				return new Vector3(this.cache_xEnabled ? this._xcrusher.Clamp(v3.x) : 0f, this.cache_yEnabled ? this._ycrusher.Clamp(v3.y) : 0f, this.cache_zEnabled ? this._zcrusher.Clamp(v3.z) : 0f);
			}
		}

		// Token: 0x06000AAA RID: 2730 RVA: 0x00037990 File Offset: 0x00035B90
		public BitCullingLevel FindBestBitCullLevel(CompressedElement a, CompressedElement b, BitCullingLevel maxCulling)
		{
			if (this.TRSType == 2)
			{
				if (a.cQuat == b.cQuat)
				{
					return 3;
				}
				return 0;
			}
			else
			{
				if (maxCulling == null || !this.TestMatchingUpper(a, b, 1))
				{
					return 0;
				}
				if (maxCulling == 1 || !this.TestMatchingUpper(a, b, 2))
				{
					return 1;
				}
				if (maxCulling == 2 || !this.TestMatchingUpper(a, b, 3))
				{
					return 2;
				}
				return 3;
			}
		}

		// Token: 0x06000AAB RID: 2731 RVA: 0x000379F5 File Offset: 0x00035BF5
		private bool TestMatchingUpper(uint a, uint b, int lowerbits)
		{
			return a >> lowerbits << lowerbits == b >> lowerbits << lowerbits;
		}

		// Token: 0x06000AAC RID: 2732 RVA: 0x00037A10 File Offset: 0x00035C10
		public bool TestMatchingUpper(CompressedElement a, CompressedElement b, BitCullingLevel bcl)
		{
			return this.TestMatchingUpper(a.cx, b.cx, this._xcrusher.GetBits(bcl)) && this.TestMatchingUpper(a.cy, b.cy, this._ycrusher.GetBits(bcl)) && this.TestMatchingUpper(a.cz, b.cz, this._zcrusher.GetBits(bcl));
		}

		// Token: 0x06000AAD RID: 2733 RVA: 0x00037A9C File Offset: 0x00035C9C
		public int TallyBits(BitCullingLevel bcl = 0)
		{
			if (this._trsType == 3 && this.uniformAxes != ElementCrusher.UniformAxes.NonUniform)
			{
				if (this._ucrusher == null || !this._ucrusher.Enabled)
				{
					return 0;
				}
				return this._ucrusher.GetBits(bcl);
			}
			else if (this._trsType == 2)
			{
				if (!(this._qcrusher != null) || !this._qcrusher.Enabled)
				{
					return 0;
				}
				return this._qcrusher.Bits;
			}
			else
			{
				if (this._trsType == null && this.useWorldBounds)
				{
					return WorldBoundsSettings.TallyBits(ref this.boundsGroupId, 0);
				}
				return ((this._xcrusher != null && this._xcrusher.Enabled) ? this._xcrusher.GetBits(bcl) : 0) + ((this._ycrusher != null && this._ycrusher.Enabled) ? this._ycrusher.GetBits(bcl) : 0) + ((this._zcrusher != null && this._zcrusher.Enabled) ? this._zcrusher.GetBits(bcl) : 0);
			}
		}

		// Token: 0x06000AAE RID: 2734 RVA: 0x00037B9C File Offset: 0x00035D9C
		public void CopyFrom(ElementCrusher src)
		{
			this._trsType = src._trsType;
			this.uniformAxes = src.uniformAxes;
			if (this._xcrusher != null && src._xcrusher != null)
			{
				this._xcrusher.CopyFrom(src._xcrusher);
			}
			if (this._ycrusher != null && src._ycrusher != null)
			{
				this._ycrusher.CopyFrom(src._ycrusher);
			}
			if (this._zcrusher != null && src._zcrusher != null)
			{
				this._zcrusher.CopyFrom(src._zcrusher);
			}
			if (this._ucrusher != null && src._ucrusher != null)
			{
				this._ucrusher.CopyFrom(src._ucrusher);
			}
			if (this._qcrusher != null && src._qcrusher != null)
			{
				this._qcrusher.CopyFrom(src._qcrusher);
			}
			this.local = src.local;
		}

		// Token: 0x06000AAF RID: 2735 RVA: 0x00037C7E File Offset: 0x00035E7E
		public override string ToString()
		{
			return "ElementCrusher [" + this._trsType + "] ";
		}

		// Token: 0x06000AB0 RID: 2736 RVA: 0x00037C9A File Offset: 0x00035E9A
		public override bool Equals(object obj)
		{
			return this.Equals(obj as ElementCrusher);
		}

		// Token: 0x06000AB1 RID: 2737 RVA: 0x00037CA8 File Offset: 0x00035EA8
		public bool Equals(ElementCrusher other)
		{
			return other != null && this._trsType == other._trsType && EqualityComparer<Transform>.Default.Equals(this.defaultTransform, other.defaultTransform) && this.uniformAxes == other.uniformAxes && ((this._xcrusher == null) ? (other._xcrusher == null) : this._xcrusher.Equals(other._xcrusher)) && ((this._ycrusher == null) ? (other._ycrusher == null) : this._ycrusher.Equals(other._ycrusher)) && ((this._zcrusher == null) ? (other._zcrusher == null) : this._zcrusher.Equals(other._zcrusher)) && ((this._ucrusher == null) ? (other._ucrusher == null) : this._ucrusher.Equals(other._ucrusher)) && ((this._qcrusher == null) ? (other._qcrusher == null) : this._qcrusher.Equals(other._qcrusher)) && this.local == other.local;
		}

		// Token: 0x06000AB2 RID: 2738 RVA: 0x00037DDC File Offset: 0x00035FDC
		public override int GetHashCode()
		{
			return (((((((-1042106911 * -1521134295 + this._trsType.GetHashCode()) * -1521134295 + this.uniformAxes.GetHashCode()) * -1521134295 + ((this._xcrusher == null) ? 0 : this._xcrusher.GetHashCode())) * -1521134295 + ((this._ycrusher == null) ? 0 : this._ycrusher.GetHashCode())) * -1521134295 + ((this._zcrusher == null) ? 0 : this._zcrusher.GetHashCode())) * -1521134295 + ((this._ucrusher == null) ? 0 : this._ucrusher.GetHashCode())) * -1521134295 + ((this._qcrusher == null) ? 0 : this._qcrusher.GetHashCode())) * -1521134295 + this.local.GetHashCode();
		}

		// Token: 0x06000AB3 RID: 2739 RVA: 0x00037EC7 File Offset: 0x000360C7
		public static bool operator ==(ElementCrusher crusher1, ElementCrusher crusher2)
		{
			return EqualityComparer<ElementCrusher>.Default.Equals(crusher1, crusher2);
		}

		// Token: 0x06000AB4 RID: 2740 RVA: 0x00037ED5 File Offset: 0x000360D5
		public static bool operator !=(ElementCrusher crusher1, ElementCrusher crusher2)
		{
			return !(crusher1 == crusher2);
		}

		// Token: 0x04000BB4 RID: 2996
		public static Dictionary<int, ElementCrusher> staticElementCrushers = new Dictionary<int, ElementCrusher>();

		// Token: 0x04000BB5 RID: 2997
		private static readonly CompressedElement reusableCE = new CompressedElement();

		// Token: 0x04000BB6 RID: 2998
		public static ElementCrusher defaultUncompressedElementCrusher = ElementCrusher.CheckAgainstStatics(new ElementCrusher(ElementCrusher.StaticTRSType.Generic)
		{
			XCrusher = FloatCrusher.defaultUncompressedCrusher,
			YCrusher = FloatCrusher.defaultUncompressedCrusher,
			ZCrusher = FloatCrusher.defaultUncompressedCrusher,
			UCrusher = FloatCrusher.defaultUncompressedCrusher
		}, true);

		// Token: 0x04000BB7 RID: 2999
		public static ElementCrusher defaultUncompressedPosCrusher = ElementCrusher.CheckAgainstStatics(new ElementCrusher(ElementCrusher.StaticTRSType.Position)
		{
			XCrusher = FloatCrusher.defaultUncompressedCrusher,
			YCrusher = FloatCrusher.defaultUncompressedCrusher,
			ZCrusher = FloatCrusher.defaultUncompressedCrusher,
			UCrusher = FloatCrusher.defaultUncompressedCrusher
		}, true);

		// Token: 0x04000BB8 RID: 3000
		public static ElementCrusher defaultUncompressedSclCrusher = ElementCrusher.CheckAgainstStatics(new ElementCrusher(ElementCrusher.StaticTRSType.Position)
		{
			XCrusher = FloatCrusher.defaultUncompressedCrusher,
			YCrusher = FloatCrusher.defaultUncompressedCrusher,
			ZCrusher = FloatCrusher.defaultUncompressedCrusher,
			UCrusher = FloatCrusher.defaultUncompressedCrusher
		}, true);

		// Token: 0x04000BB9 RID: 3001
		public static ElementCrusher defaultHalfFloatElementCrusher = ElementCrusher.CheckAgainstStatics(new ElementCrusher(ElementCrusher.StaticTRSType.Generic)
		{
			XCrusher = FloatCrusher.defaultUncompressedCrusher,
			YCrusher = FloatCrusher.defaultUncompressedCrusher,
			ZCrusher = FloatCrusher.defaultUncompressedCrusher,
			UCrusher = FloatCrusher.defaultUncompressedCrusher
		}, true);

		// Token: 0x04000BBA RID: 3002
		public static ElementCrusher defaultHalfFloatPosCrusher = ElementCrusher.CheckAgainstStatics(new ElementCrusher(ElementCrusher.StaticTRSType.Position)
		{
			XCrusher = FloatCrusher.defaulHalfFloatCrusher,
			YCrusher = FloatCrusher.defaulHalfFloatCrusher,
			ZCrusher = FloatCrusher.defaulHalfFloatCrusher,
			UCrusher = FloatCrusher.defaulHalfFloatCrusher
		}, true);

		// Token: 0x04000BBB RID: 3003
		public static ElementCrusher defaultHalfFloatSclCrusher = ElementCrusher.CheckAgainstStatics(new ElementCrusher(ElementCrusher.StaticTRSType.Scale)
		{
			XCrusher = FloatCrusher.defaulHalfFloatCrusher,
			YCrusher = FloatCrusher.defaulHalfFloatCrusher,
			ZCrusher = FloatCrusher.defaulHalfFloatCrusher,
			UCrusher = FloatCrusher.defaulHalfFloatCrusher
		}, true);

		// Token: 0x04000BBC RID: 3004
		public bool hideFieldName;

		// Token: 0x04000BBD RID: 3005
		[SerializeField]
		private TRSType _trsType;

		// Token: 0x04000BBE RID: 3006
		[SerializeField]
		public Transform defaultTransform;

		// Token: 0x04000BBF RID: 3007
		[SerializeField]
		public ElementCrusher.UniformAxes uniformAxes;

		// Token: 0x04000BC0 RID: 3008
		[SerializeField]
		private FloatCrusher _xcrusher;

		// Token: 0x04000BC1 RID: 3009
		[SerializeField]
		private FloatCrusher _ycrusher;

		// Token: 0x04000BC2 RID: 3010
		[SerializeField]
		private FloatCrusher _zcrusher;

		// Token: 0x04000BC3 RID: 3011
		[SerializeField]
		private FloatCrusher _ucrusher;

		// Token: 0x04000BC4 RID: 3012
		[SerializeField]
		private QuatCrusher _qcrusher;

		// Token: 0x04000BC5 RID: 3013
		[SerializeField]
		public bool local;

		// Token: 0x04000BC6 RID: 3014
		[SerializeField]
		private bool useWorldBounds;

		// Token: 0x04000BC7 RID: 3015
		[WorldBoundsSelectAttribute]
		[HideInInspector]
		[SerializeField]
		private int boundsGroupId;

		// Token: 0x04000BC8 RID: 3016
		[SerializeField]
		public bool enableTRSTypeSelector;

		// Token: 0x04000BC9 RID: 3017
		[SerializeField]
		public bool enableLocalSelector = true;

		// Token: 0x04000BCA RID: 3018
		[NonSerialized]
		private bool cached;

		// Token: 0x04000BCB RID: 3019
		[NonSerialized]
		private bool cache_xEnabled;

		// Token: 0x04000BCC RID: 3020
		[NonSerialized]
		private bool cache_yEnabled;

		// Token: 0x04000BCD RID: 3021
		[NonSerialized]
		private bool cache_zEnabled;

		// Token: 0x04000BCE RID: 3022
		[NonSerialized]
		private bool cache_uEnabled;

		// Token: 0x04000BCF RID: 3023
		[NonSerialized]
		private bool cache_qEnabled;

		// Token: 0x04000BD0 RID: 3024
		[NonSerialized]
		private bool cache_isUniformScale;

		// Token: 0x04000BD1 RID: 3025
		[NonSerialized]
		private readonly int[] cache_xBits = new int[4];

		// Token: 0x04000BD2 RID: 3026
		[NonSerialized]
		private readonly int[] cache_yBits = new int[4];

		// Token: 0x04000BD3 RID: 3027
		[NonSerialized]
		private readonly int[] cache_zBits = new int[4];

		// Token: 0x04000BD4 RID: 3028
		[NonSerialized]
		private readonly int[] cache_uBits = new int[4];

		// Token: 0x04000BD5 RID: 3029
		[NonSerialized]
		private readonly int[] cache_TotalBits = new int[4];

		// Token: 0x04000BD6 RID: 3030
		public ReadOnlyCollection<int> Cached_TotalBits;

		// Token: 0x04000BD7 RID: 3031
		[NonSerialized]
		private int cache_qBits;

		// Token: 0x04000BD8 RID: 3032
		[NonSerialized]
		private bool cache_mustCorrectRotationX;

		// Token: 0x04000BD9 RID: 3033
		public Bounds bounds;

		// Token: 0x04000BDA RID: 3034
		public static ulong[] reusableArray64 = new ulong[2];

		// Token: 0x020003B4 RID: 948
		public enum UniformAxes
		{
			// Token: 0x040012B2 RID: 4786
			NonUniform,
			// Token: 0x040012B3 RID: 4787
			XY = 3,
			// Token: 0x040012B4 RID: 4788
			XZ = 5,
			// Token: 0x040012B5 RID: 4789
			YZ,
			// Token: 0x040012B6 RID: 4790
			XYZ
		}

		// Token: 0x020003B5 RID: 949
		public enum StaticTRSType
		{
			// Token: 0x040012B8 RID: 4792
			Position,
			// Token: 0x040012B9 RID: 4793
			Euler,
			// Token: 0x040012BA RID: 4794
			Quaternion,
			// Token: 0x040012BB RID: 4795
			Scale,
			// Token: 0x040012BC RID: 4796
			Generic
		}
	}
}
