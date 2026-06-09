using System;
using System.Collections.Generic;
using emotitron.Compression;
using UnityEngine;

namespace Photon.Compression
{
	// Token: 0x0200020C RID: 524
	[Serializable]
	public class QuatCrusher : Crusher<QuatCrusher>, IEquatable<QuatCrusher>, ICrusherCopy<QuatCrusher>
	{
		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000ACD RID: 2765 RVA: 0x00038454 File Offset: 0x00036654
		// (set) Token: 0x06000ACE RID: 2766 RVA: 0x0003847C File Offset: 0x0003667C
		public int Bits
		{
			get
			{
				if (!this.enabled)
				{
					return 0;
				}
				if (!QuatCrusher.QC_ISPRO)
				{
					return QuatCrusher.RoundBitsToBestPreset(this.bits);
				}
				return this.bits;
			}
			set
			{
				if (QuatCrusher.QC_ISPRO)
				{
					this.bits = value;
					this.CompressLevel = CompressLevel.SetBits;
				}
				else
				{
					this.bits = QuatCrusher.RoundBitsToBestPreset(value);
					this.CompressLevel = (CompressLevel)this.bits;
				}
				if (this.OnRecalculated != null)
				{
					this.OnRecalculated.Invoke(this);
				}
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000ACF RID: 2767 RVA: 0x000384CC File Offset: 0x000366CC
		// (set) Token: 0x06000AD0 RID: 2768 RVA: 0x000384D4 File Offset: 0x000366D4
		public CompressLevel CompressLevel
		{
			get
			{
				return this._compressLevel;
			}
			set
			{
				if (QuatCrusher.QC_ISPRO)
				{
					this._compressLevel = value;
					this.bits = ((this._compressLevel == CompressLevel.SetBits) ? this.bits : ((int)this._compressLevel));
				}
				else
				{
					if (this._compressLevel == CompressLevel.SetBits)
					{
						this._compressLevel = (CompressLevel)this.bits;
					}
					this._compressLevel = (CompressLevel)QuatCrusher.RoundBitsToBestPreset((int)value);
					this.bits = (int)this._compressLevel;
				}
				if (this.OnRecalculated != null)
				{
					this.OnRecalculated.Invoke(this);
				}
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000AD1 RID: 2769 RVA: 0x0003854F File Offset: 0x0003674F
		// (set) Token: 0x06000AD2 RID: 2770 RVA: 0x00038557 File Offset: 0x00036757
		[SerializeField]
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
				if (this.OnRecalculated != null)
				{
					this.OnRecalculated.Invoke(this);
				}
			}
		}

		// Token: 0x06000AD3 RID: 2771 RVA: 0x00038574 File Offset: 0x00036774
		public QuatCrusher()
		{
			this._compressLevel = CompressLevel.uint64Hi;
			this.showEnableToggle = false;
			this.isStandalone = true;
		}

		// Token: 0x06000AD4 RID: 2772 RVA: 0x00038599 File Offset: 0x00036799
		public QuatCrusher(int bits, bool showEnableToggle = false, bool isStandalone = true)
		{
			this.bits = (QuatCrusher.QC_ISPRO ? bits : QuatCrusher.RoundBitsToBestPreset(bits));
			this._compressLevel = CompressLevel.SetBits;
			this.showEnableToggle = showEnableToggle;
			this.isStandalone = isStandalone;
		}

		// Token: 0x06000AD5 RID: 2773 RVA: 0x000385D3 File Offset: 0x000367D3
		public QuatCrusher(bool showEnableToggle = false, bool isStandalone = true)
		{
			this.bits = 32;
			this._compressLevel = (QuatCrusher.QC_ISPRO ? CompressLevel.SetBits : CompressLevel.uint32Med);
			this.showEnableToggle = showEnableToggle;
			this.isStandalone = isStandalone;
		}

		// Token: 0x06000AD6 RID: 2774 RVA: 0x0003860A File Offset: 0x0003680A
		public QuatCrusher(CompressLevel compressLevel, bool showEnableToggle = false, bool isStandalone = true)
		{
			this._compressLevel = compressLevel;
			this.bits = (int)compressLevel;
			this.showEnableToggle = showEnableToggle;
			this.isStandalone = isStandalone;
		}

		// Token: 0x06000AD7 RID: 2775 RVA: 0x00038635 File Offset: 0x00036835
		public void Initialize()
		{
			this.cache = QuatCompress.caches[this.Bits];
			this.initialized = true;
		}

		// Token: 0x06000AD8 RID: 2776 RVA: 0x000027C8 File Offset: 0x000009C8
		public override void OnBeforeSerialize()
		{
		}

		// Token: 0x06000AD9 RID: 2777 RVA: 0x00038654 File Offset: 0x00036854
		public override void OnAfterDeserialize()
		{
			if (this.OnRecalculated != null)
			{
				this.OnRecalculated.Invoke(this);
			}
		}

		// Token: 0x06000ADA RID: 2778 RVA: 0x0003866A File Offset: 0x0003686A
		public static int RoundBitsToBestPreset(int bits)
		{
			if (bits > 32)
			{
				return 64;
			}
			if (bits > 16)
			{
				return 32;
			}
			if (bits > 8)
			{
				return 16;
			}
			return 0;
		}

		// Token: 0x06000ADB RID: 2779 RVA: 0x00038684 File Offset: 0x00036884
		public ulong Compress()
		{
			if (!this.initialized)
			{
				this.Initialize();
			}
			if (this.local)
			{
				return QuatCompress.Compress(this.transform.localRotation, this.cache);
			}
			return QuatCompress.Compress(this.transform.rotation, this.cache);
		}

		// Token: 0x06000ADC RID: 2780 RVA: 0x000386D4 File Offset: 0x000368D4
		public ulong Compress(Quaternion quat)
		{
			if (!this.initialized)
			{
				this.Initialize();
			}
			return QuatCompress.Compress(quat, this.cache);
		}

		// Token: 0x06000ADD RID: 2781 RVA: 0x000386F0 File Offset: 0x000368F0
		public Quaternion Decompress(ulong compressed)
		{
			if (!this.initialized)
			{
				this.Initialize();
			}
			return QuatCompress.Decompress(compressed, this.cache);
		}

		// Token: 0x06000ADE RID: 2782 RVA: 0x0003870C File Offset: 0x0003690C
		public ulong Write(Quaternion quat, byte[] buffer, ref int bitposition)
		{
			ulong num = this.Compress(quat);
			ArraySerializeExt.Write(buffer, num, ref bitposition, this.bits);
			return num;
		}

		// Token: 0x06000ADF RID: 2783 RVA: 0x00038730 File Offset: 0x00036930
		public ulong Write(Quaternion quat, uint[] buffer, ref int bitposition)
		{
			ulong num = this.Compress(quat);
			ArraySerializeExt.Write(buffer, num, ref bitposition, this.bits);
			return num;
		}

		// Token: 0x06000AE0 RID: 2784 RVA: 0x00038754 File Offset: 0x00036954
		public ulong Write(Quaternion quat, ulong[] buffer, ref int bitposition)
		{
			ulong num = this.Compress(quat);
			ArraySerializeExt.Write(buffer, num, ref bitposition, this.bits);
			return num;
		}

		// Token: 0x06000AE1 RID: 2785 RVA: 0x00038778 File Offset: 0x00036978
		public ulong Write(ulong c, byte[] buffer, ref int bitposition)
		{
			ArraySerializeExt.Write(buffer, c, ref bitposition, this.bits);
			return c;
		}

		// Token: 0x06000AE2 RID: 2786 RVA: 0x00038789 File Offset: 0x00036989
		public ulong Write(ulong c, uint[] buffer, ref int bitposition)
		{
			ArraySerializeExt.Write(buffer, c, ref bitposition, this.bits);
			return c;
		}

		// Token: 0x06000AE3 RID: 2787 RVA: 0x0003879A File Offset: 0x0003699A
		public ulong Write(ulong c, ulong[] buffer, ref int bitposition)
		{
			ArraySerializeExt.Write(buffer, c, ref bitposition, this.bits);
			return c;
		}

		// Token: 0x06000AE4 RID: 2788 RVA: 0x000387AC File Offset: 0x000369AC
		public Quaternion Read(byte[] buffer, ref int bitposition)
		{
			ulong compressed = ArraySerializeExt.Read(buffer, ref bitposition, this.bits);
			return this.Decompress(compressed);
		}

		// Token: 0x06000AE5 RID: 2789 RVA: 0x000387D0 File Offset: 0x000369D0
		public Quaternion Read(uint[] buffer, ref int bitposition)
		{
			ulong compressed = ArraySerializeExt.Read(buffer, ref bitposition, this.bits);
			return this.Decompress(compressed);
		}

		// Token: 0x06000AE6 RID: 2790 RVA: 0x000387F4 File Offset: 0x000369F4
		public Quaternion Read(ulong[] buffer, ref int bitposition)
		{
			ulong compressed = ArraySerializeExt.Read(buffer, ref bitposition, this.bits);
			return this.Decompress(compressed);
		}

		// Token: 0x06000AE7 RID: 2791 RVA: 0x00038816 File Offset: 0x00036A16
		public ulong Write(Quaternion quat, ref ulong buffer, ref int bitposition)
		{
			ulong num = this.Compress(quat);
			PrimitiveSerializeExt.Inject(num, ref buffer, ref bitposition, this.bits);
			return num;
		}

		// Token: 0x06000AE8 RID: 2792 RVA: 0x00038830 File Offset: 0x00036A30
		public Quaternion Read(ref ulong buffer, ref int bitposition)
		{
			ulong compressed = PrimitiveSerializeExt.Read(buffer, ref bitposition, this.bits);
			return this.Decompress(compressed);
		}

		// Token: 0x06000AE9 RID: 2793 RVA: 0x00038853 File Offset: 0x00036A53
		public void CopyFrom(QuatCrusher source)
		{
			this.bits = source.bits;
			this._compressLevel = source._compressLevel;
			this.local = source.local;
		}

		// Token: 0x06000AEA RID: 2794 RVA: 0x00038879 File Offset: 0x00036A79
		public override bool Equals(object obj)
		{
			return this.Equals(obj as QuatCrusher);
		}

		// Token: 0x06000AEB RID: 2795 RVA: 0x00038887 File Offset: 0x00036A87
		public bool Equals(QuatCrusher other)
		{
			return other != null && this.bits == other.bits && this._compressLevel == other._compressLevel && this.local == other.local;
		}

		// Token: 0x06000AEC RID: 2796 RVA: 0x000388C0 File Offset: 0x00036AC0
		public override int GetHashCode()
		{
			return ((-282774512 * -1521134295 + this.bits.GetHashCode()) * -1521134295 + this._compressLevel.GetHashCode()) * -1521134295 + this.local.GetHashCode();
		}

		// Token: 0x06000AED RID: 2797 RVA: 0x0003890E File Offset: 0x00036B0E
		public static bool operator ==(QuatCrusher crusher1, QuatCrusher crusher2)
		{
			return EqualityComparer<QuatCrusher>.Default.Equals(crusher1, crusher2);
		}

		// Token: 0x06000AEE RID: 2798 RVA: 0x0003891C File Offset: 0x00036B1C
		public static bool operator !=(QuatCrusher crusher1, QuatCrusher crusher2)
		{
			return !(crusher1 == crusher2);
		}

		// Token: 0x04000BE1 RID: 3041
		public static bool QC_ISPRO = true;

		// Token: 0x04000BE2 RID: 3042
		[Range(16f, 64f)]
		[SerializeField]
		private int bits;

		// Token: 0x04000BE3 RID: 3043
		[SerializeField]
		public CompressLevel _compressLevel;

		// Token: 0x04000BE4 RID: 3044
		[SerializeField]
		public Transform transform;

		// Token: 0x04000BE5 RID: 3045
		[SerializeField]
		public bool local;

		// Token: 0x04000BE6 RID: 3046
		[HideInInspector]
		public bool isStandalone;

		// Token: 0x04000BE7 RID: 3047
		[SerializeField]
		public bool showEnableToggle;

		// Token: 0x04000BE8 RID: 3048
		[SerializeField]
		private bool enabled = true;

		// Token: 0x04000BE9 RID: 3049
		private QuatCompress.Cache cache;

		// Token: 0x04000BEA RID: 3050
		[NonSerialized]
		private bool initialized;
	}
}
