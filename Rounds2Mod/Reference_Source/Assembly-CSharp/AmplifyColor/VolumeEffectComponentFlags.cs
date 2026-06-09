using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AmplifyColor
{
	// Token: 0x02000319 RID: 793
	[Serializable]
	public class VolumeEffectComponentFlags
	{
		// Token: 0x060010DF RID: 4319 RVA: 0x00051445 File Offset: 0x0004F645
		public VolumeEffectComponentFlags(string name)
		{
			this.componentName = name;
			this.componentFields = new List<VolumeEffectFieldFlags>();
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x00051460 File Offset: 0x0004F660
		public VolumeEffectComponentFlags(VolumeEffectComponent comp) : this(comp.componentName)
		{
			this.blendFlag = true;
			foreach (VolumeEffectField volumeEffectField in comp.fields)
			{
				if (VolumeEffectField.IsValidType(volumeEffectField.fieldType))
				{
					this.componentFields.Add(new VolumeEffectFieldFlags(volumeEffectField));
				}
			}
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x000514E0 File Offset: 0x0004F6E0
		public VolumeEffectComponentFlags(Component c) : this(string.Concat(c.GetType()))
		{
			foreach (FieldInfo fieldInfo in c.GetType().GetFields())
			{
				if (VolumeEffectField.IsValidType(fieldInfo.FieldType.FullName))
				{
					this.componentFields.Add(new VolumeEffectFieldFlags(fieldInfo));
				}
			}
		}

		// Token: 0x060010E2 RID: 4322 RVA: 0x00051540 File Offset: 0x0004F740
		public void UpdateComponentFlags(VolumeEffectComponent comp)
		{
			using (List<VolumeEffectField>.Enumerator enumerator = comp.fields.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					VolumeEffectField field = enumerator.Current;
					if (this.componentFields.Find((VolumeEffectFieldFlags s) => s.fieldName == field.fieldName) == null && VolumeEffectField.IsValidType(field.fieldType))
					{
						this.componentFields.Add(new VolumeEffectFieldFlags(field));
					}
				}
			}
		}

		// Token: 0x060010E3 RID: 4323 RVA: 0x000515D8 File Offset: 0x0004F7D8
		public void UpdateComponentFlags(Component c)
		{
			FieldInfo[] fields = c.GetType().GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo pi = fields[i];
				if (!this.componentFields.Exists((VolumeEffectFieldFlags s) => s.fieldName == pi.Name) && VolumeEffectField.IsValidType(pi.FieldType.FullName))
				{
					this.componentFields.Add(new VolumeEffectFieldFlags(pi));
				}
			}
		}

		// Token: 0x060010E4 RID: 4324 RVA: 0x00051654 File Offset: 0x0004F854
		public string[] GetFieldNames()
		{
			return Enumerable.ToArray<string>(Enumerable.Select<VolumeEffectFieldFlags, string>(Enumerable.Where<VolumeEffectFieldFlags>(this.componentFields, (VolumeEffectFieldFlags r) => r.blendFlag), (VolumeEffectFieldFlags r) => r.fieldName));
		}

		// Token: 0x04000FCF RID: 4047
		public string componentName;

		// Token: 0x04000FD0 RID: 4048
		public List<VolumeEffectFieldFlags> componentFields;

		// Token: 0x04000FD1 RID: 4049
		public bool blendFlag;
	}
}
