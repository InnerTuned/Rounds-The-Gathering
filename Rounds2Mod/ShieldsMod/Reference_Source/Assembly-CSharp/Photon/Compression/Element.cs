using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Photon.Compression
{
	// Token: 0x02000211 RID: 529
	[StructLayout(2)]
	public struct Element : IEquatable<Element>
	{
		// Token: 0x06000B61 RID: 2913 RVA: 0x0003A7B7 File Offset: 0x000389B7
		public Element(Vector3 v)
		{
			this = default(Element);
			this.vectorType = Element.VectorType.Vector3;
			this.v = v;
		}

		// Token: 0x06000B62 RID: 2914 RVA: 0x0003A7CE File Offset: 0x000389CE
		public Element(Quaternion quat)
		{
			this = default(Element);
			this.vectorType = Element.VectorType.Quaternion;
			this.quat = quat;
		}

		// Token: 0x06000B63 RID: 2915 RVA: 0x0003A7E5 File Offset: 0x000389E5
		public static explicit operator Quaternion(Element e)
		{
			if (e.vectorType == Element.VectorType.Quaternion)
			{
				return e.quat;
			}
			return Quaternion.Euler(e.v);
		}

		// Token: 0x06000B64 RID: 2916 RVA: 0x0003A802 File Offset: 0x00038A02
		public static explicit operator Vector3(Element e)
		{
			if (e.vectorType == Element.VectorType.Vector3)
			{
				return e.v;
			}
			return e.quat.eulerAngles;
		}

		// Token: 0x06000B65 RID: 2917 RVA: 0x0003A820 File Offset: 0x00038A20
		public static Element Slerp(Element a, Element b, float t)
		{
			if (a.vectorType == Element.VectorType.Quaternion)
			{
				return Quaternion.Slerp((Quaternion)a, (Quaternion)b, t);
			}
			return Vector3.Slerp((Vector3)a, (Vector3)b, t);
		}

		// Token: 0x06000B66 RID: 2918 RVA: 0x0003A85A File Offset: 0x00038A5A
		public static Element SlerpUnclamped(Element a, Element b, float t)
		{
			if (a.vectorType == Element.VectorType.Quaternion)
			{
				return Quaternion.SlerpUnclamped((Quaternion)a, (Quaternion)b, t);
			}
			return Vector3.SlerpUnclamped((Vector3)a, (Vector3)b, t);
		}

		// Token: 0x06000B67 RID: 2919 RVA: 0x0003A894 File Offset: 0x00038A94
		public static bool operator ==(Element a, Element b)
		{
			if (a.vectorType != b.vectorType || a.vectorType != Element.VectorType.Vector3)
			{
				return a.quat.x == b.quat.x && a.quat.y == b.quat.y && a.quat.z == b.quat.z && a.quat.w == b.quat.w;
			}
			return a.v.x == b.v.x && a.v.y == b.v.y && a.v.z == b.v.z;
		}

		// Token: 0x06000B68 RID: 2920 RVA: 0x0003A965 File Offset: 0x00038B65
		public static bool operator !=(Element a, Element b)
		{
			return !(a == b);
		}

		// Token: 0x06000B69 RID: 2921 RVA: 0x0003A971 File Offset: 0x00038B71
		public override bool Equals(object obj)
		{
			return obj is Element && this.Equals((Element)obj);
		}

		// Token: 0x06000B6A RID: 2922 RVA: 0x0003A98C File Offset: 0x00038B8C
		public bool Equals(Element other)
		{
			if (this.vectorType != other.vectorType || this.vectorType != Element.VectorType.Vector3)
			{
				return this.quat.x == other.quat.x && this.quat.y == other.quat.y && this.quat.z == other.quat.z && this.quat.w == other.quat.w;
			}
			return this.v.x == other.v.x && this.v.y == other.v.y && this.v.z == other.v.z;
		}

		// Token: 0x06000B6B RID: 2923 RVA: 0x0003AA5D File Offset: 0x00038C5D
		public static bool Equals(Vector3 a, Vector3 b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}

		// Token: 0x06000B6C RID: 2924 RVA: 0x0003AA8B File Offset: 0x00038C8B
		public static bool Equals(Quaternion a, Quaternion b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
		}

		// Token: 0x06000B6D RID: 2925 RVA: 0x0003AAC7 File Offset: 0x00038CC7
		public static implicit operator Element(Quaternion q)
		{
			return new Element(q);
		}

		// Token: 0x06000B6E RID: 2926 RVA: 0x0003AACF File Offset: 0x00038CCF
		public static implicit operator Element(Vector3 v)
		{
			return new Element(v);
		}

		// Token: 0x06000B6F RID: 2927 RVA: 0x0003AAD8 File Offset: 0x00038CD8
		public override string ToString()
		{
			return this.vectorType + " " + ((this.vectorType == Element.VectorType.Quaternion) ? this.quat.ToString() : this.v.ToString());
		}

		// Token: 0x06000B70 RID: 2928 RVA: 0x0003AB27 File Offset: 0x00038D27
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x04000C03 RID: 3075
		[FieldOffset(0)]
		public Element.VectorType vectorType;

		// Token: 0x04000C04 RID: 3076
		[FieldOffset(4)]
		public Vector3 v;

		// Token: 0x04000C05 RID: 3077
		[FieldOffset(4)]
		public Quaternion quat;

		// Token: 0x020003B6 RID: 950
		public enum VectorType
		{
			// Token: 0x040012BE RID: 4798
			Vector3 = 1,
			// Token: 0x040012BF RID: 4799
			Quaternion
		}
	}
}
