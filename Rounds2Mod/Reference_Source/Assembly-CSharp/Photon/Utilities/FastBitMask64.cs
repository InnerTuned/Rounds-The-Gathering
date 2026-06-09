using System;

namespace Photon.Utilities
{
	// Token: 0x02000248 RID: 584
	public struct FastBitMask64
	{
		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000CA0 RID: 3232 RVA: 0x0003F94B File Offset: 0x0003DB4B
		// (set) Token: 0x06000CA1 RID: 3233 RVA: 0x0003F953 File Offset: 0x0003DB53
		public int BitCount
		{
			get
			{
				return this.bitcount;
			}
			set
			{
				this.bitcount = value;
				this.alltrue = ((this.bitcount < 64) ? ((1UL << this.bitcount) - 1UL) : ulong.MaxValue);
				this.bitmask &= this.alltrue;
			}
		}

		// Token: 0x06000CA2 RID: 3234 RVA: 0x0003F992 File Offset: 0x0003DB92
		public FastBitMask64(int bitcount)
		{
			this.bitmask = 0UL;
			this.bitcount = bitcount;
			this.alltrue = ((bitcount < 64) ? ((1UL << bitcount) - 1UL) : ulong.MaxValue);
		}

		// Token: 0x06000CA3 RID: 3235 RVA: 0x0003F9BC File Offset: 0x0003DBBC
		private FastBitMask64(FastBitMask64 copyFrom)
		{
			this.bitmask = copyFrom.bitmask;
			this.bitcount = copyFrom.bitcount;
			this.alltrue = copyFrom.alltrue;
		}

		// Token: 0x1700007D RID: 125
		public bool this[int bit]
		{
			get
			{
				return (this.bitmask & 1UL << bit) > 0UL;
			}
			set
			{
				if (value)
				{
					this.bitmask |= 1UL << bit;
					return;
				}
				this.bitmask &= ~(1UL << bit);
			}
		}

		// Token: 0x06000CA6 RID: 3238 RVA: 0x0003F9E2 File Offset: 0x0003DBE2
		public bool Get(int bit)
		{
			return (this.bitmask & 1UL << bit) > 0UL;
		}

		// Token: 0x06000CA7 RID: 3239 RVA: 0x0003F9F6 File Offset: 0x0003DBF6
		public void Set(int bit, bool value)
		{
			if (value)
			{
				this.bitmask |= 1UL << bit;
				return;
			}
			this.bitmask &= ~(1UL << bit);
		}

		// Token: 0x06000CA8 RID: 3240 RVA: 0x0003FA25 File Offset: 0x0003DC25
		public void SetTrue(int bit)
		{
			this.bitmask |= 1UL << bit;
		}

		// Token: 0x06000CA9 RID: 3241 RVA: 0x0003FA3B File Offset: 0x0003DC3B
		public void SetFalse(int bit)
		{
			this.bitmask &= ~(1UL << bit);
		}

		// Token: 0x06000CAA RID: 3242 RVA: 0x0003FA52 File Offset: 0x0003DC52
		public void SetAllTrue()
		{
			this.bitmask = this.alltrue;
		}

		// Token: 0x06000CAB RID: 3243 RVA: 0x0003FA60 File Offset: 0x0003DC60
		public void SetAllFalse()
		{
			this.bitmask = 0UL;
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000CAC RID: 3244 RVA: 0x0003FA6A File Offset: 0x0003DC6A
		public bool AllAreFalse
		{
			get
			{
				return this.bitcount != 0 && this.bitmask == 0UL;
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000CAD RID: 3245 RVA: 0x0003FA80 File Offset: 0x0003DC80
		public bool AllAreTrue
		{
			get
			{
				return this.bitcount == 0 || this.bitmask == this.alltrue;
			}
		}

		// Token: 0x06000CAE RID: 3246 RVA: 0x0003FA9A File Offset: 0x0003DC9A
		public void OR(FastBitMask64 other)
		{
			this.bitmask |= other.bitmask;
		}

		// Token: 0x06000CAF RID: 3247 RVA: 0x0003FAAF File Offset: 0x0003DCAF
		public void AND(FastBitMask64 other)
		{
			this.bitmask &= other.bitmask;
		}

		// Token: 0x06000CB0 RID: 3248 RVA: 0x0003FAC4 File Offset: 0x0003DCC4
		public void XOR(FastBitMask64 other)
		{
			this.bitmask ^= other.bitmask;
		}

		// Token: 0x06000CB1 RID: 3249 RVA: 0x0003FADC File Offset: 0x0003DCDC
		public int CountTrue()
		{
			int num;
			if (this.bitmask == 0UL)
			{
				num = 0;
			}
			else if (this.bitmask == this.alltrue)
			{
				num = this.bitcount;
			}
			else
			{
				num = 0;
				for (ulong num2 = this.bitmask; num2 != 0UL; num2 >>= 1)
				{
					if ((num2 & 1UL) == 1UL)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x06000CB2 RID: 3250 RVA: 0x0003FB2C File Offset: 0x0003DD2C
		public int CountFalse()
		{
			if (this.bitmask == 0UL)
			{
				return this.bitcount;
			}
			if (this.bitmask == this.alltrue)
			{
				return 0;
			}
			int num = 0;
			int i = 0;
			int num2 = this.bitcount;
			while (i < num2)
			{
				if ((this.bitmask & 1UL << i) == 0UL)
				{
					num++;
				}
				i++;
			}
			return num;
		}

		// Token: 0x06000CB3 RID: 3251 RVA: 0x0003FB84 File Offset: 0x0003DD84
		public void ClearBitsBefore(int start, int count)
		{
			ulong num = (count == 64) ? ulong.MaxValue : ((1UL << count) - 1UL);
			int num2 = start - count;
			ulong num3;
			ulong num4;
			if (num2 >= 0)
			{
				num3 = num << num2;
				num4 = num >> this.bitcount - num2;
			}
			else
			{
				num3 = num >> -num2;
				num4 = num << this.bitcount + num2;
			}
			this.bitmask &= (~num3 & ~num4 & this.alltrue);
		}

		// Token: 0x06000CB4 RID: 3252 RVA: 0x0003FBF4 File Offset: 0x0003DDF4
		public int CountValidRange(int start, int lookahead)
		{
			for (int i = lookahead; i >= 0; i--)
			{
				int num = start + i;
				if (num >= this.bitcount)
				{
					num -= this.bitcount;
				}
				if ((this.bitmask & 1UL << num) != 0UL)
				{
					return i + 1;
				}
			}
			return 0;
		}

		// Token: 0x06000CB5 RID: 3253 RVA: 0x0003FC38 File Offset: 0x0003DE38
		public void Copy(FastBitMask64 other)
		{
			this.bitcount = other.bitcount;
			this.bitmask = other.bitmask;
			this.alltrue = other.alltrue;
		}

		// Token: 0x06000CB6 RID: 3254 RVA: 0x0003FC5E File Offset: 0x0003DE5E
		public bool Compare(FastBitMask64 other)
		{
			return this.bitcount == other.bitcount && this.bitmask == other.bitmask;
		}

		// Token: 0x04000C62 RID: 3170
		public ulong bitmask;

		// Token: 0x04000C63 RID: 3171
		private int bitcount;

		// Token: 0x04000C64 RID: 3172
		private ulong alltrue;
	}
}
