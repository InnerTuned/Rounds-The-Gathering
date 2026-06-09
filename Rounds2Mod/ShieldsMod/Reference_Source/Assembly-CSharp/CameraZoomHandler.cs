using System;
using UnityEngine;

// Token: 0x0200001D RID: 29
public class CameraZoomHandler : MonoBehaviour
{
	// Token: 0x06000092 RID: 146 RVA: 0x00005246 File Offset: 0x00003446
	private void Start()
	{
		this.cameras = base.GetComponentsInChildren<Camera>();
	}

	// Token: 0x06000093 RID: 147 RVA: 0x00005254 File Offset: 0x00003454
	private void Update()
	{
		float b = 20f;
		if (MapManager.instance.currentMap != null)
		{
			b = MapManager.instance.currentMap.Map.size;
		}
		for (int i = 0; i < this.cameras.Length; i++)
		{
			this.cameras[i].orthographicSize = Mathf.Lerp(this.cameras[i].orthographicSize, b, Time.unscaledDeltaTime * 5f);
		}
	}

	// Token: 0x04000086 RID: 134
	private Camera[] cameras;
}
