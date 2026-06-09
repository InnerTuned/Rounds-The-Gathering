using System;

namespace UnityEngine.UI.ProceduralImage
{
	// Token: 0x020001E3 RID: 483
	[AttributeUsage(4)]
	public class ModifierID : Attribute
	{
		// Token: 0x0600098B RID: 2443 RVA: 0x00030FC5 File Offset: 0x0002F1C5
		public ModifierID(string name)
		{
			this.name = name;
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600098C RID: 2444 RVA: 0x00030FD4 File Offset: 0x0002F1D4
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		// Token: 0x04000AE7 RID: 2791
		private string name;
	}
}
