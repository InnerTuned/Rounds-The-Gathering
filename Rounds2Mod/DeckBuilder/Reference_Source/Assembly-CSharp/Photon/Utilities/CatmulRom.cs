using System;
using UnityEngine;

namespace Photon.Utilities
{
	// Token: 0x0200023F RID: 575
	public class CatmulRom
	{
		// Token: 0x06000C6E RID: 3182 RVA: 0x0003EAA8 File Offset: 0x0003CCA8
		public static float CatmullRomLerp(float pre, float start, float end, float post, float t)
		{
			while (t > 1f)
			{
				pre = start;
				start = end;
				end = post;
				post = end + (end - start);
				t -= 1f;
			}
			float num = 2f * start;
			float num2 = end - pre;
			float num3 = 2f * pre - 5f * start + 4f * end - post;
			float num4 = -pre + 3f * (start - end) + post;
			float num5 = t * t;
			return (num + num2 * t + num3 * num5 + num4 * num5 * t) * 0.5f;
		}

		// Token: 0x06000C6F RID: 3183 RVA: 0x0003EB2C File Offset: 0x0003CD2C
		public static float CatmullRomLerp(float pre, float start, float end, float t)
		{
			float num = end + (end - start);
			while (t > 1f)
			{
				pre = start;
				start = end;
				end = num;
				num = end + (end - start);
				t -= 1f;
			}
			float num2 = 2f * start;
			float num3 = end - pre;
			float num4 = 2f * pre - 5f * start + 4f * end - num;
			float num5 = -pre + 3f * (start - end) + num;
			float num6 = t * t;
			return (num2 + num3 * t + num4 * num6 + num5 * num6 * t) * 0.5f;
		}

		// Token: 0x06000C70 RID: 3184 RVA: 0x0003EBB0 File Offset: 0x0003CDB0
		public static Vector3 CatmullRomLerp(Vector2 pre, Vector2 start, Vector2 end, Vector2 post, float t)
		{
			while (t > 1f)
			{
				pre = start;
				start = end;
				end = post;
				post = end + (end - start);
				t -= 1f;
			}
			Vector2 a = 2f * start;
			Vector2 a2 = end - pre;
			Vector2 a3 = 2f * pre - 5f * start + 4f * end - post;
			Vector2 a4 = -pre + 3f * (start - end) + post;
			float d = t * t;
			return (a + a2 * t + a3 * d + a4 * d * t) * 0.5f;
		}

		// Token: 0x06000C71 RID: 3185 RVA: 0x0003EC94 File Offset: 0x0003CE94
		public static Vector3 CatmullRomLerp(Vector2 pre, Vector2 start, Vector2 end, float t)
		{
			Vector2 vector = end + (end - start);
			while (t > 1f)
			{
				pre = start;
				start = end;
				end = vector;
				vector = end + (end - start);
				t -= 1f;
			}
			Vector2 a = 2f * start;
			Vector2 a2 = end - pre;
			Vector2 a3 = 2f * pre - 5f * start + 4f * end - vector;
			Vector2 a4 = -pre + 3f * (start - end) + vector;
			float d = t * t;
			return (a + a2 * t + a3 * d + a4 * d * t) * 0.5f;
		}

		// Token: 0x06000C72 RID: 3186 RVA: 0x0003ED84 File Offset: 0x0003CF84
		public static Vector3 CatmullRomLerp(Vector3 pre, Vector3 start, Vector3 end, Vector3 post, float t)
		{
			while (t > 1f)
			{
				pre = start;
				start = end;
				end = post;
				post = end + (end - start);
				t -= 1f;
			}
			Vector3 a = 2f * start;
			Vector3 a2 = end - pre;
			Vector3 a3 = 2f * pre - 5f * start + 4f * end - post;
			Vector3 a4 = -pre + 3f * (start - end) + post;
			float d = t * t;
			return (a + a2 * t + a3 * d + a4 * d * t) * 0.5f;
		}

		// Token: 0x06000C73 RID: 3187 RVA: 0x0003EE64 File Offset: 0x0003D064
		public static Vector3 CatmullRomLerp(Vector3 pre, Vector3 start, Vector3 end, float t)
		{
			Vector3 vector = end + (end - start);
			while (t > 1f)
			{
				pre = start;
				start = end;
				end = vector;
				vector = end + (end - start);
				t -= 1f;
			}
			Vector3 a = 2f * start;
			Vector3 a2 = end - pre;
			Vector3 a3 = 2f * pre - 5f * start + 4f * end - vector;
			Vector3 a4 = -pre + 3f * (start - end) + vector;
			float d = t * t;
			return (a + a2 * t + a3 * d + a4 * d * t) * 0.5f;
		}
	}
}
