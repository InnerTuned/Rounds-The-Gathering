using System;
using System.Reflection;
using UnityEngine;

namespace AmplifyColor
{
	// Token: 0x02000314 RID: 788
	[Serializable]
	public class VolumeEffectField
	{
		// Token: 0x060010C0 RID: 4288 RVA: 0x00050597 File Offset: 0x0004E797
		public VolumeEffectField(string fieldName, string fieldType)
		{
			this.fieldName = fieldName;
			this.fieldType = fieldType;
		}

		// Token: 0x060010C1 RID: 4289 RVA: 0x000505B0 File Offset: 0x0004E7B0
		public VolumeEffectField(FieldInfo pi, Component c) : this(pi.Name, pi.FieldType.FullName)
		{
			object value = pi.GetValue(c);
			this.UpdateValue(value);
		}

		// Token: 0x060010C2 RID: 4290 RVA: 0x000505E4 File Offset: 0x0004E7E4
		public static bool IsValidType(string type)
		{
			return type == "System.Single" || type == "System.Boolean" || type == "UnityEngine.Color" || type == "UnityEngine.Vector2" || type == "UnityEngine.Vector3" || type == "UnityEngine.Vector4";
		}

		// Token: 0x060010C3 RID: 4291 RVA: 0x00050644 File Offset: 0x0004E844
		public void UpdateValue(object val)
		{
			string text = this.fieldType;
			if (text == "System.Single")
			{
				this.valueSingle = (float)val;
				return;
			}
			if (text == "System.Boolean")
			{
				this.valueBoolean = (bool)val;
				return;
			}
			if (text == "UnityEngine.Color")
			{
				this.valueColor = (Color)val;
				return;
			}
			if (text == "UnityEngine.Vector2")
			{
				this.valueVector2 = (Vector2)val;
				return;
			}
			if (text == "UnityEngine.Vector3")
			{
				this.valueVector3 = (Vector3)val;
				return;
			}
			if (!(text == "UnityEngine.Vector4"))
			{
				return;
			}
			this.valueVector4 = (Vector4)val;
		}

		// Token: 0x04000FBF RID: 4031
		public string fieldName;

		// Token: 0x04000FC0 RID: 4032
		public string fieldType;

		// Token: 0x04000FC1 RID: 4033
		public float valueSingle;

		// Token: 0x04000FC2 RID: 4034
		public Color valueColor;

		// Token: 0x04000FC3 RID: 4035
		public bool valueBoolean;

		// Token: 0x04000FC4 RID: 4036
		public Vector2 valueVector2;

		// Token: 0x04000FC5 RID: 4037
		public Vector3 valueVector3;

		// Token: 0x04000FC6 RID: 4038
		public Vector4 valueVector4;
	}
}
