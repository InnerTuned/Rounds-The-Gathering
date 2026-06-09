using System;
using UnityEngine;

// Token: 0x0200014C RID: 332
public class FRILerp : MonoBehaviour
{
	// Token: 0x060006B7 RID: 1719 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Start()
	{
	}

	// Token: 0x060006B8 RID: 1720 RVA: 0x0002566B File Offset: 0x0002386B
	public static Vector3 Lerp(Vector3 from, Vector3 target, float speed)
	{
		return Vector3.Lerp(from, target, 1f - Mathf.Exp(-speed * TimeHandler.deltaTime));
	}

	// Token: 0x060006B9 RID: 1721 RVA: 0x00025687 File Offset: 0x00023887
	public static Vector3 LerpUnclamped(Vector3 from, Vector3 target, float speed)
	{
		return Vector3.LerpUnclamped(from, target, 1f - Mathf.Exp(-speed * TimeHandler.deltaTime));
	}

	// Token: 0x060006BA RID: 1722 RVA: 0x000256A3 File Offset: 0x000238A3
	public static float Lerp(float from, float target, float speed)
	{
		return Mathf.Lerp(from, target, 1f - Mathf.Exp(-speed * TimeHandler.deltaTime));
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x000256BF File Offset: 0x000238BF
	public static float LerpUnclamped(float from, float target, float speed)
	{
		return Mathf.LerpUnclamped(from, target, 1f - Mathf.Exp(-speed * TimeHandler.deltaTime));
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x000256DB File Offset: 0x000238DB
	public static Vector3 Slerp(Vector3 from, Vector3 target, float speed)
	{
		return Vector3.Slerp(from, target, 1f - Mathf.Exp(-speed * TimeHandler.deltaTime));
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x000256F7 File Offset: 0x000238F7
	public static Vector3 SlerpUnclamped(Vector3 from, Vector3 target, float speed)
	{
		return Vector3.SlerpUnclamped(from, target, 1f - Mathf.Exp(-speed * TimeHandler.deltaTime));
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x00025713 File Offset: 0x00023913
	public static Quaternion Lerp(Quaternion from, Quaternion target, float speed)
	{
		return Quaternion.Lerp(from, target, 1f - Mathf.Exp(-speed * TimeHandler.deltaTime));
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x0002572F File Offset: 0x0002392F
	public static Quaternion LerpUnclamped(Quaternion from, Quaternion target, float speed)
	{
		return Quaternion.LerpUnclamped(from, target, 1f - Mathf.Exp(-speed * TimeHandler.deltaTime));
	}
}
