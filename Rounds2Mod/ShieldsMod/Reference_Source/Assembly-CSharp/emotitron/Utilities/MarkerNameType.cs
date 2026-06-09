using System;
using UnityEngine;

namespace emotitron.Utilities
{
	// Token: 0x020001EA RID: 490
	[Serializable]
	public struct MarkerNameType
	{
		// Token: 0x060009B1 RID: 2481 RVA: 0x00031666 File Offset: 0x0002F866
		public MarkerNameType(MarkerType vitalType)
		{
			this.type = vitalType;
			this.name = Enum.GetName(typeof(MarkerType), vitalType);
			this.hash = this.name.GetHashCode();
		}

		// Token: 0x060009B2 RID: 2482 RVA: 0x0003169B File Offset: 0x0002F89B
		public MarkerNameType(string name)
		{
			this.type = (MarkerType)NameTypeUtils.GetVitalTypeForName(name, MarkerNameType.enumNames);
			this.name = name;
			this.hash = name.GetHashCode();
		}

		// Token: 0x060009B3 RID: 2483 RVA: 0x000316C4 File Offset: 0x0002F8C4
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"NameType: ",
				this.type,
				" ",
				this.name,
				" ",
				this.hash
			});
		}

		// Token: 0x04000B00 RID: 2816
		[HideInInspector]
		public MarkerType type;

		// Token: 0x04000B01 RID: 2817
		[HideInInspector]
		public int hash;

		// Token: 0x04000B02 RID: 2818
		[HideInInspector]
		public string name;

		// Token: 0x04000B03 RID: 2819
		public static string[] enumNames = Enum.GetNames(typeof(MarkerType));
	}
}
