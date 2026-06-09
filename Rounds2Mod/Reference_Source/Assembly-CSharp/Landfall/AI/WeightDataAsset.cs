using System;
using System.Collections.Generic;
using UnityEngine;

namespace Landfall.AI
{
	// Token: 0x02000323 RID: 803
	[CreateAssetMenu(fileName = "WeightDataAsset", menuName = "Landfall/AI/Weight Data")]
	public class WeightDataAsset : ScriptableObject
	{
		// Token: 0x0600114C RID: 4428 RVA: 0x000027C8 File Offset: 0x000009C8
		public void Save()
		{
		}

		// Token: 0x04000FFA RID: 4090
		public List<WeightDataAsset.WeightData> m_weightDatas;

		// Token: 0x020003EE RID: 1006
		[Serializable]
		public struct WeightData
		{
			// Token: 0x0400134C RID: 4940
			public double[] m_weights;
		}
	}
}
