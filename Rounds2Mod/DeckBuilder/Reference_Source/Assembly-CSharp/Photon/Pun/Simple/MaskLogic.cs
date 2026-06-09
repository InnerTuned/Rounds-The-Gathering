using System;

namespace Photon.Pun.Simple
{
	// Token: 0x020002F0 RID: 752
	[Serializable]
	public abstract class MaskLogic
	{
		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06001025 RID: 4133
		protected abstract string[] EnumNames { get; }

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x06001026 RID: 4134
		protected abstract int[] EnumValues { get; }

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06001027 RID: 4135
		protected abstract bool DefinesZero { get; }

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06001028 RID: 4136
		protected abstract int DefaultValue { get; }

		// Token: 0x06001029 RID: 4137 RVA: 0x0004E657 File Offset: 0x0004C857
		public MaskLogic()
		{
			this.stateMask = this.DefaultValue;
		}

		// Token: 0x0600102A RID: 4138 RVA: 0x0004E672 File Offset: 0x0004C872
		public void RecalculateMasks()
		{
			if (this.operation == MaskLogic.Operator.EQUALS)
			{
				this.notMask = 0;
			}
			this.notMask &= this.stateMask;
			this.trueMask = (this.stateMask & ~this.notMask);
		}

		// Token: 0x0600102B RID: 4139 RVA: 0x0004E6AC File Offset: 0x0004C8AC
		public bool Evaluate(int state)
		{
			if (this.stateMask == 0)
			{
				return state == 0;
			}
			switch (this.operation)
			{
			case MaskLogic.Operator.EQUALS:
				return this.stateMask == state;
			case MaskLogic.Operator.AND:
				return (this.trueMask & state) == this.trueMask && (this.notMask & state) == 0;
			case MaskLogic.Operator.OR:
				return (this.trueMask & state) != 0 || (this.notMask & state) != this.notMask;
			default:
				this.operation = MaskLogic.Operator.EQUALS;
				return this.stateMask == state;
			}
		}

		// Token: 0x04000F3F RID: 3903
		public MaskLogic.Operator operation = MaskLogic.Operator.AND;

		// Token: 0x04000F40 RID: 3904
		public int stateMask;

		// Token: 0x04000F41 RID: 3905
		public int notMask;

		// Token: 0x04000F42 RID: 3906
		protected int trueMask;

		// Token: 0x020003DD RID: 989
		public enum Operator
		{
			// Token: 0x0400132C RID: 4908
			EQUALS,
			// Token: 0x0400132D RID: 4909
			AND,
			// Token: 0x0400132E RID: 4910
			OR
		}
	}
}
