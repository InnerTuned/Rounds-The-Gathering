using System;
using UnityEngine;

namespace Photon.Compression
{
	// Token: 0x02000213 RID: 531
	public static class IncludeAxisExtensions
	{
		// Token: 0x06000B71 RID: 2929 RVA: 0x0003AB3C File Offset: 0x00038D3C
		public static float SqrMagnitude(this Vector3 v, IncludedAxes ia)
		{
			return (((ia & IncludedAxes.X) != IncludedAxes.None) ? (v.x * v.x) : 0f) + (((ia & IncludedAxes.Y) != IncludedAxes.None) ? (v.y * v.y) : 0f) + (((ia & IncludedAxes.Z) != IncludedAxes.None) ? (v.z * v.z) : 0f);
		}

		// Token: 0x06000B72 RID: 2930 RVA: 0x0003AB98 File Offset: 0x00038D98
		public static float Magnitude(this Vector3 v, IncludedAxes ia)
		{
			return Mathf.Sqrt((((ia & IncludedAxes.X) != IncludedAxes.None) ? (v.x * v.x) : 0f) + (((ia & IncludedAxes.Y) != IncludedAxes.None) ? (v.y * v.y) : 0f) + (((ia & IncludedAxes.Z) != IncludedAxes.None) ? (v.z * v.z) : 0f));
		}

		// Token: 0x06000B73 RID: 2931 RVA: 0x0003ABF8 File Offset: 0x00038DF8
		public static Vector3 Lerp(this GameObject go, Vector3 start, Vector3 end, IncludedAxes ia, float t, bool localPosition = false)
		{
			Vector3 vector = Vector3.Lerp(start, end, t);
			return new Vector3(((ia & IncludedAxes.X) != IncludedAxes.None) ? vector[0] : (localPosition ? go.transform.localPosition[0] : go.transform.position[0]), ((ia & IncludedAxes.Y) != IncludedAxes.None) ? vector[1] : (localPosition ? go.transform.localPosition[1] : go.transform.position[1]), ((ia & IncludedAxes.Z) != IncludedAxes.None) ? vector[2] : (localPosition ? go.transform.localPosition[2] : go.transform.position[2]));
		}

		// Token: 0x06000B74 RID: 2932 RVA: 0x0003ACCC File Offset: 0x00038ECC
		public static void SetPosition(this GameObject go, Vector3 pos, IncludedAxes ia, bool localPosition = false)
		{
			Vector3 vector = new Vector3(((ia & IncludedAxes.X) != IncludedAxes.None) ? pos[0] : (localPosition ? go.transform.localPosition[0] : go.transform.position[0]), ((ia & IncludedAxes.Y) != IncludedAxes.None) ? pos[1] : (localPosition ? go.transform.localPosition[1] : go.transform.position[1]), ((ia & IncludedAxes.Z) != IncludedAxes.None) ? pos[2] : (localPosition ? go.transform.localPosition[2] : go.transform.position[2]));
			if (!localPosition)
			{
				go.transform.position = vector;
				return;
			}
			go.transform.localPosition = vector;
		}
	}
}
