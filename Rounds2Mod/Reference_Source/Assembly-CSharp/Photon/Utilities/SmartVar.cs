using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Photon.Utilities
{
	// Token: 0x0200024F RID: 591
	[Serializable]
	[StructLayout(2)]
	public struct SmartVar
	{
		// Token: 0x06000CC3 RID: 3267 RVA: 0x000401B8 File Offset: 0x0003E3B8
		public static implicit operator SmartVar(int v)
		{
			return new SmartVar
			{
				Int = v,
				TypeCode = SmartVarTypeCode.Int
			};
		}

		// Token: 0x06000CC4 RID: 3268 RVA: 0x000401E0 File Offset: 0x0003E3E0
		public static implicit operator SmartVar(uint v)
		{
			return new SmartVar
			{
				UInt = v,
				TypeCode = SmartVarTypeCode.Uint
			};
		}

		// Token: 0x06000CC5 RID: 3269 RVA: 0x00040208 File Offset: 0x0003E408
		public static implicit operator SmartVar(float v)
		{
			return new SmartVar
			{
				Float = v,
				TypeCode = SmartVarTypeCode.Float
			};
		}

		// Token: 0x06000CC6 RID: 3270 RVA: 0x00040230 File Offset: 0x0003E430
		public static implicit operator SmartVar(bool v)
		{
			return new SmartVar
			{
				Bool = v,
				TypeCode = SmartVarTypeCode.Bool
			};
		}

		// Token: 0x06000CC7 RID: 3271 RVA: 0x00040258 File Offset: 0x0003E458
		public static implicit operator SmartVar(byte v)
		{
			return new SmartVar
			{
				Byte8 = v,
				TypeCode = SmartVarTypeCode.Byte
			};
		}

		// Token: 0x06000CC8 RID: 3272 RVA: 0x00040280 File Offset: 0x0003E480
		public static implicit operator SmartVar(short v)
		{
			return new SmartVar
			{
				Short = v,
				TypeCode = SmartVarTypeCode.Short
			};
		}

		// Token: 0x06000CC9 RID: 3273 RVA: 0x000402A8 File Offset: 0x0003E4A8
		public static implicit operator SmartVar(ushort v)
		{
			return new SmartVar
			{
				UShort = v,
				TypeCode = SmartVarTypeCode.UShort
			};
		}

		// Token: 0x06000CCA RID: 3274 RVA: 0x000402D0 File Offset: 0x0003E4D0
		public static implicit operator SmartVar(char v)
		{
			return new SmartVar
			{
				Char = v,
				TypeCode = SmartVarTypeCode.Char
			};
		}

		// Token: 0x06000CCB RID: 3275 RVA: 0x000402F6 File Offset: 0x0003E4F6
		public static implicit operator int(SmartVar v)
		{
			if (v.TypeCode == SmartVarTypeCode.Int)
			{
				return v.Int;
			}
			UnityEngine.Debug.Log(v.TypeCode);
			throw new InvalidCastException();
		}

		// Token: 0x06000CCC RID: 3276 RVA: 0x0004031D File Offset: 0x0003E51D
		public static implicit operator uint(SmartVar v)
		{
			if (v.TypeCode == SmartVarTypeCode.Uint)
			{
				return v.UInt;
			}
			throw new InvalidCastException();
		}

		// Token: 0x06000CCD RID: 3277 RVA: 0x00040334 File Offset: 0x0003E534
		public static implicit operator float(SmartVar v)
		{
			if (v.TypeCode == SmartVarTypeCode.Float)
			{
				return v.Float;
			}
			UnityEngine.Debug.LogError("cant cast " + v.TypeCode + " to single float");
			throw new InvalidCastException();
		}

		// Token: 0x06000CCE RID: 3278 RVA: 0x0004036A File Offset: 0x0003E56A
		public static implicit operator bool(SmartVar v)
		{
			if (v.TypeCode == SmartVarTypeCode.Bool)
			{
				return v.Bool;
			}
			throw new InvalidCastException();
		}

		// Token: 0x06000CCF RID: 3279 RVA: 0x00040381 File Offset: 0x0003E581
		public static implicit operator byte(SmartVar v)
		{
			if (v.TypeCode == SmartVarTypeCode.Byte)
			{
				return v.Byte8;
			}
			throw new InvalidCastException();
		}

		// Token: 0x06000CD0 RID: 3280 RVA: 0x00040398 File Offset: 0x0003E598
		public static implicit operator short(SmartVar v)
		{
			if (v.TypeCode == SmartVarTypeCode.Short)
			{
				return v.Short;
			}
			throw new InvalidCastException();
		}

		// Token: 0x06000CD1 RID: 3281 RVA: 0x000403AF File Offset: 0x0003E5AF
		public static implicit operator ushort(SmartVar v)
		{
			if (v.TypeCode == SmartVarTypeCode.UShort)
			{
				return v.UShort;
			}
			throw new InvalidCastException();
		}

		// Token: 0x06000CD2 RID: 3282 RVA: 0x000403C6 File Offset: 0x0003E5C6
		public static implicit operator char(SmartVar v)
		{
			if (v.TypeCode == SmartVarTypeCode.Char)
			{
				return v.Char;
			}
			throw new InvalidCastException();
		}

		// Token: 0x06000CD3 RID: 3283 RVA: 0x000403E0 File Offset: 0x0003E5E0
		public SmartVar Copy()
		{
			return new SmartVar
			{
				TypeCode = this.TypeCode,
				Int = this.Int
			};
		}

		// Token: 0x06000CD4 RID: 3284 RVA: 0x00040410 File Offset: 0x0003E610
		public string ToStringVerbose()
		{
			string text = this.TypeCode.ToString() + " ";
			if (this.TypeCode == SmartVarTypeCode.None)
			{
				return text;
			}
			if (this.TypeCode == SmartVarTypeCode.Bool)
			{
				return text + this.Bool.ToString();
			}
			if (this.TypeCode == SmartVarTypeCode.Int)
			{
				return text + this.Int;
			}
			if (this.TypeCode == SmartVarTypeCode.Uint)
			{
				return text + this.UInt;
			}
			if (this.TypeCode == SmartVarTypeCode.Float)
			{
				return text + this.Float;
			}
			if (this.TypeCode == SmartVarTypeCode.Short)
			{
				return text + this.Short;
			}
			if (this.TypeCode == SmartVarTypeCode.UShort)
			{
				return text + this.UShort;
			}
			if (this.TypeCode == SmartVarTypeCode.Byte)
			{
				return text + this.Byte8;
			}
			if (this.TypeCode == SmartVarTypeCode.Char)
			{
				return text + this.Char.ToString();
			}
			return text;
		}

		// Token: 0x06000CD5 RID: 3285 RVA: 0x0004051C File Offset: 0x0003E71C
		public override string ToString()
		{
			if (this.TypeCode == SmartVarTypeCode.None)
			{
				return "";
			}
			if (this.TypeCode == SmartVarTypeCode.Bool)
			{
				return this.Bool.ToString();
			}
			if (this.TypeCode == SmartVarTypeCode.Int)
			{
				return this.Int.ToString();
			}
			if (this.TypeCode == SmartVarTypeCode.Uint)
			{
				return this.UInt.ToString();
			}
			if (this.TypeCode == SmartVarTypeCode.Float)
			{
				return this.Float.ToString();
			}
			if (this.TypeCode == SmartVarTypeCode.Short)
			{
				return this.Short.ToString();
			}
			if (this.TypeCode == SmartVarTypeCode.UShort)
			{
				return this.UShort.ToString();
			}
			if (this.TypeCode == SmartVarTypeCode.Byte)
			{
				return this.Byte8.ToString();
			}
			if (this.TypeCode == SmartVarTypeCode.Char)
			{
				return this.Char.ToString();
			}
			return "";
		}

		// Token: 0x04000C7B RID: 3195
		[FieldOffset(0)]
		public SmartVarTypeCode TypeCode;

		// Token: 0x04000C7C RID: 3196
		[FieldOffset(4)]
		public int Int;

		// Token: 0x04000C7D RID: 3197
		[FieldOffset(4)]
		public uint UInt;

		// Token: 0x04000C7E RID: 3198
		[FieldOffset(4)]
		public bool Bool;

		// Token: 0x04000C7F RID: 3199
		[FieldOffset(4)]
		public float Float;

		// Token: 0x04000C80 RID: 3200
		[FieldOffset(4)]
		public byte Byte8;

		// Token: 0x04000C81 RID: 3201
		[FieldOffset(4)]
		public short Short;

		// Token: 0x04000C82 RID: 3202
		[FieldOffset(4)]
		public ushort UShort;

		// Token: 0x04000C83 RID: 3203
		[FieldOffset(4)]
		public char Char;

		// Token: 0x04000C84 RID: 3204
		public static readonly SmartVar None = new SmartVar
		{
			TypeCode = SmartVarTypeCode.None
		};
	}
}
