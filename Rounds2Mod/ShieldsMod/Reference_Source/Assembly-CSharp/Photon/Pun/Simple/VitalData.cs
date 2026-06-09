using System;

namespace Photon.Pun.Simple
{
	// Token: 0x02000282 RID: 642
	public struct VitalData
	{
		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000DF7 RID: 3575 RVA: 0x00043B73 File Offset: 0x00041D73
		// (set) Token: 0x06000DF8 RID: 3576 RVA: 0x00043B7B File Offset: 0x00041D7B
		public double Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		// Token: 0x06000DF9 RID: 3577 RVA: 0x00043B84 File Offset: 0x00041D84
		public VitalData(double value, int ticksUntilDecay, int ticksUntilRegen)
		{
			this._value = value;
			this.ticksUntilDecay = ticksUntilDecay;
			this.ticksUntilRegen = ticksUntilRegen;
		}

		// Token: 0x06000DFA RID: 3578 RVA: 0x00043B9C File Offset: 0x00041D9C
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this._value,
				" ticksUntilDecay: ",
				this.ticksUntilDecay,
				" ticksUntilRegen: ",
				this.ticksUntilRegen
			});
		}

		// Token: 0x04000D34 RID: 3380
		private double _value;

		// Token: 0x04000D35 RID: 3381
		public int ticksUntilRegen;

		// Token: 0x04000D36 RID: 3382
		public int ticksUntilDecay;
	}
}
