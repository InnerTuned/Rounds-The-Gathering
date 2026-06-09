using System;
using UnityEngine;

namespace emotitron.Utilities.Networking
{
	// Token: 0x020001EC RID: 492
	public class AutoDestroyWrongNetLib : MonoBehaviour
	{
		// Token: 0x060009B6 RID: 2486 RVA: 0x00031759 File Offset: 0x0002F959
		private void Awake()
		{
			if ((this.netLib & AutoDestroyWrongNetLib.NetLib.PUN2) == AutoDestroyWrongNetLib.NetLib.UNET)
			{
				Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x04000B04 RID: 2820
		[SerializeField]
		public AutoDestroyWrongNetLib.NetLib netLib;

		// Token: 0x020003B0 RID: 944
		public enum NetLib
		{
			// Token: 0x040012A1 RID: 4769
			UNET,
			// Token: 0x040012A2 RID: 4770
			PUN,
			// Token: 0x040012A3 RID: 4771
			PUN2,
			// Token: 0x040012A4 RID: 4772
			PUNAndPUN2
		}
	}
}
