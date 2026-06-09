using System;
using UnityEngine;

// Token: 0x0200008D RID: 141
public class PhysicsFunctions : MonoBehaviour
{
	// Token: 0x06000309 RID: 777 RVA: 0x000134E8 File Offset: 0x000116E8
	public static Vector2 ObstructionPoint(Vector2 from, Vector2 to)
	{
		RaycastHit2D raycastHit2D = Physics2D.Raycast(from, to - from, Vector2.Distance(from, to), PhysicsFunctions.mask);
		if (raycastHit2D.transform)
		{
			return raycastHit2D.point;
		}
		return to;
	}

	// Token: 0x04000438 RID: 1080
	private static LayerMask mask = LayerMask.GetMask(new string[]
	{
		"Default",
		"IgnorePlayer",
		"IgnoreMap"
	});
}
