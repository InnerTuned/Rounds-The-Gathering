using System;

namespace Photon.Pun.Simple
{
	// Token: 0x020002A3 RID: 675
	[Serializable]
	public class ObjStateLogic : MaskLogic
	{
		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000ED2 RID: 3794 RVA: 0x000422B1 File Offset: 0x000404B1
		protected override bool DefinesZero
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000ED3 RID: 3795 RVA: 0x00047CCD File Offset: 0x00045ECD
		protected override string[] EnumNames
		{
			get
			{
				return ObjStateLogic.stateNames;
			}
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000ED4 RID: 3796 RVA: 0x00047CD4 File Offset: 0x00045ED4
		protected override int[] EnumValues
		{
			get
			{
				return ObjStateLogic.stateValues;
			}
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000ED5 RID: 3797 RVA: 0x000422B1 File Offset: 0x000404B1
		protected override int DefaultValue
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x04000DD8 RID: 3544
		protected static int[] stateValues = (int[])Enum.GetValues(typeof(ObjStateEditor));

		// Token: 0x04000DD9 RID: 3545
		protected static string[] stateNames = Enum.GetNames(typeof(ObjStateEditor));
	}
}
