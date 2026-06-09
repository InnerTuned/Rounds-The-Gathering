using System;
using System.Collections.Generic;
using Photon.Compression;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x0200028D RID: 653
	[Serializable]
	public class Vitals : IOnVitalValueChange, IOnVitalChange, IOnVitalParamChange
	{
		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000E2D RID: 3629 RVA: 0x0004449A File Offset: 0x0004269A
		public List<VitalDefinition> VitalDefs
		{
			get
			{
				return this.vitalDefs;
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000E2E RID: 3630 RVA: 0x000444A2 File Offset: 0x000426A2
		public Vital[] VitalArray
		{
			get
			{
				if (!this.initialized)
				{
					this.Initialize();
				}
				return this.vitalArray;
			}
		}

		// Token: 0x06000E2F RID: 3631 RVA: 0x000444B8 File Offset: 0x000426B8
		public void Initialize()
		{
			if (this.initialized)
			{
				return;
			}
			this.vitalCount = this.vitalDefs.Count;
			this.vitalArray = new Vital[this.vitalCount];
			for (int i = 0; i < this.vitalCount; i++)
			{
				VitalDefinition vitalDefinition = this.vitalDefs[i];
				Vital vital = new Vital(vitalDefinition);
				this.vitalArray[i] = vital;
				vital.Initialize(TickEngineSettings.netTickInterval);
				vital.AddIOnVitalChange(this);
				int hash = vitalDefinition.VitalName.hash;
				if (vitalDefinition.VitalName.type != VitalType.None)
				{
					if (!this.vitalLookup.ContainsKey(hash))
					{
						this.vitalLookup.Add(hash, vital);
					}
					else
					{
						global::Debug.LogWarning("VitalNameType hash collision! Vitals cannot have more than one of each Vital Type in its list.");
					}
				}
			}
			this.initialized = true;
		}

		// Token: 0x06000E30 RID: 3632 RVA: 0x00044578 File Offset: 0x00042778
		public void ResetValues()
		{
			for (int i = 0; i < this.vitalCount; i++)
			{
				this.vitalArray[i].ResetValues();
			}
		}

		// Token: 0x06000E31 RID: 3633 RVA: 0x000445A4 File Offset: 0x000427A4
		public Vital GetVital(VitalNameType vitalNameType)
		{
			if (!this.initialized)
			{
				this.Initialize();
			}
			Vital result;
			this.vitalLookup.TryGetValue(vitalNameType.hash, ref result);
			return result;
		}

		// Token: 0x06000E32 RID: 3634 RVA: 0x000445D4 File Offset: 0x000427D4
		public int GetVitalIndex(VitalNameType vitalNameType)
		{
			if (this.vitalDefs == null)
			{
				return -1;
			}
			int hash = vitalNameType.hash;
			int i = 0;
			int count = this.vitalDefs.Count;
			while (i < count)
			{
				if (this.vitalDefs[i].VitalName.hash == hash)
				{
					return i;
				}
				i++;
			}
			return -1;
		}

		// Token: 0x06000E33 RID: 3635 RVA: 0x00044628 File Offset: 0x00042828
		public SerializationFlags Serialize(VitalsData vdata, VitalsData lastVData, byte[] buffer, ref int bitposition, bool keyframe)
		{
			VitalData[] datas = vdata.datas;
			VitalData[] datas2 = lastVData.datas;
			SerializationFlags serializationFlags = SerializationFlags.None;
			int i = 0;
			int num = this.vitalCount;
			while (i < num)
			{
				serializationFlags |= this.vitalDefs[i].Serialize(datas[i], datas2[i], buffer, ref bitposition, keyframe);
				i++;
			}
			return serializationFlags;
		}

		// Token: 0x06000E34 RID: 3636 RVA: 0x00044684 File Offset: 0x00042884
		public SerializationFlags Deserialize(VitalsData vdata, byte[] buffer, ref int bitposition, bool keyframe)
		{
			VitalData[] datas = vdata.datas;
			bool flag = true;
			bool flag2 = false;
			for (int i = 0; i < this.vitalCount; i++)
			{
				datas[i] = this.vitalDefs[i].Deserialize(buffer, ref bitposition, keyframe);
				if (datas[i].Value == double.NegativeInfinity)
				{
					flag = false;
				}
				else
				{
					flag2 |= true;
				}
			}
			if (flag)
			{
				return (SerializationFlags)33;
			}
			if (!flag2)
			{
				return SerializationFlags.None;
			}
			return SerializationFlags.HasContent;
		}

		// Token: 0x06000E35 RID: 3637 RVA: 0x000446F4 File Offset: 0x000428F4
		public void Apply(VitalsData vdata)
		{
			VitalData[] datas = vdata.datas;
			for (int i = 0; i < this.vitalCount; i++)
			{
				this.vitalArray[i].Apply(datas[i]);
			}
		}

		// Token: 0x06000E36 RID: 3638 RVA: 0x00044730 File Offset: 0x00042930
		private double ApplyCharges(int vitalIdx, double discharge, bool allowOverload, bool propagate)
		{
			double num = discharge;
			double num2 = 0.0;
			for (int i = vitalIdx; i >= 0; i--)
			{
				num2 += this.vitalArray[i].ApplyCharges(num, allowOverload, !propagate);
				if (!propagate)
				{
					return num2;
				}
				num -= num2;
				if (num == 0.0)
				{
					break;
				}
			}
			return num2;
		}

		// Token: 0x06000E37 RID: 3639 RVA: 0x00044785 File Offset: 0x00042985
		public double ApplyCharges(double discharge, bool allowOverload, bool propagate)
		{
			return this.ApplyCharges(this.vitalCount - 1, discharge, allowOverload, propagate);
		}

		// Token: 0x06000E38 RID: 3640 RVA: 0x00044798 File Offset: 0x00042998
		public double ApplyCharges(VitalNameType vitalNameType, double discharge, bool allowOverload, bool propagate)
		{
			int num;
			if (vitalNameType.type == VitalType.None)
			{
				num = this.vitalCount - 1;
			}
			else
			{
				num = this.GetVitalIndex(vitalNameType);
				if (num == -1)
				{
					return 0.0;
				}
			}
			double num2 = this.ApplyCharges(num, discharge, allowOverload, propagate);
			this.CheckForDisrupt(num2);
			return num2;
		}

		// Token: 0x06000E39 RID: 3641 RVA: 0x000447E2 File Offset: 0x000429E2
		public void CheckForDisrupt(double consumed)
		{
			if (consumed == 0.0)
			{
				return;
			}
			if (consumed > 0.0)
			{
				this.DisruptDecay();
			}
			if (consumed < 0.0)
			{
				this.DisruptRegen();
			}
		}

		// Token: 0x06000E3A RID: 3642 RVA: 0x00044818 File Offset: 0x00042A18
		public void DisruptRegen()
		{
			int i = 0;
			int num = this.vitalCount;
			while (i < num)
			{
				this.vitalArray[i].DisruptRegen();
				i++;
			}
		}

		// Token: 0x06000E3B RID: 3643 RVA: 0x00044848 File Offset: 0x00042A48
		public void DisruptDecay()
		{
			int i = 0;
			int num = this.vitalCount;
			while (i < num)
			{
				this.vitalArray[i].DisruptDecay();
				i++;
			}
		}

		// Token: 0x06000E3C RID: 3644 RVA: 0x00044878 File Offset: 0x00042A78
		public void OnVitalValueChange(Vital vital)
		{
			int i = 0;
			int count = this.OnVitalValueChangeCallbacks.Count;
			while (i < count)
			{
				this.OnVitalValueChangeCallbacks[i].OnVitalValueChange(vital);
				i++;
			}
		}

		// Token: 0x06000E3D RID: 3645 RVA: 0x000448B0 File Offset: 0x00042AB0
		public void OnVitalParamChange(Vital vital)
		{
			int i = 0;
			int count = this.OnVitalParamChangeCallbacks.Count;
			while (i < count)
			{
				this.OnVitalParamChangeCallbacks[i].OnVitalParamChange(vital);
				i++;
			}
		}

		// Token: 0x06000E3E RID: 3646 RVA: 0x000448E8 File Offset: 0x00042AE8
		public void Simulate()
		{
			int i = 0;
			int num = this.vitalCount;
			while (i < num)
			{
				this.vitalArray[i].Simulate();
				i++;
			}
		}

		// Token: 0x06000E3F RID: 3647 RVA: 0x00044918 File Offset: 0x00042B18
		public Vitals()
		{
			List<VitalDefinition> list = new List<VitalDefinition>();
			list.Add(new VitalDefinition(100.0, 125U, 125.0, 1.0, 1f, 1.0, 1f, 1.0, "Health"));
			list.Add(new VitalDefinition(100.0, 125U, 50.0, 0.6669999957084656, 1f, 1.0, 1f, 0.0, "Armor"));
			list.Add(new VitalDefinition(200.0, 250U, 100.0, 1.0, 1f, 20.0, 1f, 10.0, "Shield"));
			this.vitalDefs = list;
			this.vitalLookup = new Dictionary<int, Vital>();
			base..ctor();
		}

		// Token: 0x04000D59 RID: 3417
		public List<IOnVitalValueChange> OnVitalValueChangeCallbacks = new List<IOnVitalValueChange>();

		// Token: 0x04000D5A RID: 3418
		public List<IOnVitalParamChange> OnVitalParamChangeCallbacks = new List<IOnVitalParamChange>();

		// Token: 0x04000D5B RID: 3419
		[HideInInspector]
		public List<VitalDefinition> vitalDefs;

		// Token: 0x04000D5C RID: 3420
		[NonSerialized]
		private Vital[] vitalArray;

		// Token: 0x04000D5D RID: 3421
		private Dictionary<int, Vital> vitalLookup;

		// Token: 0x04000D5E RID: 3422
		private int vitalCount;

		// Token: 0x04000D5F RID: 3423
		private bool initialized;
	}
}
