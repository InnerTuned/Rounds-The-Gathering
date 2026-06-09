using System;
using System.Reflection;

namespace AmplifyColor
{
	// Token: 0x02000318 RID: 792
	[Serializable]
	public class VolumeEffectFieldFlags
	{
		// Token: 0x060010DD RID: 4317 RVA: 0x000513F9 File Offset: 0x0004F5F9
		public VolumeEffectFieldFlags(FieldInfo pi)
		{
			this.fieldName = pi.Name;
			this.fieldType = pi.FieldType.FullName;
		}

		// Token: 0x060010DE RID: 4318 RVA: 0x0005141E File Offset: 0x0004F61E
		public VolumeEffectFieldFlags(VolumeEffectField field)
		{
			this.fieldName = field.fieldName;
			this.fieldType = field.fieldType;
			this.blendFlag = true;
		}

		// Token: 0x04000FCC RID: 4044
		public string fieldName;

		// Token: 0x04000FCD RID: 4045
		public string fieldType;

		// Token: 0x04000FCE RID: 4046
		public bool blendFlag;
	}
}
