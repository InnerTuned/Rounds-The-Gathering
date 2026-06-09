using System;
using UnityEngine;

// Token: 0x0200007C RID: 124
public class Looper : MonoBehaviour
{
	// Token: 0x06000296 RID: 662 RVA: 0x00010D8D File Offset: 0x0000EF8D
	private void Awake()
	{
		this.mainCam = MainCam.instance.transform.GetComponent<Camera>();
		this.trail = base.transform.root.GetComponentInChildren<TrailRenderer>();
		this.rayTrail = base.GetComponentInParent<RayCastTrail>();
	}

	// Token: 0x06000297 RID: 663 RVA: 0x00010DC8 File Offset: 0x0000EFC8
	private void Update()
	{
		if (this.loops <= 0 && this.sinceLoop > 0.3f)
		{
			Object.Destroy(this);
		}
		Vector3 vector = this.mainCam.WorldToScreenPoint(base.transform.position);
		vector.x /= (float)Screen.width;
		vector.y /= (float)Screen.height;
		vector = new Vector3(Mathf.Clamp(vector.x, 0f, 1f), Mathf.Clamp(vector.y, 0f, 1f), vector.z);
		if ((vector.x == 0f || vector.x == 1f || vector.y == 1f || vector.y == 0f) && this.sinceLoop > 0.1f)
		{
			if (vector.x == 0f)
			{
				vector.x = 1f;
			}
			else if (vector.x == 1f)
			{
				vector.x = 0f;
			}
			if (vector.y == 0f)
			{
				vector.y = 1f;
			}
			else if (vector.y == 1f)
			{
				vector.y = 0f;
			}
			vector.x *= (float)Screen.width;
			vector.y *= (float)Screen.height;
			base.transform.root.position = this.mainCam.ScreenToWorldPoint(vector);
			this.rayTrail.MoveRay();
			for (int i = 0; i < this.trail.positionCount; i++)
			{
				this.trail.SetPosition(i, base.transform.position);
			}
			this.sinceLoop = 0f;
			this.loops--;
			return;
		}
		this.sinceLoop += TimeHandler.deltaTime;
	}

	// Token: 0x040003B7 RID: 951
	private float sinceLoop = 1f;

	// Token: 0x040003B8 RID: 952
	private Camera mainCam;

	// Token: 0x040003B9 RID: 953
	private TrailRenderer trail;

	// Token: 0x040003BA RID: 954
	private RayCastTrail rayTrail;

	// Token: 0x040003BB RID: 955
	private int loops = 3;
}
