using System;
using UnityEngine;

// Token: 0x0200007D RID: 125
public class MainCam : MonoBehaviour
{
	// Token: 0x06000299 RID: 665 RVA: 0x00010FCC File Offset: 0x0000F1CC
	private void Awake()
	{
		this.cam = base.GetComponent<Camera>();
		MainCam.instance = this;
	}

	// Token: 0x040003BC RID: 956
	public static MainCam instance;

	// Token: 0x040003BD RID: 957
	public Camera cam;
}
