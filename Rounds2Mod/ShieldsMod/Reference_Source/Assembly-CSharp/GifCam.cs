using System;
using UnityEngine;

// Token: 0x02000063 RID: 99
public class GifCam : MonoBehaviour
{
	// Token: 0x060001D5 RID: 469 RVA: 0x0000B878 File Offset: 0x00009A78
	private void Start()
	{
		this.cameras = base.GetComponentsInChildren<Camera>();
		this.camStartPos = this.cameras[0].transform.position;
		this.cameraStartSize = this.cameras[0].orthographicSize;
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x0000B8B4 File Offset: 0x00009AB4
	private void Update()
	{
	}

	// Token: 0x04000289 RID: 649
	public static bool isGifCam;

	// Token: 0x0400028A RID: 650
	private Camera[] cameras;

	// Token: 0x0400028B RID: 651
	private Vector3 camStartPos;

	// Token: 0x0400028C RID: 652
	private float cameraStartSize;

	// Token: 0x0400028D RID: 653
	private bool follow = true;
}
