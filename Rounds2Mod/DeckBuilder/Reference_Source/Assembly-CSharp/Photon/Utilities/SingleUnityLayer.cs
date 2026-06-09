using System;
using UnityEngine;

namespace Photon.Utilities
{
	// Token: 0x02000244 RID: 580
	[Serializable]
	public class SingleUnityLayer
	{
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000C79 RID: 3193 RVA: 0x0003EF97 File Offset: 0x0003D197
		public int LayerIndex
		{
			get
			{
				return this.m_LayerIndex;
			}
		}

		// Token: 0x06000C7A RID: 3194 RVA: 0x0003EF9F File Offset: 0x0003D19F
		public void Set(int _layerIndex)
		{
			if (_layerIndex > 0 && _layerIndex < 32)
			{
				this.m_LayerIndex = _layerIndex;
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000C7B RID: 3195 RVA: 0x0003EFB1 File Offset: 0x0003D1B1
		public int Mask
		{
			get
			{
				return 1 << this.m_LayerIndex;
			}
		}

		// Token: 0x06000C7C RID: 3196 RVA: 0x0003EFBE File Offset: 0x0003D1BE
		public static implicit operator int(SingleUnityLayer m)
		{
			return m.LayerIndex;
		}

		// Token: 0x04000C58 RID: 3160
		[SerializeField]
		[HideInInspector]
		private int m_LayerIndex;
	}
}
