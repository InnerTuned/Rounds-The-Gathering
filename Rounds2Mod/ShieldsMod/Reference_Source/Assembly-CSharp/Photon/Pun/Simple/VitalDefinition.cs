using System;
using System.Collections.Generic;
using Photon.Compression;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000283 RID: 643
	[Serializable]
	public class VitalDefinition
	{
		// Token: 0x06000DFB RID: 3579 RVA: 0x00043BEE File Offset: 0x00041DEE
		public void AddIOnVitalChange(IOnVitalValueChange cb)
		{
			this.iOnVitalChange.Add(cb);
		}

		// Token: 0x06000DFC RID: 3580 RVA: 0x00043BFC File Offset: 0x00041DFC
		public void RemoveIOnVitalChange(IOnVitalValueChange cb)
		{
			this.iOnVitalChange.Remove(cb);
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000DFD RID: 3581 RVA: 0x00043C0B File Offset: 0x00041E0B
		public VitalNameType VitalName
		{
			get
			{
				return this.vitalName;
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000DFE RID: 3582 RVA: 0x00043C13 File Offset: 0x00041E13
		public double FullValue
		{
			get
			{
				return this._fullValue;
			}
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000DFF RID: 3583 RVA: 0x00043C1B File Offset: 0x00041E1B
		public double MaxValue
		{
			get
			{
				return this._maxValue;
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000E00 RID: 3584 RVA: 0x00043C25 File Offset: 0x00041E25
		public double Absorbtion
		{
			get
			{
				return this.absorption;
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000E01 RID: 3585 RVA: 0x00043C2D File Offset: 0x00041E2D
		public int DecayDelayInTicks
		{
			get
			{
				return this._decayDelayInTicks;
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000E02 RID: 3586 RVA: 0x00043C35 File Offset: 0x00041E35
		public int RegenDelayInTicks
		{
			get
			{
				return this._regenDelayInTicks;
			}
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000E03 RID: 3587 RVA: 0x00043C3D File Offset: 0x00041E3D
		public double DecayPerTick
		{
			get
			{
				return this._decayPerTick;
			}
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000E04 RID: 3588 RVA: 0x00043C45 File Offset: 0x00041E45
		public double RegenPerTick
		{
			get
			{
				return this._regenPerTick;
			}
		}

		// Token: 0x06000E05 RID: 3589 RVA: 0x00043C50 File Offset: 0x00041E50
		public VitalDefinition(double fullValue, uint maxValue, double startValue, double absorbtion, float regenDelay, double regenRate, float decayDelay, double decayRate, string name)
		{
			this._fullValue = fullValue;
			this._maxValue = maxValue;
			this.startValue = startValue;
			this.absorption = absorbtion;
			this.regenDelay = regenDelay;
			this.regenRate = regenRate;
			this.decayDelay = decayDelay;
			this.decayRate = decayRate;
			this.vitalName = new VitalNameType(name);
		}

		// Token: 0x06000E06 RID: 3590 RVA: 0x00043CB8 File Offset: 0x00041EB8
		public void Initialize(float tickDuration)
		{
			this.SetTickInterval(tickDuration);
		}

		// Token: 0x06000E07 RID: 3591 RVA: 0x00043CC4 File Offset: 0x00041EC4
		public void SetTickInterval(float tickInterval)
		{
			this._decayDelayInTicks = (int)(this.decayDelay / tickInterval);
			this._regenDelayInTicks = (int)(this.regenDelay / tickInterval);
			this._decayPerTick = this.decayRate * (double)tickInterval;
			this._regenPerTick = this.regenRate * (double)tickInterval;
			this.bitsForValue = this._maxValue.GetBitsForMaxValue();
			this.bitsForDecayDelay = this._decayDelayInTicks.GetBitsForMaxValue();
			this.bitsForRegenDelay = this._regenDelayInTicks.GetBitsForMaxValue();
		}

		// Token: 0x06000E08 RID: 3592 RVA: 0x00043D40 File Offset: 0x00041F40
		public VitalData GetDefaultData()
		{
			return new VitalData(this.startValue, this._decayDelayInTicks, this._regenDelayInTicks);
		}

		// Token: 0x06000E09 RID: 3593 RVA: 0x00043D5C File Offset: 0x00041F5C
		public SerializationFlags Serialize(VitalData vitalData, VitalData prevVitalData, byte[] buffer, ref int bitposition, bool keyframe = true)
		{
			int ticksUntilDecay = vitalData.ticksUntilDecay;
			int ticksUntilRegen = vitalData.ticksUntilRegen;
			int num = (int)vitalData.Value;
			int num2 = (int)prevVitalData.Value;
			bool flag = num != num2;
			if (keyframe)
			{
				ArraySerializeExt.Write(buffer, (ulong)((long)num), ref bitposition, this.bitsForValue);
			}
			else
			{
				ArraySerializeExt.WriteBool(buffer, flag, ref bitposition);
				if (flag)
				{
					ArraySerializeExt.Write(buffer, (ulong)((long)num), ref bitposition, this.bitsForValue);
				}
			}
			if (ticksUntilDecay > 0)
			{
				ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
				ArraySerializeExt.Write(buffer, (ulong)((long)ticksUntilDecay), ref bitposition, this.bitsForDecayDelay);
			}
			else
			{
				ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
			}
			if (ticksUntilRegen > 0)
			{
				ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
				ArraySerializeExt.Write(buffer, (ulong)((long)ticksUntilRegen), ref bitposition, this.bitsForRegenDelay);
			}
			else
			{
				ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
			}
			if (!flag && !keyframe)
			{
				return SerializationFlags.None;
			}
			return SerializationFlags.HasContent;
		}

		// Token: 0x06000E0A RID: 3594 RVA: 0x00043E20 File Offset: 0x00042020
		public VitalData Deserialize(byte[] buffer, ref int bitposition, bool keyframe = true)
		{
			double value = (keyframe || ArraySerializeExt.ReadBool(buffer, ref bitposition)) ? ArraySerializeExt.Read(buffer, ref bitposition, this.bitsForValue) : double.NegativeInfinity;
			return new VitalData(value, (int)(ArraySerializeExt.ReadBool(buffer, ref bitposition) ? ArraySerializeExt.Read(buffer, ref bitposition, this.bitsForDecayDelay) : 0UL), (int)(ArraySerializeExt.ReadBool(buffer, ref bitposition) ? ArraySerializeExt.Read(buffer, ref bitposition, this.bitsForRegenDelay) : 0UL));
		}

		// Token: 0x06000E0B RID: 3595 RVA: 0x00043E90 File Offset: 0x00042090
		public VitalData Extrapolate(VitalData prev)
		{
			int num = (prev.ticksUntilRegen > 0) ? (prev.ticksUntilRegen - 1) : 0;
			int num2 = (prev.ticksUntilDecay > 0) ? (prev.ticksUntilDecay - 1) : 0;
			double value = prev.Value;
			return new VitalData((value > this.FullValue && num2 == 0) ? (value - this._decayPerTick) : ((value < this.FullValue && num == 0) ? (value + this._regenPerTick) : value), num2, num);
		}

		// Token: 0x04000D37 RID: 3383
		public List<IOnVitalValueChange> iOnVitalChange = new List<IOnVitalValueChange>();

		// Token: 0x04000D38 RID: 3384
		[SerializeField]
		private VitalNameType vitalName;

		// Token: 0x04000D39 RID: 3385
		[Tooltip("Values greater than this will degrade at the decay rate until this value is reached.")]
		[SerializeField]
		private double _fullValue;

		// Token: 0x04000D3A RID: 3386
		[Tooltip("The absolute greatest possible value. Values above Full Value are considered overloaded, and will decay down to Full Value.")]
		[SerializeField]
		private uint _maxValue;

		// Token: 0x04000D3B RID: 3387
		public double startValue;

		// Token: 0x04000D3C RID: 3388
		[Tooltip("Number of simulation ticks after damage until regeneration resumes.")]
		[SerializeField]
		private float regenDelay;

		// Token: 0x04000D3D RID: 3389
		[Tooltip("Amount per tick values less than Full Value will increase until Full Health is reached.")]
		[SerializeField]
		private double regenRate;

		// Token: 0x04000D3E RID: 3390
		[Tooltip("Number of simulation ticks after overload until decay resumes.")]
		[SerializeField]
		private float decayDelay;

		// Token: 0x04000D3F RID: 3391
		[Tooltip("Amount per tick overloaded values greater than Full Value will degrade until Full Health is reached.")]
		[SerializeField]
		private double decayRate;

		// Token: 0x04000D40 RID: 3392
		[Range(0f, 1f)]
		[Tooltip("How much of the damage this vital absords, the remainder is passed through to the next lower stat. 0 = None (useless), 0.5 = Half, 1 = Full. The root vital (0) likely should always be 1.")]
		[SerializeField]
		private double absorption;

		// Token: 0x04000D41 RID: 3393
		private int _decayDelayInTicks;

		// Token: 0x04000D42 RID: 3394
		private int _regenDelayInTicks;

		// Token: 0x04000D43 RID: 3395
		private double _decayPerTick;

		// Token: 0x04000D44 RID: 3396
		private double _regenPerTick;

		// Token: 0x04000D45 RID: 3397
		private int bitsForValue;

		// Token: 0x04000D46 RID: 3398
		private int bitsForDecayDelay;

		// Token: 0x04000D47 RID: 3399
		private int bitsForRegenDelay;
	}
}
