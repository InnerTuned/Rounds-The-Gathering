using System;

namespace UnityEngine.UI.ProceduralImage
{
	// Token: 0x020001E6 RID: 486
	[DisallowMultipleComponent]
	public abstract class ProceduralImageModifier : MonoBehaviour
	{
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060009A7 RID: 2471 RVA: 0x00031592 File Offset: 0x0002F792
		protected Graphic _Graphic
		{
			get
			{
				if (this.graphic == null)
				{
					this.graphic = base.GetComponent<Graphic>();
				}
				return this.graphic;
			}
		}

		// Token: 0x060009A8 RID: 2472
		public abstract Vector4 CalculateRadius(Rect imageRect);

		// Token: 0x04000AF2 RID: 2802
		protected Graphic graphic;
	}
}
