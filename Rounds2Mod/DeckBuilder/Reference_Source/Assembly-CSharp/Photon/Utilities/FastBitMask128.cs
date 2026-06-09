using System;

namespace Photon.Utilities
{
	// Token: 0x02000246 RID: 582
	public struct FastBitMask128
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000C7F RID: 3199 RVA: 0x0003EFDC File Offset: 0x0003D1DC
		public ulong Seg1
		{
			get
			{
				return this.seg1;
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000C80 RID: 3200 RVA: 0x0003EFE4 File Offset: 0x0003D1E4
		public ulong Seg2
		{
			get
			{
				return this.seg2;
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000C81 RID: 3201 RVA: 0x0003EFEC File Offset: 0x0003D1EC
		public ulong AllTrue1
		{
			get
			{
				return this.alltrue1;
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000C82 RID: 3202 RVA: 0x0003EFF4 File Offset: 0x0003D1F4
		public ulong AllTrue2
		{
			get
			{
				return this.alltrue2;
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000C83 RID: 3203 RVA: 0x0003EFFC File Offset: 0x0003D1FC
		// (set) Token: 0x06000C84 RID: 3204 RVA: 0x0003F004 File Offset: 0x0003D204
		public int BitCount
		{
			get
			{
				return this.bitcount;
			}
			set
			{
				this.bitcount = value;
				this.seg1bitcount = ((this.bitcount < 64) ? this.bitcount : 64);
				this.seg2bitcount = ((this.bitcount > 64) ? (this.bitcount - 64) : 0);
				this.alltrue1 = ((this.bitcount < 64) ? ((1UL << this.bitcount) - 1UL) : ulong.MaxValue);
				this.alltrue2 = ((this.bitcount == 128) ? ulong.MaxValue : ((this.bitcount > 64) ? ((1UL << this.bitcount - 64) - 1UL) : 0UL));
				this.seg1 &= this.alltrue1;
				this.seg2 &= this.alltrue2;
			}
		}

		// Token: 0x06000C85 RID: 3205 RVA: 0x0003F0D0 File Offset: 0x0003D2D0
		public FastBitMask128(int bitcount)
		{
			this.seg1 = 0UL;
			this.seg2 = 0UL;
			this.bitcount = bitcount;
			this.seg1bitcount = ((bitcount < 64) ? bitcount : 64);
			this.seg2bitcount = ((bitcount > 64) ? (bitcount - 64) : 0);
			this.alltrue1 = ((bitcount < 64) ? ((1UL << bitcount) - 1UL) : ulong.MaxValue);
			this.alltrue2 = ((bitcount == 128) ? ulong.MaxValue : ((bitcount > 64) ? ((1UL << bitcount - 64) - 1UL) : 0UL));
		}

		// Token: 0x06000C86 RID: 3206 RVA: 0x0003F158 File Offset: 0x0003D358
		public FastBitMask128(FastBitMask128 copyFrom)
		{
			this.seg1 = copyFrom.seg1;
			this.seg2 = copyFrom.seg2;
			this.bitcount = copyFrom.bitcount;
			this.seg1bitcount = copyFrom.seg1bitcount;
			this.seg2bitcount = copyFrom.seg2bitcount;
			this.alltrue1 = copyFrom.alltrue1;
			this.alltrue2 = copyFrom.alltrue2;
		}

		// Token: 0x17000079 RID: 121
		public bool this[int bit]
		{
			get
			{
				if (bit < 64)
				{
					return (this.seg1 & 1UL << bit) > 0UL;
				}
				return (this.seg2 & 1UL << bit - 64) > 0UL;
			}
			set
			{
				if (value)
				{
					if (bit < 64)
					{
						this.seg1 |= 1UL << bit;
						return;
					}
					this.seg2 |= 1UL << bit - 64;
					return;
				}
				else
				{
					if (bit < 64)
					{
						this.seg1 &= ~(1UL << bit);
						return;
					}
					this.seg2 &= ~(1UL << bit - 64);
					return;
				}
			}
		}

		// Token: 0x06000C89 RID: 3209 RVA: 0x0003F1B9 File Offset: 0x0003D3B9
		public bool Get(int bit)
		{
			if (bit < 64)
			{
				return (this.seg1 & 1UL << bit) > 0UL;
			}
			return (this.seg2 & 1UL << bit - 64) > 0UL;
		}

		// Token: 0x06000C8A RID: 3210 RVA: 0x0003F260 File Offset: 0x0003D460
		public void Set(int bit, bool value)
		{
			if (value)
			{
				if (bit < 64)
				{
					this.seg1 |= 1UL << bit;
					return;
				}
				this.seg2 |= 1UL << bit - 64;
				return;
			}
			else
			{
				if (bit < 64)
				{
					this.seg1 &= ~(1UL << bit);
					return;
				}
				this.seg2 &= ~(1UL << bit - 64);
				return;
			}
		}

		// Token: 0x06000C8B RID: 3211 RVA: 0x0003F2D5 File Offset: 0x0003D4D5
		public void SetTrue(int bit)
		{
			if (bit < 64)
			{
				this.seg1 |= 1UL << bit;
				return;
			}
			this.seg2 |= 1UL << bit - 64;
		}

		// Token: 0x06000C8C RID: 3212 RVA: 0x0003F308 File Offset: 0x0003D508
		public void SetFalse(int bit)
		{
			if (bit < 64)
			{
				this.seg1 &= ~(1UL << bit);
				return;
			}
			this.seg2 &= ~(1UL << bit - 64);
		}

		// Token: 0x06000C8D RID: 3213 RVA: 0x0003F33D File Offset: 0x0003D53D
		public void SetAllTrue()
		{
			this.seg1 = this.alltrue1;
			this.seg2 = this.alltrue2;
		}

		// Token: 0x06000C8E RID: 3214 RVA: 0x0003F357 File Offset: 0x0003D557
		public void SetAllFalse()
		{
			this.seg1 = 0UL;
			this.seg2 = 0UL;
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000C8F RID: 3215 RVA: 0x0003F369 File Offset: 0x0003D569
		public bool AllAreFalse
		{
			get
			{
				return this.bitcount != 0 && this.seg1 == 0UL && this.seg2 == 0UL;
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000C90 RID: 3216 RVA: 0x0003F387 File Offset: 0x0003D587
		public bool AllAreTrue
		{
			get
			{
				return this.bitcount == 0 || (this.seg1 == this.alltrue1 && this.seg2 == this.alltrue2);
			}
		}

		// Token: 0x06000C91 RID: 3217 RVA: 0x0003F3B1 File Offset: 0x0003D5B1
		public void OR(FastBitMask128 other)
		{
			this.seg1 |= other.seg1;
			this.seg2 |= other.seg2;
		}

		// Token: 0x06000C92 RID: 3218 RVA: 0x0003F3DC File Offset: 0x0003D5DC
		public void OR(FastBitMask128 other, int otherOffset)
		{
			if (otherOffset == 0)
			{
				this.seg1 |= other.seg1;
				this.seg2 |= other.seg2;
				return;
			}
			if (otherOffset == 64)
			{
				this.seg2 |= other.seg1;
				return;
			}
			if (otherOffset >= 128)
			{
				return;
			}
			if (otherOffset > 64)
			{
				this.seg2 |= this.seg1 << otherOffset - 64;
				return;
			}
			this.seg1 |= other.seg1 << otherOffset;
			this.seg2 |= other.seg1 >> 64 - otherOffset;
			this.seg2 |= other.seg2 << otherOffset;
		}

		// Token: 0x06000C93 RID: 3219 RVA: 0x0003F4A1 File Offset: 0x0003D6A1
		public void AND(FastBitMask128 other)
		{
			this.seg1 &= other.seg1;
			this.seg2 &= other.seg2;
		}

		// Token: 0x06000C94 RID: 3220 RVA: 0x0003F4C9 File Offset: 0x0003D6C9
		public void XOR(FastBitMask128 other)
		{
			this.seg1 ^= other.seg1;
			this.seg2 ^= other.seg2;
		}

		// Token: 0x06000C95 RID: 3221 RVA: 0x0003F4F4 File Offset: 0x0003D6F4
		public static FastBitMask128 operator |(FastBitMask128 a, FastBitMask128 b)
		{
			return new FastBitMask128(a)
			{
				seg1 = (a.seg1 | b.seg1),
				seg2 = (a.seg2 | b.seg2)
			};
		}

		// Token: 0x06000C96 RID: 3222 RVA: 0x0003F534 File Offset: 0x0003D734
		public static FastBitMask128 operator &(FastBitMask128 a, FastBitMask128 b)
		{
			return new FastBitMask128(a)
			{
				seg1 = (a.seg1 & b.seg1),
				seg2 = (a.seg2 & b.seg2)
			};
		}

		// Token: 0x06000C97 RID: 3223 RVA: 0x0003F574 File Offset: 0x0003D774
		public static FastBitMask128 operator ^(FastBitMask128 a, FastBitMask128 b)
		{
			return new FastBitMask128(a)
			{
				seg1 = ((a.seg1 ^ b.seg1) & a.alltrue1),
				seg2 = ((a.seg2 ^ b.seg2) & a.alltrue2)
			};
		}

		// Token: 0x06000C98 RID: 3224 RVA: 0x0003F5C0 File Offset: 0x0003D7C0
		public static FastBitMask128 operator !(FastBitMask128 a)
		{
			return new FastBitMask128(a)
			{
				seg1 = (~a.seg1 & a.alltrue1),
				seg2 = (~a.seg2 & a.alltrue2)
			};
		}

		// Token: 0x06000C99 RID: 3225 RVA: 0x0003F600 File Offset: 0x0003D800
		public FastBitMask128 NOT()
		{
			return new FastBitMask128(this)
			{
				seg1 = (~this.seg1 & this.alltrue1),
				seg2 = (~this.seg2 & this.alltrue2)
			};
		}

		// Token: 0x06000C9A RID: 3226 RVA: 0x0003F648 File Offset: 0x0003D848
		public int CountTrue()
		{
			int num;
			if (this.seg1 == 0UL)
			{
				num = 0;
			}
			else if (this.seg1 == this.alltrue1)
			{
				num = this.seg1bitcount;
			}
			else
			{
				num = 0;
				for (ulong num2 = this.seg1; num2 != 0UL; num2 >>= 1)
				{
					if ((num2 & 1UL) == 1UL)
					{
						num++;
					}
				}
			}
			if (this.seg2 == 0UL)
			{
				return num;
			}
			if (this.seg2 == this.alltrue2)
			{
				return num + this.seg2bitcount;
			}
			for (ulong num3 = this.seg2; num3 != 0UL; num3 >>= 1)
			{
				if ((num3 & 1UL) == 1UL)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000C9B RID: 3227 RVA: 0x0003F6D4 File Offset: 0x0003D8D4
		public int CountFalse()
		{
			int num;
			if (this.seg1 == 0UL)
			{
				num = 0;
			}
			else if (this.seg1 == this.alltrue1)
			{
				num = this.seg1bitcount;
			}
			else
			{
				num = 0;
				for (ulong num2 = this.seg1; num2 != 0UL; num2 >>= 1)
				{
					if ((num2 & 1UL) == 1UL)
					{
						num++;
					}
				}
			}
			if (this.seg2 == 0UL)
			{
				return this.bitcount - num;
			}
			if (this.seg2 == this.alltrue2)
			{
				return this.bitcount - (num + this.seg2bitcount);
			}
			for (ulong num3 = this.seg2; num3 != 0UL; num3 >>= 1)
			{
				if ((num3 & 1UL) == 1UL)
				{
					num++;
				}
			}
			return this.bitcount - num;
		}

		// Token: 0x06000C9C RID: 3228 RVA: 0x0003F778 File Offset: 0x0003D978
		public void ClearBitsBefore(int start, int count)
		{
			ulong num = (count == 64) ? ulong.MaxValue : ((1UL << count) - 1UL);
			int num2 = start - count;
			ulong num3;
			ulong num4;
			if (this.bitcount > 64)
			{
				if (num2 >= 0)
				{
					num3 = num << num2;
					num4 = num >> this.seg2bitcount - num2;
				}
				else
				{
					ulong num5 = num << this.bitcount + num2;
					num3 = (num >> -num2 | num5);
					num4 = num << this.seg2bitcount + num2;
				}
				this.seg1 &= ~num3;
				this.seg2 &= (~num4 & this.alltrue2);
				return;
			}
			if (num2 >= 0)
			{
				num3 = num << num2;
				num4 = num >> this.seg1bitcount - num2;
			}
			else
			{
				num3 = num >> -num2;
				num4 = num << this.seg1bitcount + num2;
			}
			this.seg1 &= (~num3 & ~num4 & this.alltrue1);
		}

		// Token: 0x06000C9D RID: 3229 RVA: 0x0003F85C File Offset: 0x0003DA5C
		public int CountValidRange(int start, int lookahead)
		{
			int num = this.bitcount;
			for (int i = lookahead; i >= 0; i--)
			{
				int num2 = start + i;
				if (num2 >= num)
				{
					num2 -= num;
				}
				if (num2 < 64)
				{
					if ((this.seg1 & 1UL << num2) != 0UL)
					{
						return i + 1;
					}
				}
				else if ((this.seg2 & 1UL << num2 - 64) != 0UL)
				{
					return i + 1;
				}
			}
			return 0;
		}

		// Token: 0x06000C9E RID: 3230 RVA: 0x0003F8BC File Offset: 0x0003DABC
		public void Copy(FastBitMask128 other)
		{
			this.bitcount = other.bitcount;
			this.seg1bitcount = other.seg1bitcount;
			this.seg2bitcount = other.seg2bitcount;
			this.seg1 = other.seg1;
			this.seg2 = other.seg2;
			this.alltrue1 = other.alltrue1;
			this.alltrue2 = other.alltrue2;
		}

		// Token: 0x06000C9F RID: 3231 RVA: 0x0003F91D File Offset: 0x0003DB1D
		public bool Compare(FastBitMask128 other)
		{
			return this.bitcount == other.bitcount && this.seg1 == other.seg1 && this.seg2 == other.seg2;
		}

		// Token: 0x04000C5B RID: 3163
		private ulong seg1;

		// Token: 0x04000C5C RID: 3164
		private ulong seg2;

		// Token: 0x04000C5D RID: 3165
		private int bitcount;

		// Token: 0x04000C5E RID: 3166
		private int seg1bitcount;

		// Token: 0x04000C5F RID: 3167
		private int seg2bitcount;

		// Token: 0x04000C60 RID: 3168
		private ulong alltrue1;

		// Token: 0x04000C61 RID: 3169
		private ulong alltrue2;
	}
}
