using System;
using Photon.Compression;
using Photon.Pun.Simple;
using UnityEngine;

namespace emotitron
{
	// Token: 0x020001E8 RID: 488
	[PackObject(DefaultKeyRate.Every, defaultInclusion = DefaultPackInclusion.Explicit)]
	public class TestPackObject : NetComponent, IOnPreSimulate, IOnInterpolate
	{
		// Token: 0x060009AA RID: 2474 RVA: 0x000315B4 File Offset: 0x0002F7B4
		public void RotationHook(float newrot, float oldrot)
		{
			base.transform.localEulerAngles = new Vector3(0f, this.rotation, 0f);
		}

		// Token: 0x060009AB RID: 2475 RVA: 0x000027C8 File Offset: 0x000009C8
		public void SnapshotHook(float snap, float targ)
		{
		}

		// Token: 0x060009AC RID: 2476 RVA: 0x000315D8 File Offset: 0x0002F7D8
		public void OnPreSimulate(int frameId, int subFrameId)
		{
			if (this.photonView.IsMine)
			{
				this.rotation = (Mathf.Sin(Time.time) + 0.5f) * 120f;
				base.transform.localEulerAngles = new Vector3(0f, this.rotation, 0f);
			}
		}

		// Token: 0x060009AD RID: 2477 RVA: 0x000027C8 File Offset: 0x000009C8
		public void FixedUpdate()
		{
		}

		// Token: 0x060009AE RID: 2478 RVA: 0x000027C8 File Offset: 0x000009C8
		private void Update()
		{
		}

		// Token: 0x060009AF RID: 2479 RVA: 0x0003162E File Offset: 0x0002F82E
		public bool OnInterpolate(int snapFrameId, int targFrameId, float t)
		{
			if (!base.PhotonView.IsMine)
			{
				base.transform.localEulerAngles = new Vector3(0f, this.rotation, 0f);
			}
			return true;
		}

		// Token: 0x04000AF8 RID: 2808
		[SyncHalfFloat(IndicatorBit.None, KeyRate.UseDefault, snapshotCallback = "SnapshotHook", applyCallback = "RotationHook", setValueTiming = SetValueTiming.BeforeCallback, interpolate = true, keyRate = KeyRate.Every)]
		public float rotation;

		// Token: 0x04000AF9 RID: 2809
		[SyncRangedInt(-1, 2, IndicatorBits.None, KeyRate.UseDefault)]
		public int intoroboto;
	}
}
