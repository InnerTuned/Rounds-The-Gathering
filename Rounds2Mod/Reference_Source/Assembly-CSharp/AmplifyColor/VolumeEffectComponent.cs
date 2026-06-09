using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AmplifyColor
{
	// Token: 0x02000315 RID: 789
	[Serializable]
	public class VolumeEffectComponent
	{
		// Token: 0x060010C4 RID: 4292 RVA: 0x000506F4 File Offset: 0x0004E8F4
		public VolumeEffectComponent(string name)
		{
			this.componentName = name;
			this.fields = new List<VolumeEffectField>();
		}

		// Token: 0x060010C5 RID: 4293 RVA: 0x0005070E File Offset: 0x0004E90E
		public VolumeEffectField AddField(FieldInfo pi, Component c)
		{
			return this.AddField(pi, c, -1);
		}

		// Token: 0x060010C6 RID: 4294 RVA: 0x0005071C File Offset: 0x0004E91C
		public VolumeEffectField AddField(FieldInfo pi, Component c, int position)
		{
			VolumeEffectField volumeEffectField = VolumeEffectField.IsValidType(pi.FieldType.FullName) ? new VolumeEffectField(pi, c) : null;
			if (volumeEffectField != null)
			{
				if (position < 0 || position >= this.fields.Count)
				{
					this.fields.Add(volumeEffectField);
				}
				else
				{
					this.fields.Insert(position, volumeEffectField);
				}
			}
			return volumeEffectField;
		}

		// Token: 0x060010C7 RID: 4295 RVA: 0x00050777 File Offset: 0x0004E977
		public void RemoveEffectField(VolumeEffectField field)
		{
			this.fields.Remove(field);
		}

		// Token: 0x060010C8 RID: 4296 RVA: 0x00050788 File Offset: 0x0004E988
		public VolumeEffectComponent(Component c, VolumeEffectComponentFlags compFlags) : this(compFlags.componentName)
		{
			foreach (VolumeEffectFieldFlags volumeEffectFieldFlags in compFlags.componentFields)
			{
				if (volumeEffectFieldFlags.blendFlag)
				{
					FieldInfo field = c.GetType().GetField(volumeEffectFieldFlags.fieldName);
					VolumeEffectField volumeEffectField = VolumeEffectField.IsValidType(field.FieldType.FullName) ? new VolumeEffectField(field, c) : null;
					if (volumeEffectField != null)
					{
						this.fields.Add(volumeEffectField);
					}
				}
			}
		}

		// Token: 0x060010C9 RID: 4297 RVA: 0x00050828 File Offset: 0x0004EA28
		public void UpdateComponent(Component c, VolumeEffectComponentFlags compFlags)
		{
			using (List<VolumeEffectFieldFlags>.Enumerator enumerator = compFlags.componentFields.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					VolumeEffectFieldFlags fieldFlags = enumerator.Current;
					if (fieldFlags.blendFlag && !this.fields.Exists((VolumeEffectField s) => s.fieldName == fieldFlags.fieldName))
					{
						FieldInfo field = c.GetType().GetField(fieldFlags.fieldName);
						VolumeEffectField volumeEffectField = VolumeEffectField.IsValidType(field.FieldType.FullName) ? new VolumeEffectField(field, c) : null;
						if (volumeEffectField != null)
						{
							this.fields.Add(volumeEffectField);
						}
					}
				}
			}
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x000508EC File Offset: 0x0004EAEC
		public VolumeEffectField FindEffectField(string fieldName)
		{
			for (int i = 0; i < this.fields.Count; i++)
			{
				if (this.fields[i].fieldName == fieldName)
				{
					return this.fields[i];
				}
			}
			return null;
		}

		// Token: 0x060010CB RID: 4299 RVA: 0x00050938 File Offset: 0x0004EB38
		public static FieldInfo[] ListAcceptableFields(Component c)
		{
			if (c == null)
			{
				return new FieldInfo[0];
			}
			return Enumerable.ToArray<FieldInfo>(Enumerable.Where<FieldInfo>(c.GetType().GetFields(), (FieldInfo f) => VolumeEffectField.IsValidType(f.FieldType.FullName)));
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x00050989 File Offset: 0x0004EB89
		public string[] GetFieldNames()
		{
			return Enumerable.ToArray<string>(Enumerable.Select<VolumeEffectField, string>(this.fields, (VolumeEffectField r) => r.fieldName));
		}

		// Token: 0x04000FC7 RID: 4039
		public string componentName;

		// Token: 0x04000FC8 RID: 4040
		public List<VolumeEffectField> fields;
	}
}
