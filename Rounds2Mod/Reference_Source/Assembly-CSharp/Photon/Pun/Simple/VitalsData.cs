using System;

namespace Photon.Pun.Simple
{
	// Token: 0x02000281 RID: 641
	public class VitalsData
	{
		// Token: 0x06000DF5 RID: 3573 RVA: 0x00043B18 File Offset: 0x00041D18
		public VitalsData(Vitals vitals)
		{
			this.vitals = vitals;
			this.datas = new VitalData[vitals.VitalArray.Length];
		}

		// Token: 0x06000DF6 RID: 3574 RVA: 0x00043B3C File Offset: 0x00041D3C
		public void CopyFrom(VitalsData source)
		{
			VitalData[] array = source.datas;
			int i = 0;
			int num = array.Length;
			while (i < num)
			{
				this.datas[i] = array[i];
				i++;
			}
		}

		// Token: 0x04000D32 RID: 3378
		public Vitals vitals;

		// Token: 0x04000D33 RID: 3379
		public VitalData[] datas;
	}
}
