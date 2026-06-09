using System;
using System.Collections.Generic;
using emotitron.Utilities.GUIUtilities;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002EE RID: 750
	public class AutoZoom : MonoBehaviour
	{
		// Token: 0x06001020 RID: 4128 RVA: 0x0004E48F File Offset: 0x0004C68F
		private void Awake()
		{
			this.cam = base.GetComponent<Camera>();
			if (!this.cam)
			{
				AutoZoom.watched.Add(base.transform);
			}
		}

		// Token: 0x06001021 RID: 4129 RVA: 0x0004E4BA File Offset: 0x0004C6BA
		private void OnDestroy()
		{
			if (AutoZoom.watched.Contains(base.transform))
			{
				AutoZoom.watched.Remove(base.transform);
			}
		}

		// Token: 0x06001022 RID: 4130 RVA: 0x0004E4E0 File Offset: 0x0004C6E0
		private void LateUpdate()
		{
			if (!this.cam || !this.cam.isActiveAndEnabled)
			{
				return;
			}
			Bounds bounds = default(Bounds);
			for (int i = 0; i < AutoZoom.watched.Count; i++)
			{
				Vector2 a = this.cam.WorldToViewportPoint(AutoZoom.watched[i].position);
				bounds.Encapsulate(a + new Vector2(-0.5f, -0.5f));
			}
			if (AutoZoom.watched.Count > 0)
			{
				this.cam.transform.Rotate(new Vector3(0f, 1f, 0f), bounds.center.x * Time.deltaTime * this.panRate);
				float num = bounds.extents.x - this.window;
				float num2 = bounds.extents.y - this.window;
				float num3 = (num > num2) ? num : num2;
				float value = this.cam.fieldOfView + num3 * Time.deltaTime * this.zoomRate;
				this.cam.fieldOfView = Mathf.Clamp(value, 15f, 75f);
			}
		}

		// Token: 0x04000F32 RID: 3890
		public static List<Transform> watched = new List<Transform>();

		// Token: 0x04000F33 RID: 3891
		public const float MAX_FOV = 75f;

		// Token: 0x04000F34 RID: 3892
		public const float MIN_FOV = 15f;

		// Token: 0x04000F35 RID: 3893
		[Range(0.1f, 0.5f)]
		[HideInInspector]
		public float window = 0.25f;

		// Token: 0x04000F36 RID: 3894
		[ValueType("/sec", 48f)]
		[HideInInspector]
		public float panRate = 20f;

		// Token: 0x04000F37 RID: 3895
		[ValueType("/sec", 48f)]
		[HideInInspector]
		public float zoomRate = 200f;

		// Token: 0x04000F38 RID: 3896
		private Camera cam;
	}
}
