using System;
using emotitron.Utilities;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000285 RID: 645
	[Serializable]
	public struct VitalNameType
	{
		// Token: 0x06000E0C RID: 3596 RVA: 0x00043F03 File Offset: 0x00042103
		public VitalNameType(VitalType vitalType)
		{
			this.type = vitalType;
			this.name = Enum.GetName(typeof(VitalType), vitalType);
			this.hash = this.name.GetHashCode();
		}

		// Token: 0x06000E0D RID: 3597 RVA: 0x00043F38 File Offset: 0x00042138
		public VitalNameType(string name)
		{
			this.type = (VitalType)NameTypeUtils.GetVitalTypeForName(name, VitalNameType.enumNames);
			this.name = name;
			this.hash = name.GetHashCode();
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000E0E RID: 3598 RVA: 0x00043F5E File Offset: 0x0004215E
		public string[] EnumNames
		{
			get
			{
				return VitalNameType.enumNames;
			}
		}

		// Token: 0x06000E0F RID: 3599 RVA: 0x00043F68 File Offset: 0x00042168
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"VitalNameType: ",
				this.type,
				" ",
				this.name,
				" ",
				this.hash
			});
		}

		// Token: 0x04000D51 RID: 3409
		[HideInInspector]
		public VitalType type;

		// Token: 0x04000D52 RID: 3410
		[HideInInspector]
		public int hash;

		// Token: 0x04000D53 RID: 3411
		[HideInInspector]
		public string name;

		// Token: 0x04000D54 RID: 3412
		public static string[] enumNames = Enum.GetNames(typeof(VitalType));
	}
}
