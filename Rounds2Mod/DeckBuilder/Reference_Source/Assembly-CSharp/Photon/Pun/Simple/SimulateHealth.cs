using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002ED RID: 749
	public class SimulateHealth : MonoBehaviour
	{
		// Token: 0x0600101D RID: 4125 RVA: 0x0004E39C File Offset: 0x0004C59C
		private void Start()
		{
			IVitalsSystem componentInChildren = base.GetComponentInChildren<IVitalsSystem>();
			if (componentInChildren == null || !(componentInChildren as SyncObject).PhotonView.IsMine)
			{
				Object.Destroy(this);
				return;
			}
			this.vitals = componentInChildren.Vitals;
		}

		// Token: 0x0600101E RID: 4126 RVA: 0x0004E3D8 File Offset: 0x0004C5D8
		private void Update()
		{
			if (Input.GetKeyDown(this.AddHealthKey))
			{
				this.vitals.ApplyCharges(this.vitalType, (double)this.amount, false, false);
			}
			if (Input.GetKeyDown(this.RemHealthKey))
			{
				this.vitals.ApplyCharges(this.vitalType, (double)(-(double)this.amount), false, true);
			}
			if (Input.GetKeyDown(this.DamagehKey))
			{
				this.vitals.ApplyCharges((double)this.amount, false, true);
			}
		}

		// Token: 0x04000F2C RID: 3884
		private Vitals vitals;

		// Token: 0x04000F2D RID: 3885
		public VitalNameType vitalType = new VitalNameType(VitalType.Health);

		// Token: 0x04000F2E RID: 3886
		public KeyCode AddHealthKey = KeyCode.Alpha6;

		// Token: 0x04000F2F RID: 3887
		public KeyCode RemHealthKey = KeyCode.Alpha7;

		// Token: 0x04000F30 RID: 3888
		public KeyCode DamagehKey = KeyCode.Alpha8;

		// Token: 0x04000F31 RID: 3889
		public float amount = 20f;
	}
}
