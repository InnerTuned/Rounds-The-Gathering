using System;

namespace Photon.Compression
{
	// Token: 0x020001F2 RID: 498
	[Serializable]
	public abstract class LiteCrusher<TComp, T> : LiteCrusher where TComp : struct where T : struct
	{
		// Token: 0x060009C6 RID: 2502
		public abstract TComp Encode(T val);

		// Token: 0x060009C7 RID: 2503
		public abstract T Decode(uint val);

		// Token: 0x060009C8 RID: 2504
		public abstract TComp WriteValue(T val, byte[] buffer, ref int bitposition);

		// Token: 0x060009C9 RID: 2505
		public abstract void WriteCValue(uint val, byte[] buffer, ref int bitposition);

		// Token: 0x060009CA RID: 2506
		public abstract T ReadValue(byte[] buffer, ref int bitposition);

		// Token: 0x060009CB RID: 2507
		public abstract TComp ReadCValue(byte[] buffer, ref int bitposition);
	}
}
